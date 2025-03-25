using HotelManagement.Core.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HotelManagement.Application.Services
{
    public interface IAuthService
    {
        
            Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto);
            Task<AuthResponseDto> RegisterAsync(CreateGuestDTO createGuest);
            Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
            Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
            Task RevokeTokenAsync(RevokeTokenDto revokeTokenDto);
            Task<CurrentUserDTO> GetCurrentUserAsync(ClaimsPrincipal user);
        }
    }


