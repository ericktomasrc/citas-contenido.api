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
                mensaje.Subject = "¡Bienvenido a email de prueba!";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <h2 style='color: #ec4899;'>¡Tu cuenta está lista!</h2>
                                <p>Hola {nombreUsuario},</p>
                                <p>Tu cuenta ha sido verificada exitosamente. Ya puedes comenzar a usar a email de prueba.</p>
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

        public async Task EnviarCodigoRecuperacionPasswordAsync(string emailDestino, string nombreUsuario, string codigo)
        {
            try
            {
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(_fromName, _fromEmail));
                mensaje.To.Add(new MailboxAddress(nombreUsuario, emailDestino));
                mensaje.Subject = "🔐 Recuperación de Contraseña - correo de prueba";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GenerarHtmlRecuperacionPassword(nombreUsuario, codigo)
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
                throw new Exception($"Error inesperado al enviar email de recuperación: {ex.Message}", ex);
            }
        }

        private static string GenerarHtmlRecuperacionPassword(string nombreUsuario, string codigo)
        {
            return $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <style>
                            body {{ font-family: 'Segoe UI', Arial, sans-serif; background-color: #f9fafb; margin: 0; padding: 0; }}
                            .container {{ max-width: 600px; margin: 40px auto; background: white; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
                            .header {{ background: linear-gradient(135deg, #ec4899 0%, #8b5cf6 100%); padding: 40px 20px; text-align: center; }}
                            .header h1 {{ color: white; margin: 0; font-size: 28px; }}
                            .content {{ padding: 40px 30px; text-align: center; }}
                            .code-box {{ background: linear-gradient(135deg, #fce7f3 0%, #ede9fe 100%); border: 2px dashed #ec4899; border-radius: 12px; padding: 30px; margin: 30px 0; }}
                            .code {{ font-size: 48px; font-weight: bold; color: #ec4899; letter-spacing: 8px; font-family: 'Courier New', monospace; }}
                            .warning {{ background: #fef3c7; border-left: 4px solid #f59e0b; padding: 15px; margin: 20px 0; text-align: left; border-radius: 4px; }}
                            .footer {{ background: #f9fafb; padding: 20px; text-align: center; color: #6b7280; font-size: 14px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>🔐 Recuperación de Contraseña</h1>
                            </div>
                            <div class='content'>
                                <h2>Hola {nombreUsuario},</h2>
                                <p style='font-size: 16px; color: #4b5563; margin: 20px 0;'>
                                    Recibimos una solicitud para restablecer tu contraseña. Usa el siguiente código:
                                </p>
            
                                <div class='code-box'>
                                    <div class='code'>{codigo}</div>
                                </div>

                                <p style='font-size: 14px; color: #6b7280;'>
                                    Este código expira en <strong>30 minutos</strong>
                                </p>

                                <div class='warning'>
                                    <strong>⚠️ Importante:</strong> Si no solicitaste este cambio, ignora este email. Tu cuenta está segura.
                                </div>

                                <p style='font-size: 14px; color: #6b7280; margin-top: 30px;'>
                                    ¿Necesitas ayuda? Contáctanos en soporte@citascontenido.com
                                </p>
                            </div>
                            <div class='footer'>
                                <p>© 2025 CitasContenido. Todos los derechos reservados.</p>
                                <p>Este es un email automático, por favor no respondas.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }
    }
}  