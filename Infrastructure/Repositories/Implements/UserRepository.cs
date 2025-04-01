using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Domain.UserModels;
using Monolito_Modular.Infrastructure.Data;
using Monolito_Modular.Infrastructure.Data.DataContexts;
using Monolito_Modular.Infrastructure.Repositories.Interfaces;
using Serilog.Core;

namespace Monolito_Modular.Infrastructure.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _userContext;
        private readonly AuthContext _authContext;
        private readonly BillContext _billContext; 

        public UserRepository(UserContext userContext, AuthContext authContext, BillContext billContext)
        {
            _userContext = userContext;
            _authContext = authContext;
            _billContext = billContext;
        }

        /// <summary>
        /// Crea un nuevo usuario en el contexto de autenticación.
        /// </summary>
        /// <param name="user">usuario a crear</param>
        public async Task CreateUserInAuthContext(CreateUsersInOtherContextsDTO user)
        {
            var role = await _authContext.Roles.AsNoTracking().FirstOrDefaultAsync( r => r.Name == user.Role) ?? throw new Exception("El rol especificado no existe.");
            var newUser = new User
            {
                Id = user.Id,
                UserName =  user.Guid,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = role.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = true
            };
            await _authContext.Users.AddAsync(newUser);
            await _authContext.SaveChangesAsync();
        }

        /// <summary>
        /// Crea un nuevo usuario en el contexto de facturación.
        /// </summary>
        /// <param name="user">usuario a crear</param>¨
        public async Task CreateUserInBillContext(CreateUsersInOtherContextsDTO user)
        {
            var role = _userContext.Roles.AsNoTracking().FirstOrDefault( r => r.Name == user.Role) ?? throw new Exception("El rol especificado no existe.");
            var newUser = new User
            {
                Id = user.Id,
                UserName =  user.Guid,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = role.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = true
            };
            await _billContext.Users.AddAsync(newUser);
            await _billContext.SaveChangesAsync();
        }

        /// <summary>
        /// Hace un borrado logico del usuario
        /// </summary>
        /// <param name="user">Usuario a borrar.</param>
        public async Task DeleteUser(User user)
        {
            try
            {
                user.Status = false;
                
                await _userContext.SaveChangesAsync();
                
                
                var authUser = await _authContext.Users.FindAsync(user.Id);
                if (authUser != null)
                {
                    authUser.Status = false;
                    await _authContext.SaveChangesAsync();
                }
                var billUser = await _billContext.Users.FindAsync(user.Id);
                if (billUser != null)
                {
                    billUser.Status = false;
                    await _billContext.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error al procesar la operación", ex);
            }
        }

        /// <summary>
        /// Edita algunos parámetros del usuario
        /// </summary>
        /// <param name="updateUser">Atributos a editar</param>
        /// <param name="Id">Id del usuario a editar</param>
        public async Task UpdateUserInOtherContexts(UpdateUserDTO updateUser, int Id)
        {
            var authUser = await _authContext.Users.FindAsync(Id) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde");
            var billUser = await _billContext.Users.FindAsync(Id) ?? throw new Exception("Error en el sistema, vuelva a intentarlo más tarde");

            if(!string.IsNullOrWhiteSpace(updateUser.Email) && updateUser.Email != authUser.Email)
            {
                authUser.Email = updateUser.Email;
            }
            if(!string.IsNullOrWhiteSpace(updateUser.FirstName) && updateUser.FirstName != authUser.FirstName)
            {
                authUser.FirstName = updateUser.FirstName;

            }
            if(!string.IsNullOrWhiteSpace(updateUser.LastName) && updateUser.LastName != authUser.LastName)
            {
                authUser.LastName = updateUser.LastName;
            }
            if(!string.IsNullOrWhiteSpace(updateUser.Email) && updateUser.Email != billUser.Email)
            {
                billUser.Email = updateUser.Email;
            }
            if(!string.IsNullOrWhiteSpace(updateUser.FirstName) && updateUser.FirstName != billUser.FirstName)
            {
                billUser.FirstName = updateUser.FirstName;

            }
            if(!string.IsNullOrWhiteSpace(updateUser.LastName) && updateUser.LastName != billUser.LastName)
            {
                billUser.LastName = updateUser.LastName;
            }
            authUser.UpdatedAt = DateTime.UtcNow;
            billUser.UpdatedAt = DateTime.UtcNow;
            _authContext.Users.Update(authUser);
            await _authContext.SaveChangesAsync();
            _billContext.Users.Update(billUser);
            await _billContext.SaveChangesAsync();
        }
    }
}