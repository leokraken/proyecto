using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer.utils
{
    public class Deserializer
    {
        public static string BASE_URL = @"http://10.0.2.2:3000/";
        public static Boolean DEBUG = true;

        public Deserializer(string tenant)
        {
            BASE_URL = BASE_URL + tenant;
        }

        public T get<T>(string url)
        {
            url = BASE_URL + url;
            if(DEBUG) Console.WriteLine(url);
            using (var client = new HttpClient())
            {
                var result = client.GetAsync(url);
                string str = result.Result.Content.ReadAsStringAsync().Result;
                if(DEBUG) Console.WriteLine(str);
                var o = JsonConvert.DeserializeObject<T>(str);
                return o;
            }
        }

        public R post<T, R>(string url, object o)
        {
            url = BASE_URL + url;
            if (DEBUG) Console.WriteLine(url);
           
            using (var client = new HttpClient())
            {
                var result = client.PutAsJsonAsync(url,o);
                string str = result.Result.Content.ReadAsStringAsync().Result;
                if (DEBUG) Console.WriteLine(str);
                var obj = JsonConvert.DeserializeObject<R>(str);
                return obj;
            }
        }

        public void post<T>(string url, T o)
        {
            url = BASE_URL + url;
            if (DEBUG) Console.WriteLine(url);
            using (var client = new HttpClient())
            {
                var result = client.PostAsJsonAsync(url, o);
                string str = result.Result.Content.ReadAsStringAsync().Result;
                if (DEBUG) Console.WriteLine(str);
                if (DEBUG) Console.WriteLine(result.Result.StatusCode);
            }
        }

        public void put<T>(string url, T o)
        {
            using (var client = new HttpClient())
            {
                var result = client.PutAsJsonAsync(url, o);
                string str = result.Result.Content.ReadAsStringAsync().Result;
                if (DEBUG) Console.WriteLine(str);
            }
        }

        public void delete(string url)
        {
            /*
            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = new StringContent("[YOUR JSON GOES ", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri("[YOUR URL GOES HERE]")
            };*/
            url = BASE_URL + url;
            if (DEBUG) Console.WriteLine(url);
           
            using (var client = new HttpClient())
            {
                var result = client.DeleteAsync(url);
                string str = result.Result.Content.ReadAsStringAsync().Result;
                if (DEBUG) Console.WriteLine(str);
            }
        }

    }
}
