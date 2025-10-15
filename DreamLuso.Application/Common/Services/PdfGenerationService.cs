using DreamLuso.Domain.Core.Uow;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;

namespace DreamLuso.Application.Common.Services;

public class PdfGenerationService : IPdfGenerationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PdfGenerationService> _logger;

    public PdfGenerationService(IUnitOfWork unitOfWork, ILogger<PdfGenerationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<byte[]> GenerateContractPdfAsync(Guid contractId)
    {
        var contract = await _unitOfWork.ContractRepository.GetByIdAsync(contractId);
        if (contract == null)
            throw new InvalidOperationException($"Contrato com ID {contractId} não encontrado");

        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf);

        // Header
        document.Add(new Paragraph("DreamLuso - Contrato Imobiliário")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(18)
            .SetBold());

        document.Add(new Paragraph($"Nº Contrato: {contract.Id}")
            .SetFontSize(12));

        document.Add(new Paragraph($"Data: {DateTime.UtcNow:dd/MM/yyyy}")
            .SetFontSize(10));

        document.Add(new Paragraph("\n"));

        // Contract Details
        document.Add(new Paragraph("Detalhes do Contrato")
            .SetFontSize(14)
            .SetBold());

        document.Add(new Paragraph($"Tipo: {contract.Type}"));
        document.Add(new Paragraph($"Valor: €{contract.Value:N2}"));
        document.Add(new Paragraph($"Data Início: {contract.StartDate:dd/MM/yyyy}"));
        if (contract.EndDate.HasValue)
            document.Add(new Paragraph($"Data Fim: {contract.EndDate.Value:dd/MM/yyyy}"));
        document.Add(new Paragraph($"Estado: {contract.Status}"));

        if (!string.IsNullOrEmpty(contract.TermsAndConditions))
        {
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Termos e Condições")
                .SetFontSize(14)
                .SetBold());
            document.Add(new Paragraph(contract.TermsAndConditions));
        }

        // Footer
        document.Add(new Paragraph("\n\n"));
        document.Add(new Paragraph("_".PadRight(50, '_')));
        document.Add(new Paragraph("Assinatura do Cliente"));

        document.Add(new Paragraph("\n\n"));
        document.Add(new Paragraph("_".PadRight(50, '_')));
        document.Add(new Paragraph("Assinatura do Agente Imobiliário"));

        document.Close();

        return stream.ToArray();
    }

    public async Task<string> GenerateAndSaveContractPdfAsync(Guid contractId, string savePath)
    {
        var pdfBytes = await GenerateContractPdfAsync(contractId);
        var fileName = $"Contrato_{contractId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
        var fullPath = Path.Combine(savePath, fileName);

        Directory.CreateDirectory(savePath);
        await File.WriteAllBytesAsync(fullPath, pdfBytes);

        _logger.LogInformation("PDF do contrato {ContractId} gerado em {Path}", contractId, fullPath);
        return fullPath;
    }
}

