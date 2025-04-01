using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monolito_Modular.Application.DTOs;

namespace Monolito_Modular.Application.Services.Interfaces
{
    public interface IAuthService
    {   
        /// <summary>
        /// Login a un usuario en el sistema.
        /// </summary>
        /// <param name="login">Datos de login del usuario.</param>
        /// <returns>Los datos del usuario.</returns>  
        Task<ReturnUserDTOWithToken> Login(LoginDTO login);

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="updatePasswordDTO">Contraseñas del usuario</param>
        /// <param name="Id">Id del usuario.</param>
        /// <returns>Mensaje de éxito o error.</returns>
        Task<string> UpdatePassword(UpdatePasswordDTO updatePasswordDTO, int Id);
    }
}