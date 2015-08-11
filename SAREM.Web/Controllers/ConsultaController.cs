using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
using SAREM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SAREM.Web.Controllers
{
    public class ConsultaController : Controller
    {
        IDALAgenda agenda = new DALAgenda("test");
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
        public ActionResult Create([Bind(Include = "pacienteId,localID,especialidadID,medID,fecha_inicio,fecha_fin")]  SAREM.Web.Models.Consulta consulta)
        {

            if (ModelState.IsValid)
            {
                SAREM.Shared.Entities.Consulta c = new SAREM.Shared.Entities.Consulta();
                c.LocalID = consulta.localID;
                c.EspecialidadID = consulta.especialidadID;
                c.FuncionarioID = consulta.medID;
                //c.fecha_fin = consulta.fecha_fin.ToUniversalTime();
                //c.fecha_inicio = consulta.fecha_inicio.ToUniversalTime();
                
                agenda.agregarConsulta(c);
              
                return RedirectToAction("Index", "Tecnico");
            }

            return View(consulta);
        }



        public JsonResult GetEspecialidades(string idLocalidad)
        {
            List<SelectListItem> especialidades = new List<SelectListItem>();
            foreach (Especialidad e in agenda.listarEspecialidades()) {

                especialidades.Add(new SelectListItem { Text = e.tipo, Value = e.EspecialidadID.ToString() });
            }
            return Json(new SelectList(especialidades, "Value", "Text"));
          
        }


        public JsonResult GetMedicos(string idEspecialidad)
        {
            List<SelectListItem> medicos = new List<SelectListItem>();
            foreach (Funcionario m in agenda.listarFuncionarios())
            {

                medicos.Add(new SelectListItem { Text = m.nombre, Value = m.FuncionarioID.ToString() });
            }
            return Json(new SelectList(medicos, "Value", "Text"));
        }

        // GET: Consulta/Edit/5
        public ActionResult Edit(int id)
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
