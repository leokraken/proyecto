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
        
        [Required(ErrorMessage = "Debe ingresar un Nro de Documento")]
        public String pacienteId { get; set; }
        public IEnumerable<Local> local { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un Orígen")]
        public long localID { get; set; }
       
        public IEnumerable<Especialidad> especialidades { get; set; }
       
        public long especialidadID { get; set; }
        public IEnumerable<Funcionario> funcionarios { get; set; }
        
        public string medID { get; set; }
        [Required(ErrorMessage = "Debe ingresar la Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime? fecha_inicio { get; set; }
        [Required(ErrorMessage = "Debe ingresar la Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime? fecha_fin { get; set; }
    }
}