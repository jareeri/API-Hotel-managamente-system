using AutoMapper;
using Hotel.Context;
using Hotel.Data;
using Hotel.DTO;
using Hotel.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Hotel.Service
{
    public class BookService : IBookService
    {
        private readonly IGenric<Book> genric;
        private readonly IMapper mapper;
        private readonly DataContext dataContext;
        private readonly ILogger<BookService> logger;
        private readonly IInvoiceService invoiceService;

        public BookService(IGenric<Book> _genric, IMapper _mapper, DataContext _dataContext, ILogger<BookService> _logger
                            , IInvoiceService _InvoiceService )
        {
            genric = _genric;
            mapper = _mapper;
            dataContext = _dataContext;
            logger = _logger;
            invoiceService = _InvoiceService;
        }

        // Add a new Book
        public async Task<BookDTO> AddBook(BookDTO book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book), "Book data is required");
            }

            try
            {
                bool isAvailable = !await dataContext.Books.AnyAsync(b =>
                    b.RoomId == book.RoomId &&
                    b.CheckOutDate > book.CheckInDate &&
                    b.CheckInDate < book.CheckOutDate);

                if (!isAvailable)
                {
                    throw new InvalidOperationException("The room is already booked for the selected dates.");
                }
                if (book.CheckOutDate < book.CheckInDate)
                {
                    throw new Exception("Error in checkout date!!");
                }

                // Map BookDTO to Book and add to the database
                Book newBook = mapper.Map<Book>(book);
                await genric.Add(newBook);

                // Retrieve the room associated with the Book
                var room = await dataContext.Rooms.Where(e => e.RoomID == book.RoomId).FirstOrDefaultAsync();
                if (room == null)
                {
                    throw new KeyNotFoundException($"Room with ID {book.RoomId} not found.");
                }

                // Create and add an invoice
                InvoiceDTO invoice = new InvoiceDTO
                {
                    BookId = newBook.BookId,
                    Price = 0,
                    Status = "Not Paid"
                };
                await invoiceService.AddInvoice(invoice); // Make sure this method is async

                // Update room status to "Occupied"
                room.Status = "Occupied";

                // Save all changes asynchronously
                await dataContext.SaveChangesAsync();

                return mapper.Map<BookDTO>(newBook);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding a Book");
                throw new Exception("Error adding Book", ex);
            }
        }

        //private async bool IsAvilable(int roomId)
        //{
        //    var books = await dataContext.Books.Where(b => b.RoomId == roomId).ToListAsync();
            
        //}

        // Get a Book by ID
        public async Task<BookDTO> GetBook(int id)
        {
            try
            {
                var Book = await genric.Get(id);
                return mapper.Map<BookDTO>(Book);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving Book with ID {id}");
                throw new Exception("Error retrieving Book", ex);
            }
        }

        // Get all Books
        //public async Task<List<BookDTO>> GetAllBooks()
        //{
        //    try
        //    {
        //        //var Books = await dataContext.Books.Where(b=>b.i).Include(b => b.guest).ToListAsync();

        //        //var Books = await dataContext.Invoices
        //        //                           .Include(b => b.book)
        //        //                           .ToListAsync();

        //        var Books =  dataContext.Books.Select(b => new
        //        {
        //            name = b.guest.Name,
        //            price = b.invoices.Price,
        //            checkInDate = b.CheckInDate,
        //            checkOutDate = b.CheckOutDate,
        //        });
        //        //var Books = await genric.GetAll();
        //        return mapper.Map<List<BookDTO>>(Books);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Error occurred while retrieving all Books");
        //        throw new Exception("Error retrieving Books", ex);
        //    }
        //}

        public async Task<List<BookDTO>> GetAllBooks()
        {
            try
            {
                
                var books = await dataContext.Books
                    .Select(b => new BookDTO
                    {
                        BookId = b.BookId,
                        GuestId = b.GuestId,
                        RoomId = b.RoomId,
                        CheckInDate = b.CheckInDate,
                        CheckOutDate = b.CheckOutDate,
                        name = b.guest.Name, 
                        price = b.invoices.Price, 
                        status = b.invoices.Status
                    })
                    .ToListAsync();

                return books;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all Books");
                throw new Exception("Error retrieving Books", ex);
            }
        }

        public async Task<List<BookDTO>> 
            GetAllBooksSearch(string? GuestName , string? Status ,
                              DateTime? CheckInDate, DateTime? CheckOutDate)
        {
            try
            {

                var Query = dataContext.Books
                    .Select(b => new BookDTO
                    {
                        BookId = b.BookId,
                        GuestId = b.GuestId,
                        RoomId = b.RoomId,
                        CheckInDate = b.CheckInDate,
                        CheckOutDate = b.CheckOutDate,
                        name = b.guest.Name,
                        price = b.invoices.Price,
                        status = b.invoices.Status
                    })
                    .AsQueryable();
                if (!string.IsNullOrEmpty(GuestName))
                {
                    Query = Query.Where(e=>e.name.Contains(GuestName));
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    Query = Query.Where(e => e.status.Contains(Status));
                }
                if (CheckInDate.HasValue)
                {
                    Query = Query.Where(e => e.CheckInDate.Equals(CheckInDate));
                }
                if (CheckOutDate.HasValue)
                {
                    Query = Query.Where(e => e.CheckOutDate.Equals(CheckOutDate));
                }
                return await Query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all Books");
                throw new Exception("Error retrieving Books", ex);
            }
        }

        public async Task<List<BookDTO>> GetAllBooksForGuest(int guestId )
        {
            try
            {
                //var Books1 = dataContext.Books.Select(b => new
                //{
                //    GuestId = b.GuestId,
                //    BookId = b.BookId,
                //    RoomId = b.RoomId,
                //    CheckInDate = b.CheckInDate,
                //    CheckOutDate = b.CheckOutDate,
                //    invoices = mapper.Map<Invoice>(b.invoices)
                //});
                var Books = await dataContext.Books.Where(b=>b.GuestId ==guestId).ToListAsync();
                return mapper.Map<List<BookDTO>>(Books);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all Books");
                throw new Exception("Error retrieving Books", ex);
            }
        }

        // Delete a Book by ID
        public async Task DeleteBook(int id)
        {
            try
            {
                await genric.Remove(id);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, $"Book with ID {id} not found for deletion");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while deleting Book with ID {id}");
                throw new Exception("Error deleting Book", ex);
            }
        }

        // Update an existing Book
        public async Task<BookDTO> UpdateBook(BookDTO Book)
        {
            if (Book == null)
            {
                throw new ArgumentNullException(nameof(Book), "Book data is required for update");
            }

            try
            {
                Book updatedBook = mapper.Map<Book>(Book);
                var result = await genric.Update(updatedBook);
                return mapper.Map<BookDTO>(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating Book");
                throw new Exception("Error updating Book", ex);
            }
        }

        public async Task<InvoiceDTO> CheckOut(int invoiceId)
            {
            // Retrieve the invoice
            var invoice = await dataContext.Invoices
                                           .Where(i => i.InvoiceId == invoiceId)
                                           .Include(b=>b.book)
                                           .FirstOrDefaultAsync();
            invoice.Status = "Paid";

            if (invoice == null)
            {
                throw new KeyNotFoundException($"Invoice with ID {invoiceId} not found.");
            }

            // Retrieve the room associated with the booking
            var room = await dataContext.Rooms
                                        .Where(r => r.RoomID == invoice.book.RoomId)
                                        .FirstOrDefaultAsync();


            // Update room status to "Available"
            room.Status = "Available";
            await dataContext.SaveChangesAsync();

            // Create and return InvoiceDTO
            var invoiceDTO = new InvoiceDTO
            {
                InvoiceId = invoice.InvoiceId,
                BookId = invoice.BookId,
                Price = invoice.Price,
                Status = invoice.Status,
                book = mapper.Map<BookDTO>(invoice.book)
            };

            return invoiceDTO;
        }

        public async Task<int> BookCount()
        {
            return await dataContext.Books.CountAsync();
        }

        

    }
}
