using SimplePayrollApp.Models;

namespace SimplePayrollApp.Services
{
    public interface IPdfService
    {
        Task<string> GeneratePayrollPdfAsync(PayrollData payrollData);
    }
}
