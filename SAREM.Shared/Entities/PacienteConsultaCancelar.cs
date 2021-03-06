﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class PacienteConsultaCancelar
    {
        [Key, Column(Order=0)]
        public string PacienteID { get; set; }
        [Key, Column(Order=1)]
        public long ConsultaID { get; set; }
        public DateTime fecha { get; set; }

        public virtual Paciente paciente { get; set; }
        public virtual Consulta consulta { get; set; }
    }
}
