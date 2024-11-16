using AutoMapper;
using Hotel.Context;
using Hotel.Data;
using Hotel.DTO;
using Hotel.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Hotel.Service
{
    public class GuestService : IGuestService
    {
        private readonly IGenric<Guest> genric;
        private readonly IMapper mapper;
        private readonly DataContext dataContext;
        private readonly ILogger<GuestService> logger;

        public GuestService(IGenric<Guest> _genric, IMapper _mapper, DataContext _dataContext, ILogger<GuestService> _logger)
        {
            genric = _genric;
            mapper = _mapper;
            dataContext = _dataContext;
            logger = _logger;
        }

        // Add a new guest
        public async Task<GuestDTO> AddGuest(GuestDTO guest)
        {
            if (guest == null)
            {
                throw new ArgumentNullException(nameof(guest), "Guest data is required");
            }

            try
            {
                Guest newGuest = mapper.Map<Guest>(guest);
                await genric.Add(newGuest);
                return mapper.Map<GuestDTO>(newGuest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding a guest");
                throw new Exception("Error adding guest", ex);
            }
        }

        // Get a guest by ID
        public async Task<GuestDTO> GetGuest(int id)
        {
            try
            {
                var guest = await genric.Get(id);
                return mapper.Map<GuestDTO>(guest);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving guest with ID {id}");
                throw new Exception("Error retrieving guest", ex);
            }
        }

        // Get all guests
        public async Task<PaginatedGuestResultDTO>  GetAllGuests(int? PageSize, int? PageNumber)
        {
            try
            {
                int size = (PageSize.HasValue && PageSize.Value > 0) ? PageSize.Value : 100; 
                int page = (PageNumber.HasValue && PageNumber.Value > 0) ? PageNumber.Value : 1;

                var guests = dataContext.Guests.AsQueryable();
                var PageGuests = guests
                                 .Skip((page - 1) * size)
                                 .Take(size)
                                 .ToList();
                int count = dataContext.Guests.Count();
                bool hasnext = false;
                if (count > page  * size)
                {
                    hasnext = true;
                }
                else
                    hasnext = false;

                return new PaginatedGuestResultDTO
                {
                    Guests = mapper.Map<List<GuestDTO>>(PageGuests),
                    HasNext = hasnext
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all guests");
                throw new Exception("Error retrieving guests", ex);
            }
        }

        // Delete a guest by ID
        public async Task DeleteGuest(int id)
        {
            try
            {
                await genric.Remove(id);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, $"Guest with ID {id} not found for deletion");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while deleting guest with ID {id}");
                throw new Exception("Error deleting guest", ex);
            }
        }

        // Update an existing guest
        public async Task<GuestDTO> UpdateGuest(GuestDTO guest)
        {
            if (guest == null)
            {
                throw new ArgumentNullException(nameof(guest), "Guest data is required for update");
            }

            try
            {
                Guest updatedGuest = mapper.Map<Guest>(guest);
                var result = await genric.Update(updatedGuest);
                return mapper.Map<GuestDTO>(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating guest");
                throw new Exception("Error updating guest", ex);
            }
        }

        public async Task<int> GuestCount()
        {
            return await dataContext.Guests.CountAsync();
        }
    }
}
