using DHTMLX.Scheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class Consulta
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DHXJson(Alias = "id")]
        public long ConsultaID { get; set; }

        [DHXJson(Ignore = true)]
        public long EspecialidadID { get; set; }
        [DHXJson(Ignore = true)]
        public string FuncionarioID { get; set; }

        [Required]
        [DHXJson(Alias = "start_date")]
        public DateTime fecha_inicio { get; set; }
        [Required]
        [DHXJson(Alias = "end_date")]
        public DateTime fecha_fin { get; set; }

        [DHXJson(Alias = "text")]
        public string Description { get; set; }

        public virtual Especialidad especialidad { get; set; }
        public virtual Medico medico { get; set; }
        public virtual Local local { get; set; }
        //varios pacientes pueden agendar si uno de ellos cancela
        public virtual ICollection<PacienteConsultaAgenda> pacientes { get; set; }
        public virtual ICollection<PacienteConsultaCancelar> pacientescancelar { get; set; }
        //one to many
        public virtual Paciente ausencia { get; set; }

        public virtual ICollection<AgendaEvento> notificaciones { get; set; }
    }
}
