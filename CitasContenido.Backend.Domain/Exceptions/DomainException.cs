namespace CitasContenido.Backend.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Se lanza cuando no se encuentra una entidad
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityName, object key)
            : base($"Entidad '{entityName}' con identificador '{key}' no fue encontrada")
        {
        }
    }

    /// <summary>
    /// Se lanza cuando ya existe una entidad
    /// </summary>
    public class EntityAlreadyExistsException : DomainException
    {
        public EntityAlreadyExistsException(string entityName, string field, object value)
            : base($"Ya existe una entidad '{entityName}' con {field} = '{value}'")
        {
        }
    }

    /// <summary>
    /// Se lanza cuando una regla de negocio es violada
    /// </summary>
    public class BusinessRuleValidationException : DomainException
    {
        public BusinessRuleValidationException(string message)
            : base(message)
        {
        }
    }
}
