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
        public async Task<CreatedBillDTO> AddBill(CreateBillDTO bill)
        {
            if(bill != null){

                //Revisar si el id de usuario es valido
                var userId = await _billRepository.UserExists(bill.UserId);
                
                if(!userId){
                    throw new ArgumentException("El id de usuario no existe");
                }

                //Obtener el id del estado de la factura según el nombre
                int statusId = await _statusRepository.GetStatusIdByName(bill.StatusName);

                //Crear una nuevo objeto de factura
                var newBill = new Bill
                {
                    AmountToPay = bill.AmountToPay,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    PaymentDate = bill.StatusName == "Pagado" ? DateTime.UtcNow : null,
                    StatusId = statusId,
                    UserId = bill.UserId
                };

                var createdBill = await _billRepository.AddBill(newBill);

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

                    return createdBillDTO;
                }
            }
            else{
                throw new ArgumentNullException("La factura no puede ser nula.");
            }
        }

        /// <summary>
        /// Borra una factura de forma lógica.
        /// </summary>
        /// <param name="id">El id de la factura a borrar</param>
        public async Task DeleteBill(int id)
        {
            //Revisar si el id de la factura es valido
            if(id >= 0){
                //Obtener la factura por su id
                var bill = await _billRepository.GetBillById(id);

                //Revisar si la factura existe
                if(bill == null){
                    throw new ArgumentException("La factura no existe");
                }else{ 
                    //Revisar si la factura tiene un estado valido
                    if(bill.StatusId > 0){
                        //Obtener el nombre del estado de la factura
                        var statusName = await _statusRepository.GetStatusNameById(bill.StatusId);

                        //Revisar si el estado de la factura es 'Pagado'
                        if(statusName == "Pagado"){
                            throw new ArgumentException("No se puede eliminar una factura pagada");
                        }else {
                            //Eliminar la factura de forma lógica
                            await _billRepository.DeleteBill(id);
                        }
                    }else{
                        throw new ArgumentException("El estado de la factura no es válido");
                    }
                }
            }
            else{
                throw new ArgumentOutOfRangeException(nameof(id), "El id de la factura no puede ser menor a 0.");
            }
        }

        /// <summary>
        /// Obtener una factura por su id.
        /// </summary>
        /// <param name="id">El id de la factura a obtener</param>
        /// <returns></returns>
        public async Task<CreatedBillDTO> GetBillById(int id, int userId, string userRole)
        {
            if(id >= 0){
                if(userId >= 0){
                    //Revisar si el usuario existe
                    var userExists = await _billRepository.UserExists(userId);

                    if(!userExists){
                        throw new ArgumentException("El id de usuario no existe");
                    }

                    var bill = await _billRepository.GetBillById(id) ?? throw new ArgumentException("La factura no existe");

                    var statusName = await _statusRepository.GetStatusNameById(bill.StatusId) ?? throw new ArgumentException("El estado de la factura no existe");

                    var mappedBill = new CreatedBillDTO
                    {
                        Id = bill.Id,
                        AmountToPay = bill.AmountToPay,
                        PaymentDate = bill.PaymentDate,
                        Status = statusName,
                        UserId = bill.UserId,
                        CreatedAt = bill.CreatedAt
                    };

                    if(userRole != "Administrador" && bill.UserId != userId){
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
        /// Obtener las facturas por usuario
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<CreatedBillDTO[]> GetBills(int userId, string userRole)
        {
            if(userId >= 0){
                if(userRole == "Administrador"){
                    var allBills = await _billRepository.GetAllBills();

                    var mappedBills = allBills.Select(b => new CreatedBillDTO
                    {
                        Id = b.Id,
                        AmountToPay = b.AmountToPay,
                        PaymentDate = b.PaymentDate,
                        Status = b.StatusId.ToString(),
                        UserId = b.UserId,
                        CreatedAt = b.CreatedAt
                    }).ToArray();

                    return mappedBills;
                    
                }else if(userRole == "Usuario"){

                    var userBills = await _billRepository.GetAllBillsByUserId(userId);

                    var mappedBills = userBills.Select(b => new CreatedBillDTO
                    {
                        Id = b.Id,
                        AmountToPay = b.AmountToPay,
                        PaymentDate = b.PaymentDate,
                        Status = b.StatusId.ToString(),
                        UserId = b.UserId,
                        CreatedAt = b.CreatedAt
                    }).ToArray();

                    return mappedBills;
                }else {
                    throw new ArgumentException("El rol de usuario no es válido.");
                }
            }else {
                throw new ArgumentOutOfRangeException("El id de usuario no puede ser menor a 0.");
            }
        }

        /// <summary>
        /// Cambio el estado de una factura
        /// </summary>
        /// <param name="id">El id de la factura a actualizar</param>
        /// <param name="status">El nuevo estado a asignar</param>
        /// <returns></returns>
        public async Task UpdateBillStatus(int id, string status)
        {
            if(id >= 0){

                var bill = await _billRepository.GetBillById(id);

                if(bill == null){
                    throw new ArgumentException("La factura no existe");
                }else if(bill.IsDeleted){
                    throw new ArgumentException("No puede cambiar el estado de una factura eliminada");
                }
 
                if(status == "Pagado" || status == "Pendiente" || status == "Vencido"){

                    var statusId = await _statusRepository.GetStatusIdByName(status);

                    if(status == "Pagado"){
                        await _billRepository.UpdateBillState(id, statusId, DateTime.UtcNow);
                    }
                    else if(status == "Pendiente" || status == "Vencido"){
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