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
        /// <returns>Los datos del usuario.</returns> 
        public async Task<ReturnUserDTOWithToken> Login(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email) ?? throw new Exception("Usuario o contraseña incorrectos.");
            if(!user.Status) throw new Exception("Su cuenta ha sido inhabilitada, no puede iniciar sesión.");
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
            var ReturnUserDTO = new ReturnUserDTOWithToken(){
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                RoleName = role.Name ?? string.Empty,
                CreatedAt = TimeZoneInfo.ConvertTime(user.CreatedAt, 
                            TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                        UpdatedAt = TimeZoneInfo.ConvertTime(user.UpdatedAt, 
                            TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                        IsActive = user.Status,
                Token = token
            };
            return ReturnUserDTO;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="updatePasswordDTO">Contraseñas del usuario</param>
        /// <param name="Id">Id del usuario.</param>
        /// <returns>Mensaje de éxito o error.</returns>
        public async Task<string> UpdatePassword(UpdatePasswordDTO updatePasswordDTO, int Id)
        {
            var user = _userManager.FindByIdAsync(Id.ToString()).Result ?? throw new Exception("Usuario no encontrado.");
            var result = await _userManager.CheckPasswordAsync(user, updatePasswordDTO.CurrentPassword);
            if(!result) throw new Exception("La contraseña actual es incorrecta.");
            if(updatePasswordDTO.CurrentPassword == updatePasswordDTO.NewPassword) throw new Exception("La contraseña nueva debe ser diferente a la actual.");
            var change = await _userManager.ChangePasswordAsync(user, updatePasswordDTO.CurrentPassword, updatePasswordDTO.NewPassword);
            if(change.Succeeded)
            {
                return "Contraseña actualizada con éxito.";
            }
            else
            {
                throw new Exception("Error al actualizar la contraseña.");
            }
        }
    }
}