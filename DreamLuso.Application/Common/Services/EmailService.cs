using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace DreamLuso.Application.Common.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        _smtpUser = _configuration["EmailSettings:SmtpUser"] ?? "";
        _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
        _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@dreamluso.pt";
        _fromName = _configuration["EmailSettings:FromName"] ?? "DreamLuso";
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(message);
            _logger.LogInformation("Email enviado com sucesso para {ToEmail}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {ToEmail}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
    {
        var subject = "Bem-vindo ao DreamLuso!";
        var body = $@"
            <html>
            <body>
                <h2>Olá {userName}!</h2>
                <p>Bem-vindo ao <strong>DreamLuso</strong>, a sua plataforma de imóveis de confiança.</p>
                <p>Estamos aqui para ajudá-lo a encontrar o imóvel dos seus sonhos!</p>
                <br/>
                <p>Atenciosamente,<br/>Equipa DreamLuso</p>
            </body>
            </html>";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendPropertyVisitConfirmationAsync(string toEmail, string userName, string propertyTitle, DateTime visitDate)
    {
        var subject = "Visita ao Imóvel Agendada - DreamLuso";
        var body = $@"
            <html>
            <body>
                <h2>Olá {userName}!</h2>
                <p>A sua visita ao imóvel <strong>{propertyTitle}</strong> foi confirmada!</p>
                <p><strong>Data da Visita:</strong> {visitDate:dd/MM/yyyy HH:mm}</p>
                <p>Por favor, chegue 5 minutos antes da hora marcada.</p>
                <br/>
                <p>Atenciosamente,<br/>Equipa DreamLuso</p>
            </body>
            </html>";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendContractNotificationAsync(string toEmail, string userName, string contractNumber)
    {
        var subject = "Novo Contrato Disponível - DreamLuso";
        var body = $@"
            <html>
            <body>
                <h2>Olá {userName}!</h2>
                <p>Foi criado um novo contrato com o número <strong>{contractNumber}</strong>.</p>
                <p>Por favor, aceda à plataforma para rever e assinar o contrato.</p>
                <br/>
                <p>Atenciosamente,<br/>Equipa DreamLuso</p>
            </body>
            </html>";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendProposalNotificationAsync(string toEmail, string userName, string propertyTitle, decimal proposedValue)
    {
        var subject = "Nova Proposta Recebida - DreamLuso";
        var body = $@"
            <html>
            <body>
                <h2>Olá {userName}!</h2>
                <p>Recebeu uma nova proposta para o imóvel <strong>{propertyTitle}</strong>.</p>
                <p><strong>Valor Proposto:</strong> €{proposedValue:N2}</p>
                <p>Por favor, aceda à plataforma para rever a proposta.</p>
                <br/>
                <p>Atenciosamente,<br/>Equipa DreamLuso</p>
            </body>
            </html>";

        return await SendEmailAsync(toEmail, subject, body);
    }
}

