using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain.Models.CovidApi
{
    public class RegionApi
    {
        [JsonProperty("iso")]
        public string Iso { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("lat")]
        public string Lat { get; set; }

        [JsonProperty("long")]
        public string Long { get; set; }

        [JsonProperty("cities")]
        public List<CityApi> Cities { get; set; }
    }
}
