﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAREM.DataAccessLayer;
using System.Diagnostics;
using SAREM.Shared.Entities;
using System.Collections.Generic;
using System.Linq;
using SAREM.Shared.enums;
using SAREM.Shared.Excepciones;

namespace SAREM.Testing
{
    [TestClass]
    public class DALAgendaTest
    {
        private static string tenant = "test";
        private static SARMContext db = null;
        private static FabricaSAREM fabrica = new FabricaSAREM(tenant);

        //datos
        private static List<Comunicacion> comunicaciones = new List<Comunicacion> { 
                //new Comunicacion{ ID=1, nombre="Whatsapp", metadata="Mensaje"},
                new Comunicacion{ ID=1, nombre="Email", metadata="Email Mensaje"},
                new Comunicacion{ ID=2, nombre="Twitter", metadata="Mensaje privado Twitter"}
            };

        private static List<EventoObligatorio> eventos = new List<EventoObligatorio> { 
                new EventoObligatorio{
                    EventoID=1, 
                    nombre="Consultas Notificaciones Agregar,cancelar, modificar", 
                    mensaje="Usted ha agendado una consulta", 
                    fechanotificacion=DateTime.Now 
                }                
            };

        private static List<EventoOpcional> eventosop = new List<EventoOpcional> { 
                new EventoOpcional{ nombre="EventoOpcional1", edades= new List<int>{19, 20, 21,23,24,25,26} },
                new EventoOpcional{ nombre="EventoOpcional2", edades= new List<int>{30, 40, 50} }
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

            comunicaciones.ForEach(c => db.comunicaciones.Add(c));
            db.SaveChanges();
            Debug.WriteLine("Comunicaciones agregados...");

            eventos.ForEach(e => db.eventos.Add(e));
            eventosop.ForEach(e => db.eventos.Add(e));
            db.SaveChanges();
            Debug.WriteLine("Eventos agregados...");


            List<Pais> naciones = new List<Pais>
            {
                new Pais { PaisID = "UY", nombre = "Uruguay" },
                new Pais { PaisID = "BR", nombre = "Brasil" }
            };
            naciones.ForEach(n => db.paises.Add(n));
            db.SaveChanges();
            Debug.WriteLine("Paises agregados...");

            List<Paciente> pacientes = new List<Paciente>
            {   /*
                new Paciente{ PacienteID="50548305",
                    FN=new DateTime(1991,6,22),
                    sexo=Sexo.MASCULINO.ToString(),
                    nombre="Leonardo Clavijo",
                    PaisID = naciones.First().PaisID
                },*/
                new Paciente{ PacienteID="50548306",
                    FN=new DateTime(1991,6,22),
                    sexo=Sexo.FEMENINO.ToString(),
                    nombre="Kali la diosa",
                    PaisID = naciones.First().PaisID
                },
                  new Paciente{ PacienteID="50548307",
                    FN=new DateTime(1990,10,11),
                    sexo=Sexo.FEMENINO.ToString(), 
                    nombre="Valentina Da Silva",
                    PaisID = naciones.First().PaisID
                },
                new Paciente{ PacienteID="50548308",
                    FN=new DateTime(1990,11,11),
                    sexo=Sexo.MASCULINO.ToString(),
                    nombre="Jorge Perez",
                    PaisID = naciones.First().PaisID
                },
                new Paciente{ PacienteID="51130115",
                    FN=new DateTime(1990,11,11),
                    sexo=Sexo.FEMENINO.ToString(),
                    nombre="Juana Alvarez",
                    PaisID = naciones.First().PaisID
                }
            };
            //lista
            pacientes.ForEach(p => fabrica.ipacientes.altaPaciente(p));
            db.SaveChanges();
            Debug.WriteLine("Pacientes agregados...");

            //custom agrego 20 pacientes
            List<Paciente> customs = new List<Paciente>();
            for (int i = 0; i < 20; i++)
            {
                Paciente template = new Paciente
                {
                    PacienteID = i.ToString(),
                    FN = new DateTime(1991, 6, 22),
                    sexo = Sexo.MASCULINO.ToString(),
                    nombre = "Leonardo Clavijo" + i.ToString(),
                    PaisID = naciones.First().PaisID
                };
                customs.Add(template);
            }
            customs.ForEach(x =>db.pacientes.Add(x));
            
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
                    fecha_inicio=DateTime.UtcNow,                  
                    fecha_fin= DateTime.UtcNow.AddMinutes(20),
                    FuncionarioID=funcionarios[0].FuncionarioID,
                    LocalID=1,
                    numpacientes=10,
                    maxpacientesespera=10
                },
                new Consulta {
                    EspecialidadID = especialidades[1].EspecialidadID,
                    fecha_inicio=DateTime.UtcNow,                  
                    fecha_fin= DateTime.UtcNow.AddMinutes(300),
                    FuncionarioID=funcionarios[0].FuncionarioID,
                    LocalID=2,
                    numpacientes=10,
                    maxpacientesespera=10
                },
                new Consulta {
                    EspecialidadID = especialidades[2].EspecialidadID,
                    fecha_inicio=DateTime.UtcNow,                  
                    fecha_fin= DateTime.UtcNow.AddMinutes(300),
                    FuncionarioID=funcionarios[0].FuncionarioID,
                    LocalID=1,                 
                    numpacientes=10,
                    maxpacientesespera=10
                }
            };
            FabricaSAREM fab = new FabricaSAREM(tenant);
            consultas.ForEach(c => fab.iagenda.agregarConsulta(c));
            db.SaveChanges();
            Debug.WriteLine("Consultas agregados...");




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
            string CI = "50548306";
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
                    fabrica.iagenda.agregarConsultaPaciente(CI, c.ConsultaID, 1,false);
            }

            Debug.WriteLine("TEST1 FINALIZADO...");
 


            //listo consultas paciente
            Debug.WriteLine("Consultas paciente " + CI);
            var cpacientes = fabrica.iagenda.listarConsultasPaciente(CI);
            foreach (var c in cpacientes)
            {
                Debug.WriteLine(c.consulta.ConsultaID+c.fecha_inicio.ToString());
            }
            //debe tener 3
        }

        [TestMethod]
        public void CancelarConsulta()
        {
            string ci="50548306";
            Debug.WriteLine("TEST::Cancelar Consulta");
            var consulta = db.consultas.First();
            Debug.WriteLine(consulta.ConsultaID);
            Debug.WriteLine("Cancelar consulta "+consulta.ConsultaID+" ");
            fabrica.iagenda.cancelarConsultaPaciente(ci, consulta.ConsultaID);

            Debug.WriteLine("Consultas canceladas paciente "+ ci);
            var canceladas = fabrica.iagenda.listarConsultasCanceladasPaciente(ci);
            canceladas.ToList().ForEach(c => Debug.WriteLine(c.ConsultaID));
            Assert.AreEqual(canceladas.Count(), 1);

            //Pruebo que cancelando consulta, las consultas del paciente son 2
            var marcadas = fabrica.iagenda.listarConsultasPaciente(ci);
            //Assert.AreEqual(marcadas.Count, 2);
            Console.WriteLine("Consultas marcadas: " + marcadas.Count);
        }

        [TestMethod]
        public void TestNotificaciones()
        {
            string PacienteID = "50548306";
            //Listar comunicaciones
            List<Comunicacion> com = fabrica.inotificaciones.listarComunicaciones().ToList();
            com.ForEach(c =>
            {
                Console.WriteLine("ID: " + c.ID + " Nombre:" + c.nombre);
                Assert.IsTrue(comunicaciones.Any(x => x.ID == c.ID));
            });

            //listar eventos
            var ev = fabrica.inotificaciones.listarEventosOpcionales(PacienteID);
            Assert.AreEqual(ev.Count, 0);
            try
            {
                fabrica.inotificaciones.suscribirPacienteEvento(eventosop[1].EventoID, PacienteID, com[0].ID);
            }
            catch (Exception e)
            {
                Console.WriteLine("OK Prueba correcta::Al evento 0 opcional tiene un rango permitido" + e.Message);
            }

            try
            {
                fabrica.inotificaciones.suscribirPacienteEvento(eventos[0].EventoID, "50548306", com[0].ID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error no deberia lanzar excepcion::" + e.Message);
            }

        }

        [TestMethod]
        public void listarEspecialidadesLocalTest()
        {
            var especialidades = fabrica.iespecialidades.listarEspecialidadesLocal(1).ToList();
            especialidades.ForEach(e => Console.WriteLine(e.EspecialidadID));

        }

        [TestMethod]
        public void listarMedicosEspecialidadLocalTest()
        {
            var medicos = fabrica.imedicos.listarMedicosEspecialidadLocal(1, 1).ToList();
            medicos.ForEach(m => Console.WriteLine(m.FuncionarioID + m.nombre));
        }
        
        [TestMethod]
        public void obtenerConsulta()
        {
            var c = fabrica.iagenda.obtenerConsulta(2);
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


        [TestMethod]
        public void testReferencia()
        {
            IDALReferencias iref= new DALReferencias(tenant);
            string pacienteID = "50548306";
            string pacienteID2 = "51130115";
            string pacienteID3 = "50548306";
            string medicoID = "17299999";
            iref.agregarReferencia(pacienteID, medicoID);
        
            var refpendientes = iref.obtenerReferenciasPendientesMedico(medicoID);

            Console.WriteLine("Pacientes pendientes para medico " + medicoID);
            foreach (var p in refpendientes)
            {
                Console.WriteLine(p.PacienteID);
            }

            Assert.AreEqual(refpendientes.Count, 1);
            Console.WriteLine("PENDIENTES MEDICO(1)::" + refpendientes.Count);

            //finalizo referencia
            iref.finalizarReferencia(pacienteID, medicoID);

            //listo que efectivamente sea 0
            var refpendientespost = iref.obtenerReferenciasPendientesMedico(medicoID);
            Console.WriteLine("Lista de pacientes referenciados a medico " + medicoID);
            foreach (var p in refpendientespost)
            {
                Console.WriteLine(p.PacienteID);
            }
            Assert.AreEqual(refpendientespost.Count, 0);
            Console.WriteLine("Pacientespendientemedico::(0)"+refpendientespost.Count);

            //finalmente listo los pacientes dado el medico
            var pacientesmedico = iref.obtenerPacientesReferenciadosMedico(medicoID);
            Console.WriteLine("Listar pacientes referenciados a medico " + medicoID);
            foreach (var p in pacientesmedico)
            {
                Console.WriteLine(p.PacienteID);
            }
            Assert.AreEqual(pacientesmedico.Count, 1);
            Console.WriteLine("Numero pacientes referenciados medico(1)::" + pacientesmedico.Count);

            iref.agregarReferencia(pacienteID2, medicoID);
            iref.agregarReferencia(pacienteID3, medicoID);

        }

        [TestMethod]
        public void testParteDiario()
        {
            string medicoID = "17299999";
            FabricaSAREM factory = new FabricaSAREM(tenant);
            var partes = factory.iagenda.obtenerParteDiario(medicoID, DateTime.UtcNow);

            //ajusto ausencia y diagnostico
            foreach (var parte in partes)
            {
                foreach (var c in parte.pacientes)
                {
                    if (c.PacienteID != null)
                    {
                        factory.iagenda.actualizarParteDiario(c.ConsultaID, c.PacienteID, "Se le complica ...", true);
                        Console.WriteLine(c.paciente.PacienteID + c.paciente.nombre);
                        Console.WriteLine("Ausencia::" + c.ausencia.ToString());
                        Console.WriteLine("Diagnostico::" + c.diagnostico);
                    }
                }
            }
            

            //debe fallar
            try
            {
                var pde = factory.iagenda.obtenerParteDiario(medicoID, DateTime.UtcNow.AddDays(1));
            }
            catch (Exception e)
            {
                Console.WriteLine("OK::" + e.Message);
            }
        }

        [TestMethod]
        public void TestAgregarPacienteConsulta()
        {
            FabricaSAREM f = new FabricaSAREM(tenant);
            using(var db = SARMContext.getTenant(tenant))
            {
                //Creo consulta
                Consulta c = new Consulta
                {
                    EspecialidadID = 1,
                    fecha_inicio = DateTime.UtcNow,
                    fecha_fin = DateTime.UtcNow.AddMinutes(30),
                    FuncionarioID = "17299999",
                    LocalID = 1,
                    numpacientes =10,
                    maxpacientesespera =3

                };
                f.iagenda.agregarConsulta(c);
                //db.consultas.Add(c);
                //db.SaveChanges();

                var pacientes = (from p in db.pacientes
                                 select p).Take(14).ToList();
                int i = 0;
                foreach (var p in pacientes)
                {
                    
                    try
                    {
                        fabrica.iagenda.agregarConsultaPaciente(p.PacienteID, c.ConsultaID, (short)i);
                        i++;
                        /*    
                        if (date == null)
                            Console.WriteLine(i + ")Paciente " + p.PacienteID + " agregado a lista de espera ");
                        else
                            Console.WriteLine(i + ")Paciente " + p.PacienteID + " agregado, consulta programada "+ date.ToString());
                        */
                    }
                    catch (Exception max)
                    {
                        Console.WriteLine(i + "El paciente " + p.PacienteID + "no pudo se agregado... Max cola espera");
                    }

                }

                Console.WriteLine("Segundo TEST:: Cancelar consulta [3] y agregar el [14]");
                fabrica.iagenda.cancelarConsultaPaciente(pacientes[3].PacienteID, c.ConsultaID);
                Console.WriteLine("Consulta cancelada...");
                //veo a cual asigno...
                var enespera = fabrica.iagenda.obtenerPacientesConsultaEspera(c.ConsultaID);
                Console.WriteLine("Pacientes en lista de espera");
                foreach (var p in enespera)
                    Console.WriteLine(p.PacienteID);

            }

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var db = new SAREMAdminContext();
            //db.dropSchema(tenant);
        }

    }
}
