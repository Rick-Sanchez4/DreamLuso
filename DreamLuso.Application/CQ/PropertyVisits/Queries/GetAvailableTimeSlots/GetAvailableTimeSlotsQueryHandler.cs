using DreamLuso.Application.Common.Responses;
using DreamLuso.Domain.Core.Uow;
using DreamLuso.Domain.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.CQ.PropertyVisits.Queries.GetAvailableTimeSlots;

public class GetAvailableTimeSlotsQueryHandler : IRequestHandler<GetAvailableTimeSlotsQuery, Result<GetAvailableTimeSlotsResponse, Success, Error>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAvailableTimeSlotsQueryHandler> _logger;

    public GetAvailableTimeSlotsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAvailableTimeSlotsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<GetAvailableTimeSlotsResponse, Success, Error>> Handle(GetAvailableTimeSlotsQuery request, CancellationToken cancellationToken)
    {
        // Get all time slots
        var allTimeSlots = Enum.GetValues<TimeSlot>().ToList();

        // Check availability for each slot
        var availableSlots = new List<string>();
        
        foreach (var slot in allTimeSlots)
        {
            var isAvailable = await _unitOfWork.PropertyVisitRepository.IsTimeSlotAvailableAsync(
                request.PropertyId,
                request.VisitDate,
                slot);

            if (isAvailable)
            {
                availableSlots.Add(slot.ToString());
            }
        }

        _logger.LogInformation("Horários disponíveis para {PropertyId} em {Date}: {Count}/{Total}", 
            request.PropertyId, request.VisitDate, availableSlots.Count, allTimeSlots.Count);

        var response = new GetAvailableTimeSlotsResponse
        {
            AvailableSlots = availableSlots
        };

        return response;
    }
}

