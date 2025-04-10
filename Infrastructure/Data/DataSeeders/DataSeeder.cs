using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Domain.BillModel;
using Monolito_Modular.Domain.UserModels;
using MySql.Data.MySqlClient;
using Npgsql;
using Monolito_Modular.Infrastructure.Data.DataContexts;
using Bogus;
using Monolito_Modular.Domain.VideoModel;

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
                var videoContext = scope.ServiceProvider.GetRequiredService<VideoContext>();

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
                    try {
                        await billContext.Database.MigrateAsync();
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
                                    await authContext.Roles.AddAsync(new Role { Id = createdRole.Id, Name = roleName, NormalizedName = roleName.ToUpper() });
                                    await authContext.SaveChangesAsync();
                                }
                                var existsInBillContext = await billContext.Roles.AnyAsync( r => r.Name == roleName);
                                if(!existsInBillContext)
                                {
                                    await billContext.Roles.AddAsync(new Role { Id = createdRole.Id, Name = roleName, NormalizedName = roleName.ToUpper() });
                                    await billContext.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    if(!await userContext.Users.AnyAsync())
                    {
                        var faker = new Faker<User>()
                            .RuleFor(u => u.UserName, f => f.Internet.UserName())
                            .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName?.ToUpper())
                            .RuleFor(u => u.Email, f => f.Internet.Email())
                            .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email?.ToUpper())
                            .RuleFor(u => u.PasswordHash, (f, u) => new PasswordHasher<User>().HashPassword(u, "Password123!"))
                            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                            .RuleFor(u => u.LastName, f => f.Name.LastName())
                            .RuleFor(u => u.Status, f => f.PickRandom(new[] { true, false }))
                            .RuleFor(u => u.RoleId, f => userContext.Roles.First(r => r.Name == f.PickRandom(roles)).Id);
                        userContext.Users.AddRange(faker.Generate(150));
                        await userContext.SaveChangesAsync();
                        authContext.Users.AddRange(userContext.Users.Select(u => new User
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                            NormalizedUserName = u.NormalizedUserName,
                            Email = u.Email,
                            NormalizedEmail = u.NormalizedEmail,
                            PasswordHash = u.PasswordHash,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Status = u.Status,
                            RoleId = u.RoleId
                        }));
                        await authContext.SaveChangesAsync();
                        billContext.Users.AddRange(userContext.Users.Select(u => new User
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                            NormalizedUserName = u.NormalizedUserName,
                            Email = u.Email,
                            NormalizedEmail = u.NormalizedEmail,
                            PasswordHash = u.PasswordHash,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Status = u.Status,
                            RoleId = u.RoleId
                        }));
                        await billContext.SaveChangesAsync();
                    };
                    var statuses = new[] { "Pendiente", "Pagado", "Vencido" };
                    
                    if(!await billContext.Statuses.AnyAsync())
                    {
                        foreach(var status in statuses)
                        {
                            billContext.Statuses.Add(new Status { Name = status });
                        }
                        await billContext.SaveChangesAsync();
                    }

                    if(!await billContext.Bills.AnyAsync())
                    {

                        var paidStatusId = billContext.Statuses.First(s => s.Name == "Pagado").Id;

                        var faker = new Faker<Bill>()
                            .RuleFor(b => b.UserId, f => f.PickRandom(userContext.Users.ToList()).Id)
                            .RuleFor(b => b.StatusId, f => f.PickRandom(billContext.Statuses.ToList()).Id)
                            .RuleFor(b => b.AmountToPay, f => (int)f.Finance.Amount(10, 1000))
                            .RuleFor(b => b.CreatedAt, f => f.Date.Past(1))
                            .RuleFor(b => b.PaymentDate, (f, b) => {
                                if (b.StatusId == paidStatusId)
                                    return f.Date.Between(b.CreatedAt, DateTime.Now);
                                else
                                    return null;
                            });

                        billContext.Bills.AddRange(faker.Generate(350));
                        await billContext.SaveChangesAsync();
                    }

                    if(!await videoContext.Videos.AnyAsync())
                    {
                        var faker = new Faker<Video>()
                            .RuleFor(v => v.Title, f => f.Lorem.Sentence(3))
                            .RuleFor(v => v.Description, f => f.Lorem.Paragraph(2))
                            .RuleFor(v => v.Genre, f => f.PickRandom(new[] { "Acción", "Comedia", "Drama", "Terror", "Ciencia Ficción" }));
                        videoContext.Videos.AddRange(faker.Generate(450));
                        await videoContext.SaveChangesAsync();
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