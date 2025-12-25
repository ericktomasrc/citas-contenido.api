using CitasContenido.Backend.Domain.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _appUrl;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _smtpUser = configuration["Email:SmtpUser"]
                ?? throw new InvalidOperationException("Email:SmtpUser no configurado");
            _smtpPassword = configuration["Email:SmtpPassword"]
                ?? throw new InvalidOperationException("Email:SmtpPassword no configurado");
            _fromEmail = configuration["Email:FromEmail"] ?? _smtpUser;
            _fromName = configuration["Email:FromName"] ?? "CitasContenido";
            _appUrl = configuration["App:FrontendUrl"] ?? "http://localhost:5173";
        }

        public async Task EnviarEmailVerificacionAsync(string emailDestino, string token, string nombreUsuario)
        {
            try
            {
                var verificacionUrl = $"{_appUrl}/verificar-email?token={token}";

                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(_fromName, _fromEmail));
                mensaje.To.Add(new MailboxAddress(nombreUsuario, emailDestino));
                mensaje.Subject = "Verifica tu cuenta en pagina de prueba";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2 style='color: #ec4899;'>¡Bienvenido a pagina de prueba!</h2>
                                <p>Hola,</p>
                                <p>Gracias por registrarte. Para completar tu registro, por favor verifica tu correo electrónico haciendo clic en el botón de abajo:</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{verificacionUrl}' 
                                       style='background-color: #ec4899; color: white; padding: 12px 30px; 
                                              text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        Verificar mi cuenta
                                    </a>
                                </div>
                                <p>O copia y pega este enlace en tu navegador:</p>
                                <p style='word-break: break-all; color: #666;'>{verificacionUrl}</p>
                                <p style='margin-top: 30px; color: #999; font-size: 12px;'>
                                    Este enlace expirará en 24 horas.
                                </p>
                                <p style='color: #999; font-size: 12px;'>
                                    Si no creaste esta cuenta, ignora este correo.
                                </p>
                            </div>
                        </body>
                        </html>
                    "
                };

                mensaje.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPassword);
                await client.SendAsync(mensaje);
                await client.DisconnectAsync(true);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                throw new Exception($"Error de autenticación SMTP: Verifica las credenciales de email. {ex.Message}", ex);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex)
            {
                throw new Exception($"Error en comando SMTP: {ex.Message}", ex);
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException ex)
            {
                throw new Exception($"Error de protocolo SMTP: {ex.Message}", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw new Exception($"Error de conexión al servidor SMTP: Verifica host y puerto. {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al enviar email de verificación: {ex.Message}", ex);
            }
        }

        public async Task EnviarEmailBienvenidaAsync(string emailDestino, string nombreUsuario)
        {
            try
            {
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(_fromName, _fromEmail));
                mensaje.To.Add(new MailboxAddress(nombreUsuario, emailDestino));
                mensaje.Subject = "¡Bienvenido a CitasContenido!";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2 style='color: #ec4899;'>¡Tu cuenta está lista!</h2>
                                <p>Hola {nombreUsuario},</p>
                                <p>Tu cuenta ha sido verificada exitosamente. Ya puedes comenzar a usar CitasContenido.</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{_appUrl}/login' 
                                       style='background-color: #ec4899; color: white; padding: 12px 30px; 
                                              text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        Iniciar Sesión
                                    </a>
                                </div>
                                <p>¡Disfruta tu experiencia!</p>
                            </div>
                        </body>
                        </html>
                    "
                };

                mensaje.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPassword);
                await client.SendAsync(mensaje);
                await client.DisconnectAsync(true);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                throw new Exception($"Error de autenticación SMTP: Verifica las credenciales de email. {ex.Message}", ex);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex)
            {
                throw new Exception($"Error en comando SMTP: {ex.Message}", ex);
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException ex)
            {
                throw new Exception($"Error de protocolo SMTP: {ex.Message}", ex);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw new Exception($"Error de conexión al servidor SMTP: Verifica host y puerto. {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado al enviar email de bienvenida: {ex.Message}", ex);
            }
        }
    }
}