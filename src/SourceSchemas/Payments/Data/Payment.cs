namespace Demo.Payments.Data;

public class Payment
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
