using DreamLuso.Application.Common.Responses;
using MediatR;

namespace DreamLuso.Application.CQ.PropertyVisits.Queries.GetAvailableTimeSlots;

public class GetAvailableTimeSlotsQuery : IRequest<Result<GetAvailableTimeSlotsResponse, Success, Error>>
{
    public Guid PropertyId { get; set; }
    public DateOnly VisitDate { get; set; }
}

public class GetAvailableTimeSlotsResponse
{
    public List<string> AvailableSlots { get; set; } = [];
}

