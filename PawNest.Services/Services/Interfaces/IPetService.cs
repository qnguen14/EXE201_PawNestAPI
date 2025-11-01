using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Pet;
using PawNest.Repository.Data.Responses.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Interfaces
{
    public interface IPetService
    {
        Task<IEnumerable<CreatePetResponse>> GetAllPets();
        Task<CreatePetResponse > GetPetById(Guid petId);
        Task<Pet> CreatePet(Pet pet);
        Task<Pet> UpdatePet(Pet pet);
        Task<bool> DeletePet(Guid petId);
        Task<Pet> GetPetByCustomerId(Guid customerId);
        Task<CreatePetResponse> AddPet(CreatePetRequest request);
        Task<IEnumerable<CreatePetResponse>> GetCustomerPets();
    }
}
