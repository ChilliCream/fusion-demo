namespace Demo.Shipping.Types;

// The composition should remove it as its only used as a requirement, right?
// [Inaccessible]
public sealed class ProductDimensionInput
{
    public int Weight { get; init; }

    public double Length { get; init; }

    public double Width { get; init; }

    public double Height { get; init; }
}