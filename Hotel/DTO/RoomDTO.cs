using AutoMapper;
using Hotel.Data;

namespace Hotel.DTO
{
    [AutoMap(typeof(Room), ReverseMap = true)]

    public class RoomDTO
    {

        public int RoomID { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public DateTime? ChickInDate { get; set; }
        public DateTime? ChickOutDate { get; set; }

        public List<Book> ? Books { get; set; }
    }
}
