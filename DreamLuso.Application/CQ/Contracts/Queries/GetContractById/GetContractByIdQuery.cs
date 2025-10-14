using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Contracts.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Contracts.Queries.GetContractById;

public class GetContractByIdQuery : IRequest<Result<ContractResponse, Success, Error>>
{
    public Guid Id { get; set; }
}

