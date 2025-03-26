using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HotelManagement.Core.DTOs;
using HotelManagement.Core.Entities;
using HotelManagement.Core.Interfaces;
using HotelManagement.Infrastructure.Repositories;

namespace HotelManagement.Core.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;

        public GuestService(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public async Task<GuestDTO> CreateGuestAsync(CreateGuestDTO dto)
        {
            var guest = new Guest
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                PersonalNumber = dto.PersonalNumber
            };

            await _guestRepository.AddAsync(guest);
            return new GuestDTO(guest);
        }

        public async Task<bool> UpdateGuestAsync(int id, UpdateGuestDTO dto)
        {
            var guest = await _guestRepository.GetByIdAsync(id);
            if (guest == null) return false;

            guest.FirstName = dto.FirstName;
            guest.LastName = dto.LastName;
            guest.PhoneNumber = dto.PhoneNumber;

            await _guestRepository.UpdateAsync(guest);
            return true;
        }

        public async Task<bool> DeleteGuestAsync(int id)
        {
            return await _guestRepository.DeleteAsync(id);
        }

        public async Task<GuestDTO> GetGuestByIdAsync(int id)
        {
            var guest = await _guestRepository.GetByIdAsync(id);

            if (guest == null)
                return null;

            
            return new GuestDTO(guest);
        }

    }
}