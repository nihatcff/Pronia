using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Abstraction;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pronia.Controllers
{
    public class BasketController(IBasketService _service, AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var basketItems = await _service.GetBasketItemsAsync();
            return View(basketItems);
        }

        public async Task<IActionResult> DecreaseBasketItemCount(int productId)
        {
            var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);

            if (isExistProduct == false)
            {
                return NotFound();
            }

            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var isExistUser = await _context.Users.AnyAsync(x => x.Id == userID);

            if (!isExistUser)
                return BadRequest();

            var basketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userID && x.ProductId == productId);

            if (basketItem == null) return NotFound();

            if (basketItem.Count > 1)
            {
                basketItem.Count--;
            }

            _context.BasketItems.Update(basketItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
