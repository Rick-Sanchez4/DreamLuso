using FluentValidation.Results;

namespace DreamLuso.Application.Common.Exceptions;

public class ApplicationValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ApplicationValidationException()
        : base("Ocorreram um ou mais erros de validação.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ApplicationValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}

