namespace Demo.Payments.Types;

[MutationType]
public static class Mutation
{
    public static async Task<Payment> CreatePaymentAsync(
        [ID<Order>] int orderId,
        [Service] PaymentContext context)
    {
        var payment = new Payment { OrderId = orderId, Status = PaymentStatus.Pending };

        context.Payments.Add(payment);

        await context.SaveChangesAsync();

        return payment;
    }
}

public record Order([property: ID] int Id);
