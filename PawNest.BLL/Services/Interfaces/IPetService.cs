using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Responses.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IPetService
    {
        Task<IEnumerable<CreatePetResponse>> GetAllPets();
        Task<CreatePetResponse > GetPetById(Guid petId);
        Task<Pet> CreatePet(Pet pet);
        Task<Pet> UpdatePet(Pet pet);
        Task<bool> DeletePet(Guid petId);
        Task<Pet> GetPetByCustomerId(Guid customerId);
    }
}
