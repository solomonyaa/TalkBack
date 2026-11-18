using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Talkback.Common
{
    public class HttpHandler
    {
        private HttpClient httpClient;

        public HttpHandler()
        {
            this.httpClient = new HttpClient();
        }

        public T HttpGet<T>(string url, string path, Dictionary<string, string> reqParams = null, string token = "")
        {
            var urlParams = string.Empty;
            if (reqParams != null)
                urlParams = "?" + string.Join("&", reqParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));


            var request = new HttpRequestMessage(HttpMethod.Get, url + '/' + path + urlParams);
            if (token.Length > 0)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res= httpClient.SendAsync(request).Result;
            if (!res.IsSuccessStatusCode)
                return default(T);
            var s = res.Content.ReadAsStringAsync().Result;
            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
            return result;
        }
        public HttpResponseMessage MakeHttpGet(string url, string path,Dictionary<string,string> reqParams=null, string token = "")
        {
            if (url == "")
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            var urlParams = string.Empty;
            if (reqParams !=null)
                urlParams = "?"+string.Join("&", reqParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            

            var request = new HttpRequestMessage(HttpMethod.Get, url + '/' + path + urlParams);
            if (token.Length > 0)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return httpClient.SendAsync(request).Result;

        }
        public bool MakeHttpGetAll(IEnumerable<string> urls, string path, Dictionary<string, string> reqParams = null, string token = "")
        {
            bool success = true;
            foreach (var url in urls)
            {
                success = success && MakeHttpGet(url, path,null, token).IsSuccessStatusCode;
            }
            return success;
        }
        public T HttpPost<T>(string url, string path, Dictionary< string, string> reqParams = null, string message = null, string token = "")
        {
            var urlParams = string.Empty;
                if (reqParams != null)
                    urlParams = "?" + string.Join("&", reqParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));


                var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"message", message }
            });
                var request = new HttpRequestMessage(HttpMethod.Get, url + '/' + path + urlParams);
                request.Content = content;
                if (token.Length > 0)
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = httpClient.SendAsync(request).Result;
            if (!res.IsSuccessStatusCode)
                return default(T);

            var s = res.Content.ReadAsStringAsync().Result;
            T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
            return result;
        }
        public HttpResponseMessage MakeHttpPost(string url, string path, Dictionary<string, string> reqParams = null, string message = null, string token="")
        {
            var urlParams = string.Empty;
            if (reqParams != null)
                urlParams = "?" + string.Join("&", reqParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));


            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"message", message }
            });
            var request = new HttpRequestMessage(HttpMethod.Post, url + '/' + path + urlParams);
            request.Content = content;
            if (token.Length > 0)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return httpClient.SendAsync(request).Result;
        }
    }
}
