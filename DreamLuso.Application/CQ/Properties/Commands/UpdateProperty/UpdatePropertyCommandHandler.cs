using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Properties.Commands.UpdateProperty;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Result<UpdatePropertyResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePropertyCommandHandler> _logger;

    public UpdatePropertyCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdatePropertyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UpdatePropertyResponse, Success, Error>> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var propertyObj = await _unitOfWork.PropertyRepository.GetByIdAsync(request.Id);
        if (propertyObj == null)
        {
            _logger.LogWarning("Imóvel não encontrado: {PropertyId}", request.Id);
            return Error.PropertyNotFound;
        }

        var property = (Property)propertyObj;

        // Update fields
        property.Title = request.Title;
        property.Description = request.Description;
        property.UpdatePrice(request.Price);
        property.UpdateStatus((PropertyStatus)request.Status);
        property.Amenities = request.Amenities;

        await _unitOfWork.PropertyRepository.UpdateAsync(property);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Imóvel atualizado: {PropertyId}", property.Id);

        var response = new UpdatePropertyResponse(
            property.Id,
            property.Title,
            property.Price,
            property.Status.ToString(),
            property.UpdatedAt ?? DateTime.UtcNow
        );

        return response;
    }
}

