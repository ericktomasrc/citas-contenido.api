using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Infraestructure.Common;
using CitasContenido.Backend.Infraestructure.Config;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CitasContenido.Backend.Infraestructure.Persistence
{
    public class VerificacionEmailRepository : IVerificacionEmailRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public VerificacionEmailRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<VerificacionEmail?> ObtenerPorTokenAsync(string token)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                var sql = @"
                    SELECT Id, UsuarioId, Token, FechaExpiracion, Verificado, FechaVerificacion, FechaCreacion
                    FROM VerificacionEmail
                    WHERE Token = @Token";

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Token = token });

                if (result == null) return null;

                return MapToEntity(result);
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener verificación por token: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al obtener verificación: {ex.Message}", ex);
            }
        }

        public async Task<long> CrearAsync(VerificacionEmail verificacion, IUnitOfWork? unitOfWork = null)
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
                    connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                    await connection.OpenAsync();
                    transaction = (SqlTransaction)await connection.BeginTransactionAsync();
                }

                var sql = @"
                    INSERT INTO VerificacionEmail (UsuarioId, Token, FechaExpiracion, Verificado, FechaVerificacion, FechaCreacion)
                    VALUES (@UsuarioId, @Token, @FechaExpiracion, @Verificado, @FechaVerificacion, @FechaCreacion);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                var id = await connection.ExecuteScalarAsync<long>(
                    sql,
                    new
                    {
                        verificacion.UsuarioId,
                        verificacion.Token,
                        verificacion.FechaExpiracion,
                        verificacion.Verificado,
                        verificacion.FechaVerificacion,
                        verificacion.FechaCreacion
                    },
                    transaction
                );

                if (!transaccionExterna)
                {
                    await transaction!.CommitAsync();
                }

                return id;
            }
            catch (SqlException ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al crear verificación de email: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error inesperado al crear verificación: {ex.Message}", ex);
            }
        }

        public async Task ActualizarAsync(VerificacionEmail verificacion, IUnitOfWork? unitOfWork = null)
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
                    connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                    await connection.OpenAsync();
                    transaction = (SqlTransaction)await connection.BeginTransactionAsync();
                }

                var sql = @"
                    UPDATE VerificacionEmail SET
                        Verificado = @Verificado,
                        FechaVerificacion = @FechaVerificacion
                    WHERE Id = @Id";

                await connection.ExecuteAsync(
                    sql,
                    new
                    {
                        verificacion.Id,
                        verificacion.Verificado,
                        verificacion.FechaVerificacion
                    },
                    transaction
                );

                if (!transaccionExterna)
                {
                    await transaction!.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                if (!transaccionExterna && transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                throw new Exception($"Error al actualizar usuario: {ex.Message}", ex);
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

        public async Task<bool> ExisteTokenValidoAsync(long usuarioId)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                var sql = @"
                    SELECT COUNT(1) 
                    FROM VerificacionEmail 
                    WHERE UsuarioId = @UsuarioId 
                      AND Verificado = 0 
                      AND FechaExpiracion > @Ahora";

                var count = await connection.ExecuteScalarAsync<int>(
                    sql,
                    new
                    {
                        UsuarioId = usuarioId,
                        Ahora = DateTime.UtcNow
                    }
                );

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al verificar token válido: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al verificar token: {ex.Message}", ex);
            }
        }

        private VerificacionEmail MapToEntity(dynamic data)
        {
            try
            {
                var verificacion = VerificacionEmail.Crear(data.UsuarioId, (string)data.Token, 24);

                // Usar reflection para setear propiedades privadas
                var idProperty = typeof(VerificacionEmail).GetProperty(
                    "Id",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                );
                idProperty?.SetValue(verificacion,  data.Id);

                if ((bool)data.Verificado)
                {
                    var verificadoProperty = typeof(VerificacionEmail).GetProperty(
                        "Verificado",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    );
                    verificadoProperty?.SetValue(verificacion, true);

                    var fechaVerificacionProperty = typeof(VerificacionEmail).GetProperty(
                        "FechaVerificacion",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    );
                    fechaVerificacionProperty?.SetValue(verificacion, (DateTime?)data.FechaVerificacion);
                }

                return verificacion;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al mapear entidad VerificacionEmail: {ex.Message}", ex);
            }
        }
    }
}