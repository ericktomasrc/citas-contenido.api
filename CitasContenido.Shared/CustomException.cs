using System.Runtime.CompilerServices;

namespace Smr.Backend.Shared
{
    public class CustomException : Exception
    {

        public string? Titulo { get; set; }
        public List<string>? Errores { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }

        public CustomException(string titulo, Exception exception) : base(titulo, exception)
        {
            this.Titulo = titulo;
        }

        public CustomException(string titulo, List<string> errores, Exception exception) : base(titulo, exception)
        {
            this.Titulo = titulo;
            this.Errores = errores;
        }

        public static string GetErrorMessage(string message, [CallerMemberName] string methodName = "")
        {
            return $"Error en {methodName}: {message}";
        }

        public static string GetSuccessMessage([CallerMemberName] string methodName = "")
        {
            return $"Se ejecutó correctamente la función {methodName}";
        }
    }
}
