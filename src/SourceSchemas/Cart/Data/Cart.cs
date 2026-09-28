using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Demo.Cart.Data;

[Index(nameof(OwnerId), IsUnique = true)]
public class Cart
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The "sub" claim of the signed-in shopper that owns this cart.
    /// </summary>
    [Required]
    public required string OwnerId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}
