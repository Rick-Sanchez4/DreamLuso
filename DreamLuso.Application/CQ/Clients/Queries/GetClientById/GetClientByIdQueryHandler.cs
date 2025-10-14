using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Clients.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Clients.Queries.GetClientById;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, Result<ClientResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetClientByIdQueryHandler> _logger;

    public GetClientByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetClientByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ClientResponse, Success, Error>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var clientObj = await _unitOfWork.ClientRepository.GetByIdAsync(request.Id);
        
        if (clientObj == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", request.Id);
            return Error.ClientNotFound;
        }

        var client = (Client)clientObj;

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
            IsActive = client.IsActive,
            MinBudget = client.MinBudget,
            MaxBudget = client.MaxBudget,
            PreferredContactMethod = client.PreferredContactMethod,
            CreatedAt = client.CreatedAt
        };

        _logger.LogInformation("Cliente encontrado: {ClientId}", client.Id);

        return response;
    }
}

