using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.DTOs
{
    public class UpdateRoomDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Room name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100,000")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}