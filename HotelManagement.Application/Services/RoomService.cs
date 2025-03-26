using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.Core.DTOs;
using HotelManagement.Core.Entities;
using HotelManagement.Core.Interfaces;
using HotelManagement.Infrastructure.Repositories;

namespace HotelManagement.Core.Services
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _roomRepository;

        public RoomService(IGenericRepository<Room> roomRepository)
        {
            _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
        }

        public async Task<RoomDTO> CreateRoomAsync(CreateRoomDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var room = new Room
            {
                Name = dto.Name,
                IsAvailable = dto.IsAvailable,
                Price = dto.Price,
                HotelId = dto.HotelId
            };

            await _roomRepository.AddAsync(room);
            return new RoomDTO(room);
        }

        public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return ConvertToRoomDTOs(rooms);
        }

        public async Task<bool> UpdateRoomAsync(int id, UpdateRoomDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return false;

            room.Name = dto.Name;
            room.IsAvailable = dto.IsAvailable;
            room.Price = dto.Price;

            await _roomRepository.UpdateAsync(room);
            return true;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid room ID", nameof(id));

            return await _roomRepository.DeleteAsync(id);
        }

        // Method to get all rooms with filters applied manually
        public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync(int? hotelId, bool? isAvailable, decimal? minPrice, decimal? maxPrice)
        {
            var rooms = await _roomRepository.GetAllAsync();

            List<Room> filteredRooms = new List<Room>();

            foreach (var room in rooms)
            {
                bool matches = true;

                // Apply each filter condition manually
                if (hotelId.HasValue && room.HotelId != hotelId.Value)
                    matches = false;

                if (isAvailable.HasValue && room.IsAvailable != isAvailable.Value)
                    matches = false;

                if (minPrice.HasValue && room.Price < minPrice.Value)
                    matches = false;

                if (maxPrice.HasValue && room.Price > maxPrice.Value)
                    matches = false;

                // If the room matches all conditions, add it to the filtered list
                if (matches)
                {
                    filteredRooms.Add(room);
                }
            }

            return ConvertToRoomDTOs(filteredRooms);
        }

        // Method to get a room by its ID
        public async Task<RoomDTO> GetRoomByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid room ID", nameof(id));

            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                throw new KeyNotFoundException("Room not found");

            return new RoomDTO(room);
        }

        // Helper method to convert rooms to DTOs
        private IEnumerable<RoomDTO> ConvertToRoomDTOs(IEnumerable<Room> rooms)
        {
            List<RoomDTO> roomDTOs = new List<RoomDTO>();

            foreach (var room in rooms)
            {
                roomDTOs.Add(new RoomDTO(room));
            }

            return roomDTOs;
        }
    }
}
