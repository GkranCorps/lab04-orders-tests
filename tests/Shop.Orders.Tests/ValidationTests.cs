using Moq;
using NUnit.Framework;

namespace Shop.Orders.Tests;

public class ValidationTests
{
    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositivePrice_IsRejected(decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(price, 1));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void NonPositiveQuantity_IsRejected(int quantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(1m, quantity));
    }

    [Test]
    public void ValidItem_PreservesPriceAndQuantity()
    {
        var item = new OrderItem(125.50m, 3);
        Assert.Multiple(() =>
        {
            Assert.That(item.Price, Is.EqualTo(125.50m));
            Assert.That(item.Quantity, Is.EqualTo(3));
        });
    }

    [Test]
    public void NullGateway_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderService(null!));
    }

    [Test]
    public void InvalidOrders_DoNotCallPayment()
    {
        var gateway = new Mock<IPaymentGateway>(MockBehavior.Strict);
        var service = new OrderService(gateway.Object);
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() => service.Checkout(null!, CustomerKind.Regular));
            Assert.Throws<ArgumentException>(() => service.Checkout(Array.Empty<OrderItem>(), CustomerKind.Regular));
            Assert.Throws<ArgumentException>(() => service.Checkout(new OrderItem[] { null! }, CustomerKind.Regular));
            Assert.Throws<ArgumentOutOfRangeException>(() => service.Checkout(new[] { new OrderItem(100m, 1) }, (CustomerKind)99));
        });
        gateway.VerifyNoOtherCalls();
    }

    [Test]
    public void TooLargeOrder_ThrowsWithoutCallingPayment()
    {
        var gateway = new Mock<IPaymentGateway>(MockBehavior.Strict);
        var service = new OrderService(gateway.Object);
        Assert.Throws<OverflowException>(() => service.Checkout(new[] { new OrderItem(decimal.MaxValue, 2) }, CustomerKind.Regular));
        gateway.VerifyNoOtherCalls();
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void DemoGateway_NonPositiveAmount_IsRejected(decimal amount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DemoPaymentGateway().Pay(amount));
    }

    [Test]
    public void DemoGateway_PositiveAmount_IsAccepted()
    {
        Assert.That(new DemoPaymentGateway().Pay(0.01m), Is.True);
    }
}
