using SimplePayrollApp.Models;
using SimplePayrollApp.ViewModels;

namespace SimplePayrollApp.Views
{
    [QueryProperty(nameof(PayrollData), "PayrollData")]
    public partial class ResultsPage : ContentPage
    {
        private readonly ResultsViewModel _viewModel;

        public ResultsPage(ResultsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            // Set initial state for animations
            SummaryCard.Scale = 0.9;
            SummaryCard.Opacity = 0;

            DeductionsCard.Scale = 0.9;
            DeductionsCard.Opacity = 0;

            NetSalaryCard.Scale = 0.9;
            NetSalaryCard.Opacity = 0;

            InfoCard.Scale = 0.9;
            InfoCard.Opacity = 0;

            this.Appearing += OnPageAppearing;
        }

        // Property to receive the navigation parameter
        public PayrollData PayrollData
        {
            set
            {
                if (value != null)
                {
                    _viewModel.Initialize(value);
                }
            }
        }

        private async void OnPageAppearing(object sender, EventArgs e)
        {
            // Animate the cards in sequence
            await AnimateCard(SummaryCard);
            await Task.Delay(100);
            await AnimateCard(DeductionsCard);
            await Task.Delay(100);
            await AnimateCard(NetSalaryCard);
            await Task.Delay(100);
            await AnimateCard(InfoCard);
        }

        private async Task AnimateCard(View element)
        {
            await Task.WhenAll(
                element.ScaleTo(1.0, 300, Easing.SpringOut),
                element.FadeTo(1, 300)
            );
        }
    }
}
