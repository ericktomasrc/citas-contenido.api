using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Infraestructure.Common;
using CitasContenido.Backend.Infraestructure.Config;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CitasContenido.Backend.Infraestructure.Persistence
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public RefreshTokenRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<RefrescarToken?> ObtenerPorTokenAsync(string token)
        {
            using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);

            var sql = @"
                SELECT Id, UsuarioId, Token, FechaExpiracion, FechaCreacion, Revocado, FechaRevocacion, ReemplazadoPor
                FROM RefreshToken
                WHERE Token = @Token";

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Token = token });

            if (result == null) return null;

            return MapToEntity(result);
        }

        public async Task<Guid> CrearAsync(RefrescarToken refreshToken, IUnitOfWork? unitOfWork = null)
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
                INSERT INTO RefreshToken (UsuarioId, Token, FechaExpiracion, FechaCreacion, Revocado, FechaRevocacion, ReemplazadoPor,Habilitado)
                VALUES (@UsuarioId, @Token, @FechaExpiracion, @FechaCreacion, @Revocado, @FechaRevocacion, @ReemplazadoPor,1);";

                var id = await connection.ExecuteScalarAsync<Guid>(sql, new
                {
                    refreshToken.UsuarioId,
                    refreshToken.Token,
                    refreshToken.FechaExpiracion,
                    refreshToken.FechaCreacion,
                    refreshToken.Revocado,
                    refreshToken.FechaRevocacion,
                    refreshToken.ReemplazadoPor
                },
                transaction
                );

                if (!transaccionExterna)
                {
                    await transaction!.CommitAsync();
                }

                return id;
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

        public async Task ActualizarAsync(RefrescarToken refreshToken, IUnitOfWork? unitOfWork = null)
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
                UPDATE RefreshToken SET
                    Revocado = @Revocado,
                    FechaRevocacion = @FechaRevocacion,
                    ReemplazadoPor = @ReemplazadoPor
                WHERE Id = @Id"; 

                await connection.ExecuteAsync(sql, new
                {
                    refreshToken.Id,
                    refreshToken.Revocado,
                    refreshToken.FechaRevocacion,
                    refreshToken.ReemplazadoPor
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

        public async Task RevocarTodosPorUsuarioAsync(long usuarioId)
        {
            using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);

            var sql = @"
                UPDATE RefreshToken SET
                    Revocado = 1,
                    FechaRevocacion = @Ahora
                WHERE UsuarioId = @UsuarioId AND Revocado = 0";

            await connection.ExecuteAsync(sql, new { UsuarioId = usuarioId, Ahora = DateTime.UtcNow });
        }

        private RefrescarToken MapToEntity(dynamic data)
        {
            var refreshToken = RefrescarToken.Crear(data.UsuarioId, (string)data.Token, 7);

            var idProperty = typeof(RefrescarToken).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            idProperty?.SetValue(refreshToken, data.Id);

            if ((bool)data.Revocado)
            {
                refreshToken.Revocar(data.ReemplazadoPor);
            }

            return refreshToken;
        }
    }
}
