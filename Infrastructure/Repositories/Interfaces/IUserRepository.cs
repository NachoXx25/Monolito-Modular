using Monolito_Modular.Application.DTOs;
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

        /// <summary>
        /// Crea un nuevo usuario en el contexto de autenticación.
        /// </summary>
        /// <param name="user">usuario a crear</param>
        Task CreateUserInAuthContext(CreateUserDTO user);
    }
}