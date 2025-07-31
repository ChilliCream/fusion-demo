using eShop.Catalog.Data.Entities;
using HotChocolate.Data.Filters;

namespace eShop.Catalog.Types;

public sealed class ProductFilterInputType : FilterInputType<Product>
{
    protected override void Configure(IFilterInputTypeDescriptor<Product> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(t => t.Price);
    }
}