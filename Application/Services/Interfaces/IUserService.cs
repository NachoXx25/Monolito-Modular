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

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="userDTO">Datos del nuevo usuario</param>
        /// <returns>Retorna el usuario menos su contraseña</returns>
        Task<ReturnUserDTO> CreateUser(CreateUserDTO userDTO);

        /// <summary>
        /// Hace un borrado lógico del usuario.
        /// </summary>
        /// <param name="Id">Id del usuario.</param>
        Task DeleteUser(int Id);

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        Task<IEnumerable<ReturnUserDTO>> GetAllUsers();
    }
}