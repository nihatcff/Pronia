using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Abstraction;
using Pronia.Contexts;
using Pronia.Services;
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

            var basketItems = await _service.GetBasketItemsAsync();

            return PartialView("_BasketPartialView", basketItems);
        }

        public async Task<IActionResult> IncreaseBasketItemCount(int productId)
        {

            var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);

            if (isExistProduct == false)
                return NotFound();

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);

            if (isExistUser == false)
                return BadRequest();

            var basketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);

            if (basketItem == null)
                return NotFound();

            basketItem.Count++;


            _context.Update(basketItem);
            await _context.SaveChangesAsync();

            var basketItems = await _service.GetBasketItemsAsync();

            return PartialView("_BasketPartialView", basketItems);

        }
    }
}
