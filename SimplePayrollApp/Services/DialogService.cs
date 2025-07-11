using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;


namespace SimplePayrollApp.Services
{
    public class DialogService : IDialogService
    {
        private Page CurrentPage
        {
            get
            {
                // Get the current page safely
                if (Application.Current?.MainPage is NavigationPage navigationPage)
                    return navigationPage.CurrentPage;
                return Application.Current?.MainPage;
            }
        }

        private Grid? _loadingGrid;
        private bool _isLoading;

        public async Task ShowAlertAsync(string title, string message, string cancel)
        {
            await CurrentPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
        {
            return await CurrentPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task ShowLoadingAsync(string message)
        {
            if (_isLoading) return;

            _isLoading = true;

            if (_loadingGrid == null)
            {
                _loadingGrid = new Grid
                {
                    BackgroundColor = Colors.Black.WithAlpha(0.7f),
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill
                };

                var stackLayout = new VerticalStackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 15
                };

                var indicator = new ActivityIndicator
                {
                    IsRunning = true,
                    Color = Colors.White,
                    HeightRequest = 50,
                    WidthRequest = 50
                };

                var label = new Label
                {
                    Text = message,
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Center
                };

                stackLayout.Add(indicator);
                stackLayout.Add(label);
                _loadingGrid.Add(stackLayout);
            }

            // Add to current page
            var page = CurrentPage;
            if (page != null)
            {
                // Be sure we can access the Content as ContentPage
                if (page is ContentPage contentPage)
                {
                    // Remember original content
                    var originalContent = contentPage.Content;

                    // Create a container for both the original content and the loading overlay
                    var container = new Grid();

                    // Here's the explicit cast that's needed:
                    if (originalContent is Microsoft.Maui.IView iView)
                    {
                        container.Add((View)iView);
                    }

                    container.Add(_loadingGrid);

                    // Set as the new content
                    contentPage.Content = container;

                    // Animation
                    _loadingGrid.Opacity = 0;
                    await _loadingGrid.FadeTo(1, 200, Easing.SinOut);
                }
            }
        }

        public async Task HideLoadingAsync()
        {
            if (!_isLoading || _loadingGrid == null)
                return;

            await _loadingGrid.FadeTo(0, 200, Easing.SinIn);

            var page = CurrentPage;
            if (page is ContentPage contentPage)
            {
                if (contentPage.Content is Grid container && container.Children.Contains(_loadingGrid))
                {
                    // Get the original content (first child that's not the loading grid)
                    var originalContent = container.Children.FirstOrDefault(c => c != _loadingGrid);
                    if (originalContent != null)
                    {
                        contentPage.Content = (View)originalContent;
                    }
                }
            }

            _isLoading = false;
        }
    }
}
