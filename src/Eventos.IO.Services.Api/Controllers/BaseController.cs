using System;
using System.Linq;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.IO.Services.Api.Controllers
{
    [Produces("application/json")]
    public abstract class BaseController : Controller
    {
        private readonly IDomainNotificationHandler<DomainNotification> _notifications;

        protected Guid OrganizadorId { get; set; }

        protected BaseController(IDomainNotificationHandler<DomainNotification> notifications,
                                 IUser _user)
        {
            _notifications = notifications;

            if (_user.IsAuthenticated())
            {
                OrganizadorId = _user.GetUserId();
            }
        }

        protected new IActionResult Response(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notifications.GetNotifications().Select(c => c.Value)
            });
        }

        protected bool OperacaoValida()
        {
            return (!_notifications.HasNotifications());
        }

    }
}