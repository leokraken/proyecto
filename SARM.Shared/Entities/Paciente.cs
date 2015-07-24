using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAREM.Shared
{
    public class Paciente
    {
        [Key]
        public string PacienteID { get; set; }
        [Required]
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string celular { get; set; }
        public string telefono { get; set; }
        [Required]
        public DateTime FN { get; set; }
        [Required]
        public string nacionalidad { get; set; }
        [Required]
        public bool sexo { get; set; }

        public virtual ICollection<PacienteConsultaAgenda> agendadas { get; set; }
        public virtual ICollection<PacienteConsultaCancelar> canceladas { get; set; }
    }
}
