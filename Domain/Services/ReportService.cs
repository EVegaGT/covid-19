using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using Domain.Common;
using Domain.Enums;
using Domain.Helper;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.CovidApi;

namespace Domain.Services
{
    public class ReportService : IReportService
    {
        private readonly IOptions<DomainOptions> _options;
        private readonly IServiceThirdPartyApi _serviceThirdPartyApi;

        public ReportService(IOptions<DomainOptions> options, IServiceThirdPartyApi serviceThirdPartyApi)
        {
            _options = options;
            _serviceThirdPartyApi = serviceThirdPartyApi;
        }

        public async Task<List<Province>> GetProvincesByRegion(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new Exception(ErrorCodeEnum.RegionCodeShouldBeRequired.ToString());
            }

            var extraHeaders = GetHeaders();
            var apiResponse = await _serviceThirdPartyApi.SendToThirdPartyApi(HttpVerbs.Get, _options.Value.CovidApiUrl,
                $"reports?iso={code.ToUpper()}", typeof(object), extraHeaders);

            if (!apiResponse.IsSuccessStatusCode)
            {
                throw new Exception(apiResponse.ReasonPhrase);
            }

            var reportData = apiResponse.Content.ReadAsAsync<ReportData>().Result;
            var topTenProvinces = ReturnTopTenProvinces(reportData);
            return topTenProvinces;
        }

        public async Task<List<Region>> GetTopRegions()
        {

            var extraHeaders = GetHeaders();
            var apiResponse = await _serviceThirdPartyApi.SendToThirdPartyApi(HttpVerbs.Get, _options.Value.CovidApiUrl,
                "reports", typeof(object), extraHeaders);

            if (!apiResponse.IsSuccessStatusCode)
            {
                throw new Exception(apiResponse.ReasonPhrase);
            }

            var reportData = apiResponse.Content.ReadAsAsync<ReportData>().Result;
            var topTenRegions = ReturnTopTenRegions(reportData);
            return topTenRegions;
        }

        private List<Region> ReturnTopTenRegions(ReportData reportData)
        {
            // We get all data to handle it in memory, since the api doesn't allow us to order or use a pagination.
            return reportData.Data.GroupBy(x => x.Region.Iso).Select(group =>
                    new Region
                    {
                        Iso = group.Key,
                        Name = group.First().Region.Name,
                        Confirmed = group.Sum(x => x.Confirmed),
                        Deaths = group.Sum(x => x.Deaths)
                    }
                ).OrderByDescending(x => x.Confirmed).Take(10).ToList();
        }

        private List<Province> ReturnTopTenProvinces(ReportData reportData)
        {
            // We get all data to handle it in memory, since the api doesn't allow us to order or use a pagination.
            return reportData.Data.Select(x =>
                new Province
                {
                    Iso = x.Region.Iso,
                    Name = x.Region.Name,
                    ProvinceName = !string.IsNullOrEmpty(x.Region.Province) ? x.Region.Province : x.Region.Name,
                    Confirmed = x.Confirmed,
                    Deaths = x.Deaths
                }
            ).OrderByDescending(x => x.Confirmed).Take(10).ToList();
        }

        private Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>
            {
                {"x-rapidapi-key", _options.Value.XRapidapiKkey},
                {"x-rapidapi-host", _options.Value.XRapidAPIHost}
            };
        }

    }
}
