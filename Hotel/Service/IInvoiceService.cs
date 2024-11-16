using Hotel.DTO;

namespace Hotel.Service
{
    public interface IInvoiceService
    {
        Task<InvoiceDTO> AddInvoice(InvoiceDTO Invoice);
        Task DeleteInvoice(int id);
        Task<List<InvoiceDTO>> GetAllInvoices();
        Task<InvoiceDTO> GetInvoice(int id);
        Task<InvoiceDTO> UpdateInvoice(InvoiceDTO Invoice);
        Task<List<InvoiceDTO>> GetAllInvoicesForGuest(int guestId);

        Task<double> TotalIncome();
    }
}