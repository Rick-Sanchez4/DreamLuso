namespace DreamLuso.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entidade \"{name}\" ({key}) não foi encontrada.")
    {
    }
}

