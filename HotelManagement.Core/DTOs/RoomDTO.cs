using System.ComponentModel.DataAnnotations;
using HotelManagement.Core.Entities;

namespace HotelManagement.Core.DTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        public bool IsAvailable { get; set; }

        [Range(1, 100000)]
        public decimal Price { get; set; }

        public int HotelId { get; set; }


        public RoomDTO(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            Id = room.Id;
            Name = room.Name;
            IsAvailable = room.IsAvailable;
            Price = room.Price;
            HotelId = room.HotelId;
        }

       
    }
}