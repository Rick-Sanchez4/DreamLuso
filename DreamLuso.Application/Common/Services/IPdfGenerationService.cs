namespace DreamLuso.Application.Common.Services;

public interface IPdfGenerationService
{
    Task<byte[]> GenerateContractPdfAsync(Guid contractId);
    Task<string> GenerateAndSaveContractPdfAsync(Guid contractId, string savePath);
}

