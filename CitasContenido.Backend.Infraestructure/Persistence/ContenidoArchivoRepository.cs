using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Infraestructure.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CitasContenido.Backend.Infraestructure.Persistence
{
    public class ContenidoArchivoRepository : IContenidoArchivoRepository
    {
        private readonly string _connectionString;

        public ContenidoArchivoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<long> CrearAsync(ContenidoArchivo contenido, IUnitOfWork unitOfWork)
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

                var sql = @"
                    INSERT INTO ContenidoArchivo (
                        UsuarioId, TipoContenidoId, TipoArchivoId, EsPrincipal,
                        EstadoVerificacion, EsPublico, Orden, NombreArchivo,
                        TamanioArchivoMB, Extension, Blob_key, Blob_url,
                        Container_name, FechaCreacion, UsuarioCreacion
                    ) VALUES (
                        @UsuarioId, @TipoContenidoId, @TipoArchivoId, @EsPrincipal,
                        @EstadoVerificacion, @EsPublico, @Orden, @NombreArchivo,
                        @TamanioArchivoMB, @Extension, @BlobKey, @BlobUrl,
                        @ContainerName, @FechaCreacion, @UsuarioCreacion
                    );
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
                var id = await connection.ExecuteScalarAsync<long>(sql, new
                {
                    contenido.UsuarioId,
                    contenido.TipoContenidoId,
                    contenido.TipoArchivoId,
                    contenido.EsPrincipal,
                    contenido.EstadoVerificacion,
                    contenido.EsPublico,
                    contenido.Orden,
                    contenido.NombreArchivo,
                    contenido.TamanioArchivoMB,
                    contenido.Extension,
                    contenido.BlobKey,
                    contenido.BlobUrl,
                    contenido.ContainerName,
                    contenido.FechaCreacion,
                    contenido.UsuarioCreacion
                }, transaction);


                if (!transaccionExterna)
                {
                    await transaction!.CommitAsync();
                }

                return id;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al crear contenido archivo: {ex.Message}", ex);
            } 
        }

        public async Task<ContenidoArchivo?> ObtenerPorIdAsync(long id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT 
                        Id, UsuarioId, TipoContenidoId, TipoArchivoId, EsPrincipal,
                        EstadoVerificacion, EsPublico, Orden, NombreArchivo,
                        TamañoArchivo, Extension, Blob_key AS BlobKey, 
                        Blob_url AS BlobUrl, Container_name AS ContainerName,
                        FechaCreacion, UsuarioCreacion
                    FROM ContenidoArchivo
                    WHERE Id = @Id";

                return await connection.QueryFirstOrDefaultAsync<ContenidoArchivo>(sql, new { Id = id });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener contenido archivo: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ContenidoArchivo>> ObtenerPorUsuarioAsync(long usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT 
                        Id, UsuarioId, TipoContenidoId, TipoArchivoId, EsPrincipal,
                        EstadoVerificacion, EsPublico, Orden, NombreArchivo,
                        TamañoArchivo, Extension, Blob_key AS BlobKey, 
                        Blob_url AS BlobUrl, Container_name AS ContainerName,
                        FechaCreacion, UsuarioCreacion
                    FROM ContenidoArchivo
                    WHERE UsuarioId = @UsuarioId
                    ORDER BY Orden, FechaCreacion";

                return await connection.QueryAsync<ContenidoArchivo>(sql, new { UsuarioId = usuarioId });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener contenidos por usuario: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ContenidoArchivo>> ObtenerPorUsuarioYTipoAsync(long usuarioId, int tipoContenidoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT 
                        Id, UsuarioId, TipoContenidoId, TipoArchivoId, EsPrincipal,
                        EstadoVerificacion, EsPublico, Orden, NombreArchivo,
                        TamañoArchivo, Extension, Blob_key AS BlobKey, 
                        Blob_url AS BlobUrl, Container_name AS ContainerName,
                        FechaCreacion, UsuarioCreacion
                    FROM ContenidoArchivo
                    WHERE UsuarioId = @UsuarioId AND TipoContenidoId = @TipoContenidoId
                    ORDER BY Orden, FechaCreacion";

                return await connection.QueryAsync<ContenidoArchivo>(sql, new
                {
                    UsuarioId = usuarioId,
                    TipoContenidoId = tipoContenidoId
                });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener contenidos por usuario y tipo: {ex.Message}", ex);
            }
        }

        public async Task<ContenidoArchivo?> ObtenerFotoPrincipalAsync(long usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT TOP 1
                        Id, UsuarioId, TipoContenidoId, TipoArchivoId, EsPrincipal,
                        EstadoVerificacion, EsPublico, Orden, NombreArchivo,
                        TamañoArchivo, Extension, Blob_key AS BlobKey, 
                        Blob_url AS BlobUrl, Container_name AS ContainerName,
                        FechaCreacion, UsuarioCreacion
                    FROM ContenidoArchivo
                    WHERE UsuarioId = @UsuarioId AND EsPrincipal = 1";

                return await connection.QueryFirstOrDefaultAsync<ContenidoArchivo>(sql, new { UsuarioId = usuarioId });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener foto principal: {ex.Message}", ex);
            }
        }

        public async Task<int> ContarPorUsuarioYTipoAsync(long usuarioId, int tipoContenidoId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT COUNT(*)
                    FROM ContenidoArchivo
                    WHERE UsuarioId = @UsuarioId AND TipoContenidoId = @TipoContenidoId";

                return await connection.ExecuteScalarAsync<int>(sql, new
                {
                    UsuarioId = usuarioId,
                    TipoContenidoId = tipoContenidoId
                });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al contar contenidos: {ex.Message}", ex);
            }
        }

        public async Task<bool> ActualizarAsync(ContenidoArchivo contenido)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    UPDATE ContenidoArchivo
                    SET EsPrincipal = @EsPrincipal,
                        EstadoVerificacion = @EstadoVerificacion,
                        EsPublico = @EsPublico,
                        Orden = @Orden
                    WHERE Id = @Id";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    contenido.Id,
                    contenido.EsPrincipal,
                    contenido.EstadoVerificacion,
                    contenido.EsPublico,
                    contenido.Orden
                });

                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al actualizar contenido archivo: {ex.Message}", ex);
            }
        }

        public async Task<bool> EliminarAsync(long id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = "DELETE FROM ContenidoArchivo WHERE Id = @Id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al eliminar contenido archivo: {ex.Message}", ex);
            }
        }

        public async Task DesmarcarTodasComoPrincipalAsync(long usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    UPDATE ContenidoArchivo
                    SET EsPrincipal = 0
                    WHERE UsuarioId = @UsuarioId AND EsPrincipal = 1";

                await connection.ExecuteAsync(sql, new { UsuarioId = usuarioId });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al desmarcar fotos principales: {ex.Message}", ex);
            }
        }
    }
}
