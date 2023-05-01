using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.BasketVM;


namespace FinalProjectCode.ViewModels.OrderVM
{
    public class OrderVM
    {
        public Order Order { get; set; }

        public IEnumerable<BasketVM.BasketVM> BasketVMs { get; set; }

    }
}