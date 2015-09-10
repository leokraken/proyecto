
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SAREM.Shared.Entities
{
    public class EventoOpcional : Evento
    {
        //public long RangoID { get; set; }
        public string edadesarray { get; set; }
        public string mensaje { get; set; }

        //[ForeignKey("RangoID")]
        //public virtual Rango rango { get; set; }

        //No normalizado...
        [NotMapped]
        public virtual IList<int> edades {
            get { 
                return edadesarray.Split(',').Select(i => int.Parse(i)).ToList();
            }
            set {
                edadesarray = string.Join(",", value.ToArray());
            }
        }
    }
}
