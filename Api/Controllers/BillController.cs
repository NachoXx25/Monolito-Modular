using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monolito_Modular.Application.DTOs;
using Monolito_Modular.Application.Services.Interfaces;

namespace Monolito_Modular.Api.Controllers
{
    [ApiController]
    [Route("facturas")]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;
        
        public BillController(IBillService billService)
        {
            _billService = billService;
        }

        /// <summary>
        /// Endpoint para crear una nueva factura.
        /// </summary>
        /// <param name="bill">La nueva factura a agregar</param>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillDTO bill)
        {
            try{
                if(!ModelState.IsValid){
                    return BadRequest(ModelState);
                }
                var newBill = await _billService.AddBill(bill);
                return Ok(newBill);
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint que obtiene una factura por su id.
        /// </summary>
        /// <param name="id">El id de la factura a obtener</param>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBillById(int id)
        {
            try
            {

                if(id <= 0){
                    return BadRequest(new { Error = "El id de la factura no puede ser menor o igual a 0." });
                }

                var userId = User.FindFirstValue("Id") ?? throw new Exception("No se ha enviado un id");
                var userRole = User.FindFirstValue(ClaimTypes.Role) ?? throw new Exception("No se ha enviado un rol");

                var bill = await _billService.GetBillById(id, int.Parse(userId), userRole);
                if (bill == null)
                {
                    return NotFound(new { Error = "Factura no encontrada" });
                }
                return Ok(bill);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint que obtiene todas las facturas de un usuario.
        /// </summary>
        /// <param name="statusName">Filtro opcional de estado de las facturas</param>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBillsByUser([FromQuery] string? statusName)
        {
            try
            {
                var userId = User.FindFirstValue("Id") ?? throw new Exception("No se ha enviado un id");
                var userRole = User.FindFirstValue(ClaimTypes.Role) ?? throw new Exception("No se ha enviado un rol");

                var bills = await _billService.GetBills(int.Parse(userId), userRole, statusName);

                if (bills == null)
                {
                    return NotFound(new { Error = "No se encontraron facturas" });
                }

                return Ok(bills);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para actualizar el estado de una factura.
        /// </summary>
        /// <param name="id">El id de la factura a actualizar</param>
        /// <param name="statusName">El nuevo estado para la factura</param>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] string statusName)
        {
           try{
                if(string.IsNullOrWhiteSpace(statusName)){
                    return BadRequest(new { Error = "El estado de la factura es requerido" });
                }
                if(id <= 0){
                    return BadRequest(new { Error = "El id de la factura no puede ser menor o igual a 0." });
                }
                await _billService.UpdateBillStatus(id, statusName);
                return NoContent();
           }
           catch(Exception ex)
           {
                return BadRequest(new { Error = ex.Message });
           }
        }

        /// <summary>
        /// Endpoint para eliminar lógicamente una factura.
        /// </summary>
        /// <param name="id">El id de la factura a borrar</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            try{
                if(id <= 0){
                    return BadRequest(new { Error = "El id de la factura no puede ser menor o igual a 0." });
                }
                await _billService.DeleteBill(id);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}