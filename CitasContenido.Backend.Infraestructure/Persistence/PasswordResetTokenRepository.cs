using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Infraestructure.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CitasContenido.Backend.Infraestructure.Persistence
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly string _connectionString;

        public PasswordResetTokenRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("ConnectionString no configurado");
        }

        // ==================== CREAR (con UnitOfWork) ====================
        public async Task<long> CrearAsync(PasswordResetToken token, IUnitOfWork unitOfWork)
        {
            SqlConnection? connection = null;
            SqlTransaction? transaction = null;
            bool transaccionExterna = unitOfWork != null;

            try
            {
                if (transaccionExterna)
                {
                    var uow = unitOfWork as UnitOfWork;
                    connection = uow!.Connection;
                    transaction = uow.Transaction;
                }
                else
                {
                    connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();
                    transaction = (SqlTransaction)await connection.BeginTransactionAsync();
                }

                const string sql = @"
                    INSERT INTO PasswordResetTokens 
                    (UsuarioId, Email, Token, TokenPlainText, FechaCreacion, FechaExpiracion, 
                     Usado, IpAddress, UserAgent)
                    VALUES 
                    (@UsuarioId, @Email, @Token, @TokenPlainText, @FechaCreacion, @FechaExpiracion, 
                     @Usado, @IpAddress, @UserAgent);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                var id = await connection.ExecuteScalarAsync<long>(sql, new
                {
                    token.UsuarioId,
                    token.Email,
                    token.Token,
                    token.TokenPlainText,
                    token.FechaCreacion,
                    token.FechaExpiracion,
                    token.Usado,
                    token.IpAddress,
                    token.UserAgent
                }, transaction);

                if (!transaccionExterna)
                {
                    await transaction!.CommitAsync();
                }

                return id;
            }
            catch (SqlException ex)
            {
                if (!transaccionExterna && transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw new Exception($"Error al crear token de recuperación: {ex.Message}", ex);
            }
            finally
            {
                if (!transaccionExterna)
                {
                    transaction?.Dispose();
                    connection?.Dispose();
                }
            }
        }

        // ==================== ACTUALIZAR (con UnitOfWork) ====================
        public async Task ActualizarAsync(PasswordResetToken token, IUnitOfWork unitOfWork)
        {
            SqlConnection? connection = null;
            SqlTransaction? transaction = null;
            bool transaccionExterna = unitOfWork != null;

            try
            {
                if (transaccionExterna)
                {
                    var uow = unitOfWork as UnitOfWork;
                    connection = uow!.Connection;
                    transaction = uow.Transaction;
                }
                else
                {
                    connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();
                    transaction = (SqlTransaction)await connection.BeginTransactionAsync();
                }

                const string sql = @"
                    UPDATE PasswordResetTokens 
                    SET Usado = @Usado, 
                        FechaUso = @FechaUso
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, new
                {
                    token.Id,
                    token.Usado,
                    token.FechaUso
                }, transaction);

                if (!transaccionExterna)
                {
                    await transaction!.CommitAsync();
                }
            }
            catch (SqlException ex)
            {
                if (!transaccionExterna && transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw new Exception($"Error al actualizar token de recuperación: {ex.Message}", ex);
            }
            finally
            {
                if (!transaccionExterna)
                {
                    transaction?.Dispose();
                    connection?.Dispose();
                }
            }
        }

        // ==================== INVALIDAR TODOS LOS TOKENS (con UnitOfWork) ====================
        public async Task InvalidarTodosLosTokensDelUsuarioAsync(long usuarioId, IUnitOfWork unitOfWork)
        {
            SqlConnection? connection = null;
            SqlTransaction? transaction = null;
            bool transaccionExterna = unitOfWork != null;

            try
            {
                if (transaccionExterna)
                {
                    var uow = unitOfWork as UnitOfWork;
                    connection = uow!.Connection;
                    transaction = uow.Transaction;
                }
                else
                {
                    connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();
                    transaction = (SqlTransaction)await connection.BeginTransactionAsync();
                }

                const string sql = @"
                    UPDATE PasswordResetTokens 
                    SET Usado = 1, 
                        FechaUso = GETUTCDATE()
                    WHERE UsuarioId = @UsuarioId 
                    AND Usado = 0";

                await connection.ExecuteAsync(sql, new { UsuarioId = usuarioId }, transaction);

                if (!transaccionExterna)
                {
                    await transaction!.CommitAsync();
                }
            }
            catch (SqlException ex)
            {
                if (!transaccionExterna && transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw new Exception($"Error al invalidar tokens del usuario: {ex.Message}", ex);
            }
            finally
            {
                if (!transaccionExterna)
                {
                    transaction?.Dispose();
                    connection?.Dispose();
                }
            }
        }

        // ==================== BUSCAR POR EMAIL Y CÓDIGO (con try-catch) ====================
        public async Task<PasswordResetToken?> ObtenerPorEmailYCodigoAsync(string email, string codigo)
        {
            try
            {
                const string query = @"
                    SELECT * FROM PasswordResetTokens 
                    WHERE Email = @Email 
                    AND Usado = 0 
                    AND FechaExpiracion > GETUTCDATE()
                    ORDER BY FechaCreacion DESC";

                using var connection = new SqlConnection(_connectionString);
                var tokens = await connection.QueryAsync<dynamic>(query, new { Email = email.ToLower() });

                foreach (var tokenData in tokens)
                {
                    var token = MapToEntity(tokenData);
                    if (token.VerificarCodigo(codigo))
                    {
                        return token;
                    }
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al buscar token por email y código: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al buscar token: {ex.Message}", ex);
            }
        }

        // ==================== OBTENER ÚLTIMO TOKEN VÁLIDO (con try-catch) ====================
        public async Task<PasswordResetToken?> ObtenerUltimoTokenValidoAsync(string email)
        {
            try
            {
                const string query = @"
                    SELECT TOP 1 * FROM PasswordResetTokens 
                    WHERE Email = @Email 
                    AND Usado = 0 
                    AND FechaExpiracion > GETUTCDATE()
                    ORDER BY FechaCreacion DESC";

                using var connection = new SqlConnection(_connectionString);
                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { Email = email.ToLower() });

                return result != null ? MapToEntity(result) : null;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener último token válido: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al obtener token: {ex.Message}", ex);
            }
        }

        // ==================== LIMPIAR TOKENS EXPIRADOS (Background Job) ====================
        public async Task LimpiarTokensExpiradosAsync()
        {
            try
            {
                const string query = @"
                    DELETE FROM PasswordResetTokens 
                    WHERE FechaExpiracion < DATEADD(HOUR, -24, GETUTCDATE())";

                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(query);
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al limpiar tokens expirados: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al limpiar tokens: {ex.Message}", ex);
            }
        }

        // ==================== MAPEAR A ENTIDAD (USANDO REFLECTION) ====================
        private PasswordResetToken MapToEntity(dynamic data)
        {
            try
            {
                // Crear instancia usando constructor privado
                var token = (PasswordResetToken)Activator.CreateInstance(
                    typeof(PasswordResetToken),
                    nonPublic: true)!;

                var tipo = typeof(PasswordResetToken);

                // Mapear todas las propiedades usando reflection
                tipo.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, (long)data.Id);

                tipo.GetProperty("UsuarioId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, (long)data.UsuarioId);

                tipo.GetProperty("Email", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, (string)data.Email);

                tipo.GetProperty("Token", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, (string)data.Token);

                tipo.GetProperty("TokenPlainText", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, (string)data.TokenPlainText);

                tipo.GetProperty("FechaCreacion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, (DateTime)data.FechaCreacion);

                tipo.GetProperty("FechaExpiracion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, (DateTime)data.FechaExpiracion);

                tipo.GetProperty("Usado", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, (bool)data.Usado);

                tipo.GetProperty("FechaUso", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, data.FechaUso != null ? (DateTime?)data.FechaUso : null);

                tipo.GetProperty("IpAddress", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, data.IpAddress != null ? (string)data.IpAddress : null);

                tipo.GetProperty("UserAgent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                    .SetValue(token, data.UserAgent != null ? (string)data.UserAgent : null);

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al mapear token de recuperación: {ex.Message}", ex);
            }
        }
    }
}
