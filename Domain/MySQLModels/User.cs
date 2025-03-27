using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Monolito_Modular.Domain.MySQLModels
{
    public class User : IdentityUser<int>
    {
        public required string FirstName { get; set; }  
        public required string LastName { get; set; }  
        public required int RoleId { get; set; }  
        public required Role Role { get; set; }  
        public required bool Status { get; set; }  
        public required DateTime CreatedAt { get; set; }  = DateTime.UtcNow;
        public required DateTime UpdatedAt { get; set; }  = DateTime.UtcNow;
    }
}