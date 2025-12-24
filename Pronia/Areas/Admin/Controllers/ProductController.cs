using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using System.Threading.Tasks;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]

public class ProductController(AppDbContext _context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(x => x.Category).ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        await SendCategoriesWithViewBag();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {

        if (!ModelState.IsValid)
        {
            await SendCategoriesWithViewBag();
            return View(product);
        }
        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == product.CategoryId);

        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "There is no such category!");
            return View(product);
        }
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return NotFound();

        await SendCategoriesWithViewBag();

        return View(product);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Product product)
    {
        if (!ModelState.IsValid)
        {
            await SendCategoriesWithViewBag();
            return View(product);
        }

        var existProduct = await _context.Products.FindAsync(product.Id);
        if (existProduct == null) return BadRequest();

        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == product.CategoryId);

        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "There is no such category!");
            return View(product);
        }

        existProduct.Name = product.Name;
        existProduct.Description = product.Description;
        existProduct.Price = product.Price;
        existProduct.ImagePath = product.ImagePath;
        existProduct.CategoryId = product.CategoryId;

        _context.Products.Update(existProduct);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));


    }

    
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) { return NotFound(); }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    private async Task SendCategoriesWithViewBag()
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;
    }
}
