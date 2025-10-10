using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawNest.BLL.Services.Implements;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Metadata;
using PawNest.DAL.Data.Requests.Pet;
using PawNest.DAL.Data.Responses.Pet;



namespace PawNest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly IMapper _mapper;
        private readonly ILogger<PetController> _logger;

        public PetController(IPetService petService, IMapper mapper, ILogger<PetController> logger)
        {
            _petService = petService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
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

        [HttpGet("{petId:guid}")]
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

        [HttpGet("owner/{ownerId:guid}")]
        public async Task<IActionResult> GetPetByOwnerId(Guid ownerId)
        {
            try
            {
                var pet = await _petService.GetPetByOwnerId(ownerId);
                var response = _mapper.Map<CreatePetResponse>(pet);
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

        [HttpPost]
        public async Task<IActionResult> CreatePet([FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var pet = _mapper.Map<Pet>(request);
                var createdPet = await _petService.CreatePet(pet);
                var response = _mapper.Map<CreatePetResponse>(createdPet);

                return CreatedAtAction(nameof(GetPetById), new { petId = response.PetId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut("{petId:guid}")]
        public async Task<IActionResult> UpdatePet(Guid petId, [FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var pet = _mapper.Map<Pet>(request);
                pet.PetId = petId;

                var updatedPet = await _petService.UpdatePet(pet);
                var response = _mapper.Map<CreatePetResponse>(updatedPet);

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

        [HttpDelete("{petId:guid}")]
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
