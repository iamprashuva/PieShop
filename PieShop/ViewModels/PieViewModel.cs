 using PieShop.Models;

namespace PieShop.ViewModels
{
    public class PieViewModel
    {
        public IEnumerable<Pie>? Pies { get;}
        public string? CurrentCategory { get; }

        public PieViewModel(IEnumerable<Pie>? pies, string? currentCategory)
        {
            Pies = pies;
            CurrentCategory = currentCategory;
        }

    }
}
