using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportContracts;

public interface IReportDocumentWithChartPieContract : IReportDocumentContract
{
    /// <summary>
    /// Создать документ с круговой диаграммой.
    /// </summary>
    Task CreateDocumentAsync(
        string filePath,
        string header,
        string chartTitle,
        List<(int Parameter, double Value)> series);
}
