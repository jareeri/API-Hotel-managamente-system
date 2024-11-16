using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hotel.Data
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        [ForeignKey("Book")]
        public int BookId   { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }

        //[JsonIgnore]
        public Book book { get; set; }
    }
}
