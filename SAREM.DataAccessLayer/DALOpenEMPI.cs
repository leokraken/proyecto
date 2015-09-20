using System;
using System.Collections.Generic;
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
                client.BaseAddress = new Uri("http://10.0.2.2:8081/openempi-webapp-web-2.2.9/");
                client.DefaultRequestHeaders.TryAddWithoutValidation("OPENEMPI_SESSION_KEY", "87E20D1B1D2FF95A9CB263005E42C315");

                var response = await client.GetAsync("openempi-ws-rest/person-query-resource/getIdentifierDomains");

                TextReader reader = new StringReader(response.Content.ReadAsStringAsync().Result);
                XmlSerializer personserializer = new XmlSerializer(typeof(identifierDomain[]), new XmlRootAttribute("identifierDomains"));
                return ((identifierDomain[])personserializer.Deserialize(reader)).ToList();
            }
        }

        async Task<person> httpPaciente(string pacienteID, List<identifierDomain> dominios)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://10.0.2.2:8081/openempi-webapp-web-2.2.9/");
                client.DefaultRequestHeaders.TryAddWithoutValidation("OPENEMPI_SESSION_KEY", "87E20D1B1D2FF95A9CB263005E42C315");
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

        public person obtenerPaciente(string paisID, string pacienteID)
        {
            string search = paisID + "-" + pacienteID;
            Task<person> person = httpPaciente(search, httpDominios().Result);
            return person.Result;
        }

        public List<identifierDomain> obtenerDominios()
        {
            return httpDominios().Result;
        }
    }
}
