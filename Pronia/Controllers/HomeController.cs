using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context= context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var cards = _context.Cards.ToList();

            //ViewBag.Cards = cards;

            return View(cards);
        }
    }
}
