using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Domain.Enums;
using Newtonsoft.Json;

namespace Domain.Helper
{
    public class ServiceThirdPartyApi : IServiceThirdPartyApi
    {
        public async Task<HttpResponseMessage> SendToThirdPartyApi<T>(HttpVerbs httpType, string apiUrl,
           string endpointSegment, T payload, Dictionary<string, string> extraHeaders = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (extraHeaders != null && extraHeaders.Count > 0)
                {
                    foreach (var extraHeader in extraHeaders)
                    {
                        client.DefaultRequestHeaders.Add(extraHeader.Key, extraHeader.Value);
                    }
                }

                var serialized = JsonConvert.SerializeObject(payload);
                var content = new StringContent(serialized, Encoding.UTF8, "application/json");
                HttpResponseMessage apiResponse = null;

                try
                {
                    switch (httpType)
                    {
                        case HttpVerbs.Get:
                            apiResponse = await client.GetAsync(endpointSegment);
                            break;
                        case HttpVerbs.Post:
                            apiResponse = await client.PostAsync(endpointSegment, content);
                            break;
                        case HttpVerbs.Put:
                            apiResponse = await client.PutAsync(endpointSegment, content);
                            break;
                        case HttpVerbs.Delete:
                            apiResponse = await client.DeleteAsync(endpointSegment);
                            break;
                    }
                }
                catch (HttpRequestException)
                {
                    throw new Exception(ErrorCodeEnum.FailedHttpRequestToService.ToString());
                }
                catch (Exception)
                {
                    throw new Exception(ErrorCodeEnum.CouldNotConnectThirdPartyService.ToString());
                }
               
                return apiResponse;
            }
        }
    }
}
