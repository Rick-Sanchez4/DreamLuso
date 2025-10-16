using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class ImageUploadEndpoints
{
    public static void RegisterImageUploadEndpoints(this IEndpointRouteBuilder routes)
    {
        var images = routes.MapGroup("api/images").WithTags("Images");

        // POST /api/images/upload - Upload de imagens
        images.MapPost("/upload", UploadImages)
            .WithName("UploadImages")
            .DisableAntiforgery(); // Para aceitar multipart/form-data
    }

    private static async Task<Results<Ok<UploadImagesResponse>, BadRequest<string>>> UploadImages(
        [FromForm] IFormFileCollection files,
        IWebHostEnvironment environment)
    {
        if (files == null || files.Count == 0)
        {
            return TypedResults.BadRequest("Nenhum arquivo foi enviado");
        }

        var uploadedUrls = new List<string>();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var maxFileSize = 5 * 1024 * 1024; // 5MB

        // Criar diretório se não existir
        var uploadsPath = Path.Combine(environment.WebRootPath, "images", "properties");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        foreach (var file in files)
        {
            // Validar extensão
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                continue;
            }

            // Validar tamanho
            if (file.Length > maxFileSize)
            {
                continue;
            }

            // Gerar nome único
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Salvar arquivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Adicionar URL relativo
            uploadedUrls.Add($"/images/properties/{fileName}");
        }

        return TypedResults.Ok(new UploadImagesResponse
        {
            Urls = uploadedUrls,
            Count = uploadedUrls.Count
        });
    }
}

public class UploadImagesResponse
{
    public List<string> Urls { get; set; } = new();
    public int Count { get; set; }
}

