namespace Demo.Order.Data;

public class OrderItem
{
    // [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public double Price { get; set; }
    
    public int OrderId { get; set; }
    
    public Order? Order { get; set; }
}
