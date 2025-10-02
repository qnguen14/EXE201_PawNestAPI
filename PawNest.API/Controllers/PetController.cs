using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using static PawNest.BLL.DTO.PetDTO;

namespace PawNest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly ILogger<PetController> _logger;

        public PetController(IPetService petService, ILogger<PetController> logger)
        {
            _petService = petService;
            _logger = logger;
        }

        #region GET Methods

        [HttpGet("my-pets")]
        public async Task<ActionResult<IEnumerable<PetResponse>>> GetMyPets()
        {
            try
            {
                var pets = await _petService.GetMyPetsAsync();
                var response = pets.Select(p => new PetResponse
                {
                    PetId = p.PetId,
                    PetName = p.PetName,
                    Species = p.Species,
                    Breed = p.Breed,
                    OwnerId = p.OwnerId,
                    OwnerName = p.Owner.Name // Chỉ lấy tên
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user's pets");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetResponse>> GetPetById(Guid id)
        {
            try
            {
                var pet = await _petService.GetByIdAsync(id);

                if (pet == null)
                {
                    return NotFound(new { message = $"Pet not found" });
                }

                var response = new PetResponse
                {
                    PetId = pet.PetId,
                    PetName = pet.PetName,
                    Species = pet.Species,
                    Breed = pet.Breed,
                    OwnerId = pet.OwnerId,
                    OwnerName = pet.Owner.Name
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pet");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        #endregion

        #region POST Methods

        /// <summary>
        /// Add a new pet
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PetResponse>> AddPet([FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Map DTO to Entity
                var pet = new Pet
                {
                    PetName = request.PetName,
                    Species = request.Species,
                    Breed = request.Breed
                    // OwnerId sẽ được set trong Service từ JWT
                };

                var addedPet = await _petService.AddAsync(pet);

                var response = new PetResponse
                {
                    PetId = addedPet.PetId,
                    PetName = addedPet.PetName,
                    Species = addedPet.Species,
                    Breed = addedPet.Breed,
                    OwnerId = addedPet.OwnerId
                };

                return CreatedAtAction(nameof(GetPetById), new { id = response.PetId }, response);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding pet");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>
        /// Update an existing pet
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PetResponse>> UpdatePet(Guid id, [FromBody] UpdatePetRequest request)
        {
            try
            {
                if (id != request.PetId)
                {
                    return BadRequest(new { message = "ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Map DTO to Entity
                var pet = new Pet
                {
                    PetId = request.PetId,
                    PetName = request.PetName,
                    Species = request.Species,
                    Breed = request.Breed
                };

                var updatedPet = await _petService.UpdateAsync(pet);

                var response = new PetResponse
                {
                    PetId = updatedPet.PetId,
                    PetName = updatedPet.PetName,
                    Species = updatedPet.Species,
                    Breed = updatedPet.Breed,
                    OwnerId = updatedPet.OwnerId
                };

                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pet");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        #endregion

        #region DELETE Methods

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(Guid id)
        {
            try
            {
                var result = await _petService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Pet not found" });
                }

                return NoContent();
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pet");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        #endregion
    }
}
