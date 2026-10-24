namespace Shop.Orders;

public interface IPaymentGateway
{
    bool Pay(decimal amount);
}
