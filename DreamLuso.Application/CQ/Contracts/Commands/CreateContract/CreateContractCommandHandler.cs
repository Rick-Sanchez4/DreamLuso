using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Contracts.Commands.CreateContract;

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, Result<CreateContractResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateContractCommandHandler> _logger;

    public CreateContractCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateContractCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateContractResponse, Success, Error>> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        // Verify property exists
        var property = await _unitOfWork.PropertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null)
        {
            _logger.LogWarning("Imóvel não encontrado: {PropertyId}", request.PropertyId);
            return Error.PropertyNotFound;
        }

        var prop = (Property)property;

        // Check if property is available (permitir Reserved e UnderContract para criação de contrato)
        if (prop.Status == PropertyStatus.Sold || prop.Status == PropertyStatus.Rented)
        {
            _logger.LogWarning("Imóvel já não está disponível: {PropertyId}, Status: {Status}", 
                request.PropertyId, prop.Status);
            return Error.PropertyUnavailable;
        }
        
        // Permitir criação de contrato mesmo se estiver Reserved ou UnderContract
        // (isso acontece quando uma proposta foi aprovada e o contrato está sendo criado)

        // Verify client exists
        var client = await _unitOfWork.ClientRepository.GetByIdAsync(request.ClientId);
        if (client == null)
        {
            _logger.LogWarning("Cliente não encontrado: {ClientId}", request.ClientId);
            return Error.ClientNotFound;
        }

        // Verify agent exists
        var agent = await _unitOfWork.RealEstateAgentRepository.GetByIdAsync(request.RealEstateAgentId);
        if (agent == null)
        {
            _logger.LogWarning("Agente não encontrado: {AgentId}", request.RealEstateAgentId);
            return Error.AgentNotFound;
        }

        // Validate dates
        if (request.EndDate.HasValue && request.EndDate.Value <= request.StartDate)
        {
            _logger.LogWarning("Data de fim deve ser posterior à data de início");
            return Error.InvalidContractDates;
        }

        // Create contract
        var contract = new Contract
        {
            PropertyId = request.PropertyId,
            Property = prop,
            ClientId = request.ClientId,
            Client = (Client)client,
            RealEstateAgentId = request.RealEstateAgentId,
            RealEstateAgent = (RealEstateAgent)agent,
            Type = request.Type,
            Status = ContractStatus.Draft,
            Value = request.Value,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            SignatureDate = DateTime.UtcNow,
            MonthlyRent = request.MonthlyRent,
            SecurityDeposit = request.SecurityDeposit,
            Commission = request.Commission,
            PaymentFrequency = request.PaymentFrequency,
            PaymentDay = request.PaymentDay,
            AutoRenewal = request.AutoRenewal,
            TermsAndConditions = request.TermsAndConditions
        };

        // Save contract
        var savedContract = await _unitOfWork.ContractRepository.SaveAsync(contract);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Contrato criado: {ContractId}, Imóvel: {PropertyId}, Cliente: {ClientId}", 
            savedContract.Id, request.PropertyId, request.ClientId);

        var response = new CreateContractResponse(
            savedContract.Id,
            savedContract.PropertyId,
            savedContract.ClientId,
            savedContract.Type,
            savedContract.Value,
            savedContract.Status.ToString(),
            savedContract.CreatedAt
        );

        return response;
    }
}

