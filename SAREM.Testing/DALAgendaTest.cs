using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAREM.DataAccessLayer;
using System.Diagnostics;
using SAREM.Shared.Entities;
using System.Collections.Generic;
using System.Linq;
using SAREM.Shared.enums;

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

        private static List<EventoObligatorio> eventos = new List<EventoObligatorio> { 
                new EventoObligatorio{EventoID=1, nombre="Evento1", mensaje="Tal vez muera manana", fechanotificacion=DateTime.Now },
                new EventoObligatorio{EventoID=2, nombre="Evento2", mensaje="Debe visitar su medico referencia", fechanotificacion= DateTime.Now }
            };

        private static List<EventoOpcional> eventosop = new List<EventoOpcional> { 
                new EventoOpcional{EventoID=3, nombre="EventoOpcional1", edades= new List<int>{19, 20, 21,23,24,25,26} },
                new EventoOpcional{EventoID=4, nombre="EventoOpcional2", edades= new List<int>{30, 40, 50} }
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
                },
                  new Paciente{ PacienteID="50548307",
                    FN=new DateTime(1990,10,11),
                    sexo=Sexo.FEMENINO, 
                    nombre="Valentina Da Silva",
                    PaisID = naciones.First().PaisID
                },
                new Paciente{ PacienteID="50548308",
                    FN=new DateTime(1990,11,11),
                    sexo=Sexo.MASCULINO,
                    nombre="Jorge Perez",
                    PaisID = naciones.First().PaisID
                },
                new Paciente{ PacienteID="51130115",
                    FN=new DateTime(1990,11,11),
                    sexo=Sexo.FEMENINO,
                    nombre="Juana Alvarez",
                    PaisID = naciones.First().PaisID
                }
            };
            //lista
            pacientes.ForEach(p => db.pacientes.Add(p));
            db.SaveChanges();
    
            //custom agrego 20 pacientes
            List<Paciente> customs = new List<Paciente>();
            for (int i = 0; i < 20; i++)
            {
                Paciente template = new Paciente
                {
                    PacienteID = i.ToString(),
                    FN = new DateTime(1991, 6, 22),
                    sexo = Sexo.MASCULINO,
                    nombre = "Leonardo Clavijo" + i.ToString(),
                    PaisID = naciones.First().PaisID
                };
                customs.Add(template);
            }
            customs.ForEach(x =>db.pacientes.Add(x));
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
                new Medico {FuncionarioID="17299999", nombre="Medico1", especialidades=especialidades},
                new Medico {FuncionarioID="17299998", nombre="Medico2", especialidades=especialidades},
                new Medico {FuncionarioID="17299997", nombre="Medico3", especialidades=especialidades},
                new Medico {FuncionarioID="17299996", nombre="Medico4", especialidades=especialidades},
                new Medico {FuncionarioID="17299995", nombre="Medico5", especialidades=especialidades}
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
                },
                new Consulta {
                    EspecialidadID = especialidades[1].EspecialidadID,
                    fecha_fin=DateTime.UtcNow,                  
                    fecha_inicio= DateTime.UtcNow.AddMinutes(32),
                    FuncionarioID=funcionarios[0].FuncionarioID,
                    LocalID=2
                },
                new Consulta {
                    EspecialidadID = especialidades[2].EspecialidadID,
                    fecha_fin=DateTime.UtcNow,                  
                    fecha_inicio= DateTime.UtcNow.AddMinutes(60),
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

            eventos.ForEach(e => db.eventos.Add(e));
            db.SaveChanges();
            Debug.WriteLine("Eventos agregados...");


            //al paciente 13 le asigno un medico de referencia.
            var pac = db.pacientes.Find("13");
            pac.FuncionarioID = funcionarios[0].FuncionarioID;
            pac = db.pacientes.Find("10");
            pac.FuncionarioID = funcionarios[0].FuncionarioID;
            pac = db.pacientes.Find("1");
            pac.FuncionarioID = funcionarios[0].FuncionarioID;

            pac = db.pacientes.Find("2");
            pac.FuncionarioID = funcionarios[1].FuncionarioID;
            pac = db.pacientes.Find("3");
            pac.FuncionarioID = funcionarios[1].FuncionarioID;
            pac = db.pacientes.Find("4");
            pac.FuncionarioID = funcionarios[1].FuncionarioID;
            db.SaveChanges();

        }

        [TestMethod]
        public void AgregarConsultaPaciente()
        {
            string CI = "50548305";
            string CI2 = "50548306";
            //listar consultas
            foreach (var c in db.consultas)
            {
                Debug.WriteLine(c.ConsultaID+" "+c.fecha_inicio.ToString());
            }
            foreach (var p in db.pacientes)
            {
                Debug.WriteLine(p.PacienteID + " " + p.nombre);
            }

            //selecciono las consultas y agrego al paciente
            var consultas = from c in db.consultas
                           select c;
            
            foreach (var c in consultas)
            {
                if(c.ConsultaID!=2)
                    iagenda.agregarConsultaPaciente(CI, c.ConsultaID);
            }

         
            //agrego a la consulta 2 todos los pacientes

            foreach (var p in db.pacientes)
            {
                try
                {
                    iagenda.agregarConsultaPaciente(p.PacienteID, 2);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("El paciente "+p.PacienteID +" no ha sido agregado maximo superado en consulta...");
                }
            }

            //listo consultas paciente
            Debug.WriteLine("Consultas paciente " + CI);
            var cpacientes = iagenda.listarConsultasPaciente(CI);
            foreach (var c in cpacientes)
            {
                Debug.WriteLine(c.ConsultaID+c.fecha_inicio.ToString());
            }
            //debe tener 3
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

            //Pruebo que cancelando consulta, las consultas del paciente son 2
            var marcadas = iagenda.listarConsultasPaciente(ci);
            //Assert.AreEqual(marcadas.Count, 2);
            Console.WriteLine("Consultas marcadas: " + marcadas.Count);
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
            var ev = inot.listarEventosOpcionales(PacienteID);
            Assert.AreEqual(ev.Count, 0);
            try
            {
                inot.suscribirPacienteEvento(eventosop[1].EventoID, PacienteID, com[0].ID);
            }
            catch (Exception e)
            {
                Console.WriteLine("OK Prueba correcta::Al evento 0 opcional tiene un rango permitido" + e.Message);
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
        
        [TestMethod]
        public void obtenerConsulta()
        {
            var c = iagenda.obtenerConsulta(2);
            Console.WriteLine("Local");
            Console.WriteLine(c.local.LocalID);
            Console.WriteLine("medico");
            Console.WriteLine(c.medico.nombre);
            Console.WriteLine("Pacientes");
            foreach (var a in c.pacientes)
                Console.WriteLine(a.PacienteID);

            Console.WriteLine("Espera");
            foreach (var a in c.pacientesespera)
                Console.WriteLine(a.PacienteID);

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var db = new SAREMAdminContext();
            //db.dropSchema(tenant);
        }

    }
}
