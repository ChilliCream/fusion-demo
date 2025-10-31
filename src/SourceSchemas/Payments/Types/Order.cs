namespace Demo.Payments.Types;

[EntityKey("id")]
public record Order([property: ID] int Id)
{
    public async Task<Payment[]> GetPaymentsAsync(
        IPaymentByOrderIdDataLoader dataLoader,
        CancellationToken cancellationToken) 
        => await dataLoader.LoadAsync(Id, cancellationToken) ?? [];
}