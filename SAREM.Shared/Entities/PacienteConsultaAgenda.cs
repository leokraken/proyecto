﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class PacienteConsultaAgenda
    {
        [Key, Column(Order=0)]
        public string PacienteID { get; set; }
        [Key, Column(Order=1)]
        public long ConsultaID { get; set; }

        public virtual Paciente paciente { get; set; }
        public virtual Consulta consulta { get; set; }

        public string diagnostico { get; set; }
        public DateTime fecharegistro { get; set; }
        public bool fueralista { get; set; }
        public bool ausencia { get; set; }
    }
}
