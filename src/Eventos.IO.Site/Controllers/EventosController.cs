using System;
using Microsoft.AspNetCore.Mvc;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Eventos.IO.Site.Controllers
{
    [Route("")]
    public class EventosController : BaseController
    {
        private readonly IEventoAppService _eventoAppService;

        public EventosController(IEventoAppService eventoAppService,
                                 IDomainNotificationHandler<DomainNotification> notification,
                                 IUser user) : base(notification, user)
        {
            _eventoAppService = eventoAppService;
        }

        [Route("")]
        [Route("proximos-eventos")]
        public IActionResult Index()
        {
            return View(_eventoAppService.ObterTodos());
        }

        [Authorize(Policy = "PodeLerEventos")]
        [Route("meus-eventos")]
        public IActionResult MeusEventos()
        {
            return View(_eventoAppService.ObterEventoPorOrganizador(organizadorId));
        }

        [Route("dados-do-evento/{id:guid}")]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);
            if (eventoViewModel == null)
            {
                return NotFound();
            }

            return View(eventoViewModel);
        }

        [Route("novo-evento")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("novo-evento")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Create(EventoViewModel eventoViewModel)
        {
            if (!ModelState.IsValid) return View(eventoViewModel);

            eventoViewModel.OrganizadorId = organizadorId;

            _eventoAppService.Registrar(eventoViewModel);

            ViewBag.RetornoPost = OperacaoValida() ? "success, Evento registrado com sucesso" : "error, Evento não registrado! Verifique as mensagens";

            return View(eventoViewModel);
        }

        [Route("editar-evento/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

            if (eventoViewModel == null) return NotFound();

            if (eventoViewModel.OrganizadorId != organizadorId)
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEnderecoPorId(organizadorId));
            }

            if (eventoViewModel == null)
            {
                return NotFound();
            }

            return View(eventoViewModel);
        }

        [ValidateAntiForgeryToken] 
        [Route("editar-evento/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Edit(EventoViewModel eventoViewModel)
        {
            if (ValidarAutoridadeEvento(eventoViewModel))
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(organizadorId));
            }

            if (!ModelState.IsValid) return View(eventoViewModel);

            eventoViewModel.OrganizadorId = organizadorId;
            _eventoAppService.Atualizar(eventoViewModel);

            //TODO: Validar se a operação ocorreu com sucesso

            ViewBag.RetornoPost = OperacaoValida() ? "success, Evento atualizado com sucesso" : "error, Evento não pode ser atualizado! Verifique as mensagens";
            if (_eventoAppService.ObterPorId(eventoViewModel.Id).Online)
            {
                eventoViewModel.Endereco = null;
            }
            else
            {
                eventoViewModel = _eventoAppService.ObterPorId(eventoViewModel.Id);
            }

            return View(eventoViewModel);
        }

        [Route("excluir-evento/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

            if (ValidarAutoridadeEvento(eventoViewModel))
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(organizadorId));
            }

            if (eventoViewModel == null)
            {
                return NotFound();
            }

            return View(eventoViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("excluir-evento/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            if (ValidarAutoridadeEvento(_eventoAppService.ObterPorId(id)))
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(organizadorId));
            }

            _eventoAppService.Excluir(id);
            return RedirectToAction("Index");
        }

        [Route("incluir-endereco/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult IncluirEndereco(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

            if (ValidarAutoridadeEvento(eventoViewModel))
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(organizadorId));
            }

            return PartialView("_IncluirEndereco", eventoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("incluir-endereco/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult IncluirEndereco(EventoViewModel eventoViewModel)
        {
            ModelState.Clear();
            eventoViewModel.Endereco.EventoId = eventoViewModel.Id;
            _eventoAppService.AdicionarEndereco(eventoViewModel.Endereco);

            if (OperacaoValida())
            {
                var url = Url.Action("ObterEndereco", "Eventos", new { id = eventoViewModel.Id });
                return Json(new { success = true, url = url });
            }

            return PartialView("_IncluirEndereco", eventoViewModel);
        }

        [Route("atualizar-endereco/{id:guid}")]
        public IActionResult AtualizarEndereco(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

            if (ValidarAutoridadeEvento(eventoViewModel))
            {
                return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(organizadorId));
            }

            return PartialView("_AtualizarEndereco", eventoViewModel);
        }

        [ValidateAntiForgeryToken]
        [Route("atualizar-endereco/{id:guid}")]
        [Authorize(Policy = "PodeGravar")]
        public IActionResult AtualizarEndereco(EventoViewModel eventoViewModel)
        {
            ModelState.Clear();
            _eventoAppService.AtualizarEndereco(eventoViewModel.Endereco);

            if (OperacaoValida())
            {
                string url = Url.Action("ObterEndereco", "Eventos", new { id = eventoViewModel.Id });
                return Json(new { success = true, url = url });
            }

            return PartialView("_AtualizarEndereco", eventoViewModel);
        }

        [Route("listar-endereco/{id:guid}")]
        public IActionResult ObterEndereco(Guid id)
        {
            return PartialView("_DetalhesEndereco", _eventoAppService.ObterPorId(id));
        }

        private bool ValidarAutoridadeEvento(EventoViewModel eventoViewModel)
        {
            return eventoViewModel.OrganizadorId != organizadorId;

        }
    }
}
