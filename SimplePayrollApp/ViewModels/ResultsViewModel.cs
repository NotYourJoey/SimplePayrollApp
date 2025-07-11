using System.Windows.Input;
using Microsoft.Maui.Controls;
using SimplePayrollApp.Models;
using SimplePayrollApp.Services;

namespace SimplePayrollApp.ViewModels
{
    public class ResultsViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IPdfService _pdfService;
        private readonly IShareService _shareService;

        private PayrollData _payrollData;
        public PayrollData PayrollData
        {
            get => _payrollData;
            set => SetProperty(ref _payrollData, value);
        }

        // Formatted properties for display
        public string FormattedBasicSalary => FormatCurrency(PayrollData?.BasicSalary ?? 0);
        public string FormattedAllowances => FormatCurrency(PayrollData?.Allowances ?? 0);
        public string FormattedBonus => FormatCurrency(PayrollData?.Bonus ?? 0);
        public string FormattedOvertime => FormatCurrency(PayrollData?.Overtime ?? 0);
        public string FormattedGrossSalary => FormatCurrency(PayrollData?.GrossSalary ?? 0);
        public string FormattedSSF => FormatCurrency(PayrollData?.SSF ?? 0);
        public string FormattedPAYE => FormatCurrency(PayrollData?.PAYE ?? 0);
        public string FormattedNetSalary => FormatCurrency(PayrollData?.NetSalary ?? 0);
        public string FormattedPayPeriod => PayrollData?.PayPeriod.ToString("MMMM yyyy") ?? DateTime.Now.ToString("MMMM yyyy");

        // Commands
        public ICommand ExportPdfCommand { get; }
        public ICommand SendEmailCommand { get; }
        public ICommand ShareCommand { get; }

        public ResultsViewModel(IDialogService dialogService, IPdfService pdfService, IShareService shareService)
        {
            Title = "Payroll Results";
            _dialogService = dialogService;
            _pdfService = pdfService;
            _shareService = shareService;

            ExportPdfCommand = new Command(async () => await ExportPdf());
            SendEmailCommand = new Command(async () => await SendEmail());
            ShareCommand = new Command(async () => await Share());
        }

        public void Initialize(PayrollData payrollData)
        {
            PayrollData = payrollData;
            OnPropertyChanged(nameof(FormattedBasicSalary));
            OnPropertyChanged(nameof(FormattedAllowances));
            OnPropertyChanged(nameof(FormattedBonus));
            OnPropertyChanged(nameof(FormattedOvertime));
            OnPropertyChanged(nameof(FormattedGrossSalary));
            OnPropertyChanged(nameof(FormattedSSF));
            OnPropertyChanged(nameof(FormattedPAYE));
            OnPropertyChanged(nameof(FormattedNetSalary));
            OnPropertyChanged(nameof(FormattedPayPeriod));
        }

        private async Task ExportPdf()
        {
            if (IsBusy || PayrollData == null)
                return;

            IsBusy = true;

            try
            {
                await _dialogService.ShowLoadingAsync("Generating PDF...");

                string pdfPath = await _pdfService.GeneratePayrollPdfAsync(PayrollData);

                await _dialogService.HideLoadingAsync();

                if (File.Exists(pdfPath))
                {
                    bool shareFile = await _dialogService.ShowConfirmationAsync(
                        "PDF Created",
                        "Payroll PDF has been created successfully. Would you like to share it?",
                        "Share", "View");

                    if (shareFile)
                    {
                        await _shareService.ShareFileAsync(pdfPath, "Payroll PDF");
                    }
                    else
                    {
                        await _shareService.OpenFileAsync(pdfPath);
                    }
                }
                else
                {
                    await _dialogService.ShowAlertAsync("Error", "Failed to create PDF file", "OK");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.HideLoadingAsync();
                await _dialogService.ShowAlertAsync("Error", $"Failed to export PDF: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SendEmail()
        {
            if (IsBusy || PayrollData == null)
                return;

            IsBusy = true;

            try
            {
                await _dialogService.ShowLoadingAsync("Preparing email...");

                string pdfPath = await _pdfService.GeneratePayrollPdfAsync(PayrollData);

                await _dialogService.HideLoadingAsync();

                // Prepare email content
                string subject = $"Payroll for {PayrollData.EmployeeName} - {PayrollData.PayPeriod:MMMM yyyy}";
                string body = $"Please find attached the payroll details for {PayrollData.EmployeeName} (ID: {PayrollData.EmployeeID}).\n\n" +
                              $"Summary:\n" +
                              $"Gross Salary: {FormattedGrossSalary}\n" +
                              $"SSF: {FormattedSSF}\n" +
                              $"Tax: {FormattedPAYE}\n" +
                              $"Net Salary: {FormattedNetSalary}";

                bool success = await _shareService.SendEmailAsync(subject, body, new List<string>(), pdfPath);

                if (!success)
                {
                    await _dialogService.ShowAlertAsync("Not Supported", "Email is not supported on this device", "OK");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.HideLoadingAsync();
                await _dialogService.ShowAlertAsync("Error", $"Failed to send email: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Share()
        {
            if (IsBusy || PayrollData == null)
                return;

            IsBusy = true;

            try
            {
                await _dialogService.ShowLoadingAsync("Preparing to share...");

                string pdfPath = await _pdfService.GeneratePayrollPdfAsync(PayrollData);

                await _dialogService.HideLoadingAsync();

                await _shareService.ShareFileAsync(pdfPath, "Share Payroll PDF");
            }
            catch (Exception ex)
            {
                await _dialogService.HideLoadingAsync();
                await _dialogService.ShowAlertAsync("Error", $"Failed to share: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string FormatCurrency(double amount)
        {
            return amount.ToString("C");
        }
    }
}
