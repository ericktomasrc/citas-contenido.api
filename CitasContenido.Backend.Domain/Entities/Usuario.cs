namespace CitasContenido.Backend.Domain.Entities
{
    public class Usuario
    {
        public long Id { get; private set; }
        public Guid NGuid { get; private set; }
        public int TipoUsuarioId { get; private set; } // 1: Consumidor, 2: Creador
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string? Nombre { get; private set; }
        public string? Apellidos { get; private set; }
        public int? Edad { get; private set; }
        public int? GeneroId { get; private set; }
        public int? GeneroQueMeInteresaId1 { get; private set; }
        public int? GeneroQueMeInteresaId2 { get; private set; }
        public int? GeneroQueMeInteresaId3 { get; private set; }
        public string? CodigoQuienRecomendo { get; private set; } = string.Empty;
        public int? TipoDocumentoId { get; private set; }
        public string? NumeroDocumento { get; private set; }
        public string? Nacionalidad { get; private set; }

        // Verificaciones
        public bool EmailVerificado { get; private set; }
        public bool IdentidadVerificada { get; private set; }

        // Ubicación
        public decimal? Latitud { get; private set; }
        public decimal? Longitud { get; private set; } 
        public string? Pais { get; private set; }
        public string? Departamento { get; private set; }
        public string? Provincia { get; private set; }
        public string? Distrito { get; private set; }
        public string? Ciudad { get; private set; }
        public string? DireccionCompleta { get; private set; }
        public int RangoDistanciaKm { get; private set; }

        // Fotos
        public string? FotoDocumento { get; private set; }
        public string? FotoEnVivo { get; private set; }
        public string? UserName { get; private set; }        

        // Premium
        public bool IsPremium { get; private set; }

        // Actividad
        public DateTime UltimaActividad { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateTime FechaActualizacion { get; private set; }
        public DateTime FechaNacimiento { get; private set; }
        public bool Habilitado { get; private set; }
        public bool RegistroCompletado { get; private set; }
        public Guid SecurityStamp { get; private set; }

        // Constructor privado para EF
        private Usuario() { }

        // Factory method - Crear usuario con solo email (paso 1)
        public static Usuario CrearConEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es requerido", nameof(email));

            return new Usuario
            {
                NGuid = Guid.NewGuid(),
                Email = email.ToLower().Trim(),
                PasswordHash = string.Empty, // Se establecerá después
                EmailVerificado = false,
                IdentidadVerificada = false,
                IsPremium = false,
                TipoUsuarioId = 1,
                RangoDistanciaKm = 50,
                UltimaActividad = DateTime.UtcNow,
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid(),
                Habilitado = true
            };
        }

        // Completar registro (paso 3)
        public void CompletarRegistro(
            string username,
            string nombre,
            string apellidos,
            string passwordHash,
            DateTime fechaNacimiento,
            int generoId,
            int tipoUsuarioId,
            decimal latitud,
            decimal longitud,
            string? fotoEnVivo,
            string? fotoDocumento,
            int? tipoDocumentoId,
            string? numeroDocumento,
            string? nacionalidad,
            string? pais,
            string? departamento,
            string? provincia,
            string? distrito,
            string? ciudad,
            string? direccionCompleta,
            string? codigoQuienRecomendo = null,
            int? generoQueMeInteresaId = null
            )
        {
            if (RegistroCompletado)
                throw new InvalidOperationException("El registro ya está completado");
            UserName = username;
            Nombre = nombre;
            Apellidos = apellidos;
            PasswordHash = passwordHash;
            FechaNacimiento = fechaNacimiento;
            GeneroId = generoId;
            TipoUsuarioId = tipoUsuarioId;
            Latitud = latitud;
            Longitud = longitud;
            FotoEnVivo = fotoEnVivo;
            FotoDocumento = fotoDocumento;
            NumeroDocumento = numeroDocumento;
            Nacionalidad = nacionalidad;
            Pais = pais;
            Departamento = departamento;
            Provincia = provincia;
            Distrito = distrito;
            Ciudad = ciudad;
            DireccionCompleta = direccionCompleta;
            RegistroCompletado = true;
            UltimaActividad = DateTime.UtcNow;
            CodigoQuienRecomendo = codigoQuienRecomendo;
            GeneroQueMeInteresaId1 = generoQueMeInteresaId;
        }

        // Verificar email
        public void VerificarEmail()
        {
            EmailVerificado = true;
            FechaActualizacion = DateTime.UtcNow;
        }

        // Establecer ubicación
        public void EstablecerUbicacion(decimal latitud, decimal longitud)
        {
            Latitud = latitud;
            Longitud = longitud;
            FechaActualizacion = DateTime.UtcNow;
        }

        // Establecer fotos
        public void EstablecerFotos(string? fotoDocumento, string fotoEnVivo)
        {
            FotoDocumento = fotoDocumento;
            FotoEnVivo = fotoEnVivo;
            FechaActualizacion = DateTime.UtcNow;
        }

        // Verificar identidad
        public void VerificarIdentidad()
        {
            IdentidadVerificada = true;
            FechaActualizacion = DateTime.UtcNow;
        }

        // Actualizar actividad
        public void ActualizarActividad()
        {
            UltimaActividad = DateTime.UtcNow;
        }

        // Verificar si está inactivo (30 minutos)
        public bool EstaInactivo(int minutosInactividad = 30)
        {
            return DateTime.UtcNow - UltimaActividad > TimeSpan.FromMinutes(minutosInactividad);
        }
        // actualizar id
        public void ActualizarId(long id)
        {
            Id = id;
        }
        public void ActualizarTipoUsuarioId(int tipoUsuarioId)
        {
            TipoUsuarioId = tipoUsuarioId;
        }
        public void ActualizarRegistroCompletado(bool band)
        {
            RegistroCompletado = band;
        }

        public void CambiarContrasena(string nuevaPasswordHash)
        {
            PasswordHash = nuevaPasswordHash;
            SecurityStamp = Guid.NewGuid(); // Invalida todos los tokens JWT
            FechaActualizacion = DateTime.UtcNow;
        }
    }
}