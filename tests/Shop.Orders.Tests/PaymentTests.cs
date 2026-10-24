using Moq;
using NUnit.Framework;

namespace Shop.Orders.Tests;

public class PaymentTests
{
    [TestCase(CustomerKind.Regular, 100, 2, 500)]
    [TestCase(CustomerKind.Regular, 4999, 1, 5299)]
    [TestCase(CustomerKind.Regular, 5000, 1, 5000)]
    [TestCase(CustomerKind.Regular, 5001, 1, 5001)]
    [TestCase(CustomerKind.Vip, 1000, 1, 1200)]
    [TestCase(CustomerKind.Vip, 5500, 1, 5250)]
    [TestCase(CustomerKind.Vip, 6000, 1, 5400)]
    public void CorrectAmount_IsChargedOnce(CustomerKind customer, decimal price, int quantity, decimal expected)
    {
        var gateway = new Mock<IPaymentGateway>(MockBehavior.Strict);
        gateway.Setup(x => x.Pay(expected)).Returns(true);
        var service = new OrderService(gateway.Object);

        bool paid = service.Checkout(new[] { new OrderItem(price, quantity) }, customer);

        Assert.That(paid, Is.True);
        gateway.Verify(x => x.Pay(expected), Times.Once);
        gateway.VerifyNoOtherCalls();
    }

    [Test]
    public void MultipleItems_AreSummed()
    {
        var gateway = new Mock<IPaymentGateway>(MockBehavior.Strict);
        gateway.Setup(x => x.Pay(1000m)).Returns(true);
        var service = new OrderService(gateway.Object);
        Assert.That(service.Checkout(new[] { new OrderItem(100m, 2), new OrderItem(250m, 2) }, CustomerKind.Regular), Is.True);
        gateway.Verify(x => x.Pay(1000m), Times.Once);
        gateway.VerifyNoOtherCalls();
    }

    [Test]
    public void PaymentRejected_ReturnsFalse()
    {
        var gateway = new Mock<IPaymentGateway>();
        gateway.Setup(x => x.Pay(400m)).Returns(false);
        Assert.That(new OrderService(gateway.Object).Checkout(new[] { new OrderItem(100m, 1) }, CustomerKind.Regular), Is.False);
        gateway.Verify(x => x.Pay(400m), Times.Once);
    }

    [Test]
    public void PaymentUnavailable_IsHandled()
    {
        var gateway = new Mock<IPaymentGateway>();
        gateway.Setup(x => x.Pay(400m)).Throws(new InvalidOperationException("Учебная ошибка сервиса оплаты"));
        Assert.That(new OrderService(gateway.Object).Checkout(new[] { new OrderItem(100m, 1) }, CustomerKind.Regular), Is.False);
        gateway.Verify(x => x.Pay(400m), Times.Once);
    }
}
