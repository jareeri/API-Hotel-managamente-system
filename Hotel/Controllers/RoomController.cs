using Hotel.DTO;
using Hotel.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService RoomService;

        public RoomController(IRoomService _RoomService)
        {
            RoomService = _RoomService;
        }

        [HttpPost("AddRoom")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddRoom([FromBody] RoomDTO Room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newRoom = await RoomService.AddRoom(Room);
                return CreatedAtAction(nameof(GetRoom), new { id = newRoom.RoomID }, newRoom);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the Room", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRoom(int id)
        {
            try
            {
                var Room = await RoomService.GetRoom(id);
                return Ok(Room);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Room with ID {id} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Room", details = ex.Message });
            }
        }

        [HttpGet("AllRooms")]
        [Authorize]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var Rooms = await RoomService.GetAllRooms();
                return Ok(Rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Rooms", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                await RoomService.DeleteRoom(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Room with ID {id} not found for deletion" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Room", details = ex.Message });
            }
        }

        [HttpPut("UpdateRoom")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoom([FromBody] RoomDTO Room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRoom = await RoomService.UpdateRoom(Room);
                return Ok(updatedRoom);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Room", details = ex.Message });
            }
        }

        [HttpGet("IsAvilableRoom")]
        [Authorize]
        public async Task<IActionResult> IsAvilableRoom(int roomId)
        {
            try
            {
                var Rooms = await RoomService.IsAvilableRoom(roomId);
                return Ok(Rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Rooms", details = ex.Message });
            }
        }

    }
}
