using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Model.Context
{
    public class SqlContext : DbContext
    {
        public SqlContext() { }

        public SqlContext(DbContextOptions<SqlContext> options) : base(options) { }

        public DbSet<OrderHeader> Headers { get; set; }
        public DbSet<OrderDetail> Details { get; set; }
    }
}
