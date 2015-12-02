using SAREM.Shared.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class Paciente
    {
        [Key]
        public string PacienteID { get; set; }
        public string PaisID { get; set; }

        public string nombre { get; set; }
        public string apellido { get; set; }
        public string direccion { get; set; }
        public string celular { get; set; }
        public string telefono { get; set; }
        public string mail { get; set; }
        public string extra_account { get; set; }
        public DateTime ? FN { get; set; }


        [Required]
        public string sexo { get; set; }
        public Boolean sancion { get; set; }
        public string FuncionarioID { get; set; }

        [ForeignKey("PaisID")]
        public virtual Pais nacion { get; set; }
        public virtual ICollection<PacienteConsultaAgenda> agendadas { get; set; }
        public virtual ICollection<PacienteConsultaCancelar> canceladas { get; set; }
        public virtual ICollection<PacienteConsultaEspera> consultasespera { get; set; }
        public virtual ICollection<EventoPacienteComunicacion> eventos { get; set; }
        [ForeignKey("FuncionarioID")]
        public virtual Medico medico { get; set;}
    }
}
