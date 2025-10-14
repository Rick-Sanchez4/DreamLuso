namespace DreamLuso.Application.CQ.RealEstateAgents.Common;

public record CreateAgentRequest(
    Guid UserId,
    string? LicenseNumber,
    DateTime? LicenseExpiry,
    string? OfficeEmail,
    string? OfficePhone,
    decimal? CommissionRate,
    string? Specialization,
    List<string>? Certifications,
    List<int>? LanguagesSpoken
);

public record UpdateAgentRequest(
    string? LicenseNumber,
    DateTime? LicenseExpiry,
    string? OfficeEmail,
    string? OfficePhone,
    decimal? CommissionRate,
    string? Specialization,
    string? Bio
);

