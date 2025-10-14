using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Contracts.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Contracts.Queries.GetContracts;

public class GetContractsQueryHandler : IRequestHandler<GetContractsQuery, Result<GetContractsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetContractsQueryHandler> _logger;

    public GetContractsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetContractsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetContractsResponse, Success, Error>> Handle(GetContractsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Contract> contracts;

        // Filter by client or agent if specified
        if (request.ClientId.HasValue)
        {
            contracts = (await _unitOfWork.ContractRepository.GetByClientIdAsync(request.ClientId.Value)).Cast<Contract>();
        }
        else if (request.AgentId.HasValue)
        {
            contracts = (await _unitOfWork.ContractRepository.GetByAgentIdAsync(request.AgentId.Value)).Cast<Contract>();
        }
        else
        {
            contracts = (await _unitOfWork.ContractRepository.GetAllAsync()).Cast<Contract>();
        }

        var query = contracts.AsQueryable();

        // Filter by status
        if (request.Status.HasValue)
        {
            query = query.Where(c => (int)c.Status == request.Status.Value);
        }

        // Get total count
        var totalCount = query.Count();

        // Apply pagination
        var paginatedContracts = query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var response = new GetContractsResponse
        {
            Contracts = paginatedContracts.Select(c => new ContractResponse
            {
                Id = c.Id,
                PropertyId = c.PropertyId,
                PropertyTitle = c.Property.Title,
                ClientId = c.ClientId,
                ClientName = c.Client.User.Name.FullName,
                AgentId = c.RealEstateAgentId,
                AgentName = c.RealEstateAgent.User.Name.FullName,
                Type = c.Type.ToString(),
                Status = c.Status.ToString(),
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Value = c.Value,
                MonthlyRent = c.MonthlyRent,
                SecurityDeposit = c.SecurityDeposit,
                Commission = c.Commission,
                PaymentFrequency = c.PaymentFrequency?.ToString(),
                AutoRenewal = c.AutoRenewal,
                SignatureDate = c.SignatureDate,
                CreatedAt = c.CreatedAt
            }),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        _logger.LogInformation("Listados {Count} contratos de um total de {Total}", paginatedContracts.Count, totalCount);

        return response;
    }
}

