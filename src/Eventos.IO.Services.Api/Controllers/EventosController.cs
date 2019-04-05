using System;
using System.Collections.Generic;
using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Eventos.Commands;
using Eventos.IO.Domain.Eventos.Repository;
using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.IO.Services.Api.Controllers
{

    public class EventosController : BaseController
    {
        private readonly IEventoAppService _eventoAppService;
        private readonly IBus _bus;
        private readonly IEventoRepository _eventoRepository;
        private readonly IMapper _mapper;

        public EventosController(IDomainNotificationHandler<DomainNotification> notifications,
            IUser _user,
            IBus bus,
            IEventoAppService eventoAppService,
            IEventoRepository eventoRepository,
            IMapper mapper) : base(notifications, _user, bus)
        {
            _eventoAppService = eventoAppService;
            _bus = bus;
            _eventoRepository = eventoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("eventos")]
        public IEnumerable<EventoViewModel> Get()
        {
            return _eventoAppService.ObterTodos();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("eventos/{id:guid}")]
        public EventoViewModel Get(Guid id, int version)
        {
            return _eventoAppService.ObterPorId(id);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("eventos/categorias")]
        public IEnumerable<CategoriaViewModel> ObterCategorias()
        {
            return _mapper.Map<IEnumerable<CategoriaViewModel>>(_eventoRepository.ObterCategorias());
        }

        [HttpPost]
        [Authorize(Policy = "PodeGravar")]
        [Route("eventos")]
        public IActionResult Post([FromBody]RegistrarEventoCommand eventoCommand)
        {
            _bus.SendCommand(eventoCommand);
            return Response(eventoCommand);
        }

        [HttpPut]
        [Authorize(Policy = "PodeGravar")]
        [Route("eventos")]
        public IActionResult Put([FromBody]EventoViewModel eventoViewModel)
        {
            _eventoAppService.Atualizar(eventoViewModel);
            return Response(eventoViewModel);
        }

        [HttpDelete]
        [Authorize(Policy = "PodeGravar")]
        [Route("eventos/{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            _eventoAppService.Excluir(id);
            return Response();
        }
    }
}