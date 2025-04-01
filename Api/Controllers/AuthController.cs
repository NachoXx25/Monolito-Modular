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
    }
}