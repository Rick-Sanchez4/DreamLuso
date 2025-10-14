using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.ConfirmVisit;

public class ConfirmVisitCommandHandler : IRequestHandler<ConfirmVisitCommand, Result<ConfirmVisitResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConfirmVisitCommandHandler> _logger;

    public ConfirmVisitCommandHandler(IUnitOfWork unitOfWork, ILogger<ConfirmVisitCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ConfirmVisitResponse, Success, Error>> Handle(ConfirmVisitCommand request, CancellationToken cancellationToken)
    {
        // Get visit by confirmation token
        var visitObj = await _unitOfWork.PropertyVisitRepository.GetByConfirmationTokenAsync(request.ConfirmationToken);
        if (visitObj == null)
        {
            _logger.LogWarning("Visita não encontrada com token: {Token}", request.ConfirmationToken);
            return Error.InvalidConfirmationToken;
        }

        var visit = (PropertyVisit)visitObj;

        // Check if already confirmed
        if (visit.Status == VisitStatus.Confirmed)
        {
            _logger.LogWarning("Visita já confirmada: {VisitId}", visit.Id);
            return Error.VisitAlreadyConfirmed;
        }

        // Check if already cancelled
        if (visit.Status == VisitStatus.Cancelled)
        {
            _logger.LogWarning("Visita já cancelada: {VisitId}", visit.Id);
            return Error.VisitAlreadyCancelled;
        }

        // Confirm visit
        visit.Confirm();

        await _unitOfWork.PropertyVisitRepository.UpdateAsync(visit);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Visita confirmada com sucesso: {VisitId}", visit.Id);

        var response = new ConfirmVisitResponse(
            visit.Id,
            true,
            visit.ConfirmedAt!.Value
        );

        return response;
    }
}

