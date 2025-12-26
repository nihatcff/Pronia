using Pronia.Models.Common;

namespace Pronia.Models;

public class ProductImage : BaseEntity
{
    public int ProductID { get; set; }
    public Product product { get; set; }
    public string ImagePath { get; set; }

}
