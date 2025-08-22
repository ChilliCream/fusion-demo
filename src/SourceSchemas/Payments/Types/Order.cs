namespace Demo.Payments.Types;

public record Order([property: ID] int Id)
{
    public async Task<Payment[]> GetPaymentsAsync(
        [Service] PaymentByOrderIdDataLoader dataLoader,
        CancellationToken cancellationToken) 
        => await dataLoader.LoadAsync(Id, cancellationToken) ?? [];
}