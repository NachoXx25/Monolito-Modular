using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.UserModels;
using Monolito_Modular.Infrastructure.Data;
using Monolito_Modular.Infrastructure.Repositories.Interfaces;

namespace Monolito_Modular.Application.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<string, string> ErrorTranslations = new ()
        {
            {"DuplicateUserName", "El nombre de usuario ya está en uso"},
            {"DuplicateEmail", "El correo electrónico ya está registrado"},
            //{"InvalidUserName", "El nombre de usuario contiene caracteres inválidos"}
        };
        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IUserRepository userRepository, IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
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

        /// <summary>
        /// Hace un borrado lógico del usuario.
        /// </summary>
        /// <param name="Id">Id del usuario.</param>
        public async Task DeleteUser(int Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString()) ?? throw new Exception("El usuario especificado no existe.");
            await _userRepository.DeleteUser(user);
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="userDTO">Datos del nuevo usuario</param>
        /// <returns>Retorna el usuario menos su contraseña</returns>
        public async Task<ReturnUserDTO> CreateUser(CreateUserDTO userDTO)
        {
            try
            {
                var guid = new Guid();
                var role = await _roleManager.FindByNameAsync(userDTO.Role) ?? 
                    throw new Exception("El rol especificado no existe.");
                var user = new User(){
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    UserName = guid.ToString(),
                    RoleId = role.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = true
                };
                var result = await _userManager.CreateAsync(user, userDTO.Password);
                
                if (result.Succeeded)
                {
                    await _userRepository.CreateUserInAuthContext(userDTO);
                    return new ReturnUserDTO()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email ?? string.Empty,
                        RoleName = role.Name ?? string.Empty,
                        CreatedAt = TimeZoneInfo.ConvertTime(user.CreatedAt, 
                            TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                        UpdatedAt = TimeZoneInfo.ConvertTime(user.UpdatedAt, 
                            TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time")),
                        IsActive = user.Status
                    };
                }
                else
                {
                    throw new Exception(TranslateError(result.Errors.First()));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error en transacción: {ex.Message}");
                throw;
            }
        }

        public static string ReturnUserName(CreateUserDTO user)
        {
            return new string($"{user.FirstName}{user.LastName}".Where(c => char.IsLetterOrDigit(c)).ToArray());
        }
        

        public static string TranslateError(IdentityError error)
        {
            return ErrorTranslations.TryGetValue(error.Code, out string? translation) ? translation : error.Description;
        }
    }
}