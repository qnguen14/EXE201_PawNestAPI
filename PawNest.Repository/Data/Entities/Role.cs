using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawNest.Repository.Data.Entities;

[Table("Roles")]
public class Role
{
    [Key]
    [Required]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("name")]
    public string RoleName { get; set; }
}