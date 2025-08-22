namespace Demo.Products.Types;

[MutationType]
public static partial class ProductMutations
{
    public static async Task<FieldResult<Product, UnknownProductError>> UploadProductPictureAsync(
        int productId,
        IFile picture,
        ProductContext context,
        CancellationToken cancellationToken)
    {
        var product = await context.Products.FindAsync(
            [productId], 
            cancellationToken: cancellationToken);

        if (product is null)
        {
            return new UnknownProductError(productId);
        }

        product.PictureFileName = picture.Name;

        await context.SaveChangesAsync(cancellationToken);

        return product;
    }
}
