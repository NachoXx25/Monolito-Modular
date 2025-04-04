using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<string[]> GetAllStatuses()
        {
            var statuses = await _context.Statuses.AsNoTracking().Select(s => s.Name).ToArrayAsync();

            if(statuses != null){
                return statuses;
            }
            else{
                throw new Exception("No se encontraron estados en la base de datos.");
            }
        }

        public async Task<int> GetStatusIdByName(string stateName)
        {
            var status = await _context.Statuses.AsNoTracking().FirstOrDefaultAsync(s => s.Name == stateName);

            if(status != null){
                return status.Id;
            }else{
                throw new Exception($"El estado {stateName} no existe en la base de datos.");
            }
        }

        public async Task<string> GetStatusNameById(int statusId)
        {
            var status = await _context.Statuses.AsNoTracking().FirstOrDefaultAsync(s => s.Id == statusId);

            if(status != null){
                return status.Name;
            }else{
                throw new Exception($"El estado con id {statusId} no existe en la base de datos.");
            }
        }

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