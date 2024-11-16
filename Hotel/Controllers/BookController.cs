using Hotel.Data;
using Hotel.DTO;
using Hotel.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService BookService;
        private readonly ILogger<BookController> logger;

        public BookController(IBookService _BookService, ILogger<BookController> logger)
        {
            BookService = _BookService;
            this.logger = logger;
        }

        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBook([FromBody] BookDTO Book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newBook = await BookService.AddBook(Book);
                return CreatedAtAction(nameof(GetBook), new { id = newBook.BookId }, newBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the Book", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                var Book = await BookService.GetBook(id);
                return Ok(Book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Book", details = ex.Message });
            }
        }

        [HttpGet("AllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var Books = await BookService.GetAllBooks();
                return Ok(Books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Books", details = ex.Message });
            }
        }

        [HttpGet("AllBooksSearch")]
        public async Task<IActionResult> GetAllBooksSearch(
            [FromQuery] string? GuestName = null,
            [FromQuery] string? Status = null,
            [FromQuery] DateTime? CheckInDate = null,
            [FromQuery] DateTime? CheckOutDate = null)
        {
            try
            {


                var Books = await BookService.GetAllBooksSearch(GuestName, Status, CheckInDate, CheckOutDate);
                return Ok(Books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Books", details = ex.Message });
            }
        }



        [HttpGet("GetAllBooksForGuest/{guestId}")]
        public async Task<IActionResult> GetAllBooksForGuest(int guestId)
        {
            try
            {
                var Book = await BookService.GetAllBooksForGuest(guestId);
                return Ok(Book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Book with guestId {guestId} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Book", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await BookService.DeleteBook(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Book with ID {id} not found for deletion" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Book", details = ex.Message });
            }
        }

        [HttpPut("UpdateBook")]
        public async Task<IActionResult> UpdateBook([FromBody] BookDTO Book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedBook = await BookService.UpdateBook(Book);
                return Ok(updatedBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Book", details = ex.Message });
            }
        }

        [HttpGet("checkout/{invoiceId}")]
        public async Task<IActionResult> CheckOut(int invoiceId)
        {
            try
            {
                // Call the checkout method from the service layer
                var invoice = await BookService.CheckOut(invoiceId);

                if (invoice == null)
                {
                    logger.LogWarning("Checkout attempted for non-existing Invoice ID: {InvoiceId}", invoiceId);
                    return NotFound($"Invoice with ID {invoiceId} not found.");
                }

                // Return the invoice data after successful checkout
                return Ok(invoice);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, "Invoice ID or Room associated with the Book ID was not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during checkout for Invoice ID: {InvoiceId}", invoiceId);
                return StatusCode(500, "An error occurred while processing the checkout.");
            }
        }
    }
}
