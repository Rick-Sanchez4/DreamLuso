namespace DreamLuso.Domain.Events;

/// <summary>
/// Evento disparado quando um contrato é ativado
/// </summary>
public class ContractActivatedEvent : DomainEvent
{
    public Guid ContractId { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid ClientId { get; private set; }
    public decimal Value { get; private set; }
    public DateTime ActivatedAt { get; private set; }

    public ContractActivatedEvent(
        Guid contractId, 
        Guid propertyId, 
        Guid clientId, 
        decimal value, 
        DateTime activatedAt)
    {
        ContractId = contractId;
        PropertyId = propertyId;
        ClientId = clientId;
        Value = value;
        ActivatedAt = activatedAt;
    }
}

