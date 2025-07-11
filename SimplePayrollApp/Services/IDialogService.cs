namespace SimplePayrollApp.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string cancel);
        Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
        Task ShowLoadingAsync(string message);
        Task HideLoadingAsync();
    }
}
