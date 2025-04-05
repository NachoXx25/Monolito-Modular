using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Monolito_Modular.Domain.BillModel;
using Monolito_Modular.Infrastructure.Data.DataContexts;
using Monolito_Modular.Infrastructure.Repositories.Interfaces;

namespace Monolito_Modular.Infrastructure.Repositories.Implements
{
    public class BillRepository : IBillRepository
    {

        private readonly BillContext _context;

        public BillRepository(BillContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega una factura a la base de datos
        /// </summary>
        /// <param name="bill">Factura a agregar.</param>
        public async Task<Bill> AddBill(Bill bill)
        {
            await _context.Bills.AddAsync(bill);
            await _context.SaveChangesAsync();
            return bill;
        }

        /// <summary>
        /// Obtiene una factura por su id
        /// </summary>
        /// <param name="id">El id de la factura a buscar</param>
        /// <returns>Factura encontrada</returns>
        /// <exception cref="Exception">Si no se encuentra la factura</exception>
        public async Task<Bill> GetBillById(int id)
        {
            var bill = await _context.Bills.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            return bill ?? throw new Exception("Factura no encontrada");
        }

        /// <summary>
        /// Actualiza el estado de una factura
        /// </summary>
        /// <param name="id">El id de la factura a actualizar</param>
        /// <param name="statusId">El id del nuevo estado de la factura</param>
        /// <exception cref="Exception">Si no se encuentra la factura</exception>
        public async Task UpdateBillState(int id, int statusId, DateTime? paymentDate)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == id) ?? throw new Exception("Factura no encontrada");

            bill.StatusId = statusId;
            if(paymentDate != null){
                bill.PaymentDate = (DateTime)paymentDate;
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Hace un borrado lógico de una factura
        /// </summary>
        /// <param name="id">El id de la factura a borrar</param>
        public async Task DeleteBill(int id)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == id) ?? throw new Exception("Factura no encontrada");

            bill.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene todas las facturas
        /// </summary>
        /// <returns>Listado de facturas</returns>
        public async Task<Bill[]> GetAllBills()
        {
            return await _context.Bills.AsNoTracking().ToArrayAsync();
        }

        /// <summary>
        /// Obtiene todas las facturas de un usuario
        /// </summary>
        /// <param name="userId">El id del usuario al que le corresponden las facturas</param>
        /// <returns>Listado de las facturas del usuario</returns>
        public async Task<Bill[]> GetAllBillsByUserId(int userId)
        {
            return await _context.Bills.AsNoTracking().Where(b => b.UserId == userId).ToArrayAsync();
        }

        /// <summary>
        /// Verifica si un usuario existe en la base de datos según su id
        /// </summary>
        /// <param name="userId">El id del usuario a verificar</param>
        public async Task<bool> UserExists(int userId)
        {
            return await _context.Users.FindAsync(userId) != null;
        }
    }
}