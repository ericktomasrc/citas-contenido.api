using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper; 

namespace CitasContenido.Backend.Infraestructure.Persistence
{
    public class TipoContenidoRepository : ITipoContenidoRepository
    {
        private readonly string _connectionString;

        public TipoContenidoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<TipoContenido?> ObtenerPorIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT 
                        Id, Nombre, Descripcion, SoloCreadores, RequiereVerificacion,
                        LimiteArchivos, TamanioMaximoMB, FechaCreacion, UsuarioCreacion,
                        FechaModificacion, UsuarioModificacion, Habilitado
                    FROM TipoContenido
                    WHERE Id = @Id AND Habilitado = 1";

                return await connection.QueryFirstOrDefaultAsync<TipoContenido>(sql, new { Id = id });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener tipo de contenido por ID: {ex.Message}", ex);
            }
        }

        public async Task<TipoContenido?> ObtenerPorNombreAsync(string nombre)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT 
                        Id, Nombre, Descripcion, SoloCreadores, RequiereVerificacion,
                        LimiteArchivos, TamañoMaximoMB, FechaCreacion, UsuarioCreacion,
                        FechaModificacion, UsuarioModificacion, Habilitado
                    FROM TipoContenido
                    WHERE Nombre = @Nombre AND Habilitado = 1";

                return await connection.QueryFirstOrDefaultAsync<TipoContenido>(sql, new { Nombre = nombre });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener tipo de contenido por nombre: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TipoContenido>> ObtenerTodosAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT 
                        Id, Nombre, Descripcion, SoloCreadores, RequiereVerificacion,
                        LimiteArchivos, TamañoMaximoMB, FechaCreacion, UsuarioCreacion,
                        FechaModificacion, UsuarioModificacion, Habilitado
                    FROM TipoContenido
                    WHERE Habilitado = 1
                    ORDER BY Nombre";

                return await connection.QueryAsync<TipoContenido>(sql);
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener todos los tipos de contenido: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TipoContenido>> ObtenerPorTipoUsuarioAsync(bool esCreador)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                var sql = @"
                    SELECT 
                        Id, Nombre, Descripcion, SoloCreadores, RequiereVerificacion,
                        LimiteArchivos, TamañoMaximoMB, FechaCreacion, UsuarioCreacion,
                        FechaModificacion, UsuarioModificacion, Habilitado
                    FROM TipoContenido
                    WHERE Habilitado = 1 
                    AND (SoloCreadores = 0 OR (SoloCreadores = 1 AND @EsCreador = 1))
                    ORDER BY Nombre";

                return await connection.QueryAsync<TipoContenido>(sql, new { EsCreador = esCreador });
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al obtener tipos de contenido por tipo de usuario: {ex.Message}", ex);
            }
        }
    }
}
