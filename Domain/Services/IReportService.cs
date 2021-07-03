using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services
{
    public interface IReportService
    {
        Task<List<Province>> GetProvincesByRegion(string code);
        Task<List<Region>> GetTopRegions();
    }
}
