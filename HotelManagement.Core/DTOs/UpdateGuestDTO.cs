﻿using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.DTOs
{
    public class UpdateGuestDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        public string LastName { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 20 characters")]
        public string PhoneNumber { get; set; }
    }
}