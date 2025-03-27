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

       
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            try
            {
                
                var result = await _hotelService.CreateHotelAsync(hotelDTO);

                
                var response = new ApiResponse("Hotel created successfully", result, 201, true);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                
                var response = new ApiResponse("An error occurred while creating the hotel", null, 500, false);
                return StatusCode(response.StatusCode, response);
            }
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            try
            {
                
                await _hotelService.UpdateHotelAsync(id, hotelDTO);

                
                var response = new ApiResponse("Hotel updated successfully", hotelDTO, 200, true);
                return StatusCode(response.StatusCode, response);
            }
            catch (ArgumentException ex)
            {
                
                var response = new ApiResponse(ex.Message, null, 400, false);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                
                var response = new ApiResponse("An error occurred while updating the hotel", null, 500, false);
                return StatusCode(500, response);
            }
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                
                var result = await _hotelService.DeleteHotelAsync(id);
                if (!result)
                {
                    
                    var response = new ApiResponse("Unable to delete hotel; it may have active rooms or bookings.", null, 400, false);
                    return BadRequest(response);
                }

               
                var successResponse = new ApiResponse("Hotel deleted successfully", null, 204, true);
                return StatusCode(successResponse.StatusCode, successResponse);
            }
            catch (Exception ex)
            {
                
                var response = new ApiResponse("An error occurred while deleting the hotel", null, 500, false);
                return StatusCode(500, response);
            }
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelById(int id)
        {
            try
            {
                
                var hotel = await _hotelService.GetHotelByIdAsync(id);
                if (hotel == null)
                {
                    
                    var response = new ApiResponse("Hotel not found", null, 404, false);
                    return NotFound(response);
                }

               
                var responseSuccess = new ApiResponse("Hotel found", hotel, 200, true);
                return Ok(responseSuccess);
            }
            catch (Exception ex)
            {
                
                var response = new ApiResponse("An error occurred while retrieving the hotel", null, 500, false);
                return StatusCode(500, response);
            }
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDTO>>> GetAllHotels([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string filter = null)
        {
            try
            {
               
                var hotels = await _hotelService.GetAllHotelsAsync(page, pageSize, filter);

                
                var responseSuccess = new ApiResponse("Hotels list", hotels, 200, true);
                return Ok(responseSuccess);
            }
            catch (Exception ex)
            {
                
                var response = new ApiResponse("An error occurred while retrieving the hotels list", null, 500, false);
                return StatusCode(500, response);
            }
        }
    }
}
