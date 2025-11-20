using System.ComponentModel.DataAnnotations;

namespace Demo.Cart.Data;

public class Cart
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
