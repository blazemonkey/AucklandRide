using AucklandRide.Updater.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Services.RestService
{
    public class RestService : IRestService
    {
        private string _url = "https://api.at.govt.nz/v2/gtfs/";
        private string _subscriptionKey = "633dea42ee4c4a46a7ff49d70921664d";

        private RestClient GetRestClient()
        {
            var restClient = new RestClient(_url);
            restClient.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            restClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", _subscriptionKey);
            return restClient;
        }

        private List<T> ParseResponse<T>(IRestResponse<RestWrapper<T>> response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            if (response.ErrorException != null && response.ErrorException.GetType() == typeof(WebException))
                throw new Exception(response.ErrorException.Message);
            if (string.IsNullOrWhiteSpace(response.ErrorMessage) == false)
                throw new Exception(response.ErrorMessage);

            var result = JsonConvert.DeserializeObject<RestWrapper<T>>(response.Content);
            if (!string.IsNullOrEmpty(result.Error) || result.Status != "OK")
                throw new Exception(result.Error);

            return result.Response;
        }

        public async virtual Task<IRestResponse<RestWrapper<T>>> ExecuteRequest<T>(RestClient client, RestRequest request)
        {
            return await client.ExecuteTaskAsync<RestWrapper<T>>(request);
        }

        public async Task<IEnumerable<Models.Version>> GetVersions()
        {
            var client = GetRestClient();
            var request = new RestRequest("versions", Method.GET);
            var response = await ExecuteRequest<Models.Version>(client, request);
            var result = ParseResponse(response);
            return result;
        }
    }
}
