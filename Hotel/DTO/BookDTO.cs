using AutoMapper;
using Hotel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.DTO
{
    [AutoMap(typeof(Book), ReverseMap = true)]
    public class BookDTO
    {
        public int ? BookId { get; set; }

        public int ? GuestId { get; set; }

        public int ? RoomId { get; set; }
        public DateTime ? CheckInDate { get; set; }
        public DateTime ? CheckOutDate { get; set; }

        //public Room ? room { get; set; }
        //public GuestDTO ? guest { get; set; }

        //public Invoice ? invoices { get; set; }

        public string ? name { get; set; } // guest name
        public double ? price { get; set; } // price from invoice
        public string ? status { get; set; } // status from invoice

    }

}
