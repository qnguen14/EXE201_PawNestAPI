using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Services.Services.Implements;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Metadata;
using PawNest.Repository.Data.Requests.Pet;
using PawNest.Repository.Data.Responses.Booking;
using PawNest.Repository.Data.Responses.Pet;
using PawNest.Repository.Mappers;
using static PawNest.API.Constants.ApiEndpointConstants;


namespace PawNest.API.Controllers
{
    [ApiController]
    [Authorize]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ILogger<PetController> _logger;

        public PetController(IPetService petService, ILogger<PetController> logger)
        {
            _petService = petService;
            _logger = logger;
        }

        [HttpGet(ApiEndpointConstants.Pet.GetAllPetsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetPetResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPets()
        {
            try
            {
                var pets = await _petService.GetAllPets();
                if (pets == null || !pets.Any())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No pets found.",
                        IsSuccess = false
                    });
                }

                var apiResponse = new ApiResponse<IEnumerable<GetPetResponse>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Pets retrieved successfully.",
                    IsSuccess = true,
                    Data = pets
                };
                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving pets.",
                    IsSuccess = false
                });
            }
        }

        [HttpGet(ApiEndpointConstants.Pet.GetPetByIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetPetResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPetById([FromRoute] Guid petId)
        {
            try
            {
                var pet = await _petService.GetPetById(petId);
                if (pet == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Pet with ID {petId} not found.",
                        IsSuccess = false
                    });
                }

                var apiResponse = new ApiResponse<GetPetResponse>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Pet retrieved successfully.",
                    IsSuccess = true,
                    Data = pet
                };
                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving the pet.",
                    IsSuccess = false
                });
            }
        }

        [HttpGet(ApiEndpointConstants.Pet.OwnersPetsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetPetResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPetsByCustomerId([FromRoute] Guid customerId)
        {
            try
            {
                var pets = await _petService.GetPetsByCustomerId(customerId);
                if (pets == null || !pets.Any())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"No pets found for customer ID {customerId}.",
                        IsSuccess = false
                    });
                }

                var apiResponse = new ApiResponse<IEnumerable<GetPetResponse>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Pets retrieved successfully.",
                    IsSuccess = true,
                    Data = pets
                };
                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving pets.",
                    IsSuccess = false
                });
            }
        }

        [HttpPost(ApiEndpointConstants.Pet.CreatePetEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<CreatePetResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePet([FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid pet data.",
                        IsSuccess = false
                    });
                }

                var createdPet = await _petService.CreatePet(request);
                var apiResponse = new ApiResponse<CreatePetResponse>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Pet created successfully.",
                    IsSuccess = true,
                    Data = createdPet
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while creating the pet.",
                    IsSuccess = false
                });
            }
        }

        [HttpPut(ApiEndpointConstants.Pet.UpdatePetEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<CreatePetResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePet([FromRoute] Guid id, [FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid pet data.",
                        IsSuccess = false
                    });
                }

                var updatedPet = await _petService.UpdatePet(request, id);
                var apiResponse = new ApiResponse<CreatePetResponse>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Pet updated successfully.",
                    IsSuccess = true,
                    Data = updatedPet
                };

                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while updating the pet.",
                    IsSuccess = false
                });
            }
        }

        [HttpDelete(ApiEndpointConstants.Pet.DeletePetEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePet([FromRoute] Guid petId)
        {
            try
            {
                var result = await _petService.DeletePet(petId);
                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Pet with ID {petId} not found.",
                        IsSuccess = false
                    });
                }

                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Pet deleted successfully.",
                    IsSuccess = true,
                    Data = null
                };
                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while deleting the pet.",
                    IsSuccess = false
                });
            }
        }

        [HttpPost(ApiEndpointConstants.Pet.AddPetEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<CreatePetResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPetToCustomer([FromBody] AddPetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid pet data.",
                        IsSuccess = false
                    });
                }

                var addedPet = await _petService.AddPet(request);
                var apiResponse = new ApiResponse<CreatePetResponse>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Pet added to customer successfully.",
                    IsSuccess = true,
                    Data = addedPet
                };
                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while adding the pet.",
                    IsSuccess = false
                });
            }
        }

        [HttpPut(ApiEndpointConstants.Pet.EditPetEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<CreatePetResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditCustomerPet([FromBody] AddPetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid pet data.",
                        IsSuccess = false
                    });
                }

                var addedPet = await _petService.UpdateCustomerPet(request);
                var apiResponse = new ApiResponse<CreatePetResponse>
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Pet updated successfully.",
                    IsSuccess = true,
                    Data = addedPet
                };
                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while adding the pet.",
                    IsSuccess = false
                });
            }
        }

        [HttpGet(ApiEndpointConstants.Pet.MyPetsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetPetResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MyPets()
        {
            try
            {
                var pets = await _petService.GetCustomerPets();
                if (pets == null || !pets.Any())
                {
                    return NotFound(new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No pets found for the current customer.",
                        IsSuccess = false
                    });
                }

                var apiResponse = new ApiResponse<IEnumerable<GetPetResponse>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Pets retrieved successfully.",
                    IsSuccess = true,
                    Data = pets
                };
                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = ex.Message,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An error occurred while retrieving pets.",
                    IsSuccess = false
                });
            }
        }
    }
}