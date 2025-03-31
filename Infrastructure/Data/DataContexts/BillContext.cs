using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Domain.BillModel;

namespace Monolito_Modular.Infrastructure.Data.DataContexts
{
    public class BillContext : DbContext
    {
        public BillContext(DbContextOptions<BillContext> options) : base(options) { }

        public DbSet<Bill> Bills { get; set; }

        public DbSet<Status> Statuses { get; set; }
    }
}