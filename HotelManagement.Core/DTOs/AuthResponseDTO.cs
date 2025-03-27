namespace HotelManagement.Core.DTOs
{
    public class AuthResponseDto
    {
        // Indicates if the operation was successful (e.g., registration, login)
        public bool Success { get; set; }

        // Message providing details about the operation result (e.g., error message, success message)
        public string Message { get; set; }

        // The JWT token issued after login (can be null if not applicable)
        public string? Token { get; set; }

        // A list of roles associated with the user (e.g., "Admin", "Guest", etc.)
        public List<string> Roles { get; set; } = new List<string>();
        public string UserEmail { get; set; }
    }
}
