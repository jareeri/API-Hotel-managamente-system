namespace Hotel.Data
{
    public class Guest
    {
        public int GuestId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public List<Book> Books { get; set; }
    }
}
