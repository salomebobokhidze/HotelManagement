using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Core.DTOs
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public  string Message { get; set; }
        public  string Token { get; set; }
        
    }

}
