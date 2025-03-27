using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.UserModels;


namespace Monolito_Modular.Application.Services.Implements
{
    public class TokenService : ITokenService
    {
        /// <summary>
        /// Método que crea un token JWT para un usuario.
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <returns>Token JWT</returns>
        public Task<string> CreateToken(User user)
        {
            var Claims = new List<Claim>(){
                new Claim ("Id", user.Id.ToString()),
                new Claim ("Email", user.Email ?? throw new Exception("No se ha mandado el email.")),
                new Claim (ClaimTypes.Role, user.Role.Name ?? throw new Exception("No se ha mandado el rol")),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("JWT_SECRET")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256Signature);

            var token = new JwtSecurityToken(
                claims: Claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(jwt);
        }
    }
}