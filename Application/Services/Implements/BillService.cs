using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;
using Monolito_Modular.Domain.BillModel;
using Monolito_Modular.Infrastructure.Repositories.Interfaces;
using Org.BouncyCastle.Bcpg;

namespace Monolito_Modular.Application.Services.Implements
{
    public class BillService : IBillService

    {
        private readonly IBillRepository _billRepository;
        private readonly IStatusRepository _statusRepository;

        public BillService(IBillRepository billRepository, IStatusRepository statusRepository){

            _billRepository = billRepository;
            _statusRepository = statusRepository;
        }

        /// <summary>
        /// Agrega una nueva factura a la base de datos.
        /// </summary>
        /// <param name="bill">La factura a agregar</param>
        /// <returns>La factura creada</returns>
        public async Task<CreatedBillDTO> AddBill(CreateBillDTO bill)
        {
            if(bill != null){

                //Revisar si el id de usuario es válido
                var userId = await _billRepository.UserExists(bill.UserId);
                
                if(!userId){
                    throw new ArgumentException("El id de usuario no existe");
                }

                //Obtener el id del estado de la factura según el nombre
                int statusId = await _statusRepository.GetStatusIdByName(bill.StatusName);

                //Revisar si el id del estado de la factura es válido
                if(statusId <= 0){
                    throw new ArgumentException("El estado de la factura no es válido");
                }

                //Crear una nuevo objeto de factura
                var newBill = new Bill
                {
                    AmountToPay = bill.AmountToPay,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    PaymentDate = bill.StatusName == "Pagado" ? DateTime.UtcNow : null, //Agregar la fecha de pago si el estado es "Pagado"
                    StatusId = statusId,
                    UserId = bill.UserId
                };

                //Agregar la nueva factura a la base de datos
                var createdBill = await _billRepository.AddBill(newBill);

                //Revisar si la factura fue creada correctamente
                if(createdBill == null){
                    throw new Exception("Error al crear la factura");
                }else {
                    //Crear un nuevo objeto de DTO para la factura creada
                    var createdBillDTO = new CreatedBillDTO
                    {
                        Id = createdBill.Id,
                        AmountToPay = createdBill.AmountToPay,
                        PaymentDate = createdBill.PaymentDate,
                        Status = bill.StatusName,
                        UserId = createdBill.UserId,
                        CreatedAt = createdBill.CreatedAt
                    };

                    //Retornar la factura creada con datos mapeados
                    return createdBillDTO;
                }
            }
            else{
                throw new ArgumentNullException("La factura no puede ser nula.");
            }
        }

        /// <summary>
        /// Método para borrar una factura de forma lógica.
        /// </summary>
        /// <param name="id">El id de la factura a borrar</param>
        public async Task DeleteBill(int id)
        {
            //Revisar si el id de la factura es válido
            if(id >= 0){

                //Obtener la factura por su id
                var bill = await _billRepository.GetBillById(id) ?? throw new ArgumentException("La factura no existe");

                //Revisar si la factura tiene un estado válido
                if(bill.StatusId > 0){

                    //Obtener el nombre del estado de la factura
                    var statusName = await _statusRepository.GetStatusNameById(bill.StatusId) ?? throw new ArgumentException("El estado de la factura no existe");

                    //Revisar si el estado de la factura es 'Pagado'
                    if(statusName == "Pagado"){
                        //No permitir la eliminación de una factura pagada
                        throw new ArgumentException("No se puede eliminar una factura pagada");
                    }else {
                        //Eliminar la factura de forma lógica
                        await _billRepository.DeleteBill(id);
                    }
                }else{
                    //Si el estado de la factura no es un id válido, lanzar una excepción
                    throw new ArgumentException("El estado de la factura no es válido");
                }
            }
            else{
                throw new ArgumentOutOfRangeException("El id de la factura no puede ser menor a 0.");
            }
        }

        /// <summary>
        /// Método que perite obtener una factura por su id.
        /// </summary>
        /// <param name="id">El id de la factura a obtener</param>
        /// <returns>La factura solicitada</returns>
        public async Task<CreatedBillDTO> GetBillById(int id, int userId, string userRole)
        {
            if(id >= 0){
                if(userId >= 0){
                    //Revisar si el usuario existe
                    var userExists = await _billRepository.UserExists(userId);

                    //Si el usuario no existe, lanzar una excepción
                    if(!userExists){
                        throw new ArgumentException("El id de usuario no existe");
                    }
                    
                    //Obtener la factura por su id
                    var bill = await _billRepository.GetBillById(id) ?? throw new ArgumentException("La factura no existe");

                    //Obtener el nombre del estado de la factura
                    var statusName = await _statusRepository.GetStatusNameById(bill.StatusId) ?? throw new ArgumentException("El estado de la factura no existe");

                    //Mapear la factura a un DTO con los datos necesarios
                    var mappedBill = new CreatedBillDTO
                    {
                        Id = bill.Id,
                        AmountToPay = bill.AmountToPay,
                        PaymentDate = bill.PaymentDate,
                        Status = statusName,
                        UserId = bill.UserId,
                        CreatedAt = bill.CreatedAt
                    };

                    //Si el usuario no es administrador, verificar que el id de usuario de la factura sea igual al id del usuario que realiza la consulta
                    if(userRole != "Administrador" && bill.UserId != userId){
                        //Si no es así, lanzar una excepción
                        throw new ArgumentException("No tienes permisos para ver esta factura.");
                    }

                    return mappedBill;
                }else{
                    throw new ArgumentOutOfRangeException("El id de usuario no puede ser menor a 0.");
                }
            }
            else{
                throw new ArgumentOutOfRangeException("El id de la factura no puede ser menor a 0.");
            }
        }

