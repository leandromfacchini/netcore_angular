using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.IO.Site.Controllers
{
    public class ErrosController : Controller
    {
        private readonly IUser _user;

        public ErrosController(IUser user)
        {
            _user = user;
        }

        [Route("/erro-de-aplicacao")]
        [Route("/erro-de-aplicacao/{id}")]
        public IActionResult Erros(string id)
        {
            switch (id)
            {
                case "404":
                    {
                        return View("NotFound");

                    }
                case "401":
                case "403":
                    {
                        if (!_user.IsAuthenticated()) return RedirectToAction("Login", "Account");
                        return View("NotFound");
                    }
            }

            return View("Error");
        }
    }
}
