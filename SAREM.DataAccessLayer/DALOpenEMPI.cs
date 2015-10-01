using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SAREM.DataAccessLayer
{
    public class DALOpenEMPI : IDALOpenEMPI
    {
        private static string OPENEMPI_URL = ConfigurationManager.AppSettings["OPENEMPI_URL"].ToString();
        private static string OPENEMPI_ADMIN = ConfigurationManager.AppSettings["OPENEMPI_ADMIN"].ToString();
        private static string OPENEMPI_PASS = ConfigurationManager.AppSettings["OPENEMPI_PASS"].ToString();

        private static string OPENEMPI_SESSION_KEY = null;

        public DALOpenEMPI()
        {
            OPENEMPI_SESSION_KEY = getAuth();
        }

        private string getAuthXML(string user, string password)
        {
            return "<authenticationRequest><password>" + password + "</password><username>" + user + "</username></authenticationRequest>";
        }

        private XmlWriterSettings settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            ConformanceLevel = ConformanceLevel.Document,
            OmitXmlDeclaration = true,
            CloseOutput = true,
            Indent = true,
            IndentChars = "  ",
            NewLineHandling = NewLineHandling.Replace
        };

        async Task<List<identifierDomain>> httpDominios()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OPENEMPI_URL);
                client.DefaultRequestHeaders.TryAddWithoutValidation("OPENEMPI_SESSION_KEY", OPENEMPI_SESSION_KEY);

                var response = await client.GetAsync("openempi-ws-rest/person-query-resource/getIdentifierDomains");

                TextReader reader = new StringReader(response.Content.ReadAsStringAsync().Result);
                XmlSerializer personserializer = new XmlSerializer(typeof(identifierDomain[]), new XmlRootAttribute("identifierDomains"));
                return ((identifierDomain[])personserializer.Deserialize(reader)).ToList();
            }
        }

        async Task<person> parallelRequestPaciente(string pacienteID, List<identifierDomain> dominios)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OPENEMPI_URL);
                client.DefaultRequestHeaders.TryAddWithoutValidation("OPENEMPI_SESSION_KEY", OPENEMPI_SESSION_KEY);

                List<StringContent> content = new List<StringContent>();
                //serialize request
                foreach (var dominio in dominios)
                {
                    personIdentifier person = new personIdentifier
                    {
                        identifier = pacienteID,
                        identifierDomain = new identifierDomain
                        {
                            namespaceIdentifier = dominio.namespaceIdentifier,
                            universalIdentifier = dominio.universalIdentifier,
                            universalIdentifierTypeCode = dominio.universalIdentifierTypeCode
                        }
                    };

                    string xml;

                    XmlSerializer serializer = new XmlSerializer(typeof(personIdentifier));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");

                    using (StringWriter sww = new StringWriter())
                    using (XmlWriter writer = XmlWriter.Create(sww, settings))
                    {
                        serializer.Serialize(writer, person, ns);
                        xml = sww.ToString();
                    }

                    var httpContent = new StringContent(xml, Encoding.UTF8, "application/xml");
                    content.Add(httpContent);
                }

                //parallel!
                var count = content.Count;
                List<Task<HttpResponseMessage>> tasklist = new List<Task<HttpResponseMessage>>();
                foreach (var c in content)
                {
                    tasklist.Add(client.PostAsync("openempi-ws-rest/person-query-resource/findPersonById", c));
                }

                //finish all
                await Task.WhenAll(tasklist.ToArray());
                foreach (var res in tasklist)
                {
                    TextReader reader = new StringReader(res.Result.Content.ReadAsStringAsync().Result);
                    try
                    {
                        XmlSerializer personserializer = new XmlSerializer(typeof(person));
                        return (person)personserializer.Deserialize(reader);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Person not found at domain ");//+ dominio.identifierDomainId);
                    }
                }
                
                //var response = await client.PostAsync("openempi-ws-rest/person-query-resource/findPersonById", httpContent);
                return null;
                
            }

        }


        async Task<person> httpPaciente(string pacienteID, List<identifierDomain> dominios)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OPENEMPI_URL);
                client.DefaultRequestHeaders.TryAddWithoutValidation("OPENEMPI_SESSION_KEY", OPENEMPI_SESSION_KEY);
                foreach (var dominio in dominios)
                {
                    personIdentifier person = new personIdentifier
                    {
                        identifier = pacienteID,
                        identifierDomain = new identifierDomain { 
                            namespaceIdentifier = dominio.namespaceIdentifier,
                            universalIdentifier = dominio.universalIdentifier,
                            universalIdentifierTypeCode = dominio.universalIdentifierTypeCode
                        }
                    };

                    string xml;

                    XmlSerializer serializer = new XmlSerializer(typeof(personIdentifier));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");

                    using (StringWriter sww = new StringWriter())
                    using (XmlWriter writer = XmlWriter.Create(sww, settings))
                    {
                        serializer.Serialize(writer, person, ns);
                        xml = sww.ToString();
                    }

                    var httpContent = new StringContent(xml, Encoding.UTF8, "application/xml");
                    var response = await client.PostAsync("openempi-ws-rest/person-query-resource/findPersonById", httpContent);

                    if (response.IsSuccessStatusCode)
                    {

                    }

                    TextReader reader = new StringReader(response.Content.ReadAsStringAsync().Result);
                    try
                    {
                        XmlSerializer personserializer = new XmlSerializer(typeof(person));
                        return (person)personserializer.Deserialize(reader);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Person not found at domain "+ dominio.identifierDomainId);
                    }


                }
            }
            throw new Exception("OPENEMPI::Paciente NOT FOUND");
        }

        private async Task<string> getAuthentication()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OPENEMPI_URL);
                var body = new StringContent(getAuthXML(OPENEMPI_ADMIN, OPENEMPI_PASS), Encoding.UTF8, "application/xml");
                var response = await client.PutAsync(OPENEMPI_URL + "openempi-ws-rest/security-resource/authenticate", body);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string resultado = response.Content.ReadAsStringAsync().Result;
                    return resultado;
                }
                else
                {
                    throw new Exception("Error autenticacion...");

                }
                //Console.WriteLine(response.StatusCode.ToString());   
            }

        }

        public person obtenerPaciente(string paisID, string pacienteID)
        {
            string search = paisID + "-" + pacienteID;
            Task<person> person = httpPaciente(search, httpDominios().Result);
            return person.Result;
        }

        public person obtenerPacienteParallel(string paisID, string pacienteID)
        {
            string search = paisID + "-" + pacienteID;
            Task<person> person = parallelRequestPaciente(search, httpDominios().Result);
            return person.Result;
        }


        public List<identifierDomain> obtenerDominios()
        {
            return httpDominios().Result;
        }

        public string getAuth()
        {
            Task<string> t = getAuthentication();
            return t.Result;
        }
    }
}