        /// <summary>
        /// Método que permite obtener todas las facturas de un usuario o todas las facturas del sistema.
        /// </summary>
        /// <param name="userId">El id del usuario que realiza la consulta</param>
        /// <param name="userRole">El rol del usuario que realiza la consulta</param>
        /// <param name="statusName">Filtro opcional por estado de factura</param>
        /// <returns>Listado de las facturas según el usuario y el filtro</returns>
        public async Task<CreatedBillDTO[]> GetBills(int userId, string userRole, string? statusName)
        {
            //Obtener todos los estados de una factura la base de datos
            var statuses = await _statusRepository.GetAllStatuses() ?? throw new ArgumentException("No se pudieron cargar los estados.");

            if(userId >= 0){

                //Si el usuario es administrador, obtener todas las facturas
                if(userRole == "Administrador"){

                    var allBills = await _billRepository.GetAllBills() ?? throw new ArgumentException("No se encontraron facturas.");

                    //Mapear todas las facturas a un DTO con los datos necesarios
                    var mappedBills = allBills.Select( b => new CreatedBillDTO
                    {
                        Id = b.Id,
                        AmountToPay = b.AmountToPay,
                        PaymentDate = b.PaymentDate,
                        Status = statuses.FirstOrDefault(s => s.Id == b.StatusId)?.Name ?? "Estado no encontrado",
                        UserId = b.UserId,
                        CreatedAt = b.CreatedAt
                    }).ToArray();

                    //Si se proporciona un nombre de estado, filtrar las facturas por ese estado
                    if(!string.IsNullOrEmpty(statusName)){
                        mappedBills = mappedBills.Where(b => b.Status.Contains(statusName)).ToArray();

                        //Si no se encuentran facturas con ese estado, lanzar una excepción
                        if(mappedBills.Length == 0){
                            throw new ArgumentException("No se encontraron facturas con ese estado.");
                        }else{
                            //Si se encuentran facturas, retornar el listado filtrado
                            return mappedBills;
                        }
                    }else {
                        //Si no se proporciona un nombre de estado, retornar todas las facturas
                        return mappedBills;
                    }

                //Si el usuario es cliente, obtener solo las facturas del usuario
                }else if(userRole == "Cliente"){

                    //Obtener todas las facturas del usuario
                    var userBills = await _billRepository.GetAllBillsByUserId(userId) ?? throw new ArgumentException("No se encontraron facturas para el usuario.");

                    //Mapear las facturas del usuario a un DTO con los datos necesarios
                    var mappedBills = userBills.Select(b => new CreatedBillDTO
                    {
                        Id = b.Id,
                        AmountToPay = b.AmountToPay,
                        PaymentDate = b.PaymentDate,
                        Status = statuses.FirstOrDefault(s => s.Id == b.StatusId)?.Name ?? "Estado no encontrado",
                        UserId = b.UserId,
                        CreatedAt = b.CreatedAt
                    }).ToArray();

                    //Si se proporciona un nombre de estado, filtrar las facturas por ese estado
                    if(!string.IsNullOrEmpty(statusName)){
                        mappedBills = mappedBills.Where(b => b.Status.Contains(statusName)).ToArray();

                        //Si no se encuentran facturas con ese estado, lanzar una excepción
                        if(mappedBills.Length == 0){
                            throw new ArgumentException("No se encontraron facturas con ese estado.");
                        }else{
                            //Si se encuentran facturas, retornar el listado filtrado
                            return mappedBills;
                        }
                    }else {
                        //Si no se proporciona un nombre de estado, retornar todas las facturas del usuario
                        return mappedBills;
                    }
                }else {
                    throw new ArgumentException("El rol de usuario no es válido.");
                }
            }else {
                throw new ArgumentOutOfRangeException("El id de usuario no puede ser menor a 0.");
            }
        }

        /// <summary>
        /// Método para modificar el estado de una factura
        /// </summary>
        /// <param name="id">El id de la factura a actualizar</param>
        /// <param name="status">El nuevo estado a asignar</param>
        public async Task UpdateBillStatus(int id, string status)
        {
            //Revisar si el id de la factura es válido
            if(id >= 0){

                //Obtener la factura por su id
                var bill = await _billRepository.GetBillById(id) ?? throw new ArgumentException("La factura no existe");

                //Revisar si la factura no está eliminada
                if(bill.IsDeleted){
                    throw new ArgumentException("No puede cambiar el estado de una factura eliminada");
                }

                //Revisar si el nuevo estado de la factura es válido
                var isStatusValid = await _statusRepository.IsStatusValid(status);
 
                if(isStatusValid){

                    //Obtener el id del nuevo estado de la factura
                    var statusId = await _statusRepository.GetStatusIdByName(status);

                    //Revisar si el nuevo estado de la factura es 'Pagado'
                    if(status == "Pagado"){
                        //Actualizar la fecha de pago de la factura
                        await _billRepository.UpdateBillState(id, statusId, DateTime.UtcNow);
                    }
                    //Si el nuevo estado no es 'Pagado', actualizar solo el estado de la factura
                    else {
                        await _billRepository.UpdateBillState(id, statusId, null);
                    }
                }
                else {
                    throw new ArgumentException("El estado de la factura no es válido.");
                }
            }
            else{
                throw new ArgumentOutOfRangeException("El id de la factura no puede ser menor a 0.");
            }
        }
    }
}