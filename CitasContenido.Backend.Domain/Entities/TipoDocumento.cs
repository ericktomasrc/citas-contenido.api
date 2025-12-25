namespace CitasContenido.Backend.Domain.Entities
{
    public class TipoDocumento
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Descripcion { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public string UsuarioCreacion { get; private set; }
        public DateTime? FechaModificacion { get; private set; }
        public string? UsuarioModificacion { get; private set; }
        public bool Habilitado { get; private set; }

        private TipoDocumento() { }

        public static TipoDocumento Create(string name, string descripcion, string usuarioCreacion)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre no puede estar vacío", nameof(name));

            if (name.Trim().Length < 2)
                throw new ArgumentException("El nombre debe tener al menos 2 caracteres", nameof(name));

            if (string.IsNullOrWhiteSpace(usuarioCreacion))
                throw new ArgumentException("El usuario de creación es requerido", nameof(usuarioCreacion));

            return new TipoDocumento
            {
                Name = name.Trim().ToUpper(),
                Descripcion = descripcion?.Trim() ?? string.Empty,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = usuarioCreacion.Trim(),
                Habilitado = true
            };
        }

        public void Actualizar(string name, string descripcion, string usuarioModificacion)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre no puede estar vacío", nameof(name));

            if (name.Trim().Length < 2)
                throw new ArgumentException("El nombre debe tener al menos 2 caracteres", nameof(name));

            if (string.IsNullOrWhiteSpace(usuarioModificacion))
                throw new ArgumentException("El usuario de modificación es requerido", nameof(usuarioModificacion));

            if (!Habilitado)
                throw new InvalidOperationException("No se puede actualizar un tipo de documento deshabilitado");

            Name = name.Trim().ToUpper();
            Descripcion = descripcion?.Trim() ?? string.Empty;
            FechaModificacion = DateTime.UtcNow;
            UsuarioModificacion = usuarioModificacion.Trim();
        }

        public void Habilitar(string usuarioModificacion)
        {
            if (string.IsNullOrWhiteSpace(usuarioModificacion))
                throw new ArgumentException("El usuario de modificación es requerido", nameof(usuarioModificacion));

            if (Habilitado) return;

            Habilitado = true;
            FechaModificacion = DateTime.UtcNow;
            UsuarioModificacion = usuarioModificacion.Trim();
        }

        public void Deshabilitar(string usuarioModificacion)
        {
            if (string.IsNullOrWhiteSpace(usuarioModificacion))
                throw new ArgumentException("El usuario de modificación es requerido", nameof(usuarioModificacion));

            var horasDesdeCreacion = (DateTime.UtcNow - FechaCreacion).TotalHours;
            if (horasDesdeCreacion < 24)
                throw new InvalidOperationException("No se puede deshabilitar un tipo de documento creado hace menos de 24 horas");

            Habilitado = false;
            FechaModificacion = DateTime.UtcNow;
            UsuarioModificacion = usuarioModificacion.Trim();
        }

        public bool PuedeSerEliminado() => !Habilitado;
    }

}
