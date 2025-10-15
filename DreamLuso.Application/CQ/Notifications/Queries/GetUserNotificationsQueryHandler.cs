using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Queries;

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, Result<IEnumerable<NotificationResponse>, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserNotificationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<IEnumerable<NotificationResponse>, Success, Error>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _unitOfWork.NotificationRepository.GetByRecipientAsync(request.UserId);

        var response = notifications.Select(n => new NotificationResponse(
            n.Id,
            n.Sender?.Name.FirstName + " " + n.Sender?.Name.LastName ?? "Sistema",
            n.Message,
            n.Status.ToString(),
            n.Type.ToString(),
            n.Priority.ToString(),
            n.ReferenceId,
            n.ReferenceType,
            n.CreatedAt
        ));

        return response.ToList();
    }
}

