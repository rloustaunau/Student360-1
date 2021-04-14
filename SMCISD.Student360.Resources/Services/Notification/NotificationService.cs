using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Resources.Providers.Notifications;
using SMCISD.Student360.Resources.Services.StudentAbsencesForEmail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.Notification
{
    public interface INotificationService 
    {
        Task SendNotifications();
    }

    public class NotificationService : INotificationService
    {
        private readonly IEnumerable<INotificationProvider> _providers;
        public NotificationService(IEnumerable<INotificationProvider> providers)
        {
            _providers = providers;
        }

        public async Task SendNotifications()
        {
            foreach (var provider in _providers)
             await provider.SendNotifications();
        }
    }
}
