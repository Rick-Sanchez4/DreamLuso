namespace DreamLuso.Domain.Events;

/// <summary>
/// Evento disparado quando uma visita a uma propriedade é agendada
/// </summary>
public class PropertyVisitScheduledEvent : DomainEvent
{
    public Guid PropertyVisitId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid ClientId { get; private set; }
    public DateTime VisitDate { get; private set; }
    public string TimeSlot { get; private set; }

    public PropertyVisitScheduledEvent(
        Guid propertyVisitId, 
        Guid propertyId, 
        Guid clientId, 
        DateTime visitDate, 
        string timeSlot)
    {
        PropertyVisitId = propertyVisitId;
        PropertyId = propertyId;
        ClientId = clientId;
        VisitDate = visitDate;
        TimeSlot = timeSlot;
    }
}

