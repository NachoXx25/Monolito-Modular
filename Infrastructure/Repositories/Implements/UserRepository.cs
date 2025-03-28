using System.Transactions;
using Monolito_Modular.Domain.UserModels;
using Monolito_Modular.Infrastructure.Data;
using Monolito_Modular.Infrastructure.Repositories.Interfaces;
using Serilog.Core;

namespace Monolito_Modular.Infrastructure.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _userContext;
        private readonly AuthContext _authContext;

        public UserRepository(UserContext userContext, AuthContext authContext)
        {
            _userContext = userContext;
            _authContext = authContext;
        }

        /// <summary>
        /// Hace un borrado logico del usuario
        /// </summary>
        /// <param name="user">Usuario a borrar.</param>
        public async Task DeleteUser(User user)
        {
            using(var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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
                    
                    scope.Complete();
                }
                catch(Exception ex)
                {
                    throw new Exception("Error al procesar la operación", ex);
                }
            }
        }
    }
}