using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.Model.Data.ValueObjects;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IRabbitMQMessageSender _rabbitMQMessageSender;

        public CartController(ILogger<CartController> logger, ICartRepository cartRepository, ICouponRepository couponRepository, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
            _rabbitMQMessageSender = rabbitMQMessageSender ?? throw new ArgumentNullException(nameof(rabbitMQMessageSender));
        }

        [HttpGet("find-cart/{id}")]
        [Authorize]
        public async Task<ActionResult<CartVO>> FindById(string id)
        {
            _logger.LogWarning("Pesquisando produto");
            var cart = await _cartRepository.FindCartByUserId(id);

            if (cart == null) return NotFound();

            return Ok(cart);
        }

        [HttpPost("add-cart")]
        [Authorize]
        public async Task<ActionResult<CartVO>> Addcart([FromBody] CartVO vo)
        {
            _logger.LogWarning("Add cart");

            var cart = await _cartRepository.SaveOrUpdateCart(vo);

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }

        [HttpPut("update-cart")]
        [Authorize]
        public async Task<ActionResult<CartVO>> Updatecart([FromBody] CartVO vo)
        {
            _logger.LogWarning("updt cart");

            var cart = await _cartRepository.SaveOrUpdateCart(vo);

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }

        [HttpDelete("remove-cart/{id}")]
        [Authorize]
        public async Task<ActionResult<CartVO>> RemoveCart(int id)
        {
            _logger.LogWarning("Add cart");

            var status = await _cartRepository.RemoveFromCart(id);

            if (!status)
                return BadRequest();

            return Ok(status);
        }

        [HttpPost("apply-coupon")]
        public async Task<ActionResult<CartVO>> ApplyCoupon([FromBody] CartVO vo)
        {
            _logger.LogWarning("Add coupon");

            var status = await _cartRepository.ApplyCoupon(vo.CartHeader.UserId, vo.CartHeader.CouponCode);

            if (!status) return NotFound();

            return Ok(status);
        }

        [HttpDelete("remove-coupon/{userId}")]
        public async Task<ActionResult<CartVO>> RemoveCoupon(string userId)
        {
            _logger.LogWarning("Remove coupon");

            var status = await _cartRepository.RemoveCoupon(userId);

            if (!status) return NotFound();

            return Ok(status);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<CheckoutHeaderVO>> Checkout(CheckoutHeaderVO vo)
        {
            string token = Request.Headers["Authorization"];

            if (vo?.UserId == null) return BadRequest();

            var cart = await _cartRepository.FindCartByUserId(vo.UserId);
            if (cart == null) return NotFound();

            if (!string.IsNullOrEmpty(vo.CouponCode))
            {
                CouponVO coupon = await _couponRepository.GetCoupon(vo.CouponCode, token.Replace("Bearer", "").Trim());

                if (vo.DiscountAmount != coupon.DiscountAmount)
                {
                    return StatusCode(412);
                }
            }


            vo.CartDetails = cart.CartDetails;
            vo.DateTime = DateTime.Now;

            //TASK RabbitMQ logic comes here!!!
            _rabbitMQMessageSender.SendMessage(vo, "checkoutQueue");


            return Ok(vo);
        }

    }
}
