using System.ComponentModel.DataAnnotations;

namespace PawNest.BLL.Services.Interfaces
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Name field is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number field is required")]
        [MaxLength(10, ErrorMessage = "Phone number cannot be more than 10 characters")]
        public string PhoneNumber { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Address field is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Role field is required")]
        public string Role { get; set; }
    }
}