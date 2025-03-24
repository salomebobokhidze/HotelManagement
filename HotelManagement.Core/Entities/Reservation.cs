using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public string GuestId { get; set; }
        public Guest Guest { get; set; }

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
