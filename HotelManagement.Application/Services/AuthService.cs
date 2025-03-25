using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelManagement.Core.DTOs;
using HotelManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace HotelManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Guest> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<Guest> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                return new AuthResponseDto { Success = false, Message = "User already exists" };

            var user = new Guest
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                PersonalNumber = registerDto.PersonalNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };

            var roleExists = await _roleManager.RoleExistsAsync("Guest");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("Guest"));
            }

            await _userManager.AddToRoleAsync(user, "Guest");

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully",
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(CreateGuestDTO createGuest)
        {
            var userExists = await _userManager.FindByEmailAsync(createGuest.Email);
            if (userExists != null)
                return new AuthResponseDto { Success = false, Message = "User already exists" };

            var user = new Guest
            {
                UserName = createGuest.Email,
                Email = createGuest.Email,
                FirstName = createGuest.FirstName,
                LastName = createGuest.LastName,
                PhoneNumber = createGuest.PhoneNumber,
                PersonalNumber = createGuest.PersonalNumber
            };

            var result = await _userManager.CreateAsync(user, createGuest.Password);
            if (!result.Succeeded)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };

            var roleExists = await _roleManager.RoleExistsAsync("Guest");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("Guest"));
            }

            await _userManager.AddToRoleAsync(user, "Guest");

            return new AuthResponseDto
            {
                Success = true,
                Message = "Guest registered successfully",
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return new AuthResponseDto { Success = false, Message = "Invalid credentials" };

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token (you'll need to implement this)
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            if (principal == null)
                return new AuthResponseDto { Success = false, Message = "Invalid token" };

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return new AuthResponseDto { Success = false, Message = "Invalid refresh token" };

            var newToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Token = newToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task RevokeTokenAsync(RevokeTokenDto revokeTokenDto)
        {
            var user = await _userManager.FindByIdAsync(revokeTokenDto.UserId);
            if (user == null) return;

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task<CurrentUserDTO> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            var appUser = await _userManager.FindByIdAsync(userIdClaim.Value);

            if (appUser == null) return null;

            return new CurrentUserDTO
            {
                Id = appUser.Id,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName
            };
        }

        private string GenerateJwtToken(Guest user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles if needed
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"))),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}