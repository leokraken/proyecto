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
        public long ConsultaID { get; set; }

        public long EspecialidadID { get; set; }
        public string FuncionarioID { get; set; }
        public long LocalID { get; set; }

        [Required]
        public DateTime fecha_inicio { get; set; }
        [Required]
        public DateTime fecha_fin { get; set; }

        public string Description { get; set; }
        [Required]
        public short numpacientes { get; set; }
        [Required]
        public short maxpacientesespera { get; set; }

        public virtual Especialidad especialidad { get; set; }
        public virtual Medico medico { get; set; }
        public virtual Local local { get; set; }

        public virtual ICollection<PacienteConsultaEspera> pacientesespera { get; set; }
        //varios pacientes pueden agendar si uno de ellos cancela
        public virtual ICollection<PacienteConsultaAgenda> pacientes { get; set; }
        public virtual ICollection<PacienteConsultaCancelar> pacientescancelar { get; set; }
        public virtual ICollection<AgendaEvento> notificaciones { get; set; }
    }
}
