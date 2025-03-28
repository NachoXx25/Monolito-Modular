using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Domain.BillModel;
using Monolito_Modular.Domain.UserModels;
using Monolito_Modular.Infrastructure.Data.DataContexts;

namespace Monolito_Modular.Infrastructure.Data.DataSeeders
{
    public class DataSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userContext = scope.ServiceProvider.GetRequiredService<UserContext>();
                var authContext = scope.ServiceProvider.GetRequiredService<AuthContext>();
                var billContext = scope.ServiceProvider.GetRequiredService<BillContext>();

                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                try
                {
                    await userContext.Database.MigrateAsync();
                    await authContext.Database.MigrateAsync();
                    await billContext.Database.MigrateAsync();
                    var roles = new[] { "Administrador", "Cliente" };
                    foreach(var roleName in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            var role = new Role { Name = roleName, NormalizedName = roleName.ToUpper() };
                            await roleManager.CreateAsync(role);

                            var createdRole = await userContext.Roles.FindAsync(role.Id);
                            if(createdRole != null)
                            {
                                var existsInAuthContext = await authContext.Roles.AnyAsync( r => r.Name == roleName);
                                if(!existsInAuthContext)
                                {
                                    authContext.Roles.Add(new Role { Id = createdRole.Id, Name = roleName, NormalizedName = roleName.ToUpper() });
                                    await authContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                
                    var statuses = new[] { "Pendiente", "Pagado", "Vencido" };
                    
                    if(!await billContext.Statuses.AnyAsync())
                    {
                        foreach(var status in statuses)
                        {
                            billContext.Statuses.Add(new Status { Name = status });
                        }
                        await billContext.SaveChangesAsync();
                    }
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Un error ha ocurrido mientras se cargaban los seeders");
                    throw;
                }
            }
        }
    }
}