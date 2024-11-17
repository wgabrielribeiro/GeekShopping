using AutoMapper;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.Model.Data.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Reflection.Metadata.Ecma335;

namespace GeekShopping.CartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly SqlContext _sqlContext;
        private readonly IMapper _mapper;

        public CartRepository(SqlContext sqlContext, IMapper mapper)
        {
            _sqlContext = sqlContext;
            _mapper = mapper;
        }


        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var header = await _sqlContext.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
            if (header != null)
            {
                header.CouponCode = couponCode;
                _sqlContext.CartHeaders.Update(header);
                await _sqlContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public async Task<bool> RemoveCoupon(string userId)
        {
            var header = await _sqlContext.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
            if (header != null)
            {
                header.CouponCode = "";
                _sqlContext.CartHeaders.Update(header);
                await _sqlContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
        public async Task<bool> ClearCart(string userId)
        {
            var cartHeader = await _sqlContext.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cartHeader != null)
            {
                _sqlContext.CartDetails.RemoveRange(_sqlContext.CartDetails.Where(c => c.CartHeaderId == cartHeader.Id));

                _sqlContext.CartHeaders.Remove(cartHeader);
                await _sqlContext.SaveChangesAsync();

                return true;
            }
            return false;
        }
        public async Task<bool> RemoveFromCart(long CartDetailsId)
        {
            try
            {
                CartDetail cartDetail = await _sqlContext.CartDetails.FirstOrDefaultAsync(p => p.Id == CartDetailsId);

                int total = _sqlContext.CartDetails.Where(c => c.CartHeaderId == cartDetail.CartHeaderId).Count();

                _sqlContext.CartDetails.Remove(cartDetail);
                if (total == 1)
                {
                    var cartHeaderToRemove = await _sqlContext.CartHeaders.FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);
                    _sqlContext.CartHeaders.Remove(cartHeaderToRemove);

                }
                await _sqlContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<CartVO> FindCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _sqlContext.CartHeaders.FirstOrDefaultAsync(p => p.UserId == userId) ?? new CartHeader()
            };

            if (cart.CartHeader.CouponCode is null)
                cart.CartHeader.CouponCode = string.Empty;


            if (cart.CartHeader is null)
                return null;

            cart.CartDetails = await _sqlContext.CartDetails.Where(p => p.CartHeaderId == cart.CartHeader.Id)
                .Include(c => c.Product).ToListAsync();

            //if (!cart.CartDetails.Any())
            //    return null;

            return _mapper.Map<CartVO>(cart);
        }
        public async Task<CartVO> SaveOrUpdateCart(CartVO cartVO)
        {
            Cart cart = _mapper.Map<Cart>(cartVO);

            var product = await _sqlContext.Products.FirstOrDefaultAsync(c => c.Id == cartVO.CartDetails.FirstOrDefault().ProductId);

            if (product == null)
            {
                _sqlContext.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _sqlContext.SaveChangesAsync();
            }

            var cartHeader = await _sqlContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

            if (cartHeader == null)
            {
                _sqlContext.CartHeaders.Add(cart.CartHeader);
                await _sqlContext.SaveChangesAsync();

                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;

                _sqlContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _sqlContext.SaveChangesAsync();
            }
            else
            {
                var cartDeatil = await _sqlContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == cart.CartDetails.FirstOrDefault().ProductId
                && p.CartHeaderId == cartHeader.Id);

                if (cartDeatil == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;

                    _sqlContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _sqlContext.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDeatil.Count;
                    cart.CartDetails.FirstOrDefault().Id = cartDeatil.Id;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartDeatil.CartHeaderId;
                    _sqlContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                }

            }

            return _mapper.Map<CartVO>(cart);
        }
    }
}
