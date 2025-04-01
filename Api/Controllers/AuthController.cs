using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;

namespace Monolito_Modular.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Endpoint para iniciar sesión.
        /// </summary>
        /// <param name="login">Credenciales del usuario.</param>
        /// <returns>Token y datos del usuario o error.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try{
                var token = await _authService.Login(login);
                return Ok(new { Token = token });
            }catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message});
            }
        }

        /// <summary>
        /// Endpoint para cambiar la contraseña de un usuario.
        /// </summary>
        /// <param name="updatePasswordDTO">Datos de la nueva contraseña.</param>
        /// <param name="Id">Id del usuario.</param>
        /// <returns>Mensaje de éxito o error.</returns>
        [HttpPatch("usuarios/{Id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDTO updatePasswordDTO, int Id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try{
                var userIdClaim = User.FindFirst("Id")?.Value;
                if(userIdClaim != Id.ToString())
                {
                    return BadRequest(new { Error = "No tienes permiso para cambiar otra contraseña que no sea la tuya." });
                }
                var result = await _authService.UpdatePassword(updatePasswordDTO, Id);
                return Ok(new { Message = result });
            }catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message});
            }
        }
    }
}