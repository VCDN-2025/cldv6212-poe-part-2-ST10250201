

namespace ABCRetail.Functions.Models;

public record InventoryAdjustDto(
    string Sku,
    int Delta,
    string Reason,
    string Timestamp);

