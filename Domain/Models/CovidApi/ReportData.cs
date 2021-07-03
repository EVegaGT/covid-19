using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Models.CovidApi
{
    public class ReportData
    {
        [JsonProperty("data")]
        public List<ReportApi> Data { get; set; }
        
    }
}
