using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.DTOs
{
    public class CreateReservationDTO
    {
        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public string GuestId { get; set; }

        [Required]
        public int HotelId { get; set; }
    }
}
