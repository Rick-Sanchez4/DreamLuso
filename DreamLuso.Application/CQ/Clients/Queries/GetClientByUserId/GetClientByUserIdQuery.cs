using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Clients.Common;
using MediatR;

namespace DreamLuso.Application.CQ.Clients.Queries.GetClientByUserId;

public record GetClientByUserIdQuery(Guid UserId) : IRequest<Result<ClientResponse, Success, Error>>;

