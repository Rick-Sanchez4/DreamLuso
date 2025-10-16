using DreamLuso.Domain.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DreamLuso.Data.Infrastructure;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadDirectory;
    private readonly string _baseUrl;

    public FileStorageService(IConfiguration configuration)
    {
        _uploadDirectory = configuration["FileStorage:UploadDirectory"]
                               ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "properties");
        
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "/images/properties";

        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    public async Task<string> SaveFileAsync(object file, CancellationToken cancellationToken = default)
    {
        if (file is not IFormFile formFile || formFile.Length == 0)
        {
            throw new ArgumentException("File is empty or null", nameof(file));
        }

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}";
        var filePath = Path.Combine(_uploadDirectory, fileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await formFile.CopyToAsync(stream, cancellationToken);
        }

        return fileName;
    }

    public Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name is empty or null", nameof(fileName));
        }

        var filePath = Path.Combine(_uploadDirectory, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public string GetFileUrl(string fileName)
    {
        return $"{_baseUrl}/{fileName}";
    }
}

