using CitasContenido.Shared.Results;

namespace CitasContenido.Backend.Domain.Services
{
    public interface IFaceVerificationService
    {
        Task<FaceVerificationResult_> CompararRostrosAsync(Stream foto1, Stream foto2);
        Task<bool> DetectarRostroAsync(Stream foto);
    }
}
