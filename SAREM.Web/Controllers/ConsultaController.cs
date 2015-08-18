using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
//using SAREM.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SAREM.Web.Controllers
{
    public class ConsultaController : Controller
    {
        private IDALAgenda agenda = new DALAgenda("test");
        private IDALPacientes paciente = new DALPacientes("test");

        private static string[] formats = new string[]
        {
            "dd/MM/yyyy H:mm",
            "dd/MM/yyyy HH:mm"    
        };

        private static DateTime ParseDate(string input)
        {
            return DateTime.ParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        // GET: Consulta
        public ActionResult Index()
        {
           
            return View();
        }

        // GET: Consulta/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Consulta/Create
        [HttpGet]
        public ActionResult Create()
        {
            var model = new SAREM.Web.Models.Consulta
            {
               local = agenda.listarLocales(),
               
               //especialidades = agenda.listarEspecialidades(),
               //funcionarios = agenda.listarFuncionarios()
            };


            return View(model);
        }

        // POST: Consulta/Create
        [HttpPost]
        public JsonResult Create(SAREM.Web.Models.Consulta consulta)
        {

            if (ModelState.IsValid)
            {
              
                SAREM.Shared.Entities.Consulta c = new SAREM.Shared.Entities.Consulta();
                c.LocalID = Convert.ToInt64(consulta.localID);
                c.EspecialidadID = Convert.ToInt64(consulta.especialidadID);
                c.FuncionarioID = consulta.medID;


               
                c.fecha_fin = ParseDate(consulta.fecha_fin).ToUniversalTime();
                c.fecha_inicio = ParseDate(consulta.fecha_inicio).ToUniversalTime();
                

               
                agenda.agregarConsulta(c);
              
                return Json(new { success = true});
            }

            return Json(new { success = false});

            
        }



        public JsonResult GetEspecialidades(string idLocalidad)
        {
            List<SelectListItem> especialidades = new List<SelectListItem>();
            foreach (Especialidad e in agenda.listarEspecialidadesLocal(Convert.ToInt64(idLocalidad)))
            {

                especialidades.Add(new SelectListItem { Text = e.tipo, Value = e.EspecialidadID.ToString() });
            }
            return Json(new SelectList(especialidades, "Value", "Text"));
          
        }


        public JsonResult GetMedicos(string idEspecialidad, string idLocalidad)
        {
            List<SelectListItem> medicos = new List<SelectListItem>();
            foreach (Funcionario m in agenda.listarMedicosEspecialidadLocal(Convert.ToInt64(idLocalidad), Convert.ToInt64(idEspecialidad)))
            {

                medicos.Add(new SelectListItem { Text = m.nombre, Value = m.FuncionarioID.ToString() });
            }
            return Json(new SelectList(medicos, "Value", "Text"));
        }


        public class GetS
        {
            public List<Student> records { get; set; }
        }

        public class Student
        {
            public string id { get; set; }
            public string nombre { get; set; }
            public string apellido { get; set; }
            public DateTime fechaInicio { get; set; }
            public DateTime fechaFin { get; set; }
        }


        public ActionResult VerConsultas()
        {
            return View("VerConsultas");
        }

       
        public JsonResult GetConsultas(string sidx, string sord, int page, int rows)
        {

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-UY");
            DateTime dt = new DateTime(2015, 7, 13, 18, 0, 0);
            DateTime dt2 = new DateTime(2015, 7, 13, 19, 0, 0);

            DateTime dt21 = new DateTime(2015, 7, 14, 4, 0, 0);
            DateTime dt22 = new DateTime(2015, 7, 14, 5, 0, 0);

            DateTime dt31 = new DateTime(2015, 7, 15, 4, 0, 0);
            DateTime dt32 = new DateTime(2015, 7, 15, 5, 0, 0);

            // Defines a custom string format to display the DateTime value.
            // zzzz specifies the full time zone offset.
            String format = "dd/MM/yyyy HH:mm";

            var aux = new GetS
            {

                records = new List<Student>
                    {
                        new Student {id = "1", nombre = "Juan", apellido = "Santos", fechaInicio = dt, fechaFin = dt2},
                        new Student {id = "2", nombre = "Andrea", apellido = "Suarez", fechaInicio = dt21, fechaFin = dt22},
                        new Student {id = "3", nombre = "Laura", apellido = "Paoli", fechaInicio = dt31, fechaFin = dt32 }
                    }
            };

            int totalRecords = aux.records.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = aux.records
            };
            //return Json(aux, JsonRequestBehavior.AllowGet);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: Consulta/Edit/5
        public ActionResult Edit(SAREM.Web.Models.Consulta consulta)
        {
            return View();
        }

        // POST: Consulta/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Consulta/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Consulta/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
