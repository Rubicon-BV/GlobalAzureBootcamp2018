using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleEchoBot.Models;

namespace SimpleEchoBot.Dynamics
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class DynamicsHelper<T>
    {
        private static HttpClient httpClient;
        private static readonly HttpMethod HttpPostMethod = new HttpMethod("POST");

        public static HttpClient HttpClient
        {
            get
            {
                if (httpClient == null)
                {
                    ConnectToCRM();
                }

                return httpClient;
            }
        }

        private static void ConnectToCRM()
        {
            Config config = new Config();

            //Create a helper object to authenticate the user with this connection info.
            Authentication auth = new Authentication(config);

            //Next use a HttpClient object to connect to specified CRM Web service.
            httpClient = new HttpClient(auth.ClientHandler, true);

            //Define the Web API base address, the max period of execute time, the 
            // default OData version, and the default response payload format.
            httpClient.BaseAddress = new Uri(config.WebAPIUrl);
            httpClient.Timeout = new TimeSpan(0, 2, 0);
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<T> GetFromCrm(string query)
        {
            var response = await HttpClient.GetAsync(query, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var repsonseObject = JsonConvert.DeserializeObject<T>(responseString);
            return repsonseObject;
        }

        public static async Task<IEnumerable<T>> GetFromCrmFetchXml(string query, string entityName)
        {
            var encodeXml = Uri.EscapeDataString(query);
            var requestContactsUri = $"{entityName}?fetchXml={encodeXml}";
            var response = await HttpClient.GetAsync(requestContactsUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var wrapper = JsonConvert.DeserializeObject<Entities.EntitiesWrapper<T>>(responseString);
            return wrapper.Entities;
        }

        public static async Task<HttpResponseMessage> WriteToCrm(string content, string url)
        {
            var request = new HttpRequestMessage(HttpPostMethod, url)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            return await HttpClient.SendAsync(request);
        }
    }
}