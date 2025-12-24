using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShopLibrary.Models;

public partial class Order
{
    public DateOnly OrderDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public int CustomerId { get; set; }

    public int PickupCode { get; set; }

    public string Status { get; set; } = null!;

    public int OrderId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // Суммарная стоимость заказа с учетом скидок (вычисляемое свойство)
    [NotMapped]
    public decimal TotalPrice { get; set; }
}

