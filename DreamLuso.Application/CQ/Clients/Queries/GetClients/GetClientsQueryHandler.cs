using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Clients.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Clients.Queries.GetClients;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, Result<GetClientsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetClientsQueryHandler> _logger;

    public GetClientsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetClientsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetClientsResponse, Success, Error>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var allClients = await _unitOfWork.ClientRepository.GetAllAsync();
        var clients = allClients.Cast<Client>().AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            clients = clients.Where(c =>
                (c.User != null && c.User.Name != null && c.User.Name.FullName.ToLower().Contains(searchLower)) ||
                (c.User != null && c.User.Email != null && c.User.Email.ToLower().Contains(searchLower)) ||
                (c.Nif != null && c.Nif.Contains(searchLower)));
        }

        if (request.IsActive.HasValue)
        {
            clients = clients.Where(c => c.IsActive == request.IsActive.Value);
        }

        // Get total count
        var totalCount = clients.Count();

        // Apply pagination
        var paginatedClients = clients
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var response = new GetClientsResponse
        {
            Clients = paginatedClients.Select(c => new ClientResponse
            {
                Id = c.Id,
                UserId = c.UserId,
                FullName = c.User?.Name?.FullName ?? "Nome não disponível",
                Email = c.User?.Email ?? "",
                Phone = c.User?.Phone ?? "",
                Nif = c.Nif,
                CitizenCard = c.CitizenCard,
                Type = c.Type.ToString(),
                IsActive = c.IsActive,
                MinBudget = c.MinBudget,
                MaxBudget = c.MaxBudget,
                PreferredContactMethod = c.PreferredContactMethod,
                CreatedAt = c.CreatedAt
            }),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        _logger.LogInformation("Listados {Count} clientes de um total de {Total}", paginatedClients.Count, totalCount);

        return response;
    }
}

