using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.Contracts.Commands.CreateContract;

public record CreateContractCommand(
    Guid PropertyId,
    Guid ClientId,
    Guid RealEstateAgentId,
    ContractType Type,
    decimal Value,
    DateTime StartDate,
    DateTime? EndDate,
    decimal? MonthlyRent,
    decimal? SecurityDeposit,
    decimal? Commission,
    PaymentFrequency? PaymentFrequency,
    int? PaymentDay,
    bool AutoRenewal,
    string? TermsAndConditions
) : IRequest<Result<CreateContractResponse, Success, Error>>;

public record CreateContractResponse(
    Guid ContractId,
    Guid PropertyId,
    Guid ClientId,
    ContractType Type,
    decimal Value,
    string Status,
    DateTime CreatedAt
);

