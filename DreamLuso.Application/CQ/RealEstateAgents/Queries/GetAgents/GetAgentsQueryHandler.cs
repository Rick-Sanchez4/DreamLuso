using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.RealEstateAgents.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgents;

public class GetAgentsQueryHandler : IRequestHandler<GetAgentsQuery, Result<GetAgentsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAgentsQueryHandler> _logger;

    public GetAgentsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAgentsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetAgentsResponse, Success, Error>> Handle(GetAgentsQuery request, CancellationToken cancellationToken)
    {
        var allAgents = await _unitOfWork.RealEstateAgentRepository.GetAllAsync();
        var agents = allAgents.Cast<RealEstateAgent>().AsQueryable();

        // Get all properties to count per agent
        var allProperties = await _unitOfWork.PropertyRepository.GetAllAsync();
        var propertiesCount = allProperties.Cast<Property>()
            .GroupBy(p => p.RealEstateAgentId)
            .ToDictionary(g => g.Key, g => g.Count());

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            agents = agents.Where(a =>
                (a.User != null && a.User.Name != null && a.User.Name.FullName.ToLower().Contains(searchLower)) ||
                (a.User != null && a.User.Email != null && a.User.Email.ToLower().Contains(searchLower)) ||
                (a.LicenseNumber != null && a.LicenseNumber.Contains(searchLower)) ||
                (a.Specialization != null && a.Specialization.ToLower().Contains(searchLower)));
        }

        if (request.IsActive.HasValue)
        {
            agents = agents.Where(a => a.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Specialization))
        {
            agents = agents.Where(a => a.Specialization != null && 
                                      a.Specialization.Contains(request.Specialization));
        }

        // Get total count
        var totalCount = agents.Count();

        // Apply pagination
        var paginatedAgents = agents
            .OrderByDescending(a => a.Rating)
            .ThenByDescending(a => a.TotalSales)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var response = new GetAgentsResponse
        {
            Agents = paginatedAgents.Select(a => new AgentResponse
            {
                Id = a.Id,
                UserId = a.UserId,
                FullName = a.User?.Name?.FullName ?? "Nome não disponível",
                Email = a.User?.Email ?? "",
                Phone = a.User?.Phone ?? "",
                LicenseNumber = a.LicenseNumber,
                LicenseExpiry = a.LicenseExpiry,
                OfficeEmail = a.OfficeEmail,
                OfficePhone = a.OfficePhone,
                CommissionRate = a.CommissionRate,
                TotalSales = a.TotalSales,
                TotalListings = propertiesCount.GetValueOrDefault(a.Id, 0), // Calculate from actual properties
                TotalRevenue = a.TotalRevenue,
                Rating = a.Rating,
                ReviewCount = a.ReviewCount,
                IsActive = a.IsActive,
                Specialization = a.Specialization,
                Certifications = a.Certifications,
                LanguagesSpoken = a.LanguagesSpoken.Select(l => l.ToString()).ToList(),
                CreatedAt = a.CreatedAt,
                ApprovalStatus = a.IsActive ? "Approved" : "Pending"
            }),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        _logger.LogInformation("Listados {Count} agentes de um total de {Total}", paginatedAgents.Count, totalCount);

        return response;
    }
}

