using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.Accounts.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserDto>, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllUsersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<UserDto>, Success, Error>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync();
        
        var userDtos = users.Cast<User>().Select(u => new UserDto(
            u.Id,
            u.Email,
            u.Name.FirstName,
            u.Name.LastName,
            u.Name.FullName,
            u.Phone,
            u.Role.ToString(),
            u.IsActive,
            u.CreatedAt,
            u.LastLogin,
            u.Client?.Id,
            u.RealEstateAgent?.Id
        )).ToList();

        _logger.LogInformation("Retrieved {Count} users", userDtos.Count);

        return userDtos;
    }
}

