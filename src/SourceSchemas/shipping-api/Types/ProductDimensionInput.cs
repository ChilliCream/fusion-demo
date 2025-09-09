namespace Demo.Shipping.Types;

// [Inaccessible]
public sealed class ProductDimensionInput
{
    public int Weight { get; init; }

    public double Length { get; init; }

    public double Width { get; init; }

    public double Height { get; init; }
}