using SimplePayrollApp.ViewModels;

namespace SimplePayrollApp.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task NavigateToAsync<TViewModel>(object parameter = null) where TViewModel : BaseViewModel
        {
            var page = ResolvePage<TViewModel>();

            if (page != null)
            {
                // Set the parameter if the page has a PayrollData property
                if (parameter != null)
                {
                    var propertyInfo = page.GetType().GetProperty("PayrollData");
                    if (propertyInfo != null && propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(page, parameter);
                    }
                }

                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
        }

        public async Task GoBackAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private Page ResolvePage<TViewModel>() where TViewModel : BaseViewModel
        {
            Type viewModelType = typeof(TViewModel);
            string viewModelName = viewModelType.Name;
            string viewName = viewModelName.Replace("ViewModel", "Page");

            // Get the page type
            Type viewType = Type.GetType($"SimplePayrollApp.Views.{viewName}");

            // If can't resolve with full namespace, try just the name
            if (viewType == null)
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.Name == viewName);

                viewType = types.FirstOrDefault();
            }

            if (viewType == null)
            {
                throw new InvalidOperationException($"Cannot locate page type for {viewModelName}");
            }

            return _serviceProvider.GetService(viewType) as Page;
        }
    }
}
