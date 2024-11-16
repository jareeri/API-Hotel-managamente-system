using Hotel.DTO;

namespace Hotel.Service
{
    public interface IBookService
    {
        Task<BookDTO> AddBook(BookDTO Book);
        Task DeleteBook(int id);
        Task<List<BookDTO>> GetAllBooks();
        Task<BookDTO> GetBook(int id);
        Task<BookDTO> UpdateBook(BookDTO Book);
        Task<InvoiceDTO> CheckOut(int invoiceId);
        Task<List<BookDTO>> GetAllBooksForGuest(int guestId);


        Task<List<BookDTO>>
            GetAllBooksSearch(string? GuestName, string? Status,
                              DateTime? CheckInDate, DateTime? CheckOutDate);

        Task<int> BookCount();
    }
}