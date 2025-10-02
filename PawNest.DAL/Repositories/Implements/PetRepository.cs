using Microsoft.EntityFrameworkCore;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Repositories.Implements
{
    public class PetRepository : GenericRepository<Pet>, IPetRepository
    {

        private readonly PawNestDbContext _context;

        public PetRepository(PawNestDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Pets
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Pet?> GetPetWithOwnerAsync(Guid petId)
        {
            return await _context.Pets
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.PetId == petId);
        }

        public async Task<Pet?> GetPetWithBookingsAsync(Guid petId)
        {
            return await _context.Pets
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.PetId == petId);
        }

        public async Task<bool> HasActiveBookingsAsync(Guid petId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.PetId == petId && b.Status == "Active");
        }

        public async Task<bool> PetExistsByNameAndOwnerAsync(string petName, Guid ownerId)
        {
            return await _context.Pets
                .AnyAsync(p => p.PetName.ToLower() == petName.ToLower()
                            && p.OwnerId == ownerId);
        }


        public async Task<Pet> AddPetAsync(Pet pet)
        {
            // Validation: Check if owner exists
            var ownerExists = await _context.Users.AnyAsync(u => u.Id == pet.OwnerId);
            if (!ownerExists)
            {
                throw new ArgumentException("Owner does not exist");
            }

            // Check duplicate pet name for same owner
            var duplicate = await PetExistsByNameAndOwnerAsync(pet.PetName, pet.OwnerId);
            if (duplicate)
            {
                throw new InvalidOperationException($"Pet with name '{pet.PetName}' already exists for this owner");
            }

            pet.PetId = Guid.NewGuid();
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
            return pet;
        }

        public async Task<Pet> UpdatePetAsync(Pet pet)
        {
            var existingPet = await _context.Pets.FindAsync(pet.PetId);
            if (existingPet == null)
            {
                throw new KeyNotFoundException($"Pet with ID {pet.PetId} not found");
            }

            // Check if new name conflicts with other pets of same owner
            var duplicate = await _context.Pets
                .AnyAsync(p => p.PetName.ToLower() == pet.PetName.ToLower()
                            && p.OwnerId == pet.OwnerId
                            && p.PetId != pet.PetId);

            if (duplicate)
            {
                throw new InvalidOperationException($"Another pet with name '{pet.PetName}' already exists for this owner");
            }

            // Update properties
            existingPet.PetName = pet.PetName;
            existingPet.Species = pet.Species;
            existingPet.Breed = pet.Breed;
            existingPet.OwnerId = pet.OwnerId;

            _context.Pets.Update(existingPet);
            await _context.SaveChangesAsync();
            return existingPet;
        }

        public async Task<bool> DeletePetAsync(Guid petId)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null)
            {
                return false;
            }

            // Check if pet has active bookings
            var hasBookings = await HasActiveBookingsAsync(petId);
            if (hasBookings)
            {
                throw new InvalidOperationException("Cannot delete pet with existing bookings");
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Pet>> GetAllAsync()
        {
            return await _context.Pets.ToListAsync();
        }

        public async Task<Pet> GetByIdAsync(Guid petId)
        {
            return await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId);
        }
    }
}
