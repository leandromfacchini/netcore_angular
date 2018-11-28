﻿using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.Models.Eventos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.Eventos.Repository
{
    public interface IEventoRepository : IRepository<Evento>
    {
    }
}
