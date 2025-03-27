using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Core.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckOutDate { get; set; }

        // Foreign key for Room
        public int RoomId { get; set; }
        public Room Room { get; set; }

        // Foreign key for Guest
        public string GuestId { get; set; }
        public Guest Guest { get; set; }

        // Foreign key for Hotel
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        // Custom validation method
        public bool ValidateDates()
        {
            return CheckInDate < CheckOutDate &&
                   CheckInDate >= DateTime.Today;
        }
    }
}