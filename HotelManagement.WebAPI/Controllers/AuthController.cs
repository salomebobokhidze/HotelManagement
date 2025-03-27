using HotelManagement.Core.DTOs;
using HotelManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using HotelManagement.Application.Services;

namespace HotelManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

       
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthResponseDto))]
        public async Task<IActionResult> Register([FromBody] CreateGuestDTO guestDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid registration data received");
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid request data"
                    });
                }

                var result = await _authService.RegisterAsync(guestDTO);

                if (!result.Success)
                {
                    _logger.LogWarning("Registration failed: {Message}", result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("New guest registered: {Email}", guestDTO.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during guest registration");
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred during registration"
                });
            }
        }

        
        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]  
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid admin registration data received");
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid request data"
                    });
                }

                var result = await _authService.RegisterAdminAsync(registerDto);

                if (!result.Success)
                {
                    _logger.LogWarning("Admin registration failed: {Message}", result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("New admin registered: {Email}", registerDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin registration");
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred during admin registration"
                });
            }
        }

        
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(AuthResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid login data received");
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid request data"
                    });
                }

                var result = await _authService.LoginAsync(loginDto);

                if (!result.Success)
                {
                    _logger.LogWarning("Login failed for user: {Email}", loginDto.Email);
                    return Unauthorized(result);
                }

                _logger.LogInformation("User logged in: {Email}", loginDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred during login"
                });
            }
        }

        [HttpGet("current-user")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrentUserDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync(User);

                if (user == null)
                {
                    _logger.LogWarning("Current user not found");
                    return Unauthorized();
                }

                _logger.LogInformation("Current user retrieved: {Email}", user.Email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user");
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving user data"
                });
            }
        }
    }
}
