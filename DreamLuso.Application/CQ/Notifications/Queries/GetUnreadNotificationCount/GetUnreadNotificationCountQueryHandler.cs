using DreamLuso.Application.Common.Responses;
using DreamLuso.Application.CQ.Notifications.Common;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Queries.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, Result<UnreadNotificationCountResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUnreadNotificationCountQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<UnreadNotificationCountResponse, Success, Error>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _unitOfWork.NotificationRepository.GetUnreadCountAsync(request.UserId);
        return new UnreadNotificationCountResponse(count);
    }
}

