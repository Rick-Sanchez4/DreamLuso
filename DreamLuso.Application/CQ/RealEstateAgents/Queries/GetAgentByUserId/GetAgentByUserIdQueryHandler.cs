using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.RealEstateAgents.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.RealEstateAgents.Queries.GetAgentByUserId;

public class GetAgentByUserIdQueryHandler : IRequestHandler<GetAgentByUserIdQuery, Result<AgentResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAgentByUserIdQueryHandler> _logger;

    public GetAgentByUserIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAgentByUserIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AgentResponse, Success, Error>> Handle(GetAgentByUserIdQuery request, CancellationToken cancellationToken)
    {
        var agentObj = await _unitOfWork.RealEstateAgentRepository.GetByUserIdAsync(request.UserId);

        if (agentObj == null)
        {
            _logger.LogWarning("Agente não encontrado para userId: {UserId}", request.UserId);
            
            // Verificar se o usuário existe e tem role de agente
            var userObj = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (userObj != null)
            {
                var user = (User)userObj;
                if (user.Role == Domain.Model.UserRole.RealEstateAgent)
                {
                    _logger.LogInformation("Usuário com role RealEstateAgent encontrado mas sem perfil de agente. Criando perfil automaticamente para userId: {UserId}", request.UserId);
                    
                    // Criar perfil de agente automaticamente
                    var newAgent = new RealEstateAgent
                    {
                        UserId = user.Id,
                        User = user,
                        IsActive = false, // Inativo até aprovação do admin
                        LicenseNumber = null,
                        CommissionRate = 0,
                        TotalSales = 0,
                        TotalListings = 0,
                        TotalRevenue = 0,
                        Rating = 0,
                        ReviewCount = 0
                    };

                    await _unitOfWork.RealEstateAgentRepository.SaveAsync(newAgent);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    
                    _logger.LogInformation("Perfil de agente criado automaticamente para userId: {UserId} -> AgentId: {AgentId}", request.UserId, newAgent.Id);
                    
                    // Recarregar o agente com o User incluído
                    agentObj = await _unitOfWork.RealEstateAgentRepository.GetByUserIdAsync(request.UserId);
                    if (agentObj == null)
                    {
                        _logger.LogError("Erro ao recarregar agente após criação para userId: {UserId}", request.UserId);
                        return Error.AgentNotFound;
                    }
                }
                else
                {
                    return Error.AgentNotFound;
                }
            }
            else
            {
                return Error.AgentNotFound;
            }
        }

        var agent = (RealEstateAgent)agentObj;

        var response = new AgentResponse
        {
            Id = agent.Id,
            UserId = agent.UserId,
            FullName = agent.User?.Name?.FullName ?? "Nome não disponível",
            Email = agent.User?.Email ?? "",
            Phone = agent.User?.Phone ?? "",
            LicenseNumber = agent.LicenseNumber,
            LicenseExpiry = agent.LicenseExpiry,
            OfficeEmail = agent.OfficeEmail,
            OfficePhone = agent.OfficePhone,
            CommissionRate = agent.CommissionRate,
            TotalSales = agent.TotalSales,
            TotalListings = agent.TotalListings,
            TotalRevenue = agent.TotalRevenue,
            Rating = agent.Rating,
            ReviewCount = agent.ReviewCount,
            IsActive = agent.IsActive,
            Specialization = agent.Specialization,
            Certifications = agent.Certifications,
            LanguagesSpoken = agent.LanguagesSpoken.Select(l => l.ToString()).ToList(),
            CreatedAt = agent.CreatedAt,
            ApprovalStatus = agent.IsActive ? "Approved" : "Pending"
        };

        _logger.LogInformation("Agente encontrado por userId: {UserId} -> AgentId: {AgentId}", request.UserId, agent.Id);

        return response;
    }
}

