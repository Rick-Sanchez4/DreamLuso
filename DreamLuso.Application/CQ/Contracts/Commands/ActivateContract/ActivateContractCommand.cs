using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.Contracts.Commands.ActivateContract;

public record ActivateContractCommand(
    Guid ContractId
) : IRequest<Result<ActivateContractResponse, Success, Error>>;

public record ActivateContractResponse(
    Guid ContractId,
    string Status,
    DateTime ActivatedAt
);

