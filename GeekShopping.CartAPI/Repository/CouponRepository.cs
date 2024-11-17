using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GeekShopping.CartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;
        public const string BasePath = "api/v1/coupon";

        public CouponRepository(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<CouponVO> GetCoupon(string couponCode, string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"{BasePath}/{couponCode}");

            var content = await response.Content.ReadAsStringAsync();

            if(response.StatusCode != System.Net.HttpStatusCode.OK) return new CouponVO();


            return JsonSerializer.Deserialize<CouponVO>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
