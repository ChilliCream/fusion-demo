namespace Demo.Inventory.Types;

public record Product([property: ID<Product>] int Id, int Stock);
