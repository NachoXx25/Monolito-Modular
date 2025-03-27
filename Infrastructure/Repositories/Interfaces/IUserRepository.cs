using Monolito_Modular.Domain.UserModels;

namespace Monolito_Modular.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Hace un borrado logico del usuario
        /// </summary>
        /// <param name="user">Usuario a borrar.</param>
        Task DeleteUser(User user);
    }
}