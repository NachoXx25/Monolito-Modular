using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.UserModels;

namespace Monolito_Modular.Application.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AuthService(ITokenService tokenService, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Login a un usuario en el sistema.
        /// </summary>
        /// <param name="login">Datos de login del usuario.</param>
        /// <returns>Token de acceso.</returns> 
        public async Task<string> Login(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email) ?? throw new Exception("Usuario o contraseña incorrectos.");

            var result = await _userManager.CheckPasswordAsync(user, login.Password);

            if(!result){

                var failedAttemps = await _userManager.GetAccessFailedCountAsync(user);
                await _userManager.AccessFailedAsync(user);
                var leftAttemps = 5 - failedAttemps;
                if (leftAttemps <= 1)
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(5));
                    throw new Exception("Usuario bloqueado por 5 minutos.");
                }
                throw new Exception($"Usuario o contraseña incorrectos. Te quedan {leftAttemps - 1 } intentos.");
            }
            var role = await _roleManager.FindByIdAsync(user.RoleId.ToString()) ?? throw new Exception("No se ha encontrado el rol del usuario.");
            await _userManager.ResetAccessFailedCountAsync(user);
            var token = await _tokenService.CreateToken(user, role);
            return token;
        }
    }
}