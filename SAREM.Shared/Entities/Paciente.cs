﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAREM.Shared.Entities
{
    public enum Sexo { MASCULINO, FEMENINO };
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
        public string NacionID { get; set; }
        [Required]
        public Sexo sexo { get; set; }
        public bool sansion { get; set; }

        public virtual Nacion nacion { get; set; }
        public virtual ICollection<PacienteConsultaAgenda> agendadas { get; set; }
        public virtual ICollection<PacienteConsultaCancelar> canceladas { get; set; }
        public virtual ICollection<Consulta> ausencias { get; set; }
    }
}