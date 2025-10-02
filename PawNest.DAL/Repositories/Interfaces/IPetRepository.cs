using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Repositories.Interfaces
{
    public interface IPetRepository : IGenericRepository<Pet>
    {
        Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(Guid ownerId);
        Task<Pet> GetPetWithOwnerAsync(Guid petId);
        Task<Pet> GetPetWithBookingsAsync(Guid petId);
        Task<bool> HasActiveBookingsAsync(Guid petId);
        Task<bool> PetExistsByNameAndOwnerAsync(string petName, Guid ownerId);
        Task<List<Pet>> GetAllAsync();
        Task<Pet> GetByIdAsync(Guid petId);
        Task<Pet> AddPetAsync(Pet pet);
        Task<Pet> UpdatePetAsync(Pet pet);
        Task<bool> DeletePetAsync(Guid petId);
    }
}
