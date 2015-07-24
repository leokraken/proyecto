using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared
{
    public class PacienteConsultaCancelar
    {
        public string PacienteID { get; set; }
        public long ConsultaID { get; set; }

        [ForeignKey("PacienteID")]
        public virtual Paciente paciente { get; set; }
        [ForeignKey("ConsultaID")]
        public virtual ConsultaNormal consulta { get; set; }

        public DateTime fecha { get; set; }
    }
}
