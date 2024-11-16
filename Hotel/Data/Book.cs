using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Data
{
    public class Book
    {
        public int BookId { get; set; }

        [ForeignKey("Guest")]
        public int GuestId { get; set; }

        [ForeignKey("Room")]
        public int RoomId   { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public Room room { get; set; }
        public Guest guest { get; set; }

        public Invoice invoices { get; set; }

    }
}
