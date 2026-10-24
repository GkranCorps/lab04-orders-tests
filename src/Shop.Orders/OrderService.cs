namespace Shop.Orders;

public sealed class OrderService
{
    private readonly IPaymentGateway gateway;

    public OrderService(IPaymentGateway gateway)
    {
        this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
    }

    public bool Checkout(IEnumerable<OrderItem> items, CustomerKind customer)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        decimal subtotal = 0;
        foreach (var item in items)
        {
            if (item is null)
                throw new ArgumentException("Позиция заказа не может быть null.", nameof(items));
            subtotal += item.Price * item.Quantity;
        }

        if (subtotal == 0)
            throw new ArgumentException("Заказ пуст.", nameof(items));

        decimal discount = customer switch
        {
            CustomerKind.Regular => 0m,
            CustomerKind.Vip => 0.10m,
            _ => throw new ArgumentOutOfRangeException(nameof(customer))
        };

        decimal amount = subtotal * (1 - discount);
        if (amount < 5000m)
            amount += 300m;

        try
        {
            return gateway.Pay(amount);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
