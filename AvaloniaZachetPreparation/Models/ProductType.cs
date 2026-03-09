using System;
using System.Collections.Generic;

namespace AvaloniaZachetPreparation.Models;

public partial class ProductType
{
    public int Id { get; set; }

    public string ProductType1 { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
