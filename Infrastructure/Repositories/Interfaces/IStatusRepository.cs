using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monolito_Modular.Domain.BillModel;

namespace Monolito_Modular.Infrastructure.Repositories.Interfaces
{
    public interface IStatusRepository
    {
        /// <summary>
        /// Verifica si un estado de factura es válido
        /// </summary>
        /// <param name="statusName">El estado a verificar</param>
        Task<bool> IsStatusValid(string statusName);

        /// <summary>
        /// Obtiene todos los estados de factura
        /// </summary>
        Task<Status[]> GetAllStatuses();

        /// <summary>
        /// Obtiene el id de un estado de factura por su nombre
        /// </summary>
        /// <param name="stateName">El nombre del estado a buscar</param>
        /// <returns>El id del estado</returns>
        Task<int> GetStatusIdByName(string statusName);

        /// <summary>
        /// Obtiene el nombre de un estado de factura por su id
        /// </summary>
        /// <param name="statusId">El id de la factura a la que se le buscará el nombre</param>
        /// <returns></returns>
        Task<string> GetStatusNameById(int statusId);
    }
}