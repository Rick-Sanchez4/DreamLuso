namespace DreamLuso.Domain.Core.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(object file, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
    string GetFileUrl(string fileName);
}

