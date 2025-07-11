namespace SimplePayrollApp.Services
{
    public class ShareService : IShareService
    {
        public async Task ShareFileAsync(string filePath, string title)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(filePath)
            });
        }

        public async Task ShareTextAsync(string text, string title)
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = title
            });
        }

        public async Task<bool> SendEmailAsync(string subject, string body, List<string> recipients, string attachmentPath = null)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients
                };

                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    message.Attachments = new List<EmailAttachment>
                    {
                        new EmailAttachment(attachmentPath)
                    };
                }

                await Email.Default.ComposeAsync(message);
                return true;
            }
            catch (FeatureNotSupportedException)
            {
                // Email not supported on this device
                return false;
            }
            catch
            {
                // Some other error
                return false;
            }
        }

        public async Task OpenFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
    }
}
