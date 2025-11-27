using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportContracts;

public interface IReportDocumentWithTableColumnRowHeaderContract : IReportDocumentContract
{
    /// <summary>
    /// Создать документ с таблицей: сложная шапка (первая строка + первый столбец).
    /// </summary>
    Task CreateDocumentAsync<T>(
        string filePath,
        string header,
        List<int> columnWidths,
        List<int> rowHeights,
        bool isHeaderFirstRow,
        List<(string Header, string PropertyName, string FieldName)> headers,
        List<T> data);
}
