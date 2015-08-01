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
        private static SARMContext db = null;
        private static string tenant = "test";

        [ClassInitialize]
        public static void InitializeClass(TestContext tc)
        {

            try
            {
                SARMContext.createTenant(tenant);
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            
            db = SARMContext.getTenant(tenant);
            iagenda = new DALAgenda(tenant);

            List<Nacion> naciones = new List<Nacion>
            {
                new Nacion { NacionID = "UY", nombre = "Uruguay" },
                new Nacion { NacionID = "BR", nombre = "Brasil" }
            };
            naciones.ForEach(n => db.naciones.Add(n));
            db.SaveChanges();

            List<Paciente> pacientes = new List<Paciente>
            {
                new Paciente{ PacienteID="50548305",
                    FN=new DateTime(1991,6,22),
                    sexo=Sexo.MASCULINO,
                    nombre="Leonardo Clavijo",
                    NacionID = naciones.First().NacionID
                }
            };
            pacientes.ForEach(p => db.pacientes.Add(p));
            db.SaveChanges();
            Debug.WriteLine("Pacientes agregados...");

            //especialidades
            List<Especialidad> especialidades = new List<Especialidad>
            {
                new Especialidad{ tipo="Esp1", descripcion="especialidad 1"}
            };
            especialidades.ForEach(e => db.especialidades.Add(e));
            db.SaveChanges();
            Debug.WriteLine("Especialidades agregados...");


            //funcionarios
            List<Medico> funcionarios = new List<Medico>
            {
                new Medico {FuncionarioID="17299999", nombre="Medico1"}
            };
            funcionarios.ForEach(f => db.funcionarios.Add(f));
            db.SaveChanges();
            Debug.WriteLine("Funcionarios agregados...");

            //Creo consultas para asignar
            List<Consulta> consultas = new List<Consulta>
            {
                new Consulta {
                    EspecialidadID = especialidades[0].EspecialidadID,
                    fecha_fin=DateTime.UtcNow,
                    fecha_inicio= DateTime.UtcNow,
                    FuncionarioID=funcionarios[0].FuncionarioID,
                }
            };
            consultas.ForEach(c => db.consultas.Add(c));
            db.SaveChanges();
            Debug.WriteLine("Consultas agregados...");

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

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var db = new SAREMAdminContext();
            db.dropSchema(tenant);
        }

    }
}
