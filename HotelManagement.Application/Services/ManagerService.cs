using HotelManagement.Application.Services;
using HotelManagement.Core.DTOs;
using HotelManagement.Core.Entities;
using HotelManagement.Core.Interfaces;
using HotelManagement.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Core.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IGenericRepository<Manager> _managerRepository;

        public ManagerService(IGenericRepository<Manager> managerRepository)
        {
            _managerRepository = managerRepository ??
                throw new ArgumentNullException(nameof(managerRepository));
        }

        public async Task<ManagerDTO> RegisterManagerAsync(CreateManagerDTO managerDto)
        {
            if (managerDto == null)
                throw new ArgumentNullException(nameof(managerDto));

            
            var existingEmail = (await _managerRepository.GetAllAsync())
                .Any(m => m.Email == managerDto.Email);
            if (existingEmail)
                throw new InvalidOperationException("Email already registered");

            
            var existingPersonalNumber = (await _managerRepository.GetAllAsync())
                .Any(m => m.PersonalNumber == managerDto.PersonalNumber);
            if (existingPersonalNumber)
                throw new InvalidOperationException("Personal number already registered");

            var manager = new Manager
            {
                FirstName = managerDto.FirstName,
                LastName = managerDto.LastName,
                Email = managerDto.Email,
                PersonalNumber = managerDto.PersonalNumber,
                PhoneNumber = managerDto.PhoneNumber,
                
            };

            await _managerRepository.AddAsync(manager);
            await _managerRepository.SaveAsync(); 

            return new ManagerDTO(manager);
        }

        public async Task<bool> UpdateManagerAsync(int id, UpdateManagerDTO managerDto)
        {
            if (managerDto == null)
                throw new ArgumentNullException(nameof(managerDto));

            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                return false;

            
            manager.FirstName = managerDto.FirstName;
            manager.LastName = managerDto.LastName;
            manager.Email = managerDto.Email;
            manager.PhoneNumber = managerDto.PhoneNumber;

            await _managerRepository.UpdateAsync(manager);
            await _managerRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteManagerAsync(int id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                return false;

            await _managerRepository.DeleteAsync(id);
            await _managerRepository.SaveAsync();

            return true;
        }

        public async Task<ManagerDTO> GetManagerByIdAsync(int id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                return null;

            return new ManagerDTO(manager);
        }

        public async Task<ManagerDTO> CreateManagerAsync(CreateManagerDTO dto)
        {
            return await RegisterManagerAsync(dto);
        }
    }
}