using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Infraestructure.Common;
using CitasContenido.Backend.Infraestructure.Config;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CitasContenido.Backend.Infraestructure.Persistence
{
    public class VerificacionIdentidadRepository : IVerificacionIdentidadRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public VerificacionIdentidadRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task<long> CrearAsync(VerificacionIdentidad verificacion, IUnitOfWork? unitOfWork = null)
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
                INSERT INTO VerificacionIdentidad 
                    ( UsuarioId, FotoEnVivoUrl, FotoDocumentoUrl, EstadoVerificacion, 
                     MotivoRechazo, FechaCreacion, FechaVerificacion, VerificadoPor)
                VALUES 
                    (  @UsuarioId, @FotoEnVivoUrl, @FotoDocumentoUrl, @EstadoVerificacion,
                     @MotivoRechazo, @FechaCreacion, @FechaVerificacion, @VerificadoPor); 
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                var id = await connection.ExecuteScalarAsync<long>(sql, new
                { 
                    verificacion.UsuarioId,
                    verificacion.FotoEnVivoUrl,
                    verificacion.FotoDocumentoUrl,
                    verificacion.EstadoVerificacion,
                    verificacion.MotivoRechazo,
                    verificacion.FechaCreacion,
                    verificacion.FechaVerificacion,
                    verificacion.VerificadoPor
                }, transaction); 

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

        public async Task<VerificacionIdentidad?> ObtenerPorUsuarioIdAsync(Guid usuarioId)
        {
            using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);

            var sql = @"
                SELECT Id, UsuarioId, FotoPerfilUrl, FotoDocumentoUrl, EstadoVerificacion,
                       MotivoRechazo, FechaCreacion, FechaVerificacion, VerificadoPor
                FROM VerificacionIdentidad
                WHERE UsuarioId = @UsuarioId
                ORDER BY FechaCreacion DESC";

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { UsuarioId = usuarioId });

            if (result == null) return null;

            return MapToEntity(result);
        }

        public async Task ActualizarAsync(VerificacionIdentidad verificacion)
        {
            using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);

            var sql = @"
                UPDATE VerificacionIdentidad SET
                    EstadoVerificacion = @EstadoVerificacion,
                    MotivoRechazo = @MotivoRechazo,
                    FechaVerificacion = @FechaVerificacion,
                    VerificadoPor = @VerificadoPor
                WHERE Id = @Id";

            await connection.ExecuteAsync(sql, new
            {
                verificacion.Id,
                verificacion.EstadoVerificacion,
                verificacion.MotivoRechazo,
                verificacion.FechaVerificacion,
                verificacion.VerificadoPor
            });
        }

        private VerificacionIdentidad MapToEntity(dynamic data)
        {
            var verificacion = VerificacionIdentidad.Crear(
                data.UsuarioId,
                data.FotoPerfilUrl,
                data.FotoDocumentoUrl
            );

            var idProperty = typeof(VerificacionIdentidad).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            idProperty?.SetValue(verificacion, (Guid)data.Id);

            return verificacion;
        }
    }
}
