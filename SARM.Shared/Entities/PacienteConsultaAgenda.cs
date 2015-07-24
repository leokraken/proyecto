using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared
{
    public class PacienteConsultaAgenda
    {
        public string PacienteID { get; set; }
        public long ConsultaID { get; set; }

        public virtual Paciente paciente { get; set; }
        public virtual Consulta consulta { get; set; }

        public DateTime fecharegistro { get; set; }
    }
}
