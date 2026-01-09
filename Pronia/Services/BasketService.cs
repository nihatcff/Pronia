using Microsoft.EntityFrameworkCore;
using Pronia.Abstraction;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pronia.Services
{
    public class BasketService(AppDbContext _context, IHttpContextAccessor _accessor): IBasketService
    {
        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
           string userId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);
            if (!isExistUser) return []; 

            var baskeItems = await _context.BasketItems.Include(x=>x.Product).Where(x=>x.AppUser .Id == userId).ToListAsync();

            return baskeItems;
        }
    }
}
