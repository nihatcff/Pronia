using Microsoft.EntityFrameworkCore;
using Pronia.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models;

public class Product:BaseEntity
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    
    [Required]
    [Precision(10,2)]
    [Range(0,double.MaxValue)]
    public decimal Price { get; set; }
    public Category? Category { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public string MainImagePath { get; set; }
    public string HoverImagePath { get; set; }
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    [Range(1, 5)]
    public double? Rating { get; set; }
} 
