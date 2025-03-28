using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monolito_Modular.Domain.BillModel
{
    public class Bill
    {
        public int Id { get; set; }

        public required int AmountToPay { get; set; }
        
        public required int StatusId { get; set; }

        public required Status Status { get; set; }
        
        public required DateTime PaymentDate { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
        
        public required DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"));
    }
}