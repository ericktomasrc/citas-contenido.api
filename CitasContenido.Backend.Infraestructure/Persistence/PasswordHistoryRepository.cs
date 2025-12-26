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
    public class PasswordHistoryRepository : IPasswordHistoryRepository
    {
        private readonly string _connectionString;

        public PasswordHistoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("ConnectionString no configurado");
        }

        // ==================== CREAR (con UnitOfWork) ====================
        public async Task<long> CrearAsync(PasswordHistory data, IUnitOfWork unitOfWork)
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
                    INSERT INTO PasswordHistory 
                    (UsuarioId, PasswordHash, FechaCreacion)
                    VALUES 
                    (@UsuarioId, @PasswordHash, @FechaCreacion);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                var id = await connection.ExecuteScalarAsync<long>(sql, new
                {
                    data.UsuarioId,
                    data.PasswordHash,
                    data.FechaCreacion
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
 
    }
}
