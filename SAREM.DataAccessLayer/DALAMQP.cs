using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SAREM.Shared.Datatypes;
using Newtonsoft.Json;
using System.Configuration;
using Newtonsoft.Json.Converters;
using RabbitMQ.Client;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace SAREM.DataAccessLayer
{
    public class DALAMQP : IDALAMQP
    {
        private string tenant;
        public static string baseURL = ConfigurationManager.AppSettings["AMQP_URL"].ToString();

        public DALAMQP(string tenant)
        {
            this.tenant = tenant;
        }

        public void sendToQueue(DataMensaje m)
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = "amqp://zquztoqc:OKzBDVlGU6H3xQ12OpTEP8OaEysrW0r4@black-boar.rmq.cloudamqp.com/zquztoqc";
            IConnection conn = factory.CreateConnection();
            Console.WriteLine("Connection Created...");

            IModel model = conn.CreateModel();
            var serialized = JsonConvert.SerializeObject(m);
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(serialized);
            IBasicProperties props = model.CreateBasicProperties();
            props.ContentType = "application/json";
            //props.Expiration = "5000";
            //props.DeliveryMode = 2;

            /*Delayed queue*/
            string queue = "sarem.wait";
            if (m.inmediato)
                queue = "sarem";
            else
            {
                long milis = (long)(m.fecha_envio - DateTime.UtcNow).TotalMilliseconds;
                Console.WriteLine("Tiempo en milis" + milis.ToString());
                props.Expiration = milis.ToString();//"5000";
            }



            Console.WriteLine("Sending...");
            model.BasicPublish("",
                              queue, props,
                              messageBodyBytes);
            Console.WriteLine("Message send..");
            model.Close(200, "Goodbye");
            conn.Close();
            
        }

        public void deleteMessage(string id)
        {
            using (var client = new HttpClient())
            {
                string queue = "test";
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "OAuth eqGWnzSwFdkkjORHj0YPfD1AMhY");
                string url = baseURL + "queues/" + queue + "/messages/"+"6206749646718516612";
                var result = client.DeleteAsync(url);
                Console.WriteLine(result.Result.Content.ReadAsStringAsync().Result);
            }
        }

        public ICollection<string> getQueues(string AMQUSER, string AMQPASSWORD)
        {
            string url = baseURL + "api/queues";
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(AMQUSER + ":" + AMQPASSWORD);
                var header = new AuthenticationHeaderValue(
                           "Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Authorization = header;
                var result = client.GetAsync(url);
                string str = result.Result.Content.ReadAsStringAsync().Result;

                List<string> final = new List<string>();
                JArray list = JArray.Parse(str);
                foreach (JObject o in list.Children<JObject>())
                {
                    final.Add((string)o["name"]);
                }
                return final;
            }
        }

    }
}



/*IRONMQ OLD*/
/*
            using (var client = new HttpClient())
            {
                string queue = "sarem";

                
                //var m = new DataMensaje { medio = "mail", destinatario = "mail@mail.com", asunto = "test", mensaje = "hello world!" };
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "OAuth eqGWnzSwFdkkjORHj0YPfD1AMhY");
                JsonSerializerSettings options = new JsonSerializerSettings
                                {
                                  DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                                };
                IsoDateTimeConverter format = new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'" };
                options.Converters.Add(format);

                string json = JsonConvert.SerializeObject(m, options);
                Console.WriteLine(json);
                var mensajes = new[] { new { body = json } };
                var info = new { messages = mensajes };
                //string url = baseURL + "queues/" + queue + "/messages";
                string url = @"http://10.0.2.2:5000/sarem";
                var result = client.PostAsJsonAsync(url, json); //regresar a INFO
                string jsonr = result.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(jsonr);
                var o = JsonConvert.DeserializeObject<dynamic>(jsonr);
                Console.WriteLine((string)o.msg);
                Console.WriteLine((string)o.ids[0]);
            }*/
