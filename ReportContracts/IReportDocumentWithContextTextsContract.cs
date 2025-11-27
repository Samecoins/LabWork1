using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportContracts;

public interface IReportDocumentWithContextTextsContract : IReportDocumentContract
{
    /// <summary>
    /// Создать документ с несколькими абзацами текста.
    /// </summary>
    Task CreateDocumentAsync(
        string filePath,
        string header,
        List<string> paragraphs);
}
