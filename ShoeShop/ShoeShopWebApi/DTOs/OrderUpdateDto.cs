namespace ShoeShopWebApi.DTOs
{
    // Изменение статуса заказа и даты доставки
    public class OrderUpdateDto
    {
        public string? Status { get; set; }
        public DateOnly? DeliveryDate { get; set; }
    }
}

