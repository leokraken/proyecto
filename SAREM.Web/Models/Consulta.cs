using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAREM.Web.Models
{
    public class Consulta
    {
        [Required]
        public String pacienteId { get; set; }
        public IEnumerable<Local> local { get; set; }
        [Required]
        public long localID { get; set; }
        public IEnumerable<Especialidad> especialidades { get; set; }
        [Required]
        public long especialidadID { get; set; }
        public IEnumerable<Funcionario> funcionarios { get; set; }
        [Required]
        public string medID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime fecha_inicio { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime fecha_fin { get; set; }
    }
}