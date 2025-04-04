using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monolito_Modular.Domain.UserModels;

namespace Monolito_Modular.Domain.BillModel
{
    public class Bill
    {
        public int Id { get; set; }

        public required int AmountToPay { get; set; }

        public required bool IsDeleted { get; set; } = false;

        public DateTime? PaymentDate { get; set; } 
        
        public required int StatusId { get; set; }

        public Status Status { get; set; } = null!;

        public required int UserId { get; set; }

        public User User { get; set; } = null!;
        
        public required DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
    }
}