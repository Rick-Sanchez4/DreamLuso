using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.Common.Services;

public class FileUploadService : IFileUploadService
{
    private readonly ILogger<FileUploadService> _logger;
    private readonly string _uploadPath;
    private readonly string _baseUrl;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

    public FileUploadService(ILogger<FileUploadService> logger, string uploadPath, string baseUrl)
    {
        _logger = logger;
        _uploadPath = uploadPath;
        _baseUrl = baseUrl;

        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        if (!IsValidImageFile(file))
        {
            throw new ArgumentException("Tipo de ficheiro inválido");
        }

        var folderPath = Path.Combine(_uploadPath, folder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative path for static file serving (wwwroot/images/{folder}/{fileName})
        var fileUrl = $"/images/{folder}/{fileName}";
        _logger.LogInformation("Ficheiro carregado com sucesso: {FileUrl}", fileUrl);

        return fileUrl;
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        try
        {
            // Handle both relative paths (/images/profiles/file.jpg) and full URLs
            string fileName;
            string folder;
            
            if (fileUrl.StartsWith("http"))
            {
                // Full URL - extract path
                var uri = new Uri(fileUrl);
                var pathParts = uri.AbsolutePath.TrimStart('/').Split('/');
                if (pathParts.Length >= 3 && pathParts[0] == "images")
                {
                    folder = pathParts[1];
                    fileName = pathParts[2];
                }
                else
                {
                    fileName = Path.GetFileName(uri.AbsolutePath);
                    folder = "profiles"; // Default
                }
            }
            else
            {
                // Relative path like /images/profiles/file.jpg
                var pathParts = fileUrl.TrimStart('/').Split('/');
                if (pathParts.Length >= 3 && pathParts[0] == "images")
                {
                    folder = pathParts[1];
                    fileName = pathParts[2];
                }
                else
                {
                    fileName = Path.GetFileName(fileUrl);
                    folder = "profiles"; // Default
                }
            }
            
            var filePath = Path.Combine(_uploadPath, folder, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Ficheiro eliminado: {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao eliminar ficheiro: {FileUrl}", fileUrl);
        }

        return Task.CompletedTask;
    }

    public bool IsValidImageFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > _maxFileSize)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }
}

