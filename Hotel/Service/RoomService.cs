using AutoMapper;
using Hotel.Context;
using Hotel.Data;
using Hotel.DTO;
using Hotel.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Service
{
    public class RoomService : IRoomService
    {
        private readonly IGenric<Room> genric;
        private readonly IMapper mapper;
        private readonly DataContext dataContext;
        private readonly ILogger<RoomService> logger;

        public RoomService(IGenric<Room> _genric, IMapper _mapper, DataContext _dataContext, ILogger<RoomService> _logger)
        {
            genric = _genric;
            mapper = _mapper;
            dataContext = _dataContext;
            logger = _logger;
        }

        // Add a new Room
        public async Task<RoomDTO> AddRoom(RoomDTO Room)
        {
            if (Room == null)
            {
                throw new ArgumentNullException(nameof(Room), "Room data is required");
            }

            try
            {
                Room newRoom = mapper.Map<Room>(Room);
                await genric.Add(newRoom);
                return mapper.Map<RoomDTO>(newRoom);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding a Room");
                throw new Exception("Error adding Room", ex);
            }
        }

        // Get a Room by ID
        public async Task<RoomDTO> GetRoom(int id)
        {
            try
            {
                var Room = await genric.Get(id);
                return mapper.Map<RoomDTO>(Room);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving Room with ID {id}");
                throw new Exception("Error retrieving Room", ex);
            }
        }

        // Get all Rooms
        public async Task<List<RoomDTO>> GetAllRooms()
        {
            try
            {
                var Rooms = await genric.GetAll();
                return mapper.Map<List<RoomDTO>>(Rooms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all Rooms");
                throw new Exception("Error retrieving Rooms", ex);
            }
        }

        // Delete a Room by ID
        public async Task DeleteRoom(int id)
        {
            try
            {
                await genric.Remove(id);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, $"Room with ID {id} not found for deletion");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while deleting Room with ID {id}");
                throw new Exception("Error deleting Room", ex);
            }
        }

        // Update an existing Room
        public async Task<RoomDTO> UpdateRoom(RoomDTO Room)
        {
            if (Room == null)
            {
                throw new ArgumentNullException(nameof(Room), "Room data is required for update");
            }

            try
            {
                Room updatedRoom = mapper.Map<Room>(Room);
                var result = await genric.Update(updatedRoom);
                return mapper.Map<RoomDTO>(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating Room");
                throw new Exception("Error updating Room", ex);
            }
        }


        public async Task<int> RoomCount()
        {
            return await dataContext.Rooms.CountAsync();
        }

        public async Task<List<RoomDTO>> IsAvilableRoom(int roomId)
        {
            var rooms = await dataContext.Books.Select(r=> new RoomDTO
            {
                RoomID = r.RoomId,
                ChickInDate=r.CheckInDate,
                ChickOutDate=r.CheckOutDate,

            }).Where(r=>r.RoomID == roomId).ToListAsync();
            return rooms;
        }
    }
}
