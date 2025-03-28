﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using HotelManagement.Core.DTOs;
using HotelManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Guest> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<Guest> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        
        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
        {
            return await RegisterGuestAsync(registerDto);
        }

        
        public async Task<AuthResponseDto> RegisterAsync(CreateGuestDTO createGuest)
        {
            return await RegisterGuestAsync(createGuest);
        }

        
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            return await LoginUserAsync(loginDto);
        }

        public async Task<AuthResponseDto> RegisterGuestAsync(RegisterUserDto registerDto)
        {
            
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists"
                };

            
            var personalNumberExists = await _userManager.Users
                .AnyAsync(u => u.PersonalNumber == registerDto.PersonalNumber);
            if (personalNumberExists)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Personal number already registered"
                };

            var user = new Guest
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                PersonalNumber = registerDto.PersonalNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };

            if (!await _roleManager.RoleExistsAsync("Guest"))
                await _roleManager.CreateAsync(new IdentityRole("Guest"));

            await _userManager.AddToRoleAsync(user, "Guest");

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully",
                Token = GenerateJwtToken(user),
                Roles = roles.ToList()
            };
        }

        public async Task<AuthResponseDto> RegisterGuestAsync(CreateGuestDTO createGuest)
        {
            
            var userExists = await _userManager.FindByEmailAsync(createGuest.Email);
            if (userExists != null)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already exists"
                };

            
            var personalNumberExists = await _userManager.Users
                .AnyAsync(u => u.PersonalNumber == createGuest.PersonalNumber);
            if (personalNumberExists)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Personal number already registered"
                };

            var user = new Guest
            {
                UserName = createGuest.Email,
                Email = createGuest.Email,
                FirstName = createGuest.FirstName,
                LastName = createGuest.LastName,
                PhoneNumber = createGuest.PhoneNumber,
                PersonalNumber = createGuest.PersonalNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, createGuest.Password);
            if (!result.Succeeded)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };

            if (!await _roleManager.RoleExistsAsync("Guest"))
                await _roleManager.CreateAsync(new IdentityRole("Guest"));

            await _userManager.AddToRoleAsync(user, "Guest");

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Guest registered successfully",
                Token = GenerateJwtToken(user),
                Roles = roles.ToList()
            };
        }

        public async Task<AuthResponseDto> RegisterAdminAsync(RegisterUserDto registerDto)
        {
            var result = await RegisterGuestAsync(registerDto);
            if (!result.Success)
                return result;

            var user = await _userManager.FindByEmailAsync(registerDto.Email);

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            await _userManager.AddToRoleAsync(user, "Admin");

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Admin registered successfully",
                Token = GenerateJwtToken(user),
                Roles = roles.ToList()
            };
        }

        public async Task<AuthResponseDto> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials"
                };

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                Message = "Login successful",
                Roles = roles.ToList()
            };
        }

        public async Task<CurrentUserDTO> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return null;

            var appUser = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (appUser == null) return null;

            var roles = await _userManager.GetRolesAsync(appUser);

            return new CurrentUserDTO
            {
                Id = appUser.Id,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Roles = roles.ToList()
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

            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(_configuration["Jwt:ExpirationInMinutes"] ?? "60")),
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
                    _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"))),
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