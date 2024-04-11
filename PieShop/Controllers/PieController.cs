 using Microsoft.AspNetCore.Mvc;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Controllers
{
    public class PieController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly ICategoryRepository _categoryRepository;
        public PieController(IPieRepository pieRepository, ICategoryRepository categoryRepository)
        {
            _pieRepository = pieRepository;
            _categoryRepository = categoryRepository;
        }
        public IActionResult List()
        {
            //ViewBag.currentCategory = "Cheese cakes";
            // return View(_pieRepository.AllPies);
            PieViewModel pieViewModel = new PieViewModel(_pieRepository.AllPies,"All Pies");
            return View(pieViewModel);
        }

        public IActionResult Detail(int id)
        {
            var pie = _pieRepository.GetPieById(id);
            if (pie == null) 
            {
                return NotFound();
            }
            return View(pie);

        }
    }
}
