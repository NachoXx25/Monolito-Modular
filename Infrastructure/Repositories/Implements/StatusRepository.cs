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
    public class StatusRepository : IStatusRepository
    {

        private readonly BillContext _context;

        public StatusRepository(BillContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los estados de factura de la base de datos.
        /// </summary>
        /// <returns>Listado de las facturas con sus ids y nombres</returns>
        public async Task<Status[]> GetAllStatuses()
        {
            var statuses = await _context.Statuses.AsNoTracking().ToArrayAsync();

            if(statuses != null){
                return statuses;
            }
            else{
                throw new Exception("No se encontraron estados en la base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el id del estado de una factura a partir del nombre del estado.
        /// </summary>
        /// <param name="stateName">El nombre del estado</param>
        /// <returns>El id del estado</returns>
        public async Task<int> GetStatusIdByName(string stateName)
        {
            var status = await _context.Statuses.AsNoTracking().FirstOrDefaultAsync(s => s.Name == stateName);

            if(status != null){
                return status.Id;
            }else{
                throw new Exception($"El estado {stateName} no existe en la base de datos.");
            }
        }

        /// <summary>
        /// Obtiene el nombre del estado de una factura a partir del id del estado.
        /// </summary>
        /// <param name="statusId">El id del estado</param>
        /// <returns>El nombre del estado</returns>
        public async Task<string> GetStatusNameById(int statusId)
        {
            var status = await _context.Statuses.AsNoTracking().FirstOrDefaultAsync(s => s.Id == statusId);

            if(status != null){
                return status.Name;
            }else{
                throw new Exception($"El estado con id {statusId} no existe en la base de datos.");
            }
        }

        /// <summary>
        /// Verifica si el estado de una factura es válido a partir del nombre del estado.
        /// </summary>
        /// <param name="statusName">El estado de la factura</param>
        /// <returns>True si el estado es válido</returns>
        public async Task<bool> IsStatusValid(string statusName)
        {
            var status = await _context.Statuses.AsNoTracking().FirstOrDefaultAsync(s => s.Name == statusName);

            if(status != null){
                return true;
            }else{
                return false;
            }
        }
    }
}