using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelManagement.Core.Entities;

namespace HotelManagement.Infrastructure.Repositories
{
    
        public interface IHotelRepository
        {
            Task<IEnumerable<Hotel>> GetAllAsync();
            Task<Hotel> GetByIdAsync(int id);
            Task AddAsync(Hotel hotel);
            Task UpdateAsync(Hotel hotel);
            Task<bool> DeleteAsync(int id);
            Task<IEnumerable<Hotel>> GetFilteredHotelsAsync(string filter); 
           
        }
    }

