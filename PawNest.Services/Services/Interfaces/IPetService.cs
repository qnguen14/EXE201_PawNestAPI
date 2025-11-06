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
        // General use
        Task<IEnumerable<GetPetResponse>> GetAllPets();
        Task<GetPetResponse> GetPetById(Guid petId);

        // Admin
        Task<CreatePetResponse> CreatePet(CreatePetRequest request);
        Task<CreatePetResponse> UpdatePet(CreatePetRequest request, Guid id);
        Task<bool> DeletePet(Guid petId);

        // Customer
        Task<IEnumerable<GetPetResponse>> GetPetsByCustomerId(Guid customerId);
        Task<CreatePetResponse> AddPet(AddPetRequest request);
        Task<CreatePetResponse> UpdateCustomerPet(AddPetRequest request);
        Task<IEnumerable<GetPetResponse>> GetCustomerPets();
    }
}
