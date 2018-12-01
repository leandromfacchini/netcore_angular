using Eventos.IO.Domain.Core.Models;
using System;
using System.Collections.Generic;

namespace Eventos.IO.Domain.Eventos
{
    public class Categoria : Entity<Categoria>
    {
        public Categoria(Guid id)
        {
            Id = id;
        }
        public string Nome { get; private set; }
        public virtual ICollection<Evento> Eventos { get; set; }

        //Construtor para o EF
        public Categoria() { }

        public override bool EhValido()
        {
            return true;
        }
    }
}