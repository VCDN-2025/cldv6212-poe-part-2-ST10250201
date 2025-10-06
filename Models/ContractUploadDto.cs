

namespace ABCRetail.Functions.Models;

public record ContractUploadDto(
    string FileName,
    string ContentBase64,
    string? Directory // optional subfolder in the share
);

