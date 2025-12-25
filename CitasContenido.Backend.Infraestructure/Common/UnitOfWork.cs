using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Infraestructure.Config;
using Microsoft.Data.SqlClient;

namespace CitasContenido.Backend.Infraestructure.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseConfig _databaseConfig;
        private SqlConnection? _connection;
        private SqlTransaction? _transaction;

        public SqlTransaction? Transaction => _transaction;
        public SqlConnection? Connection => _connection;

        public UnitOfWork(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public async Task BeginTransactionAsync()
        {
            _connection = new SqlConnection(_databaseConfig.SqlServerConnection);
            await _connection.OpenAsync();
            _transaction = (SqlTransaction)await _connection.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
