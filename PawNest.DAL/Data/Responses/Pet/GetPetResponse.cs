namespace PawNest.DAL.Data.Responses.Pet;

public class GetPetResponse
{
    public Guid PetId { get; set; }
    public string PetName { get; set; }
    public string Species { get; set; }
    public string Breed { get; set; }
}