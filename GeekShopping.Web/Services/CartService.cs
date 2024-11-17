using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;
        public const string BasePath = "api/v1/Cart";
        public CartService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<CartViewModel> FindCartByUserId(string userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var teste = $"{BasePath}/find-cart/{userId}";

            var response = await _httpClient.GetAsync($"{BasePath}/find-cart/{userId}");

            var tt = response.Content.ReadAsStringAsync().Result;

            return await response.ReadContentAs<CartViewModel>();
        }
        public async Task<CartViewModel> AddItemToCart(CartViewModel cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJson($"{BasePath}/add-cart", cart);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();
            else throw new Exception("Something went wrong calling the API:" + response.Content.ReadAsStringAsync().Result);
        }
        public async Task<CartViewModel> UpdateCart(CartViewModel cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJson($"{BasePath}/update-cart", cart);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();
            else throw new Exception("Something went wrong calling the API:");
        }
        public async Task<bool> RemoveFromCart(long Cartid, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{BasePath}/remove-cart/{Cartid}");

            if (response.IsSuccessStatusCode)
                return true; //await response.ReadContentAs<bool>();
            else throw new Exception("Something went wrong calling the API:");
        }


        public async Task<bool> ApplyCoupon(CartViewModel cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJson($"{BasePath}/apply-coupon", cart);

            var kkk = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();

            else throw new Exception("Something went wrong calling the API:");
        }
        public async Task<bool> RemoveCoupon(string userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{BasePath}/remove-coupon/{userId}");

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();

            else throw new Exception("Something went wrong calling the API:");
        }

        public Task<bool> ClearCart(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<object> CheckOut(CartHeaderViewModel model, string token)
        {
            if (model.CouponCode is null)
                model.CouponCode = string.Empty;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJson($"{BasePath}/Checkout", model);

            var kkk = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                return await response.ReadContentAs<CartHeaderViewModel>();
            }
            else if (response.StatusCode.ToString().Equals("PreconditionFailed"))
            {
                return "Coupon Price has changed, please confirm!";
            }
            else throw new Exception("Something went wrong calling the API:");
        }
    }
}
