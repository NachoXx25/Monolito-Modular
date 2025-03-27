using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Domain.MySQLModels;

namespace Monolito_Modular.Infrastructure.Data
{
    public class UserContext : IdentityDbContext<User, Role, int>
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) {}
    }
}