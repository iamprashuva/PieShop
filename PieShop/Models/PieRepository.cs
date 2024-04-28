using Microsoft.EntityFrameworkCore;

namespace PieShop.Models
{
    public class PieRepository:IPieRepository
    {

        private PieShopDbContext _pieShopDbContext;
        public PieRepository(PieShopDbContext pieShopDbContext) 
        {
        _pieShopDbContext = pieShopDbContext;
        }

        public IEnumerable<Pie> AllPies 
        { get
            {
                return _pieShopDbContext.pies.Include(c => c.Category);
            }
        }
        public IEnumerable<Pie> PiesOfTheWeek
        { get
            {
                return _pieShopDbContext.pies.Include(c => c.Category).Where(p => p.IsPieOfTheWeek);
            }
        }
        public Pie? GetPieById (int pieId)
        {
            return _pieShopDbContext.pies.FirstOrDefault(p => p.PieId == pieId);
        }

        public IEnumerable<Pie> SearchPies(string searchQuery)
        {
            return _pieShopDbContext.pies.Where(p => p.Name.Contains(searchQuery));
        }
    }
}
