using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Application.Services.Implements;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.UserModels;
using Monolito_Modular.Infrastructure.Data;
using Monolito_Modular.Infrastructure.Data.DataSeeders;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<UserContext>().AddDefaultTokenProviders();

//Añadir alcance de los servicios
builder.Services.AddScoped<ITokenService, TokenService>();

//Conexión a base de datos de módulo de usuarios (MySQL)
var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
builder.Services.AddDbContextPool<AuthContext>(options =>
{
    options.UseMySql(Env.GetString("MYSQL_CONNECTION"), serverVersion,
        mySqlOptions => 
        {
            mySqlOptions.EnableRetryOnFailure
            (
                maxRetryCount: 5,                
                maxRetryDelay: TimeSpan.FromSeconds(30), 
                errorNumbersToAdd: null
            );
            mySqlOptions.MigrationsAssembly(typeof(AuthContext).Assembly.FullName);
        });
}, poolSize: 200);

//Conexión a base de datos de módulo de autenticación (PostgreSQL)
builder.Services.AddDbContext<UserContext>(options => 
    options.UseNpgsql(Env.GetString("POSTGRESQL_CONNECTION")));


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.Initialize(scope.ServiceProvider);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI();
    app.UseSwagger();
}


app.UseHttpsRedirection();
app.UseSwaggerUI();
app.UseSwagger();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

