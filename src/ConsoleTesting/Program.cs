using Eventos.IO.Domain.Models.Eventos;
using System;

namespace ConsoleTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var evento = new Evento(
                "",
                DateTime.Now,
                DateTime.Now,
                true,
                50,
                false,
                "");

            Console.WriteLine(evento.ToString());
            Console.WriteLine(evento.EhValido());

            if (!evento.ValidationResult.IsValid)
            {
                foreach (var erro in evento.ValidationResult.Errors)
                {
                    Console.WriteLine(erro.ErrorMessage);
                }
            }

            Console.ReadKey();
        }
    }
}
