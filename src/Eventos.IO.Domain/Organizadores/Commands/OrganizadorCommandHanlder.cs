using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;

namespace Eventos.IO.Domain.Organizadores.Commands
{
    public class OrganizadorCommandHanlder : CommandHandler,
        IHandler<RegistrarOrganizadorCommand>
    {
        private readonly IBus _bus;
        public OrganizadorCommandHanlder(
            IUnitOfWork uow,
            IBus bus,
            IDomainNotificationHandler<DomainNotification> notifications) : base(uow, bus, notifications)
        {
            _bus = bus;
        }

        public void Handle(RegistrarOrganizadorCommand message)
        {
            var organizador = new Organizador(message.Id, message.Nome, message.Cpf, message.Email);

            if (organizador.EhValido())
            {
                NotificarValidacoesErro(organizador.ValidationResult);
                return;
            }

            //TODO: validar CPF e email duplicados

            //TODO: add no repositorio

            if (Commit())
            {

            }
        }
    }
}
