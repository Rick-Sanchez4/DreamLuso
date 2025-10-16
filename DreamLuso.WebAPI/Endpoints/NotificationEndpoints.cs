using DreamLuso.Application.CQ.Notifications.Commands;
using DreamLuso.Application.CQ.Notifications.Queries;
using DreamLuso.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class NotificationEndpoints
{
    public static void RegisterNotificationEndpoints(this IEndpointRouteBuilder routes)
    {
        var notifications = routes.MapGroup("api/notifications").WithTags("Notifications");

        notifications.MapGet("/{userId:guid}", Commands.GetUserNotifications)
            .WithName("GetUserNotifications")
            .RequireAuthorization();

        notifications.MapGet("/unread-count/{userId:guid}", Commands.GetUnreadCount)
            .WithName("GetUnreadNotificationCount")
            .RequireAuthorization();

        notifications.MapPost("/", Commands.SendNotification)
            .WithName("SendNotification")
            .RequireAuthorization();

        notifications.MapPut("/{notificationId:guid}/mark-read", Commands.MarkAsRead)
            .WithName("MarkNotificationAsRead")
            .RequireAuthorization();

        notifications.MapPut("/{userId:guid}/mark-all-read", Commands.MarkAllAsRead)
            .WithName("MarkAllNotificationsAsRead")
            .RequireAuthorization();
    }

    private static class Commands
    {
        [Authorize]
        public static async Task<Results<Ok<IEnumerable<NotificationResponse>>, NotFound<Error>>> GetUserNotifications(
            [FromServices] ISender sender,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUserNotificationsQuery(userId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }

        [Authorize]
        public static async Task<Results<Ok<UnreadNotificationCountResponse>, NotFound<Error>>> GetUnreadCount(
            [FromServices] ISender sender,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUnreadNotificationCountQuery(userId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }

        [Authorize]
        public static async Task<Results<Ok<Guid>, BadRequest<Error>>> SendNotification(
            [FromServices] ISender sender,
            [FromBody] SendNotificationCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error!);
        }

        [Authorize]
        public static async Task<Results<Ok<object>, BadRequest<Error>>> MarkAsRead(
            [FromServices] ISender sender,
            [FromRoute] Guid notificationId,
            CancellationToken cancellationToken = default)
        {
            var command = new MarkNotificationAsReadCommand(notificationId);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Notificação marcada como lida" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        [Authorize]
        public static async Task<Results<Ok<object>, BadRequest<Error>>> MarkAllAsRead(
            [FromServices] ISender sender,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var command = new MarkAllNotificationsAsReadCommand(userId);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Todas as notificações marcadas como lidas" } as object)
                : TypedResults.BadRequest(result.Error!);
        }
    }
}

