using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Eventos.IO.Site.Controllers
{
    public class BaseController : Controller
    {
        private readonly IDomainNotificationHandler<DomainNotification> _notifications;
        private readonly IUser _user;

        protected Guid organizadorId { get; set; }

        public BaseController(IDomainNotificationHandler<DomainNotification> notifications,
            IUser user)
        {
            _notifications = notifications;
            _user = user;

            if (_user.IsAuthenticated())
                organizadorId = _user.GetUserId();

        }

        protected bool OperacaoValida()
        {
            return (!_notifications.HasNotifications());
        }
    }
}
