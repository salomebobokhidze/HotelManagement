using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Core.Entities
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        
        public string ManagerId { get; set; }

        
        public Manager Manager { get; set; }

        
        public Hotel()
        {
            Rooms = new HashSet<Room>();
            Reservations = new HashSet<Reservation>();
        }

        public ICollection<Room> Rooms { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}