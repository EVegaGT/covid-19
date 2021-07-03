using Domain.Helper;
using Domain.Services;
using StructureMap;

namespace Domain.Common
{
    public class DomainRegistry : Registry
    {
        public DomainRegistry()
        {
            // Services
            For<IReportService>().Use<ReportService>();
            For<IServiceThirdPartyApi>().Use<ServiceThirdPartyApi>();
        }
    }
}
