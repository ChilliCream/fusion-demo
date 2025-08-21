namespace Demo.Accounts.Data;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(128)]
    public string? Name { get; set; }

    [Required]
    public DateTime? Birthdate { get; set; }

    [Required]
    [MaxLength(32)]
    public string? Username { get; set; }
}
