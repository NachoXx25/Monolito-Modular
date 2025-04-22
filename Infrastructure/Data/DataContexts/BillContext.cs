using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Domain.BillModel;
using Monolito_Modular.Domain.UserModels;

namespace Monolito_Modular.Infrastructure.Data.DataContexts
{
    public class BillContext : IdentityDbContext<User, Role, int>
    {
        public BillContext(DbContextOptions<BillContext> options) : base(options) { }

        public DbSet<Bill> Bills { get; set; }

        public DbSet<Status> Statuses { get; set; }
    }
}