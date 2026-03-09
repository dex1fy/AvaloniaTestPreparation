using System;
using System.Collections.Generic;

namespace AvaloniaZachetPreparation.Models;

public partial class MaterialType
{
    public int Id { get; set; }

    public string MaterialType1 { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
