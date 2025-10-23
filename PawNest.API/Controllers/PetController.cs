
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.BLL.Services.Implements;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Metadata;
using PawNest.DAL.Data.Requests.Pet;
using PawNest.DAL.Data.Responses.Booking;
using PawNest.DAL.Data.Responses.Pet;
using PawNest.DAL.Mappers;


namespace PawNest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ILogger<PetController> _logger;
        private readonly PetMapper _petMapper;

        public PetController(IPetService petService, ILogger<PetController> logger, PetMapper petMapper)
        {
            _petService = petService;
            _logger = logger;
            _petMapper = petMapper;
        }

        [HttpGet(ApiEndpointConstants.Pet.GetAllPetsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CreatePetResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> GetAllPets()
        {
            try
            {
                var pets = await _petService.GetAllPets();
                return Ok(pets);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet(ApiEndpointConstants.Pet.GetPetByIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<CreatePetResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> GetPetById(Guid petId)
        {
            try
            {
                var pet = await _petService.GetPetById(petId);
                return Ok(pet);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet(ApiEndpointConstants.Pet.OwnersPetsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CreatePetResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> GetPetByOwnerId(Guid ownerId)
        {
            try
            {
                var pet = await _petService.GetPetByCustomerId(ownerId);
                var response = _petMapper.MapToCreatePetResponse(pet);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost(ApiEndpointConstants.Pet.CreatePetEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<CreatePetResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> CreatePet([FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var pet = _petMapper.MapToPet(request);
                var createdPet = await _petService.CreatePet(pet);
                var response = _petMapper.MapToCreatePetResponse(createdPet);

                return CreatedAtAction(nameof(GetPetById), new { petId = response.PetId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut(ApiEndpointConstants.Pet.UpdatePetEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<CreatePetResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> UpdatePet(Guid petId, [FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var pet = _petMapper.MapToPet(request);
                pet.PetId = petId;

                var updatedPet = await _petService.UpdatePet(pet);
                var response = _petMapper.MapToCreatePetResponse(updatedPet);

                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete(ApiEndpointConstants.Pet.DeletePetEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<CreatePetResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> DeletePet(Guid petId)
        {
            try
            {
                await _petService.DeletePet(petId);
                return Ok("Pet deleted successfully");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
