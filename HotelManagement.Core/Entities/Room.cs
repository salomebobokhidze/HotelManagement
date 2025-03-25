using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Core.Entities
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}


