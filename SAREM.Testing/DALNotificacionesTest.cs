using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.Testing
{
    [TestClass]
    class DALNotificacionesTest
    {
        private static IDALNotificaciones inot = null;
        private static SARMContext db = null;
        private static string tenant = "test";

        [ClassInitialize]
        public static void InitializeClass(TestContext tc)
        {
            db= SARMContext.getTenant(tenant);

            List<Comunicacion> comunicaciones = new List<Comunicacion> { 
                new Comunicacion{ ID=1, nombre="Whatsapp", metadata="Mensaje"},
                new Comunicacion{ ID=2, nombre="Email", metadata="Email Mensaje"},
                new Comunicacion{ ID=1, nombre="SMS", metadata="Mensaje SMS"}
            };

            comunicaciones.ForEach(c => db.comunicaciones.Add(c));
            db.SaveChanges();

            List<Rango> rangos = new List<Rango> { 
                new Rango{ID=1, limitei=56, limites=70, nombre="Rango1", sexo=Sexo.FEMENINO},
                new Rango{ID=1, limitei=18, limites=45, nombre="Rango2", sexo=Sexo.FEMENINO}
            };
            rangos.ForEach(r => db.rangos.Add(r));
            db.SaveChanges();

            List<Evento> eventos = new List<Evento> { 
                new EventoEstatico{EventoID=1, nombre="Evento1", mensaje="Mensaje1", dias=1, rangos=rangos},
                new EventoEstatico{EventoID=2, nombre="Evento2", mensaje="Mensaje2", dias=1, rangos=rangos},
            };
            eventos.ForEach(e => db.eventos.Add(e));
            db.SaveChanges();
        }

        [TestMethod]
        public void SuscribirPacienteEvento()
        {

        }

    }
}
