using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Entities;

namespace CitasContenido.Backend.Domain.Repositories
{
    public interface IPasswordHistoryRepository
    {
        Task<long> CrearAsync(PasswordHistory data, IUnitOfWork unitOfWork);
    }
}
