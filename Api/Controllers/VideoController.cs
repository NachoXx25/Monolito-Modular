using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;

namespace Monolito_Modular.Api.Controllers
{
    [ApiController]
    [Route("videos")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        /// <summary>
        /// Endpoint que permite subir un video a la base de datos.
        /// </summary>
        /// <param name="video">El video a subir</param>
        /// <returns>El video recientemente agregado</returns>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UploadVideo([FromBody] UploadVideoDTO video)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var uploadedVideo = await _videoService.UploadVideo(video);

                return Ok(uploadedVideo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint que permite obtener un video por su id.
        /// </summary>
        /// <param name="id">El id del video a obtener</param>
        /// <returns>El video buscado</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetVideoById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new { message = "El Id es requerido" });
                }

                var video = await _videoService.GetVideoById(id);

                if (video == null)
                {
                    return NotFound(new { message = "Video no encontrado" });
                }

                return Ok(video);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint que permite actualizar un video por su id.
        /// </summary>
        /// <param name="id">El id del video a actualizar</param>
        /// <param name="updateVideo">Los nuevos valores a actualizar</param>
        /// <returns>Las modificaciones realizadas al video</returns>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateVideo(string id, [FromForm] UpdateVideoDTO updateVideo)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "El Id es requerido" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedVideo = await _videoService.UpdateVideo(id, updateVideo);

                return Ok(updatedVideo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint que permite eliminar lógicamente un video por su id.
        /// </summary>
        /// <param name="id">El id del video a eliminar</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteVideo(string id)
        {
           try
           {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new { message = "El Id es requerido" });
                }

                await _videoService.DeleteVideo(id);

                return NoContent();
           }
           catch (Exception ex)
           {
                return BadRequest(new { message = ex.Message });
           }
        }

        /// <summary>
        /// Endpoint que permite obtener todos los videos no eliminados de la base de datos.
        /// </summary>
        /// <param name="search">Filtro opcional por género o título</param>
        /// <returns>Listado de videos encontrados según el filtrado</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllVideos([FromQuery] VideoSearchDTO? search)
        {
            try
            {
                var videos = await _videoService.GetAllVideos(search);

                if (videos == null)
                {
                    return NotFound(new { message = "No se encontraron videos" });
                }

                return Ok(videos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}