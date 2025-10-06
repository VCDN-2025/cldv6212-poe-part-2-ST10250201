using System.Text.Json;

namespace ABCRetail.Functions.Shared;

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions Options =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
}
