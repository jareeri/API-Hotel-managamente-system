namespace Hotel.DTO
{
    public class PaginatedGuestResultDTO
    {
        public List<GuestDTO> Guests { get; set; }
        public bool HasNext { get; set; }
    }
}
