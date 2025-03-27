using System;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.DTOs
{
    public class UpdateReservationDTO
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckOutDate { get; set; }

        public bool ValidateDates()
        {
            return CheckInDate < CheckOutDate &&
                   CheckInDate >= DateTime.Today;
        }
    }
}