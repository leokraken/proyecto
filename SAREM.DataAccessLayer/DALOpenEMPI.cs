using SAREM.Shared.Datatypes;
using SAREM.Shared.enums;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace SAREM.DataAccessLayer
{
    public sealed class DALOpenEMPI : IDALOpenEMPI
    {
        private static string OPENEMPI_URL = ConfigurationManager.AppSettings["OPENEMPI_URL"].ToString();
        private static string OPENEMPI_ADMIN = ConfigurationManager.AppSettings["OPENEMPI_ADMIN"].ToString();
        private static string OPENEMPI_PASS = ConfigurationManager.AppSettings["OPENEMPI_PASS"].ToString();

        private static string OPENEMPI_SESSION_KEY = null;

        private static readonly DALOpenEMPI instance = new DALOpenEMPI();
   

        public static DALOpenEMPI Instance
        {
            get 
            {
                return instance; 
            }
        }

        private DALOpenEMPI()
        {
           //getAuth();
        }

        /*PARSING*/
        public static void Parse(dynamic parent, XElement node)
        {
            if (node.HasElements)
            {
                if (node.Elements(node.Elements().First().Name.LocalName).Count() > 1)
                {
                    //list
                    var item = new ExpandoObject();
                    var list = new List<dynamic>();
                    foreach (var element in node.Elements())
                    {
                        Parse(list, element);
                    }

                    AddProperty(item, node.Elements().First().Name.LocalName, list);
                    AddProperty(parent, node.Name.ToString(), item);
                }

                else
                {

                    var item = new ExpandoObject();

                    foreach (var attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    //element
                    foreach (var element in node.Elements())
                    {
                        Parse(item, element);
                    }
                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }


        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<String, object>)[name] = value;
            }
        }

        private dynamic getDynamicXML(TextReader content)
        {
            var xDoc = XDocument.Load(content);
            dynamic root = new ExpandoObject();
            Parse(root, xDoc.Elements().First());
            return root;
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
            if (OPENEMPI_SESSION_KEY == null)
                getAuth();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OPENEMPI_URL);
                client.DefaultRequestHeaders.TryAddWithoutValidation("OPENEMPI_SESSION_KEY", OPENEMPI_SESSION_KEY);

                var response = client.GetAsync("openempi-ws-rest/person-query-resource/getIdentifierDomains");

                TextReader reader = new StringReader(response.Result.Content.ReadAsStringAsync().Result);
                XmlSerializer personserializer = new XmlSerializer(typeof(identifierDomain[]), new XmlRootAttribute("identifierDomains"));
                return ((identifierDomain[])personserializer.Deserialize(reader)).ToList();
           
            }
        }

        async Task<DataPaciente> parallelRequestPaciente(string pacienteID, List<identifierDomain> dominios)
        {
            if (OPENEMPI_SESSION_KEY == null)
                getAuth();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OPENEMPI_URL);
                client.DefaultRequestHeaders.TryAddWithoutValidation("OPENEMPI_SESSION_KEY", OPENEMPI_SESSION_KEY);

                List<StringContent> content = new List<StringContent>();
                //serialize request
                foreach (var dominio in dominios)
                {
                    Debug.WriteLine(dominio);
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
                Debug.WriteLine("parallel!!!");
                //var count = content.Count;
                List<Task<HttpResponseMessage>> tasklist = new List<Task<HttpResponseMessage>>();
                foreach (var c in content)
                {
                    tasklist.Add(client.PostAsync("openempi-ws-rest/person-query-resource/findPersonById", c));
                }
              
                //finish all
                Debug.WriteLine("await2?");
                Task.WhenAll(tasklist.ToArray());

                int count = tasklist.Count;
                for (int i=0;i<count; i++)
                {
                    Debug
                        .WriteLine(tasklist[i].Result.Content.ReadAsStringAsync().Result);
                    TextReader reader = new StringReader(tasklist[i].Result.Content.ReadAsStringAsync().Result);
                    try
                    {
                        Debug.WriteLine(tasklist[i].Result.Content.ReadAsStringAsync().Result);
                        XmlSerializer personserializer = new XmlSerializer(typeof(person));
                        person p = (person)personserializer.Deserialize(reader);

                        string s = Sexo.UNKNOWN.ToString();
                        if (p.gender != null)
                        {
                            switch (p.gender.genderCode)
                            {
                                case "M":
                                    s = Sexo.MASCULINO.ToString();
                                    break;
                                case "F":
                                    s = Sexo.FEMENINO.ToString();
                                    break;
                                default:
                                    s = Sexo.UNKNOWN.ToString();
                                    break;
                            }
                        }

                        Paciente paciente = new Paciente {
                            PacienteID= pacienteID,
                            celular = p.cell,
                            direccion = p.address1,
                            FN = p.dateOfBirth,
                            PaisID = p.countryCode,
                            nacion= new Pais{PaisID=p.countryCode, nombre= p.country},
                            nombre = p.middleName,
                            mail = p.email,
                            apellido = p.motherName,                    
                            sexo = s,
                            telefono = p.phoneNumber                        
                        };
                        DataPaciente dp = new DataPaciente { paciente = paciente, mutualista = dominios[i].identifierDomainName };

                        return dp;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Person not found at domain "+ e.Message);//+ dominio.identifierDomainId);
                    }
                }
                
                return null;
            }
        }


        async Task<person> httpPaciente(string pacienteID, List<identifierDomain> dominios)
        {
            if (OPENEMPI_SESSION_KEY == null)
                getAuth();

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
                var response = client.PutAsync(OPENEMPI_URL + "openempi-ws-rest/security-resource/authenticate", body);
                //response.W
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string resultado = response.Result.Content.ReadAsStringAsync().Result;
                    return resultado;
                }
                else
                {
                    throw new Exception("Error autenticacion...");

                }
                //Console.WriteLine(response.StatusCode.ToString());   
            }

        }

        public DataPaciente obtenerPaciente(string paisID, string pacienteID)
        {
            Debug.WriteLine("en obtener pacientes" + paisID + pacienteID);
            string search = paisID + "-" + pacienteID;
            Task<DataPaciente> person = parallelRequestPaciente(search, httpDominios().Result);
            return person.Result;
        }

        public DataPaciente obtenerPacienteParallel(string paisID, string pacienteID)
        {
            string search = paisID + "-" + pacienteID;
            Task<DataPaciente> person = parallelRequestPaciente(search, httpDominios().Result);
            return person.Result;
        }


        public List<identifierDomain> obtenerDominios()
        {
            return httpDominios().Result;
        }

        public void getAuth()
        {
            Task<string> t = getAuthentication();
            OPENEMPI_SESSION_KEY = t.Result;
        }


        #region admin
        public void agregarDominio(identifierDomain dominio)
        {
            if (OPENEMPI_SESSION_KEY == null)
                getAuth();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OPENEMPI_URL);
                client.DefaultRequestHeaders.TryAddWithoutValidation("OPENEMPI_SESSION_KEY", OPENEMPI_SESSION_KEY);

                //var response = client.PutAsync("openempi-ws-rest/person-manager-resource/addIdentifierDomain",);

                //TextReader reader = new StringReader(response.Result.Content.ReadAsStringAsync().Result);
                //XmlSerializer personserializer = new XmlSerializer(typeof(identifierDomain[]), new XmlRootAttribute("identifierDomains"));
                //return ((identifierDomain[])personserializer.Deserialize(reader)).ToList();

            }
        }



        #endregion

    }
}
