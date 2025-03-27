using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HotelManagement.Core.Entities
{
    public class Manager : IdentityUser<int>  // Use int-based Identity key
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

        // Override Email from IdentityUser with additional validation
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public override string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public override string PhoneNumber { get; set; }

        // Navigation property for Hotel
        public int? HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}