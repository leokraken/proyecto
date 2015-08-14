using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAREM.DataAccessLayer;
using System.Diagnostics;
using SAREM.Shared.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SAREM.Testing
{
    [TestClass]
    public class DALAgendaTest
    {
        private static IDALAgenda iagenda = null;
        private static IDALNotificaciones inot = null;
        private static SARMContext db = null;
        private static string tenant = "test";

        //datos
        private static List<Comunicacion> comunicaciones = new List<Comunicacion> { 
                new Comunicacion{ ID=1, nombre="Whatsapp", metadata="Mensaje"},
                new Comunicacion{ ID=2, nombre="Email", metadata="Email Mensaje"},
                new Comunicacion{ ID=1, nombre="SMS", metadata="Mensaje SMS"}
            };

        private static List<Rango> rangos = new List<Rango> { 
                new Rango{ID=1, limitei=56, limites=70, nombre="Rango1", sexo=Sexo.FEMENINO},
                new Rango{ID=1, limitei=18, limites=45, nombre="Rango2", sexo=Sexo.FEMENINO}
            };
        private static List<Evento> eventos = new List<Evento> { 
                new EventoEstatico{EventoID=1, nombre="Evento1", mensaje="Mensaje1", dias=1, rangos=rangos},
                new EventoEstatico{EventoID=2, nombre="Evento2", mensaje="Mensaje2", dias=1, rangos=rangos},
            };

        [ClassInitialize]
        public static void InitializeClass(TestContext tc)
        {

            try
            {
                var context = new SAREMAdminContext();
                context.dropSchema(tenant);
                SARMContext.createTenant(tenant);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            
            db = SARMContext.getTenant(tenant);
            iagenda = new DALAgenda(tenant);
            inot = new DALNotificaciones(tenant);

            List<Pais> naciones = new List<Pais>
            {
                new Pais { PaisID = "UY", nombre = "Uruguay" },
                new Pais { PaisID = "BR", nombre = "Brasil" }
            };
            naciones.ForEach(n => db.paises.Add(n));
            db.SaveChanges();

            List<Paciente> pacientes = new List<Paciente>
            {
                new Paciente{ PacienteID="50548305",
                    FN=new DateTime(1991,6,22),
                    sexo=Sexo.MASCULINO,
                    nombre="Leonardo Clavijo",
                    PaisID = naciones.First().PaisID
                },
                new Paciente{ PacienteID="50548306",
                    FN=new DateTime(1991,6,22),
                    sexo=Sexo.FEMENINO,
                    nombre="Kali la diosa",
                    PaisID = naciones.First().PaisID
                }
            };
            pacientes.ForEach(p => db.pacientes.Add(p));
            db.SaveChanges();
            Debug.WriteLine("Pacientes agregados...");

            //especialidades
            List<Especialidad> especialidades = new List<Especialidad>
            {
                new Especialidad{ tipo="Esp1", descripcion="especialidad 1"},
                new Especialidad{ tipo="Esp2", descripcion="especialidad 2"},
                new Especialidad{ tipo="Esp3", descripcion="especialidad 3"},
                new Especialidad{ tipo="Esp4", descripcion="especialidad 4"}
            };
            especialidades.ForEach(e => db.especialidades.Add(e));
            db.SaveChanges();
            Debug.WriteLine("Especialidades agregados...");


            //funcionarios
            List<Medico> funcionarios = new List<Medico>
            {
                new Medico {FuncionarioID="17299999", nombre="Medico1", especialidades=especialidades}
            };
            funcionarios.ForEach(f => db.funcionarios.Add(f));
            db.SaveChanges();
            Debug.WriteLine("Funcionarios agregados...");


            List<Local> locales = new List<Local>
            {
                new Local {LocalID=1, nombre = "local1", calle="calle1", numero="SN", especialidades=especialidades, medicos=funcionarios},
                new Local {LocalID=2, nombre = "local2", calle="calle2", numero="SN2"},
                new Local {LocalID=3, nombre = "local3", calle="calle3", numero="SN3"}
            };
            locales.ForEach(l => db.locales.Add(l));
            db.SaveChanges();
            //Creo consultas para asignar
            


            List<Consulta> consultas = new List<Consulta>
            {
                new Consulta {
                    EspecialidadID = especialidades[0].EspecialidadID,
                    fecha_fin=DateTime.UtcNow,                  
                    fecha_inicio= DateTime.UtcNow.AddMinutes(30),
                    FuncionarioID=funcionarios[0].FuncionarioID,
                    LocalID=1
                }
            };

            consultas.ForEach(c => db.consultas.Add(c));
            db.SaveChanges();
            Debug.WriteLine("Consultas agregados...");

            comunicaciones.ForEach(c => db.comunicaciones.Add(c));
            db.SaveChanges();
            Debug.WriteLine("Comunicaciones agregados...");

            rangos.ForEach(r => db.rangos.Add(r));
            db.SaveChanges();
            Debug.WriteLine("Rangos agregados...");

            eventos.ForEach(e => db.eventos.Add(e));
            db.SaveChanges();
            Debug.WriteLine("Eventos agregados...");
        }

        [TestMethod]
        public void AgregarConsultaPaciente()
        {
            string CI = "50548305";
            //listar consultas
            foreach (var c in db.consultas)
            {
                Debug.WriteLine(c.ConsultaID+" "+c.fecha_inicio.ToString());
            }
            foreach (var p in db.pacientes)
            {
                Debug.WriteLine(p.PacienteID + " " + p.nombre);
            }

            //selecciono la primera consulta
            var consultas = from c in db.consultas
                           select c;
            iagenda.agregarConsultaPaciente(CI,consultas.First().ConsultaID);


            //listo consultas paciente
            Debug.WriteLine("Consultas paciente " + CI);
            var cpacientes = iagenda.listarConsultasPaciente(CI);
            foreach (var c in cpacientes)
            {
                Debug.WriteLine(c.ConsultaID+c.fecha_inicio.ToString());
            }
        }

        [TestMethod]
        public void CancelarConsulta()
        {
            string ci="50548305";
            Debug.WriteLine("TEST::Cancelar Consulta");
            var consulta = db.consultas.First();
            Debug.WriteLine(consulta.ConsultaID);
            iagenda.cancelarConsultaPaciente(ci, consulta.ConsultaID);

            Debug.WriteLine("Consultas canceladas paciente "+ ci);
            var canceladas = iagenda.listarConsultasCanceladasPaciente(ci);
            canceladas.ToList().ForEach(c => Debug.WriteLine(c.ConsultaID));
            Assert.AreEqual(canceladas.Count(), 1);
        }

        [TestMethod]
        public void TestNotificaciones()
        {
            string PacienteID = "50548305";
            //Listar comunicaciones
            List<Comunicacion> com = inot.listarComunicaciones().ToList();
            com.ForEach(c =>
            {
                Console.WriteLine("ID: " + c.ID + " Nombre:" + c.nombre);
                Assert.IsTrue(comunicaciones.Any(x => x.ID == c.ID));
            });

            //listar eventos
            var ev = inot.listarEventosPosibles(PacienteID);
            Assert.AreEqual(ev.Count, 0);
            try
            {
                inot.suscribirPacienteEvento(eventos[0].EventoID, PacienteID, com[0].ID);

            }
            catch (Exception e)
            {
                Console.WriteLine("OK Prueba correcta::" + e.Message);
            }

            try
            {
                inot.suscribirPacienteEvento(eventos[0].EventoID, "50548306", com[0].ID);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error no deberia lanzar excepcion::" + e.Message);
            }

        }

        [TestMethod]
        public void listarEspecialidadesLocalTest()
        {
            var especialidades = iagenda.listarEspecialidadesLocal(1).ToList();
            especialidades.ForEach(e => Console.WriteLine(e.EspecialidadID));

        }

        [TestMethod]
        public void listarMedicosEspecialidadLocalTest()
        {
            var medicos = iagenda.listarMedicosEspecialidadLocal(1, 1).ToList();
            medicos.ForEach(m => Console.WriteLine(m.FuncionarioID + m.nombre));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var db = new SAREMAdminContext();
            //db.dropSchema(tenant);
        }

    }
}
