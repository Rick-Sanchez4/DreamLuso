using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public class ApproveProposalCommandHandler : IRequestHandler<ApproveProposalCommand, Result<bool, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveProposalCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<bool, Success, Error>> Handle(ApproveProposalCommand request, CancellationToken cancellationToken)
    {
        var proposal = await _unitOfWork.PropertyProposalRepository.GetByIdAsync(request.ProposalId);
        if (proposal == null)
            return Error.NotFound;

        proposal.Approve();
        await _unitOfWork.CommitAsync(cancellationToken);

        return Success.Ok;
    }
}

