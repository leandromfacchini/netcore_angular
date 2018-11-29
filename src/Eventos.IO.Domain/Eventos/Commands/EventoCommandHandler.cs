using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Eventos.Events;
using Eventos.IO.Domain.Eventos.Repository;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.Models.Eventos;
using System;

namespace Eventos.IO.Domain.Eventos.Commands
{
    public class EventoCommandHandler :
        CommandHandler,
        IHandler<RegistrarEventoCommand>,
        IHandler<AtualizarEventoCommand>,
        IHandler<ExcluirEventoCommand>
    {
        private readonly IEventoRepository _eventoRepository;
        private readonly IBus _bus;

        public EventoCommandHandler(IEventoRepository eventoRepository,
            IUnitOfWork unitOfWork,
            IBus bus,
            IDomainNotificationHandler<DomainNotification> notifications) : base(unitOfWork, bus, notifications)
        {
            _eventoRepository = eventoRepository;
            _bus = bus;
        }

        public void Handle(RegistrarEventoCommand message)
        {
            var evento = new Evento(
                message.Nome,
                message.DataInicio,
                message.DataFim,
                message.Gratuito,
                message.Valor,
                message.Online,
                message.NomeEmpresa);

            if (!evento.EhValido())
            {
                NotificarValidacoesErro(evento.ValidationResult);
                return;
            }

            //TODO: Validações de negócio

            //Persistencia
            _eventoRepository.Add(evento);

            if (Commit())
            {
                //Notificar processo concluído.
                Console.WriteLine("Evento registrado com sucesso");
                _bus.RaiseEvent(new EventoRegistradoEvent(evento.Id,
                    evento.Nome, evento.DataInicio, evento.DataFim, evento.Gratuito,
                    evento.Valor, evento.Online, evento.NomeEmpresa));
            }

        }

        public void Handle(AtualizarEventoCommand message)
        {

        }

        public void Handle(ExcluirEventoCommand message)
        {

        }
    }
}
