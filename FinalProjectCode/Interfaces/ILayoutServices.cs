using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.BasketVM;
using FinalProjectCode.ViewModels.WishlistVM;
using NuGet.Common;

namespace FinalProjectCode.Interfaces
{
    public interface ILayoutServices
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<BasketVM>> GetBaskets();
        Task<IEnumerable<WishlistVM>> GetWishlist();

    }
}
