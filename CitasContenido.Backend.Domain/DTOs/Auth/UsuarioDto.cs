namespace CitasContenido.Backend.Domain.DTOs.Auth
{
    public class UsuarioDto
    {
        public long Id{ get; set; }
        public Guid NGuid { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? Edad { get; set; }
        public int? GeneroId { get; set; }
        public int? TipoDocumentoId { get; set; }
        public string? NumeroDocumento { get; set; }
        public string? Nacionalidad { get; set; }
        public bool EmailVerificado { get; set; }
        public bool IdentidadVerificada { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public int RangoDistanciaKm { get; set; }
        public string? FotoPerfil { get; set; }
        public string? FotoEnVivo { get; set; }
        public bool IsPremium { get; set; }
        public DateTime UltimaActividad { get; set; }
    }
}
