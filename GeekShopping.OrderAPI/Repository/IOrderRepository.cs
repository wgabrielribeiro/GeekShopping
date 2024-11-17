using GeekShopping.OrderAPI.Model;

namespace GeekShopping.OrderAPI.Repository;
public interface IOrderRepository
{
    Task<bool> AddOrder(OrderHeader order);
    Task UpdateOrderPaymentStatus(long orderHeaderId, bool status);
}

