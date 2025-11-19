namespace Demo.Products.Types;

[ObjectType<Product>]
public static partial class ProductNode
{
    static partial void Configure(IObjectTypeDescriptor<Product> descriptor) 
        => descriptor
            .Ignore(t => t.Length)
            .Ignore(t => t.Height)
            .Ignore(t => t.Width);

    public static ProductDimension GetDimension(
        [Parent(requires: "{ Length Width Height }")] 
        Product product)
        => new(product.Length, product.Width, product.Height);

    [Tag("team-products")]
    public static Uri? GetPictureUrl(
        [Parent] Product product, 
        IHttpContextAccessor httpContextAccessor)
    {
        var context = httpContextAccessor.HttpContext;
        return product.PictureFileName is not null 
            ? new Uri($"{context!.Request.Scheme}://{context.Request.Host}/images/{product.PictureFileName}")
            : null;
    }

    public static string GetLongDescription([Parent] Product product) 
        => product.Name + " - This is a great product!";
}
