using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monolito_Modular.Application.DTOs
{
    public class UpdateVideoDTO
    {
        public string? Title { get; set; }
        
        public string? Description { get; set; }

        public string? Genre { get; set; }
    }
}