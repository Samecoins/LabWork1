using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportContracts;

namespace ReportPlugin.TextWord19;

public sealed class DiscountOrdersTextReport : IReportDocumentWithContextTextsContract
{
    public string DocumentFormat => "docx";

    public Task CreateDocumentAsync(string filePath, string header, List<string> paragraphs)
    {
        using var doc = WordprocessingDocument.Create(
            filePath,
            WordprocessingDocumentType.Document);

        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var body = mainPart.Document.Body!;

        // Заголовок
        body.Append(CreateParagraph(header, bold: true, size: 32));

        // Пустая строка
        body.Append(new Paragraph(new Run(new Text(" "))));

        // Абзацы по заказам
        foreach (var p in paragraphs)
            body.Append(CreateParagraph(p, bold: false, size: 24));

        mainPart.Document.Save();
        return Task.CompletedTask;
    }

    private static Paragraph CreateParagraph(string text, bool bold, int size)
    {
        var runProps = new RunProperties(
            new FontSize { Val = size.ToString() });
        if (bold)
            runProps.Append(new Bold());

        var run = new Run(runProps, new Text(text) { Space = SpaceProcessingModeValues.Preserve });
        return new Paragraph(run);
    }
}
