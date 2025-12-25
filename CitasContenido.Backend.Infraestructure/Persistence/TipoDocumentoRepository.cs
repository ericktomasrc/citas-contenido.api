using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Infraestructure.Config;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CitasContenido.Backend.Infraestructure.Persistence
{
    public class TipoDocumentoRepository : ITipoDocumentoRepository
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly ILogger<TipoDocumentoRepository> _logger;

        public TipoDocumentoRepository(
            DatabaseConfig databaseConfig,
            ILogger<TipoDocumentoRepository> logger)
        {
            _databaseConfig = databaseConfig;
            _logger = logger;
        }

        public async Task<TipoDocumento?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                const string query = @"
                SELECT 
                    Id, 
                    Name, 
                    Descripcion, 
                    FechaCreacion, 
                    UsuarioCreacion, 
                    FechaModificacion, 
                    UsuarioModificacion,
                    Habilitado
                FROM TipoDocumento
                WHERE Id = @Id";

                return await connection.QueryFirstOrDefaultAsync<TipoDocumento>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipo de documento por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TipoDocumento>> GetAllAsync()
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                const string query = @"
                SELECT 
                    Id, 
                    Name, 
                    Descripcion, 
                    FechaCreacion, 
                    UsuarioCreacion, 
                    FechaModificacion, 
                    UsuarioModificacion,
                    Habilitado
                FROM TipoDocumento
                ORDER BY Name";

                return await connection.QueryAsync<TipoDocumento>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los tipos de documento");
                throw;
            }
        }

        public async Task<IEnumerable<TipoDocumento>> GetHabilitadosAsync()
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                const string query = @"
                SELECT 
                    Id, 
                    Name, 
                    Descripcion, 
                    FechaCreacion, 
                    UsuarioCreacion, 
                    FechaModificacion, 
                    UsuarioModificacion,
                    Habilitado
                FROM TipoDocumento
                WHERE Habilitado = 1
                ORDER BY Name";

                return await connection.QueryAsync<TipoDocumento>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipos de documento habilitados");
                throw;
            }
        }

        public async Task<IEnumerable<TipoDocumento>> BuscarAsync(string? filtro, bool? soloHabilitados)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                var query = @"
                SELECT 
                    Id, 
                    Name, 
                    Descripcion, 
                    FechaCreacion, 
                    UsuarioCreacion, 
                    FechaModificacion, 
                    UsuarioModificacion,
                    Habilitado
                FROM TipoDocumento
                WHERE 1 = 1";

                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    query += " AND (Name LIKE @Filtro OR Descripcion LIKE @Filtro)";
                    parameters.Add("Filtro", $"%{filtro}%");
                }

                if (soloHabilitados.HasValue && soloHabilitados.Value)
                {
                    query += " AND Habilitado = 1";
                }

                query += " ORDER BY Name";

                return await connection.QueryAsync<TipoDocumento>(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar tipos de documento con filtro: {Filtro}", filtro);
                throw;
            }
        }

        public async Task<int> CreateAsync(TipoDocumento tipoDocumento)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                const string query = @"
                INSERT INTO TipoDocumento 
                (Name, Descripcion, FechaCreacion, UsuarioCreacion, Habilitado)
                VALUES 
                (@Name, @Descripcion, @FechaCreacion, @UsuarioCreacion, @Habilitado);
                
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var id = await connection.ExecuteScalarAsync<int>(query, new
                {
                    tipoDocumento.Name,
                    tipoDocumento.Descripcion,
                    tipoDocumento.FechaCreacion,
                    tipoDocumento.UsuarioCreacion,
                    tipoDocumento.Habilitado
                });

                _logger.LogInformation("Tipo de documento creado con ID: {Id}", id);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tipo de documento");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(TipoDocumento tipoDocumento)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                const string query = @"
                UPDATE TipoDocumento
                SET 
                    Name = @Name,
                    Descripcion = @Descripcion,
                    FechaModificacion = @FechaModificacion,
                    UsuarioModificacion = @UsuarioModificacion,
                    Habilitado = @Habilitado
                WHERE Id = @Id";

                var affectedRows = await connection.ExecuteAsync(query, new
                {
                    tipoDocumento.Id,
                    tipoDocumento.Name,
                    tipoDocumento.Descripcion,
                    tipoDocumento.FechaModificacion,
                    tipoDocumento.UsuarioModificacion,
                    tipoDocumento.Habilitado
                });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tipo de documento: {Id}", tipoDocumento.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                const string query = "DELETE FROM TipoDocumento WHERE Id = @Id";

                var affectedRows = await connection.ExecuteAsync(query, new { Id = id });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tipo de documento: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.SqlServerConnection);
                await connection.OpenAsync();

                const string query = "SELECT COUNT(1) FROM TipoDocumento WHERE UPPER(Name) = UPPER(@Name)";

                var count = await connection.ExecuteScalarAsync<int>(query, new { Name = name });

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de tipo de documento: {Name}", name);
                throw;
            }
        }
    }
}
