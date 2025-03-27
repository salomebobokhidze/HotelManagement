using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.DTOs
{
    public class UpdateHotelDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Hotel name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City name must be between 2 and 100 characters")]
        public string City { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Country name must be between 2 and 100 characters")]
        public string Country { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; }
    }
}