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
        var clients = (await _unitOfWork.ClientRepository.GetAllAsync()).Cast<Client>();
        var client = clients.FirstOrDefault(c => c.UserId == request.UserId);

        if (client == null)
        {
            _logger.LogWarning("Cliente não encontrado para userId: {UserId}", request.UserId);
            return Error.ClientNotFound;
        }

        var response = new ClientResponse
        {
            Id = client.Id,
            UserId = client.UserId,
            FullName = client.User.Name.FullName,
            Email = client.User.Email,
            Phone = client.User.Phone,
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

