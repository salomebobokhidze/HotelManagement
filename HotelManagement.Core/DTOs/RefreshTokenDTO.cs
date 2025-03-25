using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Core.DTOs
{
    public class RefreshTokenDto
    {
       
            public string Token { get; set; }
            public string RefreshToken { get; set; }
        }
    }

