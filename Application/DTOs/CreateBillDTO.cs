using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monolito_Modular.Application.DTOs
{
    public class CreateBillDTO
    {
        public required int UserId { get; set; }

        public required string StatusName { get; set; }

        public required int AmountToPay { get; set; }
    }
}