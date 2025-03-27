using HotelManagement.Core.DTOs;
using HotelManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelManagement.API.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
        }

        // POST: api/hotels
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            try
            {
                // Create hotel and get the result
                var result = await _hotelService.CreateHotelAsync(hotelDTO);

                // Return response with 201 status and success message
                var response = new ApiResponse("Hotel created successfully", result, 201, true);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                // Handle errors and return a 500 error if something goes wrong
                var response = new ApiResponse("An error occurred while creating the hotel", null, 500, false);
                return StatusCode(response.StatusCode, response);
            }
        }

        // PUT: api/hotels/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            try
            {
                // Update hotel details
                await _hotelService.UpdateHotelAsync(id, hotelDTO);

                // Return success response with 200 status
                var response = new ApiResponse("Hotel updated successfully", hotelDTO, 200, true);
                return StatusCode(response.StatusCode, response);
            }
            catch (ArgumentException ex)
            {
                // Return bad request response if hotel doesn't exist
                var response = new ApiResponse(ex.Message, null, 400, false);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                // Handle other errors and return 500 internal server error
                var response = new ApiResponse("An error occurred while updating the hotel", null, 500, false);
                return StatusCode(500, response);
            }
        }

        // DELETE: api/hotels/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                // Attempt to delete hotel
                var result = await _hotelService.DeleteHotelAsync(id);
                if (!result)
                {
                    // Return bad request if hotel cannot be deleted
                    var response = new ApiResponse("Unable to delete hotel; it may have active rooms or bookings.", null, 400, false);
                    return BadRequest(response);
                }

                // Return success response with 204 No Content
                var successResponse = new ApiResponse("Hotel deleted successfully", null, 204, true);
                return StatusCode(successResponse.StatusCode, successResponse);
            }
            catch (Exception ex)
            {
                // Handle errors and return 500 error
                var response = new ApiResponse("An error occurred while deleting the hotel", null, 500, false);
                return StatusCode(500, response);
            }
        }

        // GET: api/hotels/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelById(int id)
        {
            try
            {
                // Retrieve hotel by ID
                var hotel = await _hotelService.GetHotelByIdAsync(id);
                if (hotel == null)
                {
                    // Return Not Found response if hotel doesn't exist
                    var response = new ApiResponse("Hotel not found", null, 404, false);
                    return NotFound(response);
                }

                // Return hotel details with 200 OK response
                var responseSuccess = new ApiResponse("Hotel found", hotel, 200, true);
                return Ok(responseSuccess);
            }
            catch (Exception ex)
            {
                // Handle errors and return 500 error
                var response = new ApiResponse("An error occurred while retrieving the hotel", null, 500, false);
                return StatusCode(500, response);
            }
        }

        // GET: api/hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDTO>>> GetAllHotels([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string filter = null)
        {
            try
            {
                // Retrieve filtered hotels based on query parameters
                var hotels = await _hotelService.GetAllHotelsAsync(page, pageSize, filter);

                // Return list of hotels with 200 OK response
                var responseSuccess = new ApiResponse("Hotels list", hotels, 200, true);
                return Ok(responseSuccess);
            }
            catch (Exception ex)
            {
                // Handle errors and return 500 error
                var response = new ApiResponse("An error occurred while retrieving the hotels list", null, 500, false);
                return StatusCode(500, response);
            }
        }
    }
}
