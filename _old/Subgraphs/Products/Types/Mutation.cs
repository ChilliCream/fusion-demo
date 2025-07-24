namespace Demo.Products.Types;

[MutationType]
public static  class Mutation
{
    public static async Task<MutationResult<Product, UnknownProductError>> UploadProductPictureAsync(
        int productId,
        IFile picture,
        [Service] ProductContext context,
        CancellationToken cancellationToken)
    {
        var product = await context.Products.FindAsync(
            new object?[] { productId }, 
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
