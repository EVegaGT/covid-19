using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Domain.Helper
{
    public interface IServiceThirdPartyApi
    {
        Task<HttpResponseMessage> SendToThirdPartyApi<T>(HttpVerbs httpType, string apiUrl,
            string endpointSegment, T payload, Dictionary<string, string> extraHeaders = null);
    }
}
