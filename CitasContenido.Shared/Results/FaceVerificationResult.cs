// Elimine las interfaces desconocidas para corregir CS0246
namespace CitasContenido.Shared.Results
{
    public class FaceVerificationResult_
    {
        public bool SonIguales { get; set; }
        public double Confianza { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
