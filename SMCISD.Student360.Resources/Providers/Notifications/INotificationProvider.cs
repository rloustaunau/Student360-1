using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Providers.Notifications
{
    public interface INotificationProvider
    {
        Task<int> SendNotifications();
    }
}
