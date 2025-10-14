using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Clients.Commands.UpdateClient;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Result<UpdateClientResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateClientCommandHandler> _logger;

    public UpdateClientCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateClientCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UpdateClientResponse, Success, Error>> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var clientObj = await _unitOfWork.ClientRepository.GetByIdAsync(request.Id);
        if (clientObj == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", request.Id);
            return Error.ClientNotFound;
        }

        var client = (Client)clientObj;

        // Check if NIF already exists (excluding current client)
        if (!string.IsNullOrWhiteSpace(request.Nif) && request.Nif != client.Nif)
        {
            var existingNif = await _unitOfWork.ClientRepository.GetByNifAsync(request.Nif);
            if (existingNif != null)
            {
                _logger.LogWarning("NIF já existe: {Nif}", request.Nif);
                return Error.NifAlreadyExists;
            }
        }

        // Update fields
        client.Nif = request.Nif;
        client.UpdateBudgetRange(request.MinBudget, request.MaxBudget);
        client.PreferredContactMethod = request.PreferredContactMethod;
        client.PropertyPreferences = request.PropertyPreferences;

        await _unitOfWork.ClientRepository.UpdateAsync(client);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Cliente atualizado: {ClientId}", client.Id);

        var response = new UpdateClientResponse(
            client.Id,
            client.UpdatedAt ?? DateTime.UtcNow
        );

        return response;
    }
}

