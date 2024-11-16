using Hotel.DTO;

namespace Hotel.Service
{
    public interface IRoomService
    {
        Task<RoomDTO> AddRoom(RoomDTO Room);
        Task DeleteRoom(int id);
        Task<List<RoomDTO>> GetAllRooms();
        Task<RoomDTO> GetRoom(int id);
        Task<RoomDTO> UpdateRoom(RoomDTO Room);
        Task<int> RoomCount();
        Task<List<RoomDTO>> IsAvilableRoom(int roomId);
    }
}