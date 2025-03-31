using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.EntityFrameworkCore;

namespace Monolito_Modular.Domain.VideoModel
{
    [Collection("videos")]
    public class Video
    {
        public int Id { get; set; }

        public required string Title { get; set; }
        
        public required string Description { get; set; }

        public required string Genre { get; set; }

        public required bool IsDeleted { get; set; }  = false;
    }
}