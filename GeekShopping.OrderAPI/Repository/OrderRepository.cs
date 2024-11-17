using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<SqlContext> _Context;

        public OrderRepository(DbContextOptions<SqlContext> Context)
        {
            _Context = Context;
        }

        public async Task<bool> AddOrder(OrderHeader order)
        {
            if (order == null)
                return false;

            await using var _db = new SqlContext(_Context);
            _db.Headers.Add(order);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(long orderHeaderId, bool status)
        {
            await using var _db = new SqlContext(_Context);
            var header = await _db.Headers.FirstOrDefaultAsync(o => o.Id == orderHeaderId);

            if (header != null)
            {
                header.PaymentStatus = status;
                await _db.SaveChangesAsync();
            }
        }
    }
}
