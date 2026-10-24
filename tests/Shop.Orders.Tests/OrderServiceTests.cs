using NUnit.Framework;

namespace Shop.Orders.Tests;

public class OrderServiceTests
{
    [Test]
    public void ValidRegularOrder_IsPaid()
    {
        var service = new OrderService();
        var items = new[] { new OrderItem(100m, 2) };

        bool result = service.Checkout(items, CustomerKind.Regular);

        Assert.That(result, Is.True);
    }
}
