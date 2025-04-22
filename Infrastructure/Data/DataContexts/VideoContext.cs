using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Monolito_Modular.Domain.VideoModel;

namespace Monolito_Modular.Infrastructure.Data.DataContexts
{
    public class VideoContext : DbContext
    {
        public VideoContext(DbContextOptions<VideoContext> options) : base(options) { 

            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        public DbSet<Video> Videos { get; set; }
    }
}