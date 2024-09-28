using GeekShopping.CartAPI.Model.Data.ValueObjects;
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

        public CartController(ILogger<CartController> logger, ICartRepository cartRepository)
        {
            _logger = logger;
            _cartRepository = cartRepository ?? throw new
           ArgumentNullException(nameof(cartRepository));
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


    }
}
