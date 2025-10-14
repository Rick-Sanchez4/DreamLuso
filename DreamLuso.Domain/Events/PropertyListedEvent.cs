namespace DreamLuso.Domain.Events;

/// <summary>
/// Evento disparado quando uma propriedade é listada no sistema
/// </summary>
public class PropertyListedEvent : DomainEvent
{
    public Guid PropertyId { get; private set; }
    public string Title { get; private set; }
    public decimal Price { get; private set; }
    public Guid RealEstateAgentId { get; private set; }

    public PropertyListedEvent(Guid propertyId, string title, decimal price, Guid realEstateAgentId)
    {
        PropertyId = propertyId;
        Title = title;
        Price = price;
        RealEstateAgentId = realEstateAgentId;
    }
}

