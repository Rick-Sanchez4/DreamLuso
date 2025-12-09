using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Clients.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Clients.Queries.GetClientByUserId;

public class GetClientByUserIdQueryHandler : IRequestHandler<GetClientByUserIdQuery, Result<ClientResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetClientByUserIdQueryHandler> _logger;

    public GetClientByUserIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetClientByUserIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ClientResponse, Success, Error>> Handle(GetClientByUserIdQuery request, CancellationToken cancellationToken)
    {
        var clientObj = await _unitOfWork.ClientRepository.GetByUserIdAsync(request.UserId);

        if (clientObj == null)
        {
            _logger.LogWarning("Cliente não encontrado para userId: {UserId}", request.UserId);
            
            // Verificar se o usuário existe e tem role de cliente
            var userObj = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (userObj != null)
            {
                var user = (User)userObj;
                if (user.Role == Domain.Model.UserRole.Client)
                {
                    _logger.LogInformation("Usuário com role Client encontrado mas sem perfil de cliente. Criando perfil automaticamente para userId: {UserId}", request.UserId);
                    
                    // Criar perfil de cliente automaticamente
                    var newClient = new Client
                    {
                        UserId = user.Id,
                        User = user,
                        Type = ClientType.Individual, // Default to Individual
                        IsActive = true
                    };

                    await _unitOfWork.ClientRepository.SaveAsync(newClient);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    
                    _logger.LogInformation("Perfil de cliente criado automaticamente para userId: {UserId} -> ClientId: {ClientId}", request.UserId, newClient.Id);
                    
                    // Recarregar o cliente com o User incluído
                    clientObj = await _unitOfWork.ClientRepository.GetByUserIdAsync(request.UserId);
                    if (clientObj == null)
                    {
                        _logger.LogError("Erro ao recarregar cliente após criação para userId: {UserId}", request.UserId);
                        return Error.ClientNotFound;
                    }
                }
                else
                {
                    return Error.ClientNotFound;
                }
            }
            else
            {
                return Error.ClientNotFound;
            }
        }

        var client = (Client)clientObj;

        var response = new ClientResponse
        {
            Id = client.Id,
            UserId = client.UserId,
            FullName = client.User?.Name?.FullName ?? "Nome não disponível",
            Email = client.User?.Email ?? "",
            Phone = client.User?.Phone ?? "",
            Nif = client.Nif,
            CitizenCard = client.CitizenCard,
            Type = client.Type.ToString(),
            MinBudget = client.MinBudget,
            MaxBudget = client.MaxBudget,
            PreferredContactMethod = client.PreferredContactMethod,
            IsActive = client.IsActive,
            CreatedAt = client.CreatedAt
        };

        _logger.LogInformation("Cliente encontrado por userId: {UserId} -> ClientId: {ClientId}", request.UserId, client.Id);

        return response;
    }
}

