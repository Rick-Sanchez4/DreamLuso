using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public class AddNegotiationCommandHandler : IRequestHandler<AddNegotiationCommand, Result<Guid, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddNegotiationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid, Success, Error>> Handle(AddNegotiationCommand request, CancellationToken cancellationToken)
    {
        var proposal = await _unitOfWork.PropertyProposalRepository.GetByIdAsync(request.ProposalId);
        if (proposal == null)
            return Error.NotFound;

        var negotiation = proposal.AddNegotiation(request.SenderId, request.Message, request.CounterOffer);
        
        await _unitOfWork.CommitAsync(cancellationToken);

        return negotiation.Id;
    }
}

