namespace Hotel.Data
{
    public class Room
    {
        public int RoomID {  get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }

        public List<Book> Books { get; set; }

    }
}
