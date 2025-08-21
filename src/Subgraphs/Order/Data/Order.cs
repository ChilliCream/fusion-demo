namespace Demo.Order.Data;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; } = default!;

    [Required]
    [UsePaging]
    public ICollection<OrderItem> Items { get; set; } = default!;

    [Required]
    public int Weight { get; set; }
}
