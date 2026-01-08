using Microsoft.EntityFrameworkCore;
using Pronia.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models;

public class  Product:BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } 
    public Category Category { get; set; } 
    public int CategoryId { get; set; }
    public string MainImagePath { get; set; } = string.Empty;
    public string HoverImagePath { get; set; } = string.Empty;  
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    public ICollection<ProductTag> ProductTags { get; set; } = [];
    public ICollection<BasketItem> BasketItems { get; set; } = [];
    [Range(1, 5)]
    public double? Rating { get; set; }
} 
