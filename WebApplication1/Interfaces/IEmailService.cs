namespace WebApplication1.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(HashSet<string> emails);
    }
}