using HotelManagement.Application.Services;
using HotelManagement.Core.DTOs;
using HotelManagement.Core.Entities;
using HotelManagement.Core.Interfaces;
using HotelManagement.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace HotelManagement.Core.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IGenericRepository<Manager> _managerRepository;

        // Constructor injection of the generic repository
        public ManagerService(IGenericRepository<Manager> managerRepository)
        {
            _managerRepository = managerRepository ?? throw new ArgumentNullException(nameof(managerRepository));
        }

        public async Task<ManagerDTO> RegisterManagerAsync(CreateManagerDTO managerDto)
        {
            if (managerDto == null)
                throw new ArgumentNullException(nameof(managerDto));

            var manager = new Manager
            {
                FirstName = managerDto.FirstName,
                LastName = managerDto.LastName,
                Email = managerDto.Email,
                PersonalNumber = managerDto.PersonalNumber,
                PhoneNumber = managerDto.PhoneNumber
            };

            // Add the manager using the generic repository
            await _managerRepository.AddAsync(manager);

            return new ManagerDTO(manager);
        }

        public async Task UpdateManagerAsync(int id, UpdateManagerDTO managerDto)
        {
            if (managerDto == null)
                throw new ArgumentNullException(nameof(managerDto));

            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                throw new KeyNotFoundException("Manager not found");

            // Update the manager's details
            manager.FirstName = managerDto.FirstName;
            manager.LastName = managerDto.LastName;
            manager.Email = managerDto.Email;
            manager.PhoneNumber = managerDto.PhoneNumber;

            // Update the manager using the generic repository
            await _managerRepository.UpdateAsync(manager);
        }

        public async Task DeleteManagerAsync(int id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                throw new KeyNotFoundException("Manager not found");

            // Delete the manager using the generic repository
            await _managerRepository.DeleteAsync(id);
        }

        public async Task<ManagerDTO> GetManagerByIdAsync(int id)
        {
            var manager = await _managerRepository.GetByIdAsync(id);
            if (manager == null)
                throw new KeyNotFoundException("Manager not found");

            return new ManagerDTO(manager);
        }

        public Task<ManagerDTO> CreateManagerAsync(CreateManagerDTO dto)
        {
            throw new NotImplementedException();
        }

        Task<bool> IManagerService.UpdateManagerAsync(int id, UpdateManagerDTO dto)
        {
            throw new NotImplementedException();
        }

        Task<bool> IManagerService.DeleteManagerAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
