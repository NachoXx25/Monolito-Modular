using Monolito_Modular.Application.DTOs;

namespace Monolito_Modular.Application.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Obtiene los datos de un usuario según su Id
        /// </summary>
        /// <param name="Id">Id del usuario</param>
        /// <returns>Datos del usuario</returns>
        Task<UserDTO> GetUserById(int Id);
    }
}