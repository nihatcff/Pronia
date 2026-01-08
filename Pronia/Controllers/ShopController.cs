using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Abstraction;
using Pronia.ViewModels.ProductViewModels;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Pronia.Controllers
{
    public class ShopController(AppDbContext _context, IEmailService _emailService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Test()
        {
            await _emailService.SendEMailAsync("nihatrkh-mpa101@code.edu.az", "Pronia", "<h1 style='color:red'>Email service is done</h1>");
            return Ok("Ok");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products.Select(x => new ProductGetVM()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                AdditionalImagePaths = x.ProductImages.Select(x => x.ImagePath).ToList(),
                CategoryName = x.Category.Name,
                HoverImagePath = x.HoverImagePath,
                MainImagePath = x.MainImagePath,
                Price = x.Price,
                TagNames = x.ProductTags.Select(x => x.Tag.Name).ToList(),
                Rating = x.Rating

            }).FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

       /* public async Task<IActionResult> AddToBasket(int productId)
        {
            var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);

            if (isExistProduct == false)
            {
                return NotFound();
            }

            string userID = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var isExistUser = await _context.Users.AnyAsync(x => x.Id == userID) ;

            if (!isExistUser)
                return BadRequest();


            var existBasketItem = await _context

            BasketItem basketItem = new()
            {
                ProductId = productId,
                AppUserId = userID,
                Count = 1
            };

        }*/

    }

   
}
