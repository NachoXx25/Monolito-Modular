using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.UserModels;

namespace Monolito_Modular.Api.Controllers
{
    [ApiController]
    [Route("")]
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
        
        [HttpGet("usuarios")]
        //[Authorize (Roles = "Administrador" )]
        public Task<IActionResult> GetAllUsers()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Endpoint para crear un nuevo usuario.
        /// </summary>
        /// <param name="userDTO">Datos del usuario.</param>
        /// <returns>Ok en caso de exito o bad request en caso de errror.</returns>
        [HttpPost("usuarios")]
        public async Task<IActionResult> CreateUser(CreateUserDTO userDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                /*
                var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);
                if (userRoleClaim != "Administrador" && userDTO.Role == "Administrador")
                {
                    return Forbid("No tienes permisos para crear usuarios administradores.");
                }
                */
                var user = await _userService.CreateUser(userDTO);
                return Ok(user);
            }catch(Exception ex)
            {
                return BadRequest( new { Error = ex.Message});
            }
        }
        /*
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
        //[Authorize( Roles = "Administrador" )]
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