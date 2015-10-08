using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAREM.Shared.Entities;

namespace SAREM.Shared.Datatypes
{
    public class DataPaciente
    {
        public Paciente paciente { get; set; }
        public string mutualista { get; set; }
    }
}
