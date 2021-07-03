using Domain.Common;
using Domain.Helper;
using Domain.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace Covid.Unit.Test.Base
{
    public class MockBase
    {
        public IOptions<DomainOptions> Options { get; }

        public readonly Mock<IReportService> MockReportService;
        public readonly Mock<IServiceThirdPartyApi> MockServiceThirdPartyApi;

        public MockBase()
        {
            MockReportService = new Mock<IReportService>();
            MockServiceThirdPartyApi = new Mock<IServiceThirdPartyApi>();
            Options = new OptionsWrapper<DomainOptions>(new DomainOptions
            {
                CovidApiUrl = "https://dev-eventapi.frontstream.com/api/",
                XRapidAPIHost = "bb2d78be04msh639052c3e53e1f3p1ea481jsndf9d588bc58b",
                XRapidapiKkey = "covid-19-statistics.p.rapidapi.com"
            });
        }
    }
}
