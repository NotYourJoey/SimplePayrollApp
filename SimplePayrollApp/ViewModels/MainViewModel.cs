using System.Windows.Input;
using Microsoft.Maui.Controls;
using SimplePayrollApp.Models;
using SimplePayrollApp.Services;
using SimplePayrollApp.Views;

namespace SimplePayrollApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        // Properties for binding
        private string _employeeName;
        public string EmployeeName
        {
            get => _employeeName;
            set => SetProperty(ref _employeeName, value);
        }

        private string _employeeID;
        public string EmployeeID
        {
            get => _employeeID;
            set => SetProperty(ref _employeeID, value);
        }

        private string _basicSalary;
        public string BasicSalary
        {
            get => _basicSalary;
            set => SetProperty(ref _basicSalary, value);
        }

        private string _allowances;
        public string Allowances
        {
            get => _allowances;
            set => SetProperty(ref _allowances, value);
        }

        private string _bonus;
        public string Bonus
        {
            get => _bonus;
            set => SetProperty(ref _bonus, value);
        }

        private string _overtime;
        public string Overtime
        {
            get => _overtime;
            set => SetProperty(ref _overtime, value);
        }

        // Commands
        public ICommand CalculateCommand { get; }
        public ICommand ClearCommand { get; }

        public MainViewModel(IDialogService dialogService, INavigationService navigationService)
        {
            Title = "Payroll Calculator";
            _dialogService = dialogService;
            _navigationService = navigationService;

            CalculateCommand = new Command(async () => await CalculatePayroll());
            ClearCommand = new Command(ClearFields);

            // Initialize with empty values
            ClearFields();
        }

        private async Task CalculatePayroll()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                // Parse input values
                if (!TryParseDouble(BasicSalary, out double basicSalary) || basicSalary <= 0)
                {
                    await _dialogService.ShowAlertAsync("Invalid Input", "Please enter a valid basic salary.", "OK");
                    return;
                }

                // Parse other values (defaulting to 0 if invalid)
                double allowances = ParseDoubleOrDefault(Allowances);
                double bonus = ParseDoubleOrDefault(Bonus);
                double overtime = ParseDoubleOrDefault(Overtime);

                // Calculate payroll
                var payrollData = TaxCalculator.CalculatePayroll(
                    EmployeeName ?? "Employee",
                    EmployeeID ?? "---",
                    basicSalary,
                    allowances,
                    bonus,
                    overtime
                );

                // Navigate to results page
                await _navigationService.NavigateToAsync<ResultsViewModel>(payrollData);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ClearFields()
        {
            EmployeeName = string.Empty;
            EmployeeID = string.Empty;
            BasicSalary = string.Empty;
            Allowances = string.Empty;
            Bonus = string.Empty;
            Overtime = string.Empty;
        }

        private bool TryParseDouble(string value, out double result)
        {
            return double.TryParse(value, out result);
        }

        private double ParseDoubleOrDefault(string value)
        {
            return double.TryParse(value, out double result) ? result : 0;
        }
    }
}
