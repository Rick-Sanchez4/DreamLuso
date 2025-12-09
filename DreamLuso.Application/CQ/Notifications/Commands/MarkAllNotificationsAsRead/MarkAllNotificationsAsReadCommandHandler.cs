using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllNotificationsAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<bool, Success, Error>> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.NotificationRepository.MarkAllAsReadAsync(request.UserId);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

