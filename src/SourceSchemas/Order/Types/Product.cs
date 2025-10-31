namespace Demo.Order.Types;

[EntityKey("id")]
public record Product([property: ID<Product>] int Id);
