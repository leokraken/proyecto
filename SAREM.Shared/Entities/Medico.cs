﻿using System.Collections.Generic;

namespace SAREM.Shared.Entities
{
    public class Medico : Funcionario
    {
        public virtual ICollection<Paciente> referenciados { get; set; }
        public virtual ICollection<Consulta> consultas { get; set; }
    }
}
