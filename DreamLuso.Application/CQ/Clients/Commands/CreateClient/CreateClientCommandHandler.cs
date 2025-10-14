using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Clients.Commands.CreateClient;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<CreateClientResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClientCommandHandler> _logger;

    public CreateClientCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateClientCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateClientResponse, Success, Error>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var userObj = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (userObj == null)
        {
            _logger.LogWarning("Utilizador não encontrado: {UserId}", request.UserId);
            return Error.UserNotFound;
        }

        var user = (User)userObj;

        // Check if client profile already exists for this user
        var existingClient = await _unitOfWork.ClientRepository.GetByUserIdAsync(request.UserId);
        if (existingClient != null)
        {
            _logger.LogWarning("Perfil de cliente já existe para o utilizador: {UserId}", request.UserId);
            return Error.ClientExists;
        }

        // Check if NIF already exists
        if (!string.IsNullOrWhiteSpace(request.Nif))
        {
            var existingNif = await _unitOfWork.ClientRepository.GetByNifAsync(request.Nif);
            if (existingNif != null)
            {
                _logger.LogWarning("NIF já existe: {Nif}", request.Nif);
                return Error.NifAlreadyExists;
            }
        }

        // Create client
        var client = new Client
        {
            UserId = request.UserId,
            User = user,
            Nif = request.Nif,
            CitizenCard = request.CitizenCard,
            Type = request.Type,
            MinBudget = request.MinBudget,
            MaxBudget = request.MaxBudget,
            PreferredContactMethod = request.PreferredContactMethod,
            IsActive = true
        };

        // Save client
        var savedClient = await _unitOfWork.ClientRepository.SaveAsync(client);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Perfil de cliente criado com sucesso: {ClientId}, User: {UserId}", savedClient.Id, request.UserId);

        var response = new CreateClientResponse(
            savedClient.Id,
            savedClient.UserId,
            user.Name.FullName,
            savedClient.Type,
            savedClient.CreatedAt
        );

        return response;
    }
}

