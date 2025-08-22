namespace Demo.Products.Types;

[ObjectType<Product>]
public static partial class ProductNode
{
    static partial void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor
            .Ignore(t => t.Length)
            .Ignore(t => t.Height)
            .Ignore(t => t.Width);
        
    }

    public static ProductDimension GetDimension(
        [Parent(requires: "{ length width height }")] 
        Product product)
        => new ProductDimension(product.Length, product.Width, product.Height);

    public static Uri? GetPictureUrl(
        [Parent] Product product, 
        IHttpContextAccessor httpContextAccessor)
    {
        var context = httpContextAccessor.HttpContext;
        return product.PictureFileName is not null 
            ? new Uri($"{context!.Request.Scheme}://{context.Request.Host}/images/{product.PictureFileName}")
            : null;
    }
}
