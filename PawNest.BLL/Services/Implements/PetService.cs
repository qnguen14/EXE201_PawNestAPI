using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Implements
{
    public  class PetService :BaseService<Pet> , IPetService
    {
        private readonly IPetRepository _petRepository;

        public PetService(
            IUnitOfWork<PawNestDbContext> unitOfWork,
            ILogger<Pet> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IPetRepository petRepository)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _petRepository = petRepository;
        }

        #region Base CRUD Methods

        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all pets");
                return await _petRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all pets");
                throw;
            }
        }

        public async Task<Pet?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting pet with ID: {PetId}", id);
                var pet = await _petRepository.GetByIdAsync(id);

                if (pet == null)
                {
                    _logger.LogWarning("Pet not found with ID: {PetId}", id);
                }

                return pet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet with ID: {PetId}", id);
                throw;
            }
        }

        public async Task<Pet> AddAsync(Pet pet)
        {
            try
            {
                _logger.LogInformation("Adding new pet: {PetName}", pet.PetName);

                // Set owner to current user if not specified
                if (pet.OwnerId == Guid.Empty)
                {
                    pet.OwnerId = GetCurrentUserId();
                }

                // Validate that current user is the owner (for security)
                var currentUserId = GetCurrentUserId();
                if (pet.OwnerId != currentUserId)
                {
                    throw new UnauthorizedException("You can only add pets for yourself");
                }

                var addedPet = await _petRepository.AddPetAsync(pet);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Pet added successfully with ID: {PetId}", addedPet.PetId);
                return addedPet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding pet: {PetName}", pet.PetName);
                throw;
            }
        }

        public async Task<Pet> UpdateAsync(Pet pet)
        {
            try
            {
                _logger.LogInformation("Updating pet with ID: {PetId}", pet.PetId);

                // Verify ownership
                var existingPet = await _petRepository.GetByIdAsync(pet.PetId);
                if (existingPet == null)
                {
                    throw new NotFoundException($"Pet with ID {pet.PetId} not found");
                }

                var currentUserId = GetCurrentUserId();
                if (existingPet.OwnerId != currentUserId)
                {
                    throw new UnauthorizedException("You can only update your own pets");
                }

                var updatedPet = await _petRepository.UpdatePetAsync(pet);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Pet updated successfully: {PetId}", pet.PetId);
                return updatedPet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pet with ID: {PetId}", pet.PetId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting pet with ID: {PetId}", id);

                // Verify ownership
                var pet = await _petRepository.GetByIdAsync(id);
                if (pet == null)
                {
                    throw new NotFoundException($"Pet with ID {id} not found");
                }

                var currentUserId = GetCurrentUserId();
                if (pet.OwnerId != currentUserId)
                {
                    throw new UnauthorizedException("You can only delete your own pets");
                }

                var result = await _petRepository.DeletePetAsync(id);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Pet deleted successfully: {PetId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pet with ID: {PetId}", id);
                throw;
            }
        }

        #endregion

        #region Pet-Specific Methods

        public async Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(Guid ownerId)
        {
            try
            {
                _logger.LogInformation("Getting pets for owner: {OwnerId}", ownerId);
                return await _petRepository.GetPetsByOwnerIdAsync(ownerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pets for owner: {OwnerId}", ownerId);
                throw;
            }
        }

        public async Task<IEnumerable<Pet>> GetMyPetsAsync()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                _logger.LogInformation("Getting pets for current user: {UserId}", currentUserId);
                return await _petRepository.GetPetsByOwnerIdAsync(currentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pets for current user");
                throw;
            }
        }

        public async Task<Pet?> GetPetWithOwnerAsync(Guid petId)
        {
            try
            {
                _logger.LogInformation("Getting pet with owner for pet ID: {PetId}", petId);
                return await _petRepository.GetPetWithOwnerAsync(petId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet with owner: {PetId}", petId);
                throw;
            }
        }

        public async Task<Pet?> GetPetWithBookingsAsync(Guid petId)
        {
            try
            {
                _logger.LogInformation("Getting pet with bookings for pet ID: {PetId}", petId);
                return await _petRepository.GetPetWithBookingsAsync(petId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet with bookings: {PetId}", petId);
                throw;
            }
        }

        public async Task<bool> HasActiveBookingsAsync(Guid petId)
        {
            try
            {
                return await _petRepository.HasActiveBookingsAsync(petId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active bookings for pet: {PetId}", petId);
                throw;
            }
        }

        public async Task<bool> PetExistsAsync(string petName, Guid ownerId)
        {
            try
            {
                return await _petRepository.PetExistsByNameAndOwnerAsync(petName, ownerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking pet existence: {PetName}, {OwnerId}", petName, ownerId);
                throw;
            }
        }

        public Task<bool> IsOwnerOfPetAsync(Guid petId)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
