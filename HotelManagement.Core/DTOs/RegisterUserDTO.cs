using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.DTOs
{
 
        public class RegisterUserDto
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Phone]
            public string PhoneNumber { get; set; }

            [Required]
            public string PersonalNumber { get; set; }

            [Required]
            
            public string Password { get; set; }
        }
    }
