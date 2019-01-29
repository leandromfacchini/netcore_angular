using System.ComponentModel.DataAnnotations;

namespace Eventos.IO.Infra.CrossCutting.Identity.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
