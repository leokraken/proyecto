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
        
       
        public IEnumerable<Local> local { get; set; }
       
        [Required(ErrorMessage = "Debe seleccionar un Orígen")]
        [RegularExpression(@"^[1-9][0-9]*$")]
        public String localID { get; set; }
       
        public IEnumerable<Especialidad> especialidades { get; set; }
        //esp id es un long
        [RegularExpression(@"^[1-9][0-9]*$")]
        [Required(ErrorMessage = "Debe ingresar una Especialidad")]
        public String especialidadID { get; set; }
        public IEnumerable<Funcionario> funcionarios { get; set; }

        [RegularExpression(@"^[1-9][0-9]*$")]
        [Required(ErrorMessage = "Debe ingresar un Médico")]
        public String medID { get; set; }
        
        [Required(ErrorMessage = "Debe ingresar la Fecha de Inicio")]
        public String fecha_inicio { get; set; }
        
        [Required(ErrorMessage = "Debe ingresar la Fecha de Fin")]
        public String fecha_fin { get; set; }
    }
}