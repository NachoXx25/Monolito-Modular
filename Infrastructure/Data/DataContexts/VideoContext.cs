using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Monolito_Modular.Domain.VideoModel;

namespace Monolito_Modular.Infrastructure.Data.DataContexts
{
    public class VideoContext : DbContext
    {
        public VideoContext(DbContextOptions<VideoContext> options) : base(options) { }

        public DbSet<Video> Videos { get; set; }
    }
}