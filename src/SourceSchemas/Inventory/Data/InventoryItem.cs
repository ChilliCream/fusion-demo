namespace Demo.Inventory.Data;

public class InventoryItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}
