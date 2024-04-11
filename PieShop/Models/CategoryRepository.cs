namespace PieShop.Models
{
    public class CategoryRepository:ICategoryRepository
    {
        private PieShopDbContext _pieShopDbContext;

        public CategoryRepository(PieShopDbContext pieShopDbContext)
        {
            _pieShopDbContext = pieShopDbContext;
        }

        public IEnumerable<Category> AllCategories =>
            _pieShopDbContext.categories.OrderBy(p => p.CategoryName);
         
  
    }
}
