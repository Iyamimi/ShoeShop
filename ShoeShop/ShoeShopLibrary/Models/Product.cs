using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShopLibrary.Models;

public partial class Product
{
    public string Article { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal Price { get; set; }

    public int SupplierId { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public int Discount { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public string? Photo { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Supplier Supplier { get; set; } = null!;

    // Цена с учетом скидки (рассчитывается на основе базовой цены и процента скидки)
    [NotMapped]
    public decimal DiscountPrice => Discount > 0 ? Price * (100 - Discount) / 100 : Price;

    // Флаг выбора элемента в пользовательском интерфейсе
    [NotMapped]
    public bool IsSelected { get; set; }
}
