using SimplePayrollApp.ViewModels;

namespace SimplePayrollApp.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync<TViewModel>(object parameter = null) where TViewModel : BaseViewModel;
        Task GoBackAsync();
    }
}
