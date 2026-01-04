using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Helpers;
using Pronia.Models;
using Pronia.ViewModels.ProductViewModels;
using System.Threading.Tasks;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]

public class ProductController(AppDbContext _context, IWebHostEnvironment _environment) : Controller
{
    public async Task<IActionResult> Index()
    {
        List<ProductGetVM> vm = await _context.Products.Include(x => x.Category)
            .Select(product => new ProductGetVM()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.Category.Name,
                MainImagePath = product.MainImagePath,
                HoverImagePath = product.HoverImagePath,
                Price = product.Price
            }).ToListAsync();

        List<ProductGetVM> vms = new();




        return View(vm);
    }

    public async Task<IActionResult> Create()
    {
        await SendItemsWithViewBag();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {

        if (!ModelState.IsValid)
        {
            await SendItemsWithViewBag();
            return View(vm);
        }


        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "There is no such category!");
            return View(vm);
        }

        foreach (var tagId in vm.TagIds)
        {
            var isExistTag = await _context.Tags.AnyAsync(x => x.Id == tagId);

            if (!isExistTag)
            {
                ModelState.AddModelError("TagIds", "This Tag doesn't exist.");
                return View(vm);
            }
        }


        if (!vm.MainImage.CheckType())
        {
            ModelState.AddModelError("MainImage", "You can only add image types");
            return View(vm);
        }

        if (!vm.MainImage.CheckSize(2))
        {
            ModelState.AddModelError("MainImage", "Size must be maximum 2MB");
            return View(vm);
        }
        if (!vm.HoverImage.CheckType())
        {
            ModelState.AddModelError("HoverImage", "You can only add image types");
            return View(vm);
        }

        if (!vm.HoverImage.CheckSize(2))
        {
            ModelState.AddModelError("HoverImage", "Size must be maximum 2MB");
            return View(vm);
        }


        foreach (var image in vm.Images)    
        {
            if (!image.CheckType())
            {
                ModelState.AddModelError("Images", "You can only add image types");
                return View(vm);
            }

            if (!image.CheckSize(2))
            {
                ModelState.AddModelError("Images", "Size must be maximum 2MB");
                return View(vm);
            }
        }

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        /*string uniqueMainImageName = Guid.NewGuid().ToString() + vm.MainImage.FileName;
        string mainImagePath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images",uniqueMainImageName);

        using FileStream mainStream = new FileStream(mainImagePath, FileMode.Create);

        await vm.MainImage.CopyToAsync(mainStream);

        string uniqueHoverImageName = Guid.NewGuid().ToString() + vm.HoverImage.FileName;
        string hoverImagePath = @$"{_environment.WebRootPath}/assets/images/website-images/{uniqueHoverImageName}";

        using FileStream hoverStream = new FileStream(hoverImagePath, FileMode.Create);

        await vm.HoverImage.CopyToAsync(hoverStream);*/

        string uniqueMainImageName = await vm.MainImage.SaveFileAsync(folderPath);
        string uniqueHoverImageName = await vm.HoverImage.SaveFileAsync(folderPath);


        Product product = new()
        {
            Name = vm.Name,
            Description = vm.Description,
            CategoryId = vm.CategoryId,
            Price = vm.Price,
            MainImagePath = uniqueMainImageName,
            HoverImagePath = uniqueHoverImageName,
            Rating = vm.Rating,
            ProductTags = [],
            ProductImages = [],
        };


        foreach (var image in vm.Images)
        {
            string uniqueFilePath = await image.SaveFileAsync(folderPath);

            ProductImage productImage = new()
            {
                ImagePath = uniqueFilePath,
                product = product
            };

            product.ProductImages.Add(productImage);

        }

        foreach (var tag in vm.TagIds)
        {
            ProductTag productTag = new()
            {
                TagId = tag,
                Product = product
            };

            product.ProductTags.Add(productTag);
        }


        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var product = await _context.Products.Include(x => x.ProductTags).Include(x=>x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);

        if (product == null) return NotFound();

        await SendItemsWithViewBag();

        ProductUpdateVM vm = new ProductUpdateVM()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            Price = product.Price,
            Rating = product.Rating,
            MainImagePath = product.MainImagePath,
            HoverImagePath = product.HoverImagePath,
            TagIds = product.ProductTags.Select(x => x.TagId).ToList(),
            AdditionalImagePaths = product.ProductImages.Select(x => x.ImagePath).ToList(),
            AdditionalImageIds = product.ProductImages.Select(x=> x.Id).ToList(),
        };

        return View(vm);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ProductUpdateVM vm)
    {

        if (!ModelState.IsValid)
        {
            await SendItemsWithViewBag();
            return View(vm);
        }

        foreach (var tagId in vm.TagIds)
        {
            var isExistTag = await _context.Tags.AnyAsync(x => x.Id == tagId);

            if (!isExistTag)
            {
                ModelState.AddModelError("TagIds", "This Tag doesn't exist.");
                return View(vm);
            }
        }

        if (!vm.MainImage?.CheckType() ?? false)
        {
            ModelState.AddModelError("MainImage", "You can only add image types");
            return View(vm);
        }

        if (!vm.MainImage?.CheckSize(2) ?? false)
        {
            ModelState.AddModelError("MainImage", "Size must be maximum 2MB");
            return View(vm);
        }
        if (!vm.HoverImage?.CheckType() ?? false)
        {
            ModelState.AddModelError("HoverImage", "You can only add image types");
            return View(vm);
        }

        if (!vm.HoverImage?.CheckSize(2) ?? false)
        {
            ModelState.AddModelError("HoverImage", "Size must be maximum 2MB");
            return View(vm);
        }

        foreach (var image in vm.Images ?? [])
        {
            if (!image.CheckType())
            {
                ModelState.AddModelError("Images", "You can only add image types");
                return View(vm);
            }

            if (!image.CheckSize(2))
            {
                ModelState.AddModelError("Images", "Size must be maximum 2MB");
                return View(vm);
            }
        } 


        var existProduct = await _context.Products.Include(x => x.ProductTags).Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == vm.Id);
        if (existProduct == null) return BadRequest();

        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "There is no such category!");
            return View(vm);
        }

        existProduct.Name = vm.Name;
        existProduct.Description = vm.Description;
        existProduct.Price = vm.Price;
        existProduct.Rating = vm.Rating;
        //existProduct.ImagePath = product.ImagePath;
        existProduct.CategoryId = vm.CategoryId;

        existProduct.ProductTags = [];

        foreach (var tagId in vm.TagIds)
        {
            ProductTag productTag = new()
            {
                TagId = tagId,
                ProductId = existProduct.Id
            };

            existProduct.ProductTags.Add(productTag);
        }


        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        if (vm.MainImage is { })
        {
            string newMainImagePath = await vm.MainImage.SaveFileAsync(folderPath);

            string existMainImagePath = Path.Combine(folderPath, existProduct.MainImagePath);

            ExtensionMethods.DeleteFile(existMainImagePath);

            existProduct.MainImagePath = newMainImagePath;
        }

        if (vm.HoverImage is { })
        {
            string newHoverImagePath = await vm.HoverImage.SaveFileAsync(folderPath);

            string existHoverImagePath = Path.Combine(folderPath, existProduct.HoverImagePath);

            ExtensionMethods.DeleteFile(existHoverImagePath);

            existProduct.HoverImagePath = newHoverImagePath;
        }

        var existImages = existProduct.ProductImages.ToList();

        foreach (var image in existImages)
        {
            var isExistImageId = vm.AdditionalImageIds?.Any(x => x == image.Id) ?? false;

            if (!isExistImageId)
            {
                string deletedImagePath = Path.Combine(folderPath, image.ImagePath);
                ExtensionMethods.DeleteFile(deletedImagePath);
                existProduct.ProductImages.Remove(image);
            }
        }

        foreach (var image in vm.Images ?? [])
        {
            string uniqueFilePath = await image.SaveFileAsync(folderPath);

            ProductImage productImage = new()
            {
                ImagePath = uniqueFilePath,
                ProductID    = existProduct.Id
            };

            existProduct.ProductImages.Add(productImage);

        }



        _context.Products.Update(existProduct);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) { return NotFound(); }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        string mainImagePath = Path.Combine(folderPath, product.MainImagePath);
        string hoverImagePath = Path.Combine(folderPath, product.HoverImagePath);


        ExtensionMethods.DeleteFile(mainImagePath); 
        ExtensionMethods.DeleteFile(hoverImagePath);


        foreach (var productImage in product.ProductImages)
        {
            string imagePath = Path.Combine(folderPath, productImage.ImagePath);

            ExtensionMethods.DeleteFile(imagePath);
        }

        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Detail(int id)
    {
        var product = await _context.Products.Include(x => x.Category).Select(product => new ProductGetVM()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            CategoryName = product.Category.Name,
            MainImagePath = product.MainImagePath,
            HoverImagePath = product.HoverImagePath,
            Price = product.Price,
            TagNames = product.ProductTags.Select(x => x.Tag.Name).ToList(),
            AdditionalImagePaths = product.ProductImages.Select(x => x.ImagePath).ToList()
        }).FirstOrDefaultAsync(x => x.Id == id);


        if (product is null)
            return NotFound();

        return View(product);
    }


    private async Task SendItemsWithViewBag()
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;


        var tags = await _context.Tags.ToListAsync();

        ViewBag.Tags = tags;
    }
}
