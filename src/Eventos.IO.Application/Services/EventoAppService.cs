using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Eventos.Commands;
using System;
using System.Collections.Generic;

namespace Eventos.IO.Application.Services
{
    public class EventoAppService : IEventoAppService
    {
        private readonly IBus _bus;

        public EventoAppService(IBus bus)
        {
            _bus = bus;
        }

        public void Registrar(EventoViewModel eventoViewModel)
        {
            var registroCommand = new RegistrarEventoCommand();
            _bus.SendCommand(registroCommand);
        }

        public IEnumerable<EventoViewModel> ObterEventoPorOrganizador(Guid organizadorId)
        {
            throw new NotImplementedException();
        }

        public EventoViewModel ObterPorId(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EventoViewModel> ObterTodos()
        {
            throw new NotImplementedException();
        }

        public void Atualizar(EventoViewModel eventoViewModel)
        {
            throw new NotImplementedException();
        }

        public void Excluir(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
