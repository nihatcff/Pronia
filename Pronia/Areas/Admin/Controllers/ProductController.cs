using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.ViewModels.ProductViewModels;
using System;
using System.Threading.Tasks;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]

public class ProductController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
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
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
            
        if (!ModelState.IsValid)
        {
            await SendCategoriesWithViewBag();
            return View(vm);
        }
        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "There is no such category!");
            return View(vm);
        }

        if (!vm.MainImage.ContentType.Contains("image"))
        {
            ModelState.AddModelError("MainImage", "You can only add image types");
            return View(vm);
        }

        if (vm.MainImage.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("MainImage", "Size must be maximum 2MB");
            return View(vm);
        }
        if (!vm.HoverImage.ContentType.Contains("image"))
        {
            ModelState.AddModelError("HoverImage", "You can only add image types");
            return View(vm);
        }

        if (vm.HoverImage.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("HoverImage", "Size must be maximum 2MB");
            return View(vm);
        }
        string uniqueMainImageName = Guid.NewGuid().ToString() + vm.MainImage.FileName;
        string mainImagePath = @$"{_environment.WebRootPath}/assets/images/website-images/{uniqueMainImageName}";

        using FileStream mainStream = new FileStream(mainImagePath, FileMode.Create);

        await vm.MainImage.CopyToAsync(mainStream);

        string uniqueHoverImageName = Guid.NewGuid().ToString() + vm.HoverImage.FileName;
        string hoverImagePath = @$"{_environment.WebRootPath}/assets/images/website-images/{uniqueHoverImageName}";

        using FileStream hoverStream = new FileStream(hoverImagePath, FileMode.Create);

        await vm.HoverImage.CopyToAsync(hoverStream);


        Product product = new()
        {
            Name = vm.Name,
            Description = vm.Description,
            CategoryId = vm.CategoryId,
            Price = vm.Price,
            MainImagePath = uniqueMainImageName,
            HoverImagePath = uniqueHoverImageName,
            Rating = vm.Rating
        };



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
        //existProduct.ImagePath = product.ImagePath;
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

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        string mainImagePath = Path.Combine(folderPath, product.MainImagePath);
        string hoverImagePath = Path.Combine(folderPath, product.HoverImagePath);


        if (System.IO.File.Exists(mainImagePath))
            System.IO.File.Delete(mainImagePath);

        if (System.IO.File.Exists(hoverImagePath))
            System.IO.File.Delete(hoverImagePath);

        return RedirectToAction(nameof(Index));
    }


    private async Task SendCategoriesWithViewBag()
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;
    }
}
