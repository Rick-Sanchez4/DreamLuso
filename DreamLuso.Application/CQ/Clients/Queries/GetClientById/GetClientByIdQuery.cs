using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Clients.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Clients.Queries.GetClientById;

public class GetClientByIdQuery : IRequest<Result<ClientResponse, Success, Error>>
{
    public Guid Id { get; set; }
}

