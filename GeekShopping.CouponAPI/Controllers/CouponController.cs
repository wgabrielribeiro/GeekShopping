using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ILogger<CouponController> _logger;
        private readonly ICouponRepository _repository;

        public CouponController(ILogger<CouponController> logger, ICouponRepository _repository)
        {
            _logger = logger;
            this._repository = _repository ?? throw new
           ArgumentNullException(nameof(_repository));
        }


        [HttpGet("{couponCode}")]
        [Authorize]
        public async Task<ActionResult<CouponVO>> GetCouponByCode(string couponCode)
        {
            try
            {
                _logger.LogWarning("Pesquisando produto");
                var coupon = await _repository.GetCouponByCode(couponCode);

                if (coupon == null) return NotFound();

                return Ok(coupon);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
