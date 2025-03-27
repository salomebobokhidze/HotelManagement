using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Core.Entities
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100,000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        // Initialize collection to prevent null reference issues
        public Room()
        {
            Reservations = new HashSet<Reservation>();
        }

        public ICollection<Reservation> Reservations { get; set; }

        // Method to check room availability
        public bool IsRoomAvailable(DateTime checkInDate, DateTime checkOutDate)
        {
            return Reservations == null ||
                   !Reservations.Any(r =>
                       (checkInDate < r.CheckOutDate && checkOutDate > r.CheckInDate));
        }
    }
}