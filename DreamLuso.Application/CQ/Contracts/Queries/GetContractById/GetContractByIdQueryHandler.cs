using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Contracts.Common;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Contracts.Queries.GetContractById;

public class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, Result<ContractResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetContractByIdQueryHandler> _logger;

    public GetContractByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetContractByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContractResponse, Success, Error>> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contractObj = await _unitOfWork.ContractRepository.GetByIdWithDetailsAsync(request.Id);
        
        if (contractObj == null)
        {
            _logger.LogWarning("Contrato não encontrado: {ContractId}", request.Id);
            return Error.ContractNotFound;
        }

        var contract = (Contract)contractObj;

        var response = new ContractResponse
        {
            Id = contract.Id,
            PropertyId = contract.PropertyId,
            PropertyTitle = contract.Property.Title,
            ClientId = contract.ClientId,
            ClientName = contract.Client.User.Name.FullName,
            AgentId = contract.RealEstateAgentId,
            AgentName = contract.RealEstateAgent.User.Name.FullName,
            Type = contract.Type.ToString(),
            Status = contract.Status.ToString(),
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Value = contract.Value,
            MonthlyRent = contract.MonthlyRent,
            SecurityDeposit = contract.SecurityDeposit,
            Commission = contract.Commission,
            PaymentFrequency = contract.PaymentFrequency?.ToString(),
            AutoRenewal = contract.AutoRenewal,
            SignatureDate = contract.SignatureDate,
            CreatedAt = contract.CreatedAt
        };

        _logger.LogInformation("Contrato encontrado: {ContractId}", contract.Id);

        return response;
    }
}

