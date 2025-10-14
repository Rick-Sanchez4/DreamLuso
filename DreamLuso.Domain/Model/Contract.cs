using DreamLuso.Domain.Common;
using DreamLuso.Domain.Interfaces;

namespace DreamLuso.Domain.Model;

public class Contract : AuditableEntity, IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public required Property Property { get; set; }
    public Guid ClientId { get; set; }
    public required Client Client { get; set; }
    public Guid RealEstateAgentId { get; set; }
    public required RealEstateAgent RealEstateAgent { get; set; }
    
    // Tipo e Status
    public ContractType Type { get; set; }
    public ContractStatus Status { get; set; }
    
    // Datas
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime SignatureDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    
    // Valores
    public decimal Value { get; set; }
    public decimal? MonthlyRent { get; set; }        // Para contratos de arrendamento
    public decimal? SecurityDeposit { get; set; }
    public decimal? Commission { get; set; }         // Comissão da agência
    public decimal? AdditionalFees { get; set; }
    
    // Termos e condições
    public string? TermsAndConditions { get; set; }
    public string? PaymentTerms { get; set; }
    public PaymentFrequency? PaymentFrequency { get; set; }
    public int? PaymentDay { get; set; }             // Dia do mês para pagamento
    public bool AutoRenewal { get; set; } = false;
    public string? TerminationClauses { get; set; }
    
    // Documentação
    public string? DocumentPath { get; set; }
    public string? InsuranceDetails { get; set; }
    public string? Notes { get; set; }
    
    // Garantias e seguros
    public bool RequiresGuarantor { get; set; }
    public string? GuarantorInfo { get; set; }
    public bool HasInsurance { get; set; }
    public DateTime? InsuranceExpiry { get; set; }

    public Contract()
    {
        Id = Guid.NewGuid();
    }

    public Contract(Guid propertyId, Guid clientId, Guid realEstateAgentId, 
                   ContractType type, decimal value)
    {
        Id = Guid.NewGuid();
        PropertyId = propertyId;
        ClientId = clientId;
        RealEstateAgentId = realEstateAgentId;
        Type = type;
        Value = value;
        Status = ContractStatus.Draft;
        StartDate = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = ContractStatus.Active;
        UpdateTimestamp();
    }

    public void Terminate(string? reason = null)
    {
        Status = ContractStatus.Terminated;
        TerminationDate = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(reason))
        {
            Notes = string.IsNullOrEmpty(Notes) 
                ? $"Termination: {reason}" 
                : $"{Notes}\nTermination: {reason}";
        }
        UpdateTimestamp();
    }

    public void Complete()
    {
        Status = ContractStatus.Completed;
        UpdateTimestamp();
    }

    public bool IsExpired()
    {
        return EndDate.HasValue && EndDate.Value < DateTime.UtcNow;
    }

    public bool IsActive()
    {
        return Status == ContractStatus.Active && !IsExpired();
    }
}

public enum ContractType
{
    Sale,           // Venda
    Rent,           // Arrendamento
    Lease           // Leasing
}

public enum ContractStatus
{
    Draft,          // Rascunho
    PendingSignature, // Pendente de assinatura
    Active,         // Ativo
    Suspended,      // Suspenso
    Completed,      // Completo
    Terminated,     // Terminado
    Expired,        // Expirado
    Cancelled       // Cancelado
}

public enum PaymentFrequency
{
    Monthly,        // Mensal
    Quarterly,      // Trimestral
    Biannual,       // Semestral
    Annual,         // Anual
    OneTime         // Pagamento único
}

