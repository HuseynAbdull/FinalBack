using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Interfaces;
using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.BasketVM;
using FinalProjectCode.ViewModels.WishlistVM;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace FinalProjectCode.Services
{
    public class LayoutServices :ILayoutServices
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutServices(AppDbContext appDbContext,IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BasketVM>> GetBaskets()
        {
            string basket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;
          

            if (!string.IsNullOrEmpty(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                foreach(BasketVM basketVM in basketVMs)
                {
                    Product product = await _appDbContext.Products
                        .FirstOrDefaultAsync(p =>p.Id == basketVM.Id && p.IsDeleted == false);
                    if (product != null) 
                    {
                        basketVM.Price = product.Price;
                        basketVM.DiscountedPrice = product.DiscountedPrice;
                        basketVM.Title = product.Title;
                        basketVM.Image = product.MainImage;
                        basketVM.Id = basketVM.Id;
                        
                    }
                }
            }


            return basketVMs;
        }

        public async Task<IEnumerable<WishlistVM>> GetWishlist()
        {
            string wishlist = _httpContextAccessor.HttpContext.Request.Cookies["wishlist"];

            List<WishlistVM> wishlistVMs = null;
            if (!string.IsNullOrWhiteSpace(wishlist))
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(wishlist);

                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product product = await _appDbContext.Products
                        .FirstOrDefaultAsync(p => p.Id == wishlistVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        wishlistVM.Price = product.Price;
                        wishlistVM.Title = product.Title;
                        wishlistVM.Image = product.MainImage;
                    }

                }
            }
            else
            {
                wishlistVMs = new List<WishlistVM>();
            }

            return wishlistVMs;
        }

        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string,string> settings = await _appDbContext.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
        
            return settings;
        }

    }
}
