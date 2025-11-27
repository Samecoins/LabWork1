using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using ReportContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPlugin.PieExcel19;

public sealed class PaidOrdersPieChartReport : IReportDocumentWithChartPieContract
{
    public string DocumentFormat => "xlsx";

    public async Task CreateDocumentAsync(
        string filePath,
        string header,
        string chartTitle,
        List<(int Parameter, double Value)> series)
    {

        ExcelPackage.License.SetNonCommercialPersonal("Student");

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Report");

        ws.Cells[1, 1].Value = header;

        // данные для диаграммы
        const int startRow = 4;
        ws.Cells[startRow - 1, 1].Value = "Parameter";
        ws.Cells[startRow - 1, 2].Value = "Value";

        for (int i = 0; i < series.Count; i++)
        {
            var s = series[i];
            ws.Cells[startRow + i, 1].Value = s.Parameter;
            ws.Cells[startRow + i, 2].Value = s.Value;
        }

        var chart = ws.Drawings.AddChart("pie", eChartType.PieExploded3D) as ExcelPieChart;
        chart.Title.Text = chartTitle;

        var valueRange = ws.Cells[startRow, 2, startRow + series.Count - 1, 2];
        var labelRange = ws.Cells[startRow, 1, startRow + series.Count - 1, 1];
        chart.Series.Add(valueRange, labelRange);

        chart.SetPosition(1, 0, 3, 0);
        chart.SetSize(600, 400);

        var fi = new FileInfo(filePath);
        await package.SaveAsAsync(fi);
    }
}
