﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    public class EventoPacienteComunicacion
    {
        [Key, Column(Order=1)]
        public long EventoID { get; set; }
        [Key, Column(Order = 2)]
        public string PacienteID { get; set; }
        [Key, Column(Order = 3)]
        public long ComunicacionID { get; set; }

        public virtual Evento evento { get; set; }
        public virtual Paciente paciente { get; set; }
        public virtual Comunicacion comunicacion { get; set; }
    }
}
