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
        /// <returns>Token de acceso.</returns> 
        Task<string> Login(LoginDTO login);
    }
}