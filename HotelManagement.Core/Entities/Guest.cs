using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.Entities
{
    public class Guest : IdentityUser
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [MaxLength(11)]
        public string PersonalNumber { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
       
    }
}
