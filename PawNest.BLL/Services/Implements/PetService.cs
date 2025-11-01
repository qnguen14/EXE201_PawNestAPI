using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Requests.Pet;
using PawNest.DAL.Data.Responses.Pet;
using PawNest.DAL.Mappers;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Implements
{
    public class PetService : BaseService<PetService>, IPetService
    {
        private readonly IMapperlyMapper _petMapper;
        
        public PetService(
            IUnitOfWork<PawNestDbContext> unitOfWork,
            ILogger<PetService> logger,
            IMapperlyMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _petMapper = mapper;
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

                return _petMapper.MapToCreatePetResponseList(pets);
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

                return _petMapper.MapToCreatePetResponse(pet);
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

                _petMapper.UpdatePetFromRequest(pet, existingPet);
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

        public async Task<Pet> GetPetByCustomerId(Guid customerId)
        {
            try
            {
                var pet = await _unitOfWork.GetRepository<Pet>()
                    .FirstOrDefaultAsync(
                        predicate: p => p.CustomerId == customerId,
                        orderBy: null,
                        include: null
                    );

                if (pet == null)
                {
                    throw new NotFoundException($"Pet with Customer ID {customerId} not found.");
                }

                return pet;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the pet by owner ID: " + ex.Message);
                throw;
            }
        }

        public async Task<CreatePetResponse> AddPet(CreatePetRequest request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Pet>();
                var customerId = GetCurrentUserId();
                var existingPet = await repo.FirstOrDefaultAsync(predicate: p => p.PetName == request.PetName && p.CustomerId == customerId);
                if (existingPet != null)
                {
                    throw new Exception("A pet with the same name already exists for this customer.");
                }

                var petEntity = _petMapper.MapToPet(request);
                petEntity.CustomerId = customerId;
                return _petMapper.MapToCreatePetResponse(await CreatePet(petEntity));

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<CreatePetResponse>> GetCustomerPets()
        {
            try
            {
                var customerId = GetCurrentUserId();
                var pets = await _unitOfWork.GetRepository<Pet>()
                    .GetListAsync(
                        predicate: p => p.CustomerId == customerId,
                        orderBy: p => p.OrderBy(n => n.PetName)
                    );
                if (pets == null || !pets.Any())
                {
                    throw new NotFoundException("No pets found for the current customer.");
                }
                return _petMapper.MapToCreatePetResponseList(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving customer's pets: " + ex.Message);
                throw;
            }
        }
    }
}