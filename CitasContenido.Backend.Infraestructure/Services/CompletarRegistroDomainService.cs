using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Enum;
using CitasContenido.Shared.Results;
using Microsoft.Extensions.Configuration;
using Smr.Backend.Shared;

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
    private readonly IGeocodingService _geocodingService;
	private readonly int DuracionToken = 0;

    public CompletarRegistroDomainService(IUnitOfWork unitOfWork, 
		IUsuarioRepository usuarioRepository,
		IContenidoArchivoRepository contenidoArchivoRepository, 
		ITipoContenidoRepository tipoContenidoRepository, 
		IVerificacionIdentidadRepository verificacionIdentidadRepository,
		IRefreshTokenRepository refreshTokenRepository, IJwtService jwtService, 
		IPasswordHasher passwordHasher, 
		IAzureStorageService azureStorageService, 
		IEmailService emailService, 
		IFaceVerificationService faceVerificationService,
        IGeocodingService geocodingService,
        IConfiguration configuration)
	{
        DuracionToken = int.Parse(configuration["Configuracion:DuracionToken"]
               ?? "0");

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
        _geocodingService= geocodingService;
    }

	public async Task<Result<CompletarRegistroResult>> CompletarRegistroAsync(long usuarioId, int tipoUsuarioId,
		string username, string nombre, string apellidos, DateTime fechaNacimiento, int generoId, 
		string password, decimal latitud, decimal longitud, Stream? fotoDocumentoStream, 
		string fotoDocumentoNombre, Stream? fotoEnVivoStream, string fotoEnVivoNombre, string? codigoQuienRecomendo, int? generoQueMeInteresaId, 
		int? tipoDocumentoId = null, string? numeroDocumento = null, string? nacionalidad = null,
		string? whatsapp = null, string? numeroYape = null, string? numeroPlin = null, string? bancoNombre = null,
		string? numeroCuenta = null, string? bio = null)
	{
		try
		{
			await _unitOfWork.BeginTransactionAsync();
			Usuario? usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);

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

			var x = await _geocodingService.ObtenerUbicacionPorCoordenadas(latitud, longitud);
			string pais = x.Pais ?? Constantes.DESCONOCIDO_PAIS;
			string departamento = x.Departamento ?? Constantes.DESCONOCIDO_DEPARTAMENTO;
			string provincia = x.Provincia ?? Constantes.DESCONOCIDO_PROVINCIA;
			string distrito = x.Distrito ?? Constantes.DESCONOCIDO_DISTRITO;
			string ciudad = x.Ciudad ?? Constantes.DESCONOCIDO_CIUDAD;
			string direccionCompleta = x.DireccionCompleta ?? Constantes.DESCONOCIDO_DIRECCION_COMPLETA;

			//         if (fotoPerfilStream != null && fotoEnVivoStream != null)
			//{
			//	FaceVerificationResult_ verificacionSiLasFotosTienenSimilitud = await _faceVerificationService.CompararRostrosAsync(fotoPerfilStream, fotoEnVivoStream);
			//	if (!verificacionSiLasFotosTienenSimilitud.SonIguales)
			//	{
			//		return Result<CompletarRegistroResult>.Failure("Verificación facial fallida: " + verificacionSiLasFotosTienenSimilitud.Mensaje);
			//	}
			//}

			string? UrlfotoEnVivo = "xx";//string.Empty;
			string? urlFotoDocumento = "yy";// string.Empty;

			//if (fotoDocumentoStream != null)
			//{
			//	string extension = Path.GetExtension(fotoDocumentoNombre);
   //             urlFotoDocumento = await _azureStorageService.SubirArchivoAsync(fotoDocumentoStream, fotoDocumentoNombre, extension);
			//}

			//if (fotoEnVivoStream != null)
			//{
			//	string extension2 = Path.GetExtension(fotoEnVivoNombre);
			//	UrlfotoEnVivo = await _azureStorageService.SubirArchivoAsync(fotoEnVivoStream, fotoEnVivoNombre, extension2);
			//}

			string passwordHash = _passwordHasher.HashPassword(password);

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
                UrlfotoEnVivo,
                urlFotoDocumento,
                tipoDocumentoId,
				numeroDocumento, 
				nacionalidad, 
				pais,
				departamento,
				provincia
				,distrito,
				ciudad,
				direccionCompleta,
				codigoQuienRecomendo,
				generoQueMeInteresaId
				);

			usuario.ActualizarRegistroCompletado(true);

            await _usuarioRepository.ActualizarAsync(usuario, _unitOfWork);            

            if (!string.IsNullOrEmpty(urlFotoDocumento))
			{
				TipoContenido? tipoFotoVerfica = await _tipoContenidoRepository.ObtenerPorIdAsync((int)EnumTipoContenido.FotoVerificacionIdentidad);
				if (tipoFotoVerfica != null && fotoDocumentoStream != null)
				{
					ContenidoArchivo contenido = ContenidoArchivo.Crear(usuarioId, tipoFotoVerfica.Id, 1,
                        fotoDocumentoNombre, fotoDocumentoStream.Length, Path.GetExtension(fotoDocumentoNombre), 
						Guid.NewGuid().ToString(), urlFotoDocumento, Constantes.BLOB_STORAGE_CONTAINERNAME, false, true, 1, Constantes.SYSTEM);

					await _contenidoArchivoRepository.CrearAsync(contenido, _unitOfWork);
				}
			}           

            if (!string.IsNullOrEmpty(UrlfotoEnVivo) )
			{
				TipoContenido? tipoFotoVivo = await _tipoContenidoRepository.ObtenerPorIdAsync((int)EnumTipoContenido.FotoVerificacionEnVivo);
				if (tipoFotoVivo != null && fotoEnVivoStream != null)
				{
					ContenidoArchivo contenido2 = ContenidoArchivo.Crear(usuarioId, tipoFotoVivo.Id, 1,
						fotoEnVivoNombre, fotoEnVivoStream.Length, Path.GetExtension(fotoEnVivoNombre),
						Guid.NewGuid().ToString(), UrlfotoEnVivo, Constantes.BLOB_STORAGE_CONTAINERNAME, false, false, 1, Constantes.SYSTEM);

					await _contenidoArchivoRepository.CrearAsync(contenido2, _unitOfWork);
				}
			}
			VerificacionIdentidad verificacion = VerificacionIdentidad.Crear(usuarioId, UrlfotoEnVivo ?? "", urlFotoDocumento ?? "" );
			await _verificacionIdentidadRepository.CrearAsync(verificacion, _unitOfWork);

			string token = _jwtService.GenerarToken(usuario.NGuid, usuario.Email);
			RefrescarToken refreshToken = RefrescarToken.Crear(usuario.Id, token, DuracionToken);

			await _refreshTokenRepository.CrearAsync(refreshToken, _unitOfWork);			

			try
			{
				await _emailService.EnviarEmailBienvenidaAsync(usuario.Email, usuario.Nombre ?? string.Empty);
			}
			catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result<CompletarRegistroResult>.Failure("Error: " + ex.Message);
            }

            await _unitOfWork.CommitAsync();

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
