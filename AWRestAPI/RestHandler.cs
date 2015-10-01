using System;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Threading.Tasks;

using RestSharp;

namespace RestAPIApp
{
    public class RestHandler
    {
        private string m_baseURL;
        public RestHandler(string baseURL)
        {
            m_baseURL = baseURL;
        }

        public IRestResponse RestGetEndpoint(string endpoint, NameValueCollection headers)
        {
            IRestResponse callResponse = handleGetRequest(endpoint, headers);
            return callResponse;
        }

        public IRestResponse RestGetEndpointBasicAuth(string endpoint, string username, string password, NameValueCollection headers)
        {
            string basicAuthString = GetBasicAuthentication(username, password);
            headers.Add("Authorization", basicAuthString);
            IRestResponse callResponse = handleGetRequest(endpoint, headers);

            return callResponse;
        }

        public IRestResponse RestPostEndpointBasicAuth(string endpoint, string username, string password, NameValueCollection headers, string jsonToPost)
        {
            string basicAuthString = GetBasicAuthentication(username, password);
            headers.Add("Authorization", basicAuthString);
            headers.Add("Content-Type", "application/json");

            IRestResponse response = handlePostRequest(endpoint, headers, jsonToPost);
            return response;
            
        }

        public IRestResponse vIDMRestGetWithEndpoint(string endpoint, NameValueCollection headers)
        {
            IRestResponse restResponse = handleGetRequest(endpoint, headers);
            return restResponse;
        }

        
        public IRestResponse vIDMRestPostWithEndpoint(string endpoint, NameValueCollection headers, string jsonToSend)
        {
            IRestResponse restResponse = handlePostRequest(endpoint, headers, jsonToSend);
            return restResponse;
        }
        
        private IRestResponse handleGetRequest(string endpoint, NameValueCollection headers)
        {
            var restClient = new RestClient();
            restClient.BaseUrl = new Uri(m_baseURL);

            var restRequest = new RestRequest();
            restRequest.Resource = endpoint;
            restRequest.Method = Method.GET;

            foreach (string key in headers.AllKeys)
            {
                restRequest.AddHeader(key, headers[key]);
            }

            IRestResponse restResponse = restClient.Execute(restRequest);

            return restResponse;
        }

        private IRestResponse handlePostRequest(string endpoint, NameValueCollection headers, string jsonToSend)
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string jsonText = Path.Combine(documentsFolder, "json.txt");
            StreamWriter sw = new StreamWriter(jsonText);
            sw.Write(jsonToSend);
            sw.Close();

            var restClient = new RestClient();
            restClient.BaseUrl = new Uri(m_baseURL);

            var restRequest = new RestRequest();
            restRequest.Resource = endpoint;
            restRequest.Method = Method.POST;
            restRequest.AddParameter("application/json", jsonToSend, ParameterType.RequestBody);
            
            foreach (string key in headers.AllKeys)
            {
                restRequest.AddHeader(key, headers[key]);
            }

            IRestResponse restResponse = restClient.Execute(restRequest);
            return restResponse;
        }

        private static string GetBasicAuthentication(string username, string password)
        {
            string stringToConvert = username + ":" + password;
            byte[] stringAsByteArray = Encoding.ASCII.GetBytes(stringToConvert);
            string stringToReturn = Convert.ToBase64String(stringAsByteArray);

            return "Basic " + stringToReturn;
        }
    }
}
