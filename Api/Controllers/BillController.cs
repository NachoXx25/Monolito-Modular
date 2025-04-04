using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillDTO bill)
        {
            try{
                if(bill == null ){
                    return BadRequest(new { Error = "La factura no puede ser nula" });
                }
                var newBill = await _billService.AddBill(bill);
                return Ok(newBill);
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<IActionResult> GetBillById(int id)
        {
            try
            {
                var userId = User.FindFirstValue("Id");

                var userRole = User.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { Error = "El id de usuario no existe" });
                }

                if(string.IsNullOrEmpty(userRole)){
                    return BadRequest(new { Error = "El rol de usuario no existe" });
                }

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

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetBillsByUser()
        {
            var userId = User.FindFirstValue("Id");
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Error = "El id de usuario no existe" });
            }
            if(string.IsNullOrEmpty(userRole)){
                return BadRequest(new { Error = "El rol de usuario no existe" });
            }

            var bills = await _billService.GetBills(int.Parse(userId), userRole);

            if (bills == null || bills.Length == 0)
            {
                return NotFound(new { Error = "No se encontraron facturas" });
            }

            return Ok(bills);
        }

        [HttpPatch("{id}")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] string statusName)
        {
           try{
                if(string.IsNullOrEmpty(statusName)){
                    return BadRequest(new { Error = "El estado de la factura no puede estar vacío." });
                }
                await _billService.UpdateBillStatus(id, statusName);
                return NoContent();
           }
           catch(Exception ex)
           {
                return BadRequest(new { Error = ex.Message });
           }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            try{
                if(id < 0){
                    return BadRequest(new { Error = "El id de la factura no puede ser menor a 0." });
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