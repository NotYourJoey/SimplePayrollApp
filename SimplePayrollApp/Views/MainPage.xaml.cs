using SimplePayrollApp.ViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace SimplePayrollApp.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            // Add entrance animations
            try
            {
                SetInitialState();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetInitialState: {ex.Message}");
            }

            this.Loaded += OnPageLoaded;
        }

        private void SetInitialState()
        {
            // Set initial state for animations
            if (EmployeeSection != null)
            {
                EmployeeSection.Opacity = 0;
                EmployeeSection.Scale = 0.8;
            }

            if (CompensationSection != null)
            {
                CompensationSection.Opacity = 0;
                CompensationSection.Scale = 0.8;
            }
        }

        private async void OnPageLoaded(object sender, EventArgs e)
        {
            try
            {
                // Animate sections when page loads
                await Task.Delay(200);
                if (EmployeeSection != null)
                {
                    await AnimateIn(EmployeeSection);
                }

                await Task.Delay(150);
                if (CompensationSection != null)
                {
                    await AnimateIn(CompensationSection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in animations: {ex.Message}");
            }
        }

        private async Task AnimateIn(View element)
        {
            if (element == null) return;

            await Task.WhenAll(
                element.FadeTo(1, 300),
                element.ScaleTo(1.0, 300, Easing.SpringOut)
            );
        }
    }
}
