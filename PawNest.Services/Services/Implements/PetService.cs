using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Requests.Pet;
using PawNest.Repository.Data.Requests.Post;
using PawNest.Repository.Data.Responses.Pet;
using PawNest.Repository.Mappers;
using PawNest.Repository.Repositories.Interfaces;
using PawNest.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Implements
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

        public async Task<IEnumerable<GetPetResponse>> GetAllPets()
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var petRepo = _unitOfWork.GetRepository<Pet>();
                    var existingPets = await petRepo.GetListAsync();
                    if (existingPets == null || !existingPets.Any())
                    {
                        throw new NotFoundException("No pets found.");
                    }
                    var petList = existingPets.ToList();
                    return petList.Select(p => _petMapper.MapToGetPetResponse(p));
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving pets: " + ex.Message);
                throw;
            }
        }

        public async Task<GetPetResponse> GetPetById(Guid petId)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var petRepo = _unitOfWork.GetRepository<Pet>();
                    var existingPet = await petRepo.FirstOrDefaultAsync(predicate: p => p.PetId == petId);
                    if (existingPet == null)
                    {
                        throw new NotFoundException($"Pet with ID {petId} not found.");
                    }
                    return _petMapper.MapToGetPetResponse(existingPet);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the pet: " + ex.Message);
                throw;
            }
        }

        public async Task<CreatePetResponse> CreatePet(CreatePetRequest request)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async() =>
                {
                    var petRepo = _unitOfWork.GetRepository<Pet>();

                    var pet = _petMapper.MapToPet(request);
                    await petRepo.InsertAsync(pet);

                    return _mapper.MapToCreatePetResponse(pet);

                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the pet: " + ex.Message);
                throw;
            }
        }

        public async Task<CreatePetResponse> UpdatePet(UpdatePetRequest request, Guid id)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var petRepo = _unitOfWork.GetRepository<Pet>();

                    // Lấy pet hiện tại từ database
                    var existingPet = await petRepo.FirstOrDefaultAsync(predicate: p => p.PetId == id);
                    if (existingPet == null)
                    {
                        throw new NotFoundException($"Pet with ID {id} not found.");
                    }

                    // Update các fields từ request vào existingPet
                    _petMapper.UpdatePetFromRequest(request, existingPet);

                    // Update entity trong DbContext
                    petRepo.UpdateAsync(existingPet);

                    // Return response với dữ liệu đã update
                    return _mapper.MapToCreatePetResponse(existingPet);
                });
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
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var petRepo = _unitOfWork.GetRepository<Pet>();
                    var existingPet = await petRepo.FirstOrDefaultAsync(predicate: p => p.PetId == petId);
                    if (existingPet == null)
                    {
                        throw new NotFoundException($"Pet with ID {petId} not found.");
                    }
                    petRepo.DeleteAsync(existingPet);
                    return true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the pet: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<GetPetResponse>> GetPetsByCustomerId(Guid customerId)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var petRepo = _unitOfWork.GetRepository<Pet>();
                    var existingPet = await petRepo.GetListAsync(predicate: p => p.CustomerId == customerId);
                    if (existingPet == null || !existingPet.Any())
                    {
                        throw new NotFoundException($"Pet for Customer ID {customerId} not found.");
                    }

                    var petList = existingPet.ToList();

                    return petList.Select(p => _petMapper.MapToGetPetResponse(p));
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the pet by owner ID: " + ex.Message);
                throw;
            }
        }

        public async Task<CreatePetResponse> AddPet(AddPetRequest request)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var repo = _unitOfWork.GetRepository<Pet>();
                    var customerId = GetCurrentUserId();
                    var existingPet = await repo.FirstOrDefaultAsync(predicate: p => p.PetName == request.PetName && p.CustomerId == customerId);
                    if (existingPet != null)
                    {
                        throw new Exception("A pet with the same name already exists for this customer.");
                    }

                    var petEntity = _petMapper.AddRequestMapToPet(request);
                    petEntity.CustomerId = customerId;
                    await repo.InsertAsync(petEntity);
                    return _petMapper.MapToCreatePetResponse(petEntity);
                });

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CreatePetResponse> UpdateCustomerPet(Guid petId, EditPetRequest request)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var repo = _unitOfWork.GetRepository<Pet>();
                    var customerId = GetCurrentUserId();

                    // Tìm pet theo petId và customerId để đảm bảo user chỉ update pet của mình
                    var existingPet = await repo.FirstOrDefaultAsync(
                        predicate: p => p.PetId == petId && p.CustomerId == customerId
                    );

                    if (existingPet == null)
                    {
                        throw new NotFoundException($"Pet with ID {petId} not found for current customer.");
                    }

                    // Check trùng tên (nếu tên thay đổi)
                    if (existingPet.PetName != request.PetName)
                    {
                        var duplicatePet = await repo.FirstOrDefaultAsync(
                            predicate: p => p.PetName == request.PetName &&
                                           p.CustomerId == customerId &&
                                           p.PetId != petId
                        );

                        if (duplicatePet != null)
                        {
                            throw new Exception("A pet with the same name already exists for this customer.");
                        }
                    }

                    // Update fields
                    _petMapper.UpdatePetFromEditRequest(request, existingPet);

                    // Update trong DbContext
                    repo.UpdateAsync(existingPet);

                    return _petMapper.MapToCreatePetResponse(existingPet);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating customer's pet: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<GetPetResponse>> GetCustomerPets()
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async() =>
                {
                    var petRepo = _unitOfWork.GetRepository<Pet>();
                    var customerId = GetCurrentUserId();
                    var existingPets = await petRepo.GetListAsync(predicate: p => p.CustomerId == customerId);
                    if (existingPets == null || !existingPets.Any())
                    {
                        throw new NotFoundException("No pets found for the current customer.");
                    }
                    var petList = existingPets.ToList();
                    return petList.Select(p => _petMapper.MapToGetPetResponse(p));
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving customer's pets: " + ex.Message);
                throw;
            }
        }
    }
}