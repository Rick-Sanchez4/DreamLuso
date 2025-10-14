using MediatR;
using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyVisits.Commands.CancelVisit;

public class CancelVisitCommandHandler : IRequestHandler<CancelVisitCommand, Result<CancelVisitResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelVisitCommandHandler> _logger;

    public CancelVisitCommandHandler(IUnitOfWork unitOfWork, ILogger<CancelVisitCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CancelVisitResponse, Success, Error>> Handle(CancelVisitCommand request, CancellationToken cancellationToken)
    {
        // Get visit
        var visitObj = await _unitOfWork.PropertyVisitRepository.GetByIdAsync(request.VisitId);
        if (visitObj == null)
        {
            _logger.LogWarning("Visita não encontrada: {VisitId}", request.VisitId);
            return Error.VisitNotFound;
        }

        var visit = (PropertyVisit)visitObj;

        // Check if already cancelled
        if (visit.Status == VisitStatus.Cancelled)
        {
            _logger.LogWarning("Visita já cancelada: {VisitId}", visit.Id);
            return Error.VisitAlreadyCancelled;
        }

        // Cancel visit
        visit.Cancel(request.CancellationReason);

        await _unitOfWork.PropertyVisitRepository.UpdateAsync(visit);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Visita cancelada: {VisitId}, Razão: {Reason}", visit.Id, request.CancellationReason);

        var response = new CancelVisitResponse(
            visit.Id,
            true,
            visit.CancelledAt!.Value
        );

        return response;
    }
}

