using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AppiumSample.helpers
{
    public class RESTClient
    {

        public void Get(string baseAddress, string path)
        {
            try
            {
                string url = Path.Combine(baseAddress, path);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.GetAsync(url);
            }
            catch (WebException exp)
            {
                throw exp;
            }
            catch (Exception exp)
            {
                throw new Exception("Error occurred during server Get request", exp);
            }
        }

        public T Get<T>(string baseAddress, string path)
        {
            try
            {
                string url = Path.Combine(baseAddress, path);

                WebClient client = GetWebClient();
                string responseJson = client.DownloadString(url);
                T response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseJson);
                return response;
            }
            catch (WebException exp)
            {
                throw exp;
            }
            catch (Exception exp)
            {
                throw new Exception("Error occurred during server Get request", exp);
            }
        }

        public string Get(string url)
        {
            try
            {
                WebClient client = GetWebClient();
                string responseJson = client.DownloadString(url);
                return responseJson;
            }
            catch (WebException exp)
            {
                throw exp;
            }
            catch (Exception exp)
            {
                throw new Exception("Error occurred during server Get request", exp);
            }
        }

        public T Get<T>(string url)
        {
            try
            {
                WebClient client = GetWebClient();
                string responseJson = client.DownloadString(url);
                T response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseJson);
                return response;
            }
            catch (WebException exp)
            {
                throw exp;
            }
            catch (Exception exp)
            {
                throw new Exception("Error occurred during server Get request", exp);
            }
        }

        public string Post(string baseAddress, string path)
        {
            return Post(baseAddress, path, "");
        }

        public string Post(string baseAddress, string path, string requestJson)
        {
            try
            {
                string url = Path.Combine(baseAddress, path);

                WebClient client = GetWebClient();
                return client.UploadString(url, "POST", requestJson);
            }
            catch (WebException exp)
            {
                throw exp;
            }
            catch (Exception exp)
            {
                throw new Exception("Error occurred during server Post request", exp);
            }
        }

        public string PostUrl(string url, string requestJson)
        {
            try
            {
                WebClient client = GetWebClient();
                return client.UploadString(url, "POST", requestJson);
            }
            catch (WebException exp)
            {
                throw exp;
            }
            catch (Exception exp)
            {
                throw new Exception("Error occurred during server Post request", exp);
            }
        }

        public string Post<T>(string baseAddress, string path, string request)
        {
            try
            {
                string url = Path.Combine(baseAddress, path);

                WebClient client = GetWebClient();
                client.Headers.Add("content-type", "application/json");
                client.Headers.Add("accept", "*/*");
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.89 Safari/537.36");
                client.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                client.Headers.Add("Accept-Language", "en-US,en;q=0.8,en-GB;q=0.6,it;q=0.4");

                //string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                string responseJson = client.UploadString(url, "POST", request);
                return responseJson;
            }
            catch (WebException exp)
            {
                throw exp;
            }
            catch (Exception exp)
            {
                throw new Exception("Error occurred during server Post request", exp);
            }
        }
        

        public string PostFormAsync(string url, FormUrlEncodedContent content)
        {
            //HttpResponseMessage response = new HttpResponseMessage();
            HttpClient client = new HttpClient();
            var result = client.PostAsync(url, content).Result;
            return result.Content.ReadAsStringAsync().Result;
        }

       

        public void DeleteAsync(string baseAddress, string path)
        {
            string url = Path.Combine(baseAddress, path);

            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DeleteAsync(url);
        }

        public T Post<T, T2>(string baseAddress, string path, T2 request)
        {
            try
            {
                string url = Path.Combine(baseAddress, path);

                WebClient client = GetWebClient();
                string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                string responseJson = client.UploadString(url, "POST", requestJson);
                T response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseJson);
                return response;
            }
            catch (WebException exp)
            {
                throw exp;
            }
            catch (Exception exp)
            {
                throw new Exception("Error occurred during server Post request", exp);
            }
        }

        private WebClient GetWebClient()
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.Headers.Add("content-type", "application/json");
            return client;
        }
    }
}
