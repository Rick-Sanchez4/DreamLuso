using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Clients.Commands.RemoveFavorite;

public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Result<RemoveFavoriteResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveFavoriteCommandHandler> _logger;

    public RemoveFavoriteCommandHandler(IUnitOfWork unitOfWork, ILogger<RemoveFavoriteCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RemoveFavoriteResponse, Success, Error>> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
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

        // Check if favorited
        var favoriteProperty = client.FavoriteProperties.FirstOrDefault(p => p.Id == request.PropertyId);
        if (favoriteProperty == null)
        {
            _logger.LogInformation("Propriedade não está nos favoritos: ClientId={ClientId}, PropertyId={PropertyId}", 
                request.ClientId, request.PropertyId);
            return Error.PropertyNotFavorited;
        }

        // Remove from favorites
        client.FavoriteProperties.Remove(favoriteProperty);
        if (property.FavoriteCount > 0)
        {
            property.FavoriteCount--;
        }

        // Update changes
        await _unitOfWork.ClientRepository.UpdateAsync(client);
        await _unitOfWork.PropertyRepository.UpdateAsync(property);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Propriedade removida dos favoritos: ClientId={ClientId}, PropertyId={PropertyId}", 
            request.ClientId, request.PropertyId);

        return new RemoveFavoriteResponse(
            client.Id,
            property.Id,
            DateTime.UtcNow
        );
    }
}

