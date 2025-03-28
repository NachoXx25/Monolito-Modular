using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.UserModels;

namespace Monolito_Modular.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene los datos de un usuario en función de su Id
        /// </summary>
        /// <param name="Id">Id del usuario.</param>
        /// <returns>Los datos del usuario</returns> 
        [HttpGet("usuarios/{Id}")]
        [Authorize( Roles = "Administrador" )]
        public async Task<IActionResult> GetUserById(int Id)
        {
            try{
                return Ok(await _userService.GetUserById(Id));
            }catch(Exception ex)
            {
                return NotFound(new { Error = ex.Message});
            }
        }
        /*
        [HttpGet("usuarios")]
        public Task<IActionResult> GetAllUsers()
        {
            throw new NotImplementedException();
        }
        
        [HttpPost("usuarios")]
        public async Task<IActionResult> CreateUser()
        {
            try
            {
                throw new NotImplementedException();
            }catch(Exception ex)
            {
                return BadRequest( new { Error = ex.Message});
            }
        }

        [HttpPatch("usuarios/{Id}")]
        public async Task<IActionResult> UpdateUser()
        {
            try
            {
                throw new NotImplementedException();
            }catch(Exception ex)
            {
                return BadRequest( new { Error = ex.Message});
            }
        }
        */
        [HttpDelete("usuarios/{Id}")]
        [Authorize( Roles = "Administrador" )]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            try
            {
                await _userService.DeleteUser(Id);
                return NoContent();
            }catch(Exception ex)
            {
                return BadRequest( new { Error = ex.Message });
            }
        }
    }
}