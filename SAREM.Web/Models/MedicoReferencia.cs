using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAREM.Web.Models
{
    public class MedicoReferencia
    {
        public IEnumerable<Especialidad> especialidad { get; set; }
        public String EspecialidadID { get; set; }
    }
}