using GeekShopping.OrderAPI.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.OrderAPI.Model
{
    [Table("Order_header")]
    public class OrderHeader : BaseEntity
    {
        [Column("uder_id")]
        public string UserId { get; set; } = "";

        [Column("coupon_code")]
        public string CouponCode { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime OrderTime { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryMothYear { get; set; }

        public long CartTotalItens { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
        public bool PaymentStatus { get; set; }

    }
}
