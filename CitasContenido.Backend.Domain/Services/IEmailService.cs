namespace CitasContenido.Backend.Domain.Services
{
    public interface IEmailService
    {
        Task EnviarEmailVerificacionAsync(string emailDestino, string token, string nombreUsuario);
        Task EnviarEmailBienvenidaAsync(string emailDestino, string nombreUsuario);
    }
}
