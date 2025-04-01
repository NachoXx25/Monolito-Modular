using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monolito_Modular.Application.DTOs
{
    public class SearchByDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }
}