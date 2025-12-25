using CitasContenido.Shared.Results;
namespace CitasContenido.Backend.Domain.Services
{
    public interface ICompletarRegistroDomainService
    {
        Task<Result<CompletarRegistroResult>> CompletarRegistroAsync(
            long usuarioId,
            int tipoUsuarioId,
            string username,
            string nombre,
            string apellidos,
            DateTime fechaNacimiento,
            int generoId,
            string password,
            decimal latitud,
            decimal longitud,
            Stream? fotoDocumentoStream,
            string fotoDocumentoNombre,
            Stream? fotoEnVivoStream,
            string fotoEnVivoNombre,
            string? codigoQuienRecomendo, int? generoQueMeInteresaId,
            int? tipoDocumentoId = null,
            string? numeroDocumento = null,
            string? nacionalidad = null,
            string? whatsapp = null,
            string? numeroYape = null,
            string? numeroPlin = null,
            string? bancoNombre = null,
            string? numeroCuenta = null,
            string? bio = null);
    }

    public class CompletarRegistroResult
    {
        public long UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
