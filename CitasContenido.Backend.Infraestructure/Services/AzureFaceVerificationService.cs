using Azure;
using Azure.AI.Vision.Face;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Shared.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CitasContenido.Backend.Infraestructure.Services
{
    public class AzureFaceVerificationService : IFaceVerificationService
    {
        private readonly FaceClient _faceClient;
        private readonly ILogger<AzureFaceVerificationService> _logger;

        public AzureFaceVerificationService(
            IConfiguration configuration,
            ILogger<AzureFaceVerificationService> logger)
        {
            _logger = logger;

            var subscriptionKey = configuration["AzureFace:SubscriptionKey"]
                ?? throw new ArgumentNullException("AzureFace:SubscriptionKey no configurado");

            var endpoint = configuration["AzureFace:Endpoint"]
                ?? throw new ArgumentNullException("AzureFace:Endpoint no configurado");

            _faceClient = new FaceClient(new Uri(endpoint), new AzureKeyCredential(subscriptionKey));
        }

        public async Task<FaceVerificationResult_> CompararRostrosAsync(Stream foto1, Stream foto2)
        {
            try
            {
                // Resetear posición de streams
                foto1.Position = 0;
                foto2.Position = 0;

                // Detectar rostros en ambas fotos
                var facesResponse1 = await _faceClient.DetectAsync(
                    BinaryData.FromStream(foto1),
                    FaceDetectionModel.Detection03,
                    FaceRecognitionModel.Recognition04,
                    returnFaceId: true
                );
                var faces1 = facesResponse1.Value;

                var facesResponse2 = await _faceClient.DetectAsync(
                  BinaryData.FromStream(foto2),
                  FaceDetectionModel.Detection03,
                  FaceRecognitionModel.Recognition04,
                  returnFaceId: true
              );
                var faces2 = facesResponse1.Value;

                // Validar que se detectó exactamente 1 rostro en cada foto
                if (faces1.Count == 0)
                {
                    return new FaceVerificationResult_
                    {
                        SonIguales = false,
                        Confianza = 0,
                        Mensaje = "No se detectó ningún rostro en la primera foto"
                    };
                }

                if (faces1.Count > 1)
                {
                    return new FaceVerificationResult_
                    {
                        SonIguales = false,
                        Confianza = 0,
                        Mensaje = "Se detectaron múltiples rostros en la primera foto. Solo debe haber uno"
                    };
                }

                if (faces2.Count == 0)
                {
                    return new FaceVerificationResult_
                    {
                        SonIguales = false,
                        Confianza = 0,
                        Mensaje = "No se detectó ningún rostro en la segunda foto"
                    };
                }

                if (faces2.Count > 1)
                {
                    return new FaceVerificationResult_
                    {
                        SonIguales = false,
                        Confianza = 0,
                        Mensaje = "Se detectaron múltiples rostros en la segunda foto. Solo debe haber uno"
                    };
                }

                // Verificar similitud entre los dos rostros
                var verifyResult = await _faceClient.VerifyFaceToFaceAsync(
                    faces1[0].FaceId.Value,
                    faces2[0].FaceId.Value
                );

                // Umbral de confianza: 70%
                const double umbralConfianza = 0.7;

                return new FaceVerificationResult_
                {
                    SonIguales = verifyResult.Value.IsIdentical && verifyResult.Value.Confidence >= umbralConfianza,
                    Confianza = verifyResult.Value.Confidence,
                    Mensaje = verifyResult.Value.IsIdentical && verifyResult.Value.Confidence >= umbralConfianza
                           ? "Verificación exitosa: Los rostros coinciden"
                           : $"Los rostros no coinciden (confianza: {verifyResult.Value.Confidence:P0})"
                };
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error de Azure Face API: {Message}", ex.Message);
                return new FaceVerificationResult_
                {
                    SonIguales = false,
                    Confianza = 0,
                    Mensaje = $"Error al procesar las imágenes: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en verificación facial");
                return new FaceVerificationResult_
                {
                    SonIguales = false,
                    Confianza = 0,
                    Mensaje = "Error al verificar las imágenes"
                };
            }
        }

        public async Task<bool> DetectarRostroAsync(Stream foto)
        {
            try
            {
                foto.Position = 0;

                var facesResponse = await _faceClient.DetectAsync(
                    BinaryData.FromStream(foto),
                    FaceDetectionModel.Detection03,
                    FaceRecognitionModel.Recognition04,
                    returnFaceId: true
                );
                var faces = facesResponse.Value;

                return faces.Count == 1; // Solo debe haber exactamente 1 rostro
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al detectar rostro");
                return false;
            }
        }
    }
}
