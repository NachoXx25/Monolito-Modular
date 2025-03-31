using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monolito_Modular.Domain.BillModel
{
    public class Status
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}