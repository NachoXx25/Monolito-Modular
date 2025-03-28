using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Monolito_Modular.Application.Services.Implements;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.UserModels;
using Monolito_Modular.Infrastructure.Data;
using Monolito_Modular.Infrastructure.Data.DataSeeders;
using Monolito_Modular.Infrastructure.Repositories.Implements;
using Monolito_Modular.Infrastructure.Repositories.Interfaces;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


//Añadir alcance de los servicios
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

//Añadir alcance de los repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();


//Conexión a base de datos de módulo de usuarios (MySQL)
var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
builder.Services.AddDbContextPool<UserContext>(options =>
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
            mySqlOptions.MigrationsAssembly(typeof(UserContext).Assembly.FullName);
        });
}, poolSize: 200);

//Conexión a base de datos de módulo de autenticación (PostgreSQL)
builder.Services.AddDbContext<AuthContext>(options => 
    options.UseNpgsql(Env.GetString("POSTGRESQL_CONNECTION")));

//Configuración de middleware de autenticación
builder.Services.AddAuthentication( options => {

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer( options => {

    options.TokenValidationParameters = new TokenValidationParameters (){
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("JWT_SECRET"))),
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero 
    };
});

//Configuración de identity
builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<UserContext>().AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    //Configuración de contraseña
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;

    //Configuración de Email
    options.User.RequireUniqueEmail = true;

    //Configuración de UserName 
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

    //Configuración de retrys
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

var app = builder.Build();

//Llamado al dataseeder
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

