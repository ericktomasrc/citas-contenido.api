// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// CitasContenido.Backend.Infraestructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// CitasContenido.Backend.Infraestructure.Services.CompletarRegistroDomainService
using System;
using System.IO;
using System.Threading.Tasks;
using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;

public class CompletarRegistroDomainService : ICompletarRegistroDomainService
{
	private readonly IUnitOfWork _unitOfWork;

	private readonly IUsuarioRepository _usuarioRepository;

	private readonly IContenidoArchivoRepository _contenidoArchivoRepository;

	private readonly ITipoContenidoRepository _tipoContenidoRepository;

	private readonly IVerificacionIdentidadRepository _verificacionIdentidadRepository;

	private readonly IRefreshTokenRepository _refreshTokenRepository;

	private readonly IJwtService _jwtService;

	private readonly IPasswordHasher _passwordHasher;

	private readonly IAzureStorageService _azureStorageService;

	private readonly IEmailService _emailService;

	private readonly IFaceVerificationService _faceVerificationService;

	public CompletarRegistroDomainService(IUnitOfWork unitOfWork, IUsuarioRepository usuarioRepository, IContenidoArchivoRepository contenidoArchivoRepository, ITipoContenidoRepository tipoContenidoRepository, IVerificacionIdentidadRepository verificacionIdentidadRepository, IRefreshTokenRepository refreshTokenRepository, IJwtService jwtService, IPasswordHasher passwordHasher, IAzureStorageService azureStorageService, IEmailService emailService, IFaceVerificationService faceVerificationService)
	{
		_unitOfWork = unitOfWork;
		_usuarioRepository = usuarioRepository;
		_contenidoArchivoRepository = contenidoArchivoRepository;
		_tipoContenidoRepository = tipoContenidoRepository;
		_verificacionIdentidadRepository = verificacionIdentidadRepository;
		_refreshTokenRepository = refreshTokenRepository;
		_jwtService = jwtService;
		_passwordHasher = passwordHasher;
		_azureStorageService = azureStorageService;
		_emailService = emailService;
		_faceVerificationService = faceVerificationService;
	}

	public async Task<Result<CompletarRegistroResult>> CompletarRegistroAsync(long usuarioId, int tipoUsuarioId, string username, string nombre, string apellidos, DateTime fechaNacimiento, int generoId, string password, decimal latitud, decimal longitud, Stream? fotoPerfilStream, string fotoPerfilNombre, Stream? fotoEnVivoStream, string fotoEnVivoNombre, int? tipoDocumentoId = null, string? numeroDocumento = null, string? nacionalidad = null, string? whatsapp = null, string? numeroYape = null, string? numeroPlin = null, string? bancoNombre = null, string? numeroCuenta = null, string? bio = null)
	{
		try
		{
			await _unitOfWork.BeginTransactionAsync();
			Usuario usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
			if (usuario == null)
			{
				return Result<CompletarRegistroResult>.Failure("Usuario no encontrado");
			}
			if (!usuario.EmailVerificado)
			{
				return Result<CompletarRegistroResult>.Failure("Email no verificado");
			}
			if (usuario.RegistroCompletado)
			{
				return Result<CompletarRegistroResult>.Failure("Registro ya completado");
			}
			if (await _usuarioRepository.ObtenerPorEmailAsync(username) != null)
			{
				return Result<CompletarRegistroResult>.Failure("Username no disponible");
			}
			string passwordHash = _passwordHasher.HashPassword(password);
			if (fotoPerfilStream != null && fotoEnVivoStream != null)
			{
				FaceVerificationResult_ verificacionSiLasFotosTienenSimilitud = await _faceVerificationService.CompararRostrosAsync(fotoPerfilStream, fotoEnVivoStream);
				if (!verificacionSiLasFotosTienenSimilitud.SonIguales)
				{
					return Result<CompletarRegistroResult>.Failure("Verificación facial fallida: " + verificacionSiLasFotosTienenSimilitud.Mensaje);
				}
			}
			string fotoEnVivo = null;
			if (fotoPerfilStream != null)
			{
				string extension = Path.GetExtension(fotoPerfilNombre);
				fotoEnVivo = await _azureStorageService.SubirArchivoAsync(fotoPerfilStream, fotoPerfilNombre, extension);
			}
			string urlFotoVerificacion = null;
			if (fotoEnVivoStream != null)
			{
				string extension2 = Path.GetExtension(fotoEnVivoNombre);
				urlFotoVerificacion = await _azureStorageService.SubirArchivoAsync(fotoEnVivoStream, fotoEnVivoNombre, extension2);
			}
			usuario.CompletarRegistro(
				username,
				nombre,
				apellidos, 
				passwordHash, 
				fechaNacimiento, 
				generoId,
				tipoUsuarioId,
				latitud, 
				longitud, 
				fotoEnVivo, 
				tipoDocumentoId,
				numeroDocumento, 
				nacionalidad, "","","","","",""); 
            //string? pais,
            //string? departamento,
            //string? provincia,
            //string? distrito,
            //string? ciudad,
            //string? direccionCompleta

            await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);
			if (!string.IsNullOrEmpty(fotoEnVivo))
			{
				TipoContenido tipoFotoPerfil = await _tipoContenidoRepository.ObtenerPorNombreAsync("FotoPerfilPrincipal");
				if (tipoFotoPerfil != null)
				{
					ContenidoArchivo contenido = ContenidoArchivo.Crear(usuarioId, tipoFotoPerfil.Id, 1, fotoPerfilNombre, fotoPerfilStream.Length, Path.GetExtension(fotoPerfilNombre), Guid.NewGuid().ToString(), fotoEnVivo, "citascontenido", true, true, 1, "SYSTEM");
					await _contenidoArchivoRepository.CrearAsync(contenido);
				}
			}
			if (!string.IsNullOrEmpty(urlFotoVerificacion))
			{
				TipoContenido tipoFotoVerif = await _tipoContenidoRepository.ObtenerPorNombreAsync("FotoVerificacionEnVivo");
				if (tipoFotoVerif != null)
				{
					ContenidoArchivo contenido2 = ContenidoArchivo.Crear(usuarioId, tipoFotoVerif.Id, 1, fotoEnVivoNombre, fotoEnVivoStream.Length, Path.GetExtension(fotoEnVivoNombre), Guid.NewGuid().ToString(), urlFotoVerificacion, "citascontenido", false, false, 1, "SYSTEM");
					await _contenidoArchivoRepository.CrearAsync(contenido2);
				}
			}
			VerificacionIdentidad verificacion = VerificacionIdentidad.Crear(usuarioId, urlFotoVerificacion ?? "", "SYSTEM");
			await _verificacionIdentidadRepository.CrearAsync(verificacion, _unitOfWork);
			string token = _jwtService.GenerarToken(usuario.NGuid, usuario.Email);
			RefrescarToken refreshToken = RefrescarToken.Crear(usuario.Id, "SYSTEM", 7);
			await _refreshTokenRepository.CrearAsync(refreshToken, (IUnitOfWork)null);
			await _unitOfWork.CommitAsync();
			try
			{
				await _emailService.EnviarEmailBienvenidaAsync(usuario.Email, usuario.Nombre);
			}
			catch
			{
			}
			return Result<CompletarRegistroResult>.Success(new CompletarRegistroResult
			{
				UsuarioId = usuario.Id,
				Token = token,
				RefreshToken = refreshToken.Token
			});
		}
		catch (Exception ex)
		{
			await _unitOfWork.RollbackAsync();
			return Result<CompletarRegistroResult>.Failure("Error: " + ex.Message);
		}
	}
}
