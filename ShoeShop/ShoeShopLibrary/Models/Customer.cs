using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShoeShopLibrary.Models;

public partial class Customer
{
    public string FullName { get; set; } = null!;

    public int CustomerId { get; set; }

    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
