

namespace ABCRetail.Functions.Models;

public record OrderCreatedDto(
    string OrderId,
    string CustomerId,
    decimal Total,
    string Timestamp);

