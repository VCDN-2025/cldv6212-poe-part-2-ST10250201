namespace ABCRetail.Functions.Models;

public sealed class OrderEvent
{
    public string? OrderId { get; set; }
    public string? CustomerId { get; set; }
    public string? Sku { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}
