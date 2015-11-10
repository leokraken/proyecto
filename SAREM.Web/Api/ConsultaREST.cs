using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAREM.Shared.Datatypes;
using SAREM.DataAccessLayer;

namespace SAREM.Web
{
    public class ConsultaREST : ApiController
    {
        // GET api/<controller>
        [Route("api/consulta/{pacienteID}")]
        public IEnumerable<APIDataConsulta> Get([FromUri]string pacienteID)
        {
            FabricaSAREM f = new FabricaSAREM();
            ICollection<APIDataConsulta> lista = new List<APIDataConsulta>();
            return lista;

            //return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}