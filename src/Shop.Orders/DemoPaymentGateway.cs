namespace Shop.Orders;

// Учебная заглушка: реальные платежи не выполняются.
public sealed class DemoPaymentGateway : IPaymentGateway
{
    public bool Pay(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        return true;
    }
}
