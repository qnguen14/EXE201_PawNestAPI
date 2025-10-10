using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Responses.Pet;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Implements
{
    public class PetService : BaseService<PetService>, IPetService
    {
        public PetService(
            IUnitOfWork<PawNestDbContext> unitOfWork,
            ILogger<PetService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CreatePetResponse>> GetAllPets()
        {
            try
            {
                var pets = await _unitOfWork.GetRepository<Pet>()
                    .GetListAsync(
                        predicate: null, // Get all pets (both active and inactive)
                        orderBy: p => p.OrderBy(n => n.PetName)
                    );

                if (pets == null || !pets.Any())
                {
                    throw new NotFoundException("No pets found.");
                }

                return _mapper.Map<IEnumerable<CreatePetResponse>>(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving pets: " + ex.Message);
                throw;
            }
        }

        public async Task<CreatePetResponse> GetPetById(Guid petId)
        {
            try
            {
                var pet = await _unitOfWork.GetRepository<Pet>()
                    .FirstOrDefaultAsync(
                        predicate: p => p.PetId == petId
                    );

                if (pet == null)
                {
                    throw new NotFoundException($"Pet with ID {petId} not found.");
                }

                return _mapper.Map<CreatePetResponse>(pet);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the pet: " + ex.Message);
                throw;
            }
        }

        public async Task<Pet> CreatePet(Pet pet)
        {
            try
            {
                await _unitOfWork.GetRepository<Pet>().InsertAsync(pet);
                await _unitOfWork.SaveChangesAsync();
                return pet;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the pet: " + ex.Message);
                throw;
            }
        }

        public async Task<Pet> UpdatePet(Pet pet)
        {
            try
            {
                var existingPet = await _unitOfWork.GetRepository<Pet>()
                    .FirstOrDefaultAsync(
                        predicate: p => p.PetId == pet.PetId,
                        orderBy: null,
                        include: null
                    );

                if (existingPet == null)
                {
                    throw new NotFoundException($"Pet with ID {pet.PetId} not found.");
                }

                _mapper.Map(pet, existingPet);
                _unitOfWork.GetRepository<Pet>().UpdateAsync(existingPet);
                await _unitOfWork.SaveChangesAsync();

                return existingPet;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the pet: " + ex.Message);
                throw;
            }
        }

        public async Task<bool> DeletePet(Guid petId)
        {
            try
            {
                var pet = await _unitOfWork.GetRepository<Pet>()
                    .FirstOrDefaultAsync(
                        predicate: p => p.PetId == petId,
                        orderBy: null,
                        include: null
                    );

                if (pet == null)
                {
                    throw new NotFoundException($"Pet with ID {petId} not found.");
                }

                _unitOfWork.GetRepository<Pet>().DeleteAsync(pet);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the pet: " + ex.Message);
                throw;
            }
        }

        public async Task<Pet> GetPetByOwnerId(Guid ownerId)
        {
            try
            {
                var pet = await _unitOfWork.GetRepository<Pet>()
                    .FirstOrDefaultAsync(
                        predicate: p => p.CustomerId == ownerId,
                        orderBy: null,
                        include: null
                    );

                if (pet == null)
                {
                    throw new NotFoundException($"Pet with Owner ID {ownerId} not found.");
                }

                return pet;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the pet by owner ID: " + ex.Message);
                throw;
            }
        }
    }
}