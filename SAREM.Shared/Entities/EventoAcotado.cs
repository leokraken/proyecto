using SAREM.Shared.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Shared.Entities
{
    public class EventoAcotado : EventoPersonalizado
    {
        // tiene un conjunto de edades max=min, para el cual el 
        // paciente sera alertado
        // se define hora, dia y mes de la notificacion
        // se ignora el año se considera mes-dia-hora
        [Required]
        DateTime fechanotificacion { get; set; }
        [Required]
        public Sexo sexo { get; set; }

        public virtual ICollection<Rango> edades { get; set; }
    }
}
