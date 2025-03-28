using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Domain.UserModels;
using MySql.Data.MySqlClient;
using Npgsql;

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

                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                try
                {
                     try {
                        await userContext.Database.MigrateAsync();
                    }
                        catch (PostgresException ex) when (ex.SqlState == "42P07") {
                            logger.LogWarning("Algunas tablas ya existen en la base de datos PostgreSQL: {Message}", ex.Message);
                    }
                    
                    try {
                        await authContext.Database.MigrateAsync();
                    }
                        catch (MySqlException ex) when (ex.Message.Contains("already exists")) {
                            logger.LogWarning("Algunas tablas ya existen en la base de datos MySQL: {Message}", ex.Message);
                    }
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