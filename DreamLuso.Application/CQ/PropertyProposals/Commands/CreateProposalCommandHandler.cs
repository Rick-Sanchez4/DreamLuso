using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyProposals.Commands;

public class CreateProposalCommandHandler : IRequestHandler<CreateProposalCommand, Result<Guid, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProposalCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid, Success, Error>> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
    {
        // Verificar se a propriedade existe
        var property = await _unitOfWork.PropertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null)
            return Error.NotFound;

        // Verificar se o cliente existe
        var client = await _unitOfWork.ClientRepository.GetByIdAsync(request.ClientId);
        if (client == null)
            return Error.NotFound;

        // Verificar se já existe proposta pendente
        var hasPendingProposal = await _unitOfWork.PropertyProposalRepository.HasPendingProposalAsync(request.ClientId, request.PropertyId);
        if (hasPendingProposal)
            return new Error("ProposalAlreadyExists", "Já existe uma proposta pendente para este imóvel");

        var proposal = new PropertyProposal(
            request.PropertyId,
            request.ClientId,
            request.ProposedValue,
            request.Type,
            request.PaymentMethod,
            request.IntendedMoveDate
        )
        {
            AdditionalNotes = request.AdditionalNotes
        };

        await _unitOfWork.PropertyProposalRepository.SaveAsync(proposal);
        await _unitOfWork.CommitAsync(cancellationToken);

        return proposal.Id;
    }
}

