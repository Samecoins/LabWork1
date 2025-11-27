namespace ReportContracts;

public interface IReportDocumentContract
{
    /// <summary>
    /// Формат документа: "docx", "xlsx", "pdf" 
    /// </summary>
    string DocumentFormat { get; }
}
