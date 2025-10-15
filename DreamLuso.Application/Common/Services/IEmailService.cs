namespace DreamLuso.Application.Common.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
    Task<bool> SendPropertyVisitConfirmationAsync(string toEmail, string userName, string propertyTitle, DateTime visitDate);
    Task<bool> SendContractNotificationAsync(string toEmail, string userName, string contractNumber);
    Task<bool> SendProposalNotificationAsync(string toEmail, string userName, string propertyTitle, decimal proposedValue);
}

