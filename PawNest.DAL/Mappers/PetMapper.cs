using Riok.Mapperly.Abstractions;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Pet;
using PawNest.DAL.Data.Responses.Pet;

namespace PawNest.DAL.Mappers;

[Mapper]
public partial class PetMapper
{
    // CreatePetRequest to Pet
    public partial Pet MapToPet(CreatePetRequest request);
    
    // UpdatedPetRequest to Pet (for updating)
    public partial Pet MapToPetFromUpdate(UpdatedPetRequest request);
    
    // Pet to CreatePetResponse
    public partial CreatePetResponse MapToCreatePetResponse(Pet pet);
    
    // IEnumerable mapping
    public partial IEnumerable<CreatePetResponse> MapToCreatePetResponseList(IEnumerable<Pet> pets);
    
    // For update scenario - map request to existing pet
    public partial void UpdatePetFromRequest(Pet source, Pet target);
}
