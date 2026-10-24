namespace Shop.Orders;

public sealed class OrderItem
{
    public decimal Price { get; }
    public int Quantity { get; }

    public OrderItem(decimal price, int quantity)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Price = price;
        Quantity = quantity;
    }
}
