using System;
using System.Linq;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.IO.Services.Api.Controllers
{
    [Produces("application/json")]
    public abstract class BaseController : Controller
    {
        private readonly IDomainNotificationHandler<DomainNotification> _notifications;
        private readonly IBus _bus;
        private IDomainNotificationHandler<DomainNotification> notifications;
        private IUser _user;

        protected Guid OrganizadorId { get; set; }

        protected BaseController(IDomainNotificationHandler<DomainNotification> notifications,
                                 IUser _user,
                                 IBus bus)
        {
            _notifications = notifications;
            _bus = bus;

            if (_user.IsAuthenticated())
            {
                OrganizadorId = _user.GetUserId();
            }

        }

        protected BaseController(IDomainNotificationHandler<DomainNotification> notifications, IUser user)
        {
            this.notifications = notifications;
            _user = user;
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
            return !_notifications.HasNotifications();
        }

        protected void NotificarErroModelInvalida()
        {
            var erros = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var erro in erros)
            {
                NotificarErro(string.Empty, erro.ErrorMessage);
            }
        }

        protected void NotificarErro(string codigo, string mensagem)
        {
            _bus.RaiseEvent(new DomainNotification(codigo, mensagem));
        }

        protected void AdicionarErrosIdentity(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                NotificarErro(result.ToString(), error.Description);
            }
        }

    }
}