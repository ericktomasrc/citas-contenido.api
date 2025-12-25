// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// CitasContenido.Backend.Infraestructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// CitasContenido.Backend.Infraestructure.Persistence.UsuarioRepository
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Infraestructure.Common;
using CitasContenido.Backend.Infraestructure.Config;
using Dapper;
using Microsoft.Data.SqlClient;

public class UsuarioRepository : IUsuarioRepository
{
	private readonly DatabaseConfig _databaseConfig;

	public UsuarioRepository(DatabaseConfig databaseConfig)
	{
		_databaseConfig = databaseConfig;
	}

	public async Task<Usuario?> ObtenerPorIdAsync(long id)
	{
		try
		{
			SqlConnection connection = new SqlConnection(_databaseConfig.SqlServerConnection);
			try
			{
				await ((DbConnection)(object)connection).OpenAsync();
				string sql = "\r\n                    SELECT  * FROM Usuarios\r\n                    WHERE Id = @Id AND Habilitado = 1";
				dynamic result = await SqlMapper.QueryFirstOrDefaultAsync<object>((IDbConnection)connection, sql, (object)new
				{
					Id = id
				}, (IDbTransaction)null, (int?)null, (CommandType?)null);
				if (result == null)
				{
					return null;
				}
				return (Usuario)MapToEntity(result);
			}
			finally
			{
				((IDisposable)connection)?.Dispose();
			}
		}
		catch (SqlException ex)
		{
			SqlException ex2 = ex;
			SqlException ex3 = ex2;
			throw new Exception("Error al obtener usuario por ID: " + ((Exception)(object)ex3).Message, (Exception?)(object)ex3);
		}
		catch (Exception ex4)
		{
			Exception ex5 = ex4;
			throw new Exception("Error inesperado al obtener usuario: " + ex5.Message, ex5);
		}
	}

	public async Task<Usuario?> ObtenerPorEmailAsync(string email)
	{
		try
		{
			SqlConnection connection = new SqlConnection(_databaseConfig.SqlServerConnection);
			try
			{
				await ((DbConnection)(object)connection).OpenAsync();
				string sql = "SELECT * FROM USUARIO WHERE Email = @Email AND Habilitado = 1";
				dynamic result = await SqlMapper.QueryFirstOrDefaultAsync<object>((IDbConnection)connection, sql, (object)new
				{
					Email = email.ToLower()
				}, (IDbTransaction)null, (int?)null, (CommandType?)null);
				if (result == null)
				{
					return null;
				}
				return (Usuario)MapToEntity(result);
			}
			finally
			{
				((IDisposable)connection)?.Dispose();
			}
		}
		catch (SqlException ex)
		{
			SqlException ex2 = ex;
			SqlException ex3 = ex2;
			throw new Exception("Error al obtener usuario por email: " + ((Exception)(object)ex3).Message, (Exception?)(object)ex3);
		}
		catch (Exception ex4)
		{
			Exception ex5 = ex4;
			throw new Exception("Error inesperado al obtener usuario: " + ex5.Message, ex5);
		}
	}

