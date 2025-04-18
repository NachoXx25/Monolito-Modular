using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.VideoModel;
using Monolito_Modular.Infrastructure.Repositories.Interfaces;

namespace Monolito_Modular.Application.Services.Implements
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;

        public VideoService(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        /// <summary>
        /// Método que permite eliminar lógicamente un video por su id
        /// </summary>
        /// <param name="id">El id del video a eliminar</param>
        /// <returns></returns>
        public async Task DeleteVideo(string id)
        {
            await _videoRepository.DeleteVideo(id);
        }

        /// <summary>
        /// Método que permite obtener todos los videos no eliminados de la base de datos
        /// </summary>
        /// <param name="search">Filtro opcional por género o título</param>
        /// <returns>Listado de todos los videos según el filtrado</returns>
        public async Task<GetVideoDTO[]?> GetAllVideos(VideoSearchDTO? search)
        {
            // Obtener todos los videos
            var videos = await _videoRepository.GetAllVideos(search);

            if (videos == null || videos.Length == 0)
            {
                return null;
            }

            // Mapear los videos a DTOs
            var mappedVideos = videos.Select(video => new GetVideoDTO
            {
                Id = video.Id.ToString(),
                Title = video.Title,
                Description = video.Description,
                Genre = video.Genre,
            }).ToArray();

            return mappedVideos;
        }

        /// <summary>
        /// Método que permite obtener un video por su id
        /// </summary>
        /// <param name="id">El id del video a obtener</param>
        /// <returns>El video buscado</returns>
        public async Task<GetVideoDTO?> GetVideoById(string id)
        {
            // Obtener el video por id
            var video = await _videoRepository.GetVideoById(id);

            //Si es nulo, lo retorno
            if(video == null)
            {
                return null;
            }

            // Mapear el video a un DTO con la información necesaria
            var mappedVideo = new GetVideoDTO
            {
                Id = video.Id.ToString(),
                Title = video.Title,
                Description = video.Description,
                Genre = video.Genre,
            };

            return mappedVideo;
        }

        /// <summary>
        /// Método que permite actualizar un video por su id
        /// </summary>
        /// <param name="id">El id del video a actualizar</param>
        /// <param name="updateVideo">Los valores a actualizar del video</param>
        /// <returns>El video actualizado</returns>
        public async Task<UpdateVideoDTO> UpdateVideo(string id, UpdateVideoDTO updateVideo)
        {
           // Obtener el video por id
            var video = await _videoRepository.GetVideoById(id) ?? throw new ArgumentException("Video no encontrado");

            // Validar que el video no esté eliminado antes de actualizarlo
            if(video.IsDeleted){
                throw new ArgumentException("No se puede actualizar un video eliminado");
            }

            //Actualizar el video y obtenerlo
            var updatedVideo = await _videoRepository.UpdateVideo(id, updateVideo) ?? throw new ArgumentException("Error al actualizar el video");

            // Mapear el video actualizado a un DTO con la información necesaria
            var mappedVideo = new UpdateVideoDTO
            {
                Title = updatedVideo.Title,
                Description = updatedVideo.Description,
                Genre = updatedVideo.Genre,
            };

            return mappedVideo;
        }

        /// <summary>
        /// Método que permite subir un video a la base de datos
        /// </summary>
        /// <param name="video">El video a subir</param>
        /// <returns>El video subido</returns>
        public async Task<GetVideoDTO> UploadVideo(UploadVideoDTO video)
        {
            //Mapear los datos del video a un objeto Video
            var toUploadVideo = new Video
            {
                Title = video.Title,
                Description = video.Description,
                Genre = video.Genre,
                IsDeleted = false,
            };

            // Subir el video y obtenerlo
            var uploadedVideo = await _videoRepository.UploadVideo(toUploadVideo) ?? throw new ArgumentException("Error al subir el video");

            // Mapear el video subido a un DTO con la información necesaria
            var mappedVideo = new GetVideoDTO
            {
                Id = uploadedVideo.Id.ToString(),
                Title = uploadedVideo.Title,
                Description = uploadedVideo.Description,
                Genre = uploadedVideo.Genre,
            };

            return mappedVideo;
        }
    }
}