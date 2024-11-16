using Hotel.DTO;

namespace Hotel.Service
{
    public interface IGuestService
    {
        Task<GuestDTO> AddGuest(GuestDTO guest);
        Task DeleteGuest(int id);
        //Task<List<GuestDTO>> GetAllGuests();
        Task<PaginatedGuestResultDTO> GetAllGuests(int? PageSize, int? PageNumber);
        Task<GuestDTO> GetGuest(int id);
        Task<GuestDTO> UpdateGuest(GuestDTO guest);
        Task<int> GuestCount();
    }
}