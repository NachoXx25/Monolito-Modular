using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.UserModels;

namespace Monolito_Modular.Application.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        /// <summary>
        /// Obtiene los datos de un usuario según su Id
        /// </summary>
        /// <param name="Id">Id del usuario</param>
        /// <returns>Datos del usuario</returns>
        public async Task<UserDTO> GetUserById(int Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString()) ?? throw new Exception("El usuario especificado no existe.");
            var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());
            var userDTO = new UserDTO(){
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = role?.Name ?? string.Empty,
                CreatedAt = TimeZoneInfo.ConvertTime(user.CreatedAt, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"))
            };
            return userDTO;
        }  
    }
}