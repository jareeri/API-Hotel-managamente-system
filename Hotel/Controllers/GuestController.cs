using Hotel.DTO;
using Hotel.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly IGuestService guestService;

        public GuestController(IGuestService _guestService)
        {
            guestService = _guestService;
        }

        [HttpPost("AddGuest")]
        public async Task<IActionResult> AddGuest([FromBody] GuestDTO guest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newGuest = await guestService.AddGuest(guest);
                return CreatedAtAction(nameof(GetGuest), new { id = newGuest.GuestId }, newGuest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the guest", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGuest(int id)
        {
            try
            {
                var guest = await guestService.GetGuest(id);
                return Ok(guest);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Guest with ID {id} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the guest", details = ex.Message });
            }
        }

        [HttpGet("AllGuests/{pageSize?}/{pageNumber?}")]
        public async Task<IActionResult> GetAllGuests(int? pageSize, int? pageNumber)
        {
            try
            {
                var guests = await guestService.GetAllGuests(pageSize, pageNumber);
                return Ok(guests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving guests", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            try
            {
                await guestService.DeleteGuest(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Guest with ID {id} not found for deletion" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the guest", details = ex.Message });
            }
        }

        [HttpPut("UpdateGuest")]
        public async Task<IActionResult> UpdateGuest([FromBody] GuestDTO guest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedGuest = await guestService.UpdateGuest(guest);
                return Ok(updatedGuest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the guest", details = ex.Message });
            }
        }
    }
}
