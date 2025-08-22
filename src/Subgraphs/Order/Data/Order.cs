namespace Demo.Order.Data;

public class Order
{
    // [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required] 
    public ICollection<OrderItem> Items { get; set; } = [];

    [Required]
    public int Weight { get; set; }
}
