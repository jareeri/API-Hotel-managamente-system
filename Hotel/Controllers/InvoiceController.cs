using Hotel.DTO;
using Hotel.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService InvoiceService;

        public InvoiceController(IInvoiceService _InvoiceService)
        {
            InvoiceService = _InvoiceService;
        }

        [HttpPost("AddInvoice")]
        public async Task<IActionResult> AddInvoice([FromBody] InvoiceDTO Invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newInvoice = await InvoiceService.AddInvoice(Invoice);
                return CreatedAtAction(nameof(GetInvoice), new { id = newInvoice.InvoiceId }, newInvoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the Invoice", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            try
            {
                var Invoice = await InvoiceService.GetInvoice(id);
                return Ok(Invoice);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Invoice with ID {id} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Invoice", details = ex.Message });
            }
        }

        [HttpGet("AllInvoices")]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var Invoices = await InvoiceService.GetAllInvoices();
                return Ok(Invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Invoices", details = ex.Message });
            }
        }

        [HttpGet("AllInvoicesForGuest/{guestId}")]
        public async Task<IActionResult> GetAllInvoicesForGuest(int guestId)
        {
            try
            {
                var Invoices = await InvoiceService.GetAllInvoicesForGuest(guestId);
                return Ok(Invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Invoices", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                await InvoiceService.DeleteInvoice(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Invoice with ID {id} not found for deletion" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Invoice", details = ex.Message });
            }
        }

        [HttpPut("UpdateInvoice")]
        public async Task<IActionResult> UpdateInvoice([FromBody] InvoiceDTO Invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedInvoice = await InvoiceService.UpdateInvoice(Invoice);
                return Ok(updatedInvoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Invoice", details = ex.Message });
            }
        }
    }
}
