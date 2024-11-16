using AutoMapper;
using Hotel.Context;
using Hotel.Data;
using Hotel.DTO;
using Hotel.Generic;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Hotel.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IGenric<Invoice> genric;
        private readonly IMapper mapper;
        private readonly DataContext dataContext;
        private readonly ILogger<InvoiceService> logger;

        public InvoiceService(IGenric<Invoice> _genric, IMapper _mapper, DataContext _dataContext, ILogger<InvoiceService> _logger)
        {
            genric = _genric;
            mapper = _mapper;
            dataContext = _dataContext;
            logger = _logger;
        }

        // Add a new Invoice
        public async Task<InvoiceDTO> AddInvoice(InvoiceDTO Invoice)
        {
            if (Invoice == null)
            {
                throw new ArgumentNullException(nameof(Invoice), "Invoice data is required");
            }

            try
            {
                // Check if an invoice already exists for the given BookId
                bool invoiceExists = await dataContext.Invoices.AnyAsync(i => i.BookId == Invoice.BookId);
                if (invoiceExists)
                {
                    throw new InvalidOperationException("An invoice for this BookId already exists.");
                }

                // Retrieve the Book with related Room data
                Book book = await dataContext.Books.Include(b => b.room).FirstOrDefaultAsync(b => b.BookId == Invoice.BookId);
                if (book == null)
                {
                    throw new InvalidOperationException("Book not found.");
                }

                double pricePerDay = book.room.Price;
                int days = (book.CheckOutDate - book.CheckInDate).Days ;
                double price = pricePerDay * days;

                Invoice newInvoice = new Invoice()
                {
                    BookId = Invoice.BookId,
                    Price = price,
                    Status = Invoice.Status,
                };

                await genric.Add(newInvoice);
                return mapper.Map<InvoiceDTO>(newInvoice);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding an Invoice");
                throw new Exception("Error adding Invoice", ex);
            }
        }

        public async Task<double> Totalprice(int BookId)
        {
            Book book =  await dataContext.Books.Include(b => b.room).FirstOrDefaultAsync(b=>b.BookId == BookId);

            double priceperday = book.room.Price;

            int Days = (book.CheckOutDate - book.CheckInDate).Days;
              
            return priceperday * Days;
        }

        // Get a Invoice by ID
        public async Task<InvoiceDTO> GetInvoice(int id)
        {
            try
            {
                var Invoice = await dataContext.Invoices.Where(e => e.InvoiceId == id).Include("book").FirstOrDefaultAsync();
                return mapper.Map<InvoiceDTO>(Invoice);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving Invoice with ID {id}");
                throw new Exception("Error retrieving Invoice", ex);
            }
        }

        // Get all Invoices
        public async Task<List<InvoiceDTO>> GetAllInvoices()
        {
            try
            {
                var Invoices = await  dataContext.Invoices.Include(u => u.book).ToListAsync();
                return mapper.Map<List<InvoiceDTO>>(Invoices);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all Invoices");
                throw new Exception("Error retrieving Invoices", ex);
            }
        }

        public async Task<List<InvoiceDTO>> GetAllInvoicesForGuest(int guestId)
        {
            try
            {
                var Invoices = await dataContext.Invoices.Include(u => u.book).Where(i=>i.book.GuestId == guestId).ToListAsync();
                return mapper.Map<List<InvoiceDTO>>(Invoices);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all Invoices");
                throw new Exception("Error retrieving Invoices", ex);
            }
        }


        // Delete a Invoice by ID
        public async Task DeleteInvoice(int id)
        {
            try
            {
                await genric.Remove(id);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, $"Invoice with ID {id} not found for deletion");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while deleting Invoice with ID {id}");
                throw new Exception("Error deleting Invoice", ex);
            }
        }

        // Update an existing Invoice
        public async Task<InvoiceDTO> UpdateInvoice(InvoiceDTO Invoice)
        {
            if (Invoice == null)
            {
                throw new ArgumentNullException(nameof(Invoice), "Invoice data is required for update");
            }

            try
            {
                Invoice updatedInvoice = mapper.Map<Invoice>(Invoice);
                var result = await genric.Update(updatedInvoice);
                return mapper.Map<InvoiceDTO>(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating Invoice");
                throw new Exception("Error updating Invoice", ex);
            }
        }

        public async Task<double> TotalIncome()
        {
            return await dataContext.Invoices.Where(i=>i.Status == "Paid").SumAsync(i => i.Price);
        }

    }
}
