namespace TaskManagementApp.Services
{
    public interface IReportService
    {
        string GenerateReport();
    }

    public class ReportService : IReportService
    {
        public string GenerateReport()
        {
            return "Raport z realizacji zadań – wersja demonstracyjna.";
        }
    }
}
