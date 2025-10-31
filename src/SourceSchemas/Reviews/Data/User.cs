namespace Demo.Reviews.Data;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(128)]
    public string? Name { get; set; }

    public IList<Review> Reviews { get; set; } = [];
}
