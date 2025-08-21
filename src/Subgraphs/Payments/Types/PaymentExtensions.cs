namespace Demo.Payments.Types;

[ExtendObjectType<Payment>]
public static class PaymentExtensions
{
    [BindMember(nameof(Payment.OrderId))]
    public static Order GetOrder([Parent] Payment payment)
    {
        return new Order(payment.OrderId);
    }
}
