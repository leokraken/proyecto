using SAREM.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAREM.Web.Controllers
{
    public class PacienteController : Controller
    {
        private FabricaSAREM fabrica = new FabricaSAREM("test");
        // GET: Paciente
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CheckPaciente(string idP)
        {

            try
            {

                Boolean existe = fabrica.ipacientes.checkPaciente(idP);
               
                return Json(new { success = existe });
              
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}