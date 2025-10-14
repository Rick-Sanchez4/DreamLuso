using Microsoft.AspNetCore.Http;

namespace DreamLuso.Application.Common.Services;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(IFormFile file, string folder);
    Task DeleteFileAsync(string fileUrl);
    bool IsValidImageFile(IFormFile file);
}

