using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Properties.Commands.DeleteProperty;

public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Result<DeletePropertyResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePropertyCommandHandler> _logger;

    public DeletePropertyCommandHandler(IUnitOfWork unitOfWork, ILogger<DeletePropertyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<DeletePropertyResponse, Success, Error>> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var propertyObj = await _unitOfWork.PropertyRepository.GetByIdAsync(request.Id);
        if (propertyObj == null)
        {
            _logger.LogWarning("Imóvel não encontrado: {PropertyId}", request.Id);
            return Error.PropertyNotFound;
        }

        var property = (Property)propertyObj;

        // Soft delete - just mark as inactive
        property.IsActive = false;
        property.UpdateStatus(PropertyStatus.Unavailable);

        await _unitOfWork.PropertyRepository.UpdateAsync(property);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Imóvel desativado (soft delete): {PropertyId}", property.Id);

        var response = new DeletePropertyResponse(
            property.Id,
            true
        );

        return response;
    }
}

