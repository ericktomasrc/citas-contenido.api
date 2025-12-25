namespace CitasContenido.Backend.Domain.DTOs
{
    public class TipoDocumentoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public bool Habilitado { get; set; }
    }
}
