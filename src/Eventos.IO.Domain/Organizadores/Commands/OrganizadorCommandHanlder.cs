using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.Organizadores.Repository;
using System.Linq;

namespace Eventos.IO.Domain.Organizadores.Commands
{
    public class OrganizadorCommandHanlder : CommandHandler,
        IHandler<RegistrarOrganizadorCommand>
    {
        private readonly IBus _bus;
        private readonly IOrganizadorRepository _organizadorRepository;

        public OrganizadorCommandHanlder(
            IUnitOfWork uow,
            IBus bus,
            IDomainNotificationHandler<DomainNotification> notifications,
            IOrganizadorRepository organizadorRepository) : base(uow, bus, notifications)
        {
            _bus = bus;
            _organizadorRepository = organizadorRepository;
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
            var organizadorExistente = _organizadorRepository.Buscar(o => o.CPF == organizador.CPF ||
            o.Email == organizador.Email);

            if (organizadorExistente.Any())
            {
                _bus.RaiseEvent(new DomainNotification(message.MessageType, "CPF ou E-mail já utilizados"));
            }

            _organizadorRepository.Adicionar(organizador);

            //TODO: add no repositorio

            if (Commit())
            {
                _bus.RaiseEvent();
            }
        }
    }
}
