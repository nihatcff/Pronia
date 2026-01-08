namespace Pronia.Abstraction
{
    public interface IEmailService
    {
        Task SendEMailAsync(string email, string subject, string body);  
    }
}
