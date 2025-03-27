using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.Core.Entities;
using HotelManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Infrastructure.Repositories
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        private readonly AppDbContext _hotelContext;

        public HotelRepository(AppDbContext hotelContext) : base(hotelContext)
        {
            _hotelContext = hotelContext ?? throw new ArgumentNullException(nameof(hotelContext));
        }

        // Save changes to the database
        public async Task SaveAsync()
        {
            await _hotelContext.SaveChangesAsync();
        }

        // Get filtered hotels based on the provided filter
        public async Task<IEnumerable<Hotel>> GetFilteredHotelsAsync(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("Filter cannot be null or empty", nameof(filter));

            return await _hotelContext.Hotels
                .Where(h => h.Name.Contains(filter) || h.City.Contains(filter))
                .ToListAsync();
        }
    }
}
