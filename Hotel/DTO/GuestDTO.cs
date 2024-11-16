using AutoMapper;
using Hotel.Data;
using Hotel.Generic;

namespace Hotel.DTO
{
    [AutoMap(typeof(Guest), ReverseMap = true)]
    public class GuestDTO
    {
        public int GuestId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public List<BookDTO> ? Books { get; set; }
    }
}
