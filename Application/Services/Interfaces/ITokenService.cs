using Monolito_Modular.Domain.UserModels;

namespace Monolito_Modular.Application.Services.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Método que crea un token JWT para un usuario.
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="role">Rol del usuario</param>
        /// <returns>Token JWT</returns>
        Task<string> CreateToken(User user, Role role);
    }
}