	public async Task<long> CrearAsync(Usuario usuario, IUnitOfWork unitOfWork)
	{
		SqlConnection connection = null;
		SqlTransaction transaction = null;
		bool transaccionExterna = unitOfWork != null;
		try
		{
			if (transaccionExterna)
			{
				UnitOfWork uow = unitOfWork as UnitOfWork;
				connection = uow.Connection;
				transaction = uow.Transaction;
			}
			else
			{
				connection = new SqlConnection(_databaseConfig.SqlServerConnection);
				await ((DbConnection)(object)connection).OpenAsync();
				transaction = (SqlTransaction)(await ((DbConnection)(object)connection).BeginTransactionAsync(default(CancellationToken)));
			}
			string sql = "\r\n                    INSERT INTO Usuarios (\r\n                        NGuid, \r\n                        Email,   \r\n                        PasswordHash,\r\n                        RangoDistanciaKm,\r\n                        IsPremium, \r\n                        UltimaActividad,\r\n                        FechaCreacion,\r\n                        UsuarioCreacion,\r\n                        EmailVerificado,\r\n                        IdentidadVerificada,\r\n                        TipoUsuarioId,\r\n                        Habilitado\r\n                    )\r\n                    VALUES (\r\n                        @NGuid,\r\n                        @Email,   \r\n                        @PasswordHash,\r\n                        @RangoDistanciaKm,\r\n                        @IsPremium,\r\n                        @UltimaActividad,\r\n                        @FechaCreacion,\r\n                        'SYSTEM',\r\n                        @EmailVerificado,\r\n                        @IdentidadVerificada,\r\n                        @TipoUsuarioId,\r\n                        @Habilitado\r\n                    );\r\n                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
			long id = await SqlMapper.ExecuteScalarAsync<long>((IDbConnection)connection, sql, (object)new
			{
				usuario.NGuid, usuario.Email, usuario.PasswordHash, usuario.RangoDistanciaKm, usuario.IsPremium, usuario.UltimaActividad, usuario.FechaCreacion, usuario.EmailVerificado, usuario.IdentidadVerificada, usuario.TipoUsuarioId,
				usuario.Habilitado
			}, (IDbTransaction)transaction, (int?)null, (CommandType?)null);
			if (!transaccionExterna)
			{
				await ((DbTransaction)(object)transaction).CommitAsync(default(CancellationToken));
			}
			return id;
		}
		catch (Exception ex)
		{
			if (!transaccionExterna && transaction != null)
			{
				await ((DbTransaction)(object)transaction).RollbackAsync(default(CancellationToken));
			}
			throw new Exception("Error al actualizar usuario: " + ex.Message, ex);
		}
		finally
		{
			if (!transaccionExterna)
			{
				((DbTransaction)(object)transaction)?.Dispose();
				((Component)(object)connection)?.Dispose();
			}
		}
	}

	public async Task ActualizarAsync(Usuario usuario, IUnitOfWork? unitOfWork = null)
	{
		SqlConnection connection = null;
		SqlTransaction transaction = null;
		bool transaccionExterna = unitOfWork != null;
		try
		{
			if (transaccionExterna)
			{
				UnitOfWork uow = unitOfWork as UnitOfWork;
				connection = uow.Connection;
				transaction = uow.Transaction;
			}
			else
			{
				connection = new SqlConnection(_databaseConfig.SqlServerConnection);
				await ((DbConnection)(object)connection).OpenAsync();
				transaction = (SqlTransaction)(await ((DbConnection)(object)connection).BeginTransactionAsync(default(CancellationToken)));
			}
			string sql = "\r\n                    UPDATE Usuarios SET\r\n                        Email = @Email,\r\n                        PasswordHash = @PasswordHash,\r\n                        Nombre = @Nombre,                       \r\n                        EmailVerificado = @EmailVerificado,\r\n                        IdentidadVerificada = @IdentidadVerificada,        \r\n                        RangoDistanciaKm = @RangoDistanciaKm,\r\n                        IsPremium = @IsPremium,\r\n                        UltimaActividad = @UltimaActividad,\r\n                        FechaModificacion = @FechaActualizacion\r\n                    WHERE Id = @Id";
			await SqlMapper.ExecuteAsync((IDbConnection)connection, sql, (object)new { usuario.Id, usuario.Email, usuario.PasswordHash, usuario.Nombre, usuario.EmailVerificado, usuario.IdentidadVerificada, usuario.RangoDistanciaKm, usuario.IsPremium, usuario.UltimaActividad, usuario.FechaActualizacion }, (IDbTransaction)transaction, (int?)null, (CommandType?)null);
			if (!transaccionExterna)
			{
				await ((DbTransaction)(object)transaction).CommitAsync(default(CancellationToken));
			}
		}
		catch (Exception ex)
		{
			if (!transaccionExterna && transaction != null)
			{
				await ((DbTransaction)(object)transaction).RollbackAsync(default(CancellationToken));
			}
			throw new Exception("Error al actualizar usuario: " + ex.Message, ex);
		}
		finally
		{
			if (!transaccionExterna)
			{
				((DbTransaction)(object)transaction)?.Dispose();
				((Component)(object)connection)?.Dispose();
			}
		}
	}

	public async Task<bool> ExisteEmailAsync(string email)
	{
		try
		{
			SqlConnection connection = new SqlConnection(_databaseConfig.SqlServerConnection);
			try
			{
				await ((DbConnection)(object)connection).OpenAsync();
				string sql = "SELECT COUNT(1) FROM Usuarios WHERE Email = @Email AND Habilitado = 1";
				return await SqlMapper.ExecuteScalarAsync<int>((IDbConnection)connection, sql, (object)new
				{
					Email = email.ToLower()
				}, (IDbTransaction)null, (int?)null, (CommandType?)null) > 0;
			}
			finally
			{
				((IDisposable)connection)?.Dispose();
			}
		}
		catch (SqlException ex)
		{
			SqlException ex2 = ex;
			SqlException ex3 = ex2;
			throw new Exception("Error al verificar existencia de email: " + ((Exception)(object)ex3).Message, (Exception?)(object)ex3);
		}
		catch (Exception ex4)
		{
			Exception ex5 = ex4;
			throw new Exception("Error inesperado al verificar email: " + ex5.Message, ex5);
		}
	}

	public async Task ActualizarUltimaActividadAsync(Guid usuarioId)
	{
		SqlConnection connection = new SqlConnection(_databaseConfig.SqlServerConnection);
		try
		{
			await ((DbConnection)(object)connection).OpenAsync();
			await using DbTransaction transaction = await ((DbConnection)(object)connection).BeginTransactionAsync(default(CancellationToken));
			int num = 0;
			object obj = default(object);
			try
			{
				string sql = "UPDATE Usuario SET UltimaActividad = @Ahora WHERE Id = @UsuarioId";
				await SqlMapper.ExecuteAsync((IDbConnection)connection, sql, (object)new
				{
					UsuarioId = usuarioId,
					Ahora = DateTime.UtcNow
				}, (IDbTransaction)transaction, (int?)null, (CommandType?)null);
				await transaction.CommitAsync();
			}
			catch (SqlException ex)
			{
				SqlException ex2 = ex;
				obj = ex2;
				num = 1;
			}
			catch (Exception ex3)
			{
				await transaction.RollbackAsync();
				throw new Exception("Error inesperado al actualizar actividad: " + ex3.Message, ex3);
			}
			if (num != 1)
			{
				return;
			}
			SqlException ex4 = (SqlException)obj;
			await transaction.RollbackAsync();
			throw new Exception("Error al actualizar última actividad: " + ((Exception)(object)ex4).Message, (Exception?)(object)ex4);
		}
		finally
		{
			((IDisposable)connection)?.Dispose();
		}
	}

	private Usuario MapToEntity(dynamic data)
	{
		try
		{
			dynamic val = Usuario.CrearConEmail(data.Email);
			typeof(Usuario).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)?.SetValue(val, data.Id);
			PropertyInfo property = typeof(Usuario).GetProperty("PasswordHash", BindingFlags.Instance | BindingFlags.Public);
			property?.SetValue(val, (string)data.PasswordHash);
			if (data.Nombre != null)
			{
				val.CompletarRegistro(data.Nombre, data.Apellidos, data.Edad, data.Genero, data.TipoDocumentoId, data.NumeroDocumento, data.Nacionalidad, property);
			}
			if ((bool)data.EmailVerificado)
			{
				val.VerificarEmail();
			}
			if ((bool)data.IdentidadVerificada)
			{
				val.VerificarIdentidad();
			}
			return (Usuario)val;
		}
		catch (Exception ex)
		{
			throw new Exception("Error al mapear entidad Usuario: " + ex.Message, ex);
		}
	}
}
