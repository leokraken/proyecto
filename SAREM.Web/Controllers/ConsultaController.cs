using SAREM.DataAccessLayer;
using SAREM.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            var model = new Consulta
            {
               local = agenda.listarLocales(),
               especialidades = agenda.listarEspecialidades(),
               funcionarios = agenda.listarFuncionarios()
            };


            return View(model);
        }

        // POST: Consulta/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "pacienteId,localID,especialidadID,medID,fecha_inicio,fecha_fin")] Consulta consulta)
        {

            if (ModelState.IsValid)
            {
                SAREM.Shared.Entities.Consulta c = new SAREM.Shared.Entities.Consulta();
                c.LocalID = consulta.localID;
                c.EspecialidadID = consulta.especialidadID;
                c.FuncionarioID = consulta.medID;
                c.fecha_fin = consulta.fecha_fin.ToUniversalTime();
                c.fecha_inicio = consulta.fecha_inicio.ToUniversalTime();
                
                agenda.agregarConsulta(c);
              
                return RedirectToAction("Index", "Tecnico");
            }

            return View(consulta);
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
