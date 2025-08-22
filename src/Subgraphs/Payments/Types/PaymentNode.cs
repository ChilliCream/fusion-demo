namespace Demo.Payments.Types;

[ObjectType<Payment>]
public static partial class PaymentNode
{
    [BindMember(nameof(Payment.OrderId))]
    public static Order GetOrder([Parent] Payment payment) 
        => new(payment.OrderId);
}
