using GeekShopping.CouponAPI.Model.Base;

namespace GeekShopping.CouponAPI.Data.ValueObjects;

public class CouponVO : BaseEntity
{
    public string? CouponCode { get; set; }
    public decimal? DiscountAmount { get; set; }
}

