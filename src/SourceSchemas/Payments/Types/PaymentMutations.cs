namespace Demo.Payments.Types;

[MutationType]
public static partial class PaymentMutations
{
    public static async Task<Payment> CreatePaymentAsync(
        int orderId,
        [Service] PaymentContext context)
    {
        var payment = new Payment { OrderId = orderId, Status = PaymentStatus.Pending };

        context.Payments.Add(payment);

        await context.SaveChangesAsync();

        return payment;
    }
}