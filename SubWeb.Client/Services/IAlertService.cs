using System.Threading.Tasks;

namespace SubWeb.Client.Services
{
    public interface IAlertService
    {
        Task SuccessAsync(string message);
        Task InfoAsync(string message);
        Task WarnAsync(string message);
        Task ErrorAsync(string message);
    }
}