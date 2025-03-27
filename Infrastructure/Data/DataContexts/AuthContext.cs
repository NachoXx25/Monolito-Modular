using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Domain.MySQLModels;

namespace Monolito_Modular.Infrastructure.Data
{
    public class AuthContext : IdentityDbContext<User, Role, int>
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) {}
    }
}