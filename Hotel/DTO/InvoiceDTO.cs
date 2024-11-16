using AutoMapper;
using Hotel.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hotel.DTO
{
    [AutoMap(typeof(Invoice), ReverseMap = true)]

    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }

        public int BookId { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }

        //[JsonIgnore]
        public BookDTO ? book { get; set; }
    }
}
