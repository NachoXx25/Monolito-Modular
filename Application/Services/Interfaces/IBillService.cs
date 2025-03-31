using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monolito_Modular.Domain.BillModel;

namespace Monolito_Modular.Application.Services.Interfaces
{
    public interface IBillService
    {
        Task AddBill(Bill bill);
    }
}