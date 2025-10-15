using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public class RejectProposalCommandHandler : IRequestHandler<RejectProposalCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RejectProposalCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<bool, Success, Error>> Handle(RejectProposalCommand request, CancellationToken cancellationToken)
    {
        var proposal = await _unitOfWork.PropertyProposalRepository.GetByIdAsync(request.ProposalId);
        if (proposal == null)
            return Error.NotFound;

        proposal.Reject(request.RejectionReason);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

