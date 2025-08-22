namespace Demo.Inventory.Data;

public class InventoryItem
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}
