namespace HotelManagement.Core.DTOs
{
    public class AuthResponseDto
    {
       
        public bool Success { get; set; }

        
        public string Message { get; set; }

        
        public string? Token { get; set; }

        public string RefreshToken { get; set; }
        
        public List<string> Roles { get; set; } = new List<string>();
        public string UserEmail { get; set; }
    }
}
