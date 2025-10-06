
namespace ABCRetail.Functions.Models;

public record ImageUploadDto(
    string FileName,
    string ContentBase64,
    string? Container // optional; default handled in function
);

