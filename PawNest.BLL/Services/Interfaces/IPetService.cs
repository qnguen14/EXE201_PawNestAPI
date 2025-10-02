using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IPetService
    {
        Task<IEnumerable<Pet>> GetAllAsync();
        Task<Pet> GetByIdAsync(Guid id);
        Task<Pet> AddAsync(Pet pet);
        Task<Pet> UpdateAsync(Pet pet);
        Task<bool> DeleteAsync(Guid id);

        // Pet-specific methods
        Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<Pet>> GetMyPetsAsync(); // Lấy pets của user hiện tại
        Task<Pet> GetPetWithOwnerAsync(Guid petId);
        Task<Pet> GetPetWithBookingsAsync(Guid petId);
        Task<bool> HasActiveBookingsAsync(Guid petId);
        Task<bool> PetExistsAsync(string petName, Guid ownerId);
        Task<bool> IsOwnerOfPetAsync(Guid petId);
    }
}
