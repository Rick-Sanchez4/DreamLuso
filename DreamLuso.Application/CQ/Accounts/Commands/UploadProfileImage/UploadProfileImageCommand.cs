using DreamLuso.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DreamLuso.Application.CQ.Accounts.Commands.UploadProfileImage;

public record UploadProfileImageCommand(
    Guid UserId,
    IFormFile Image
) : IRequest<Result<string, Success, Error>>;

