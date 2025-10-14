using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Contracts.Commands.ActivateContract;

public class ActivateContractCommandHandler : IRequestHandler<ActivateContractCommand, Result<ActivateContractResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateContractCommandHandler> _logger;

    public ActivateContractCommandHandler(IUnitOfWork unitOfWork, ILogger<ActivateContractCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ActivateContractResponse, Success, Error>> Handle(ActivateContractCommand request, CancellationToken cancellationToken)
    {
        // Get contract
        var contractObj = await _unitOfWork.ContractRepository.GetByIdAsync(request.ContractId);
        if (contractObj == null)
        {
            _logger.LogWarning("Contrato não encontrado: {ContractId}", request.ContractId);
            return Error.ContractNotFound;
        }

        var contract = (Contract)contractObj;

        // Check if already active
        if (contract.Status == ContractStatus.Active)
        {
            _logger.LogWarning("Contrato já está ativo: {ContractId}", contract.Id);
            return Error.ContractAlreadyActive;
        }

        // Activate contract
        contract.Activate();

        // Update property status based on contract type
        var property = await _unitOfWork.PropertyRepository.GetByIdAsync(contract.PropertyId);
        if (property != null)
        {
            var prop = (Property)property;
            if (contract.Type == ContractType.Sale)
            {
                prop.MarkAsSold();
            }
            else if (contract.Type == ContractType.Rent)
            {
                prop.MarkAsRented();
            }
            await _unitOfWork.PropertyRepository.UpdateAsync(prop);
        }

        await _unitOfWork.ContractRepository.UpdateAsync(contract);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Contrato ativado: {ContractId}, Tipo: {Type}", contract.Id, contract.Type);

        var response = new ActivateContractResponse(
            contract.Id,
            contract.Status.ToString(),
            DateTime.UtcNow
        );

        return response;
    }
}

