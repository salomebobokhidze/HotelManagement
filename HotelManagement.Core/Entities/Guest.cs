using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Core.Entities
{
    public class Guest : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Personal Number must be 11 digits")]
        public string PersonalNumber { get; set; }

        // Consider initializing the collection in the constructor
        public Guest()
        {
            Reservations = new HashSet<Reservation>();
        }

        public ICollection<Reservation> Reservations { get; set; }
    }
}