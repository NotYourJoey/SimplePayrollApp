namespace SimplePayrollApp.Services
{
    public interface IShareService
    {
        Task ShareFileAsync(string filePath, string title);
        Task ShareTextAsync(string text, string title);
        Task<bool> SendEmailAsync(string subject, string body, List<string> recipients, string attachmentPath = null);
        Task OpenFileAsync(string filePath);
    }
}
