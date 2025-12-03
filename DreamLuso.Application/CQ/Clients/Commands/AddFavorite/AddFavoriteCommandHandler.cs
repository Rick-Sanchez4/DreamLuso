using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Clients.Commands.AddFavorite;

public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Result<AddFavoriteResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddFavoriteCommandHandler> _logger;

    public AddFavoriteCommandHandler(IUnitOfWork unitOfWork, ILogger<AddFavoriteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AddFavoriteResponse, Success, Error>> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        // Verify client exists and load with favorites
        var clientObj = await _unitOfWork.ClientRepository.GetByIdWithFavoritesAsync(request.ClientId);
        if (clientObj == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", request.ClientId);
            return Error.ClientNotFound;
        }

        var client = (Client)clientObj;

        // Verify property exists
        var propertyObj = await _unitOfWork.PropertyRepository.GetByIdAsync(request.PropertyId);
        if (propertyObj == null)
        {
            _logger.LogWarning("Propriedade não encontrada: {PropertyId}", request.PropertyId);
            return Error.PropertyNotFound;
        }

        var property = (Property)propertyObj;

        // Check if already favorited
        if (client.FavoriteProperties.Any(p => p.Id == request.PropertyId))
        {
            _logger.LogInformation("Propriedade já está nos favoritos: ClientId={ClientId}, PropertyId={PropertyId}", 
                request.ClientId, request.PropertyId);
            return Error.PropertyAlreadyFavorited;
        }

        // Add to favorites
        client.FavoriteProperties.Add(property);
        property.FavoriteCount++;

        // Update changes
        await _unitOfWork.ClientRepository.UpdateAsync(client);
        await _unitOfWork.PropertyRepository.UpdateAsync(property);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Propriedade adicionada aos favoritos: ClientId={ClientId}, PropertyId={PropertyId}", 
            request.ClientId, request.PropertyId);

        return new AddFavoriteResponse(
            client.Id,
            property.Id,
            DateTime.UtcNow
        );
    }
}

