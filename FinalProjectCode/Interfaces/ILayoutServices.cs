using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.BasketVM;
using NuGet.Common;

namespace FinalProjectCode.Interfaces
{
    public interface ILayoutServices
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<BasketVM>> GetBaskets();

    }
}
