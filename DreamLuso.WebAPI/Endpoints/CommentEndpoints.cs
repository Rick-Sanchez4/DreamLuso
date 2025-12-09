using DreamLuso.Application.CQ.Comments.Commands.CreateComment;
using DreamLuso.Application.CQ.Comments.Commands.DeleteComment;
using DreamLuso.Application.CQ.Comments.Commands.FlagComment;
using DreamLuso.Application.CQ.Comments.Commands.IncrementHelpful;
using DreamLuso.Application.CQ.Comments.Queries.GetPropertyComments;
using DreamLuso.Application.CQ.Comments.Queries.GetPropertyRating;
using DreamLuso.Application.CQ.Comments.Common;
using DreamLuso.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DreamLuso.WebAPI.Endpoints;

public static class CommentEndpoints
{
    public static void RegisterCommentEndpoints(this IEndpointRouteBuilder routes)
    {
        var comments = routes.MapGroup("api/comments").WithTags("Comments");

        comments.MapPost("/", Commands.CreateComment)
            .WithName("CreateComment")
            .RequireAuthorization();

        comments.MapGet("/property/{propertyId:guid}", Commands.GetPropertyComments)
            .WithName("GetPropertyComments");

        comments.MapGet("/property/{propertyId:guid}/rating", Commands.GetPropertyRating)
            .WithName("GetPropertyRating");

        comments.MapPut("/{commentId:guid}/helpful", Commands.IncrementHelpful)
            .WithName("IncrementHelpfulCount");

        comments.MapPut("/{commentId:guid}/flag", Commands.FlagComment)
            .WithName("FlagComment")
            .RequireAuthorization();

        comments.MapDelete("/{commentId:guid}", Commands.DeleteComment)
            .WithName("DeleteComment")
            .RequireAuthorization();
    }

    private static class Commands
    {
        public static async Task<Results<Ok<Guid>, BadRequest<Error>>> CreateComment(
            [FromServices] ISender sender,
            [FromBody] CreateCommentCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<IEnumerable<CommentResponse>>, NotFound<Error>>> GetPropertyComments(
            [FromServices] ISender sender,
            [FromRoute] Guid propertyId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPropertyCommentsQuery(propertyId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }

        public static async Task<Results<Ok<PropertyRatingResponse>, NotFound<Error>>> GetPropertyRating(
            [FromServices] ISender sender,
            [FromRoute] Guid propertyId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPropertyRatingQuery(propertyId);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value!)
                : TypedResults.NotFound(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> IncrementHelpful(
            [FromServices] ISender sender,
            [FromRoute] Guid commentId,
            CancellationToken cancellationToken = default)
        {
            var command = new IncrementHelpfulCommand(commentId);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Contador incrementado" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> FlagComment(
            [FromServices] ISender sender,
            [FromRoute] Guid commentId,
            CancellationToken cancellationToken = default)
        {
            var command = new FlagCommentCommand(commentId);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Comentário sinalizado" } as object)
                : TypedResults.BadRequest(result.Error!);
        }

        public static async Task<Results<Ok<object>, BadRequest<Error>>> DeleteComment(
            [FromServices] ISender sender,
            [FromRoute] Guid commentId,
            CancellationToken cancellationToken = default)
        {
            var command = new DeleteCommentCommand(commentId);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(new { message = "Comentário eliminado" } as object)
                : TypedResults.BadRequest(result.Error!);
        }
    }
}

