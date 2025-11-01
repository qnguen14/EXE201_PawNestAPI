using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawNest.Repository.Data.Entities;

public enum PostStatus
{
    Draft, // Happens when the author decides to not submit yet
    Pending, // Submitted, waiting for response
    Rejected, // Disapproved along with reasons 
    Approved // Will be shown on front-end after approval
}

public enum PostCategory
{
    DogCare,
    CatCare,
    PetNutrition,
    PetTraining
}


[Table("Post")]
public class Post
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; }
    
    [Required]
    [Column("image_url")]
    public string ImageUrl { get; set; }
    
    [Required]
    [Column("PostStatus")]
    [EnumDataType(typeof(PostStatus))]
    public PostStatus Status { get; set; } = PostStatus.Pending;

    [Required]
    [Column("PostCategory")]
    [EnumDataType(typeof(PostCategory))]
    public PostCategory Category { get; set; } 

    [Required]
    [Column("staff_id")]
    [ForeignKey("Staff")]
    public Guid StaffId { get; set; }
    public virtual User Staff { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}