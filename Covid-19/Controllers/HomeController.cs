using System;
using System.Collections.Generic;
using Covid_19.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Domain.Common;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Export;
using Domain.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Covid_19.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReportService _reportService;
        private readonly IOptions<DomainOptions> _options;

        public HomeController(ILogger<HomeController> logger, IReportService reportService, IOptions<DomainOptions> options)
        {
            _logger = logger;
            _reportService = reportService;
            _options = options;
        }

        public async Task<IActionResult> Index()
        { 
            var topRegions = await _reportService.GetTopRegions();
            var codeList = topRegions.Select(x => x.Iso).ToList();
            ViewBag.topRegions = topRegions;
            ViewBag.codeList = codeList;
            return View();
        }

        public async Task<IActionResult> Province(string code)
        {
            var topProvinces = await _reportService.GetProvincesByRegion(code);
            ViewBag.topProvinces = topProvinces;
            ViewBag.code = code;
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportData(ExportType type)
        {
            var exportFile = new ExportFileDescriptor();
            var dataToExport = new byte[] { };
            var topRegion = await _reportService.GetTopRegions();
            var listToExport = topRegion.Select(x => new RegionExport
            {
                Region = x.Name,
                Confirmed = x.Confirmed,
                Deaths = x.Deaths
            }).ToList();
            var dataForCsv = listToExport.Select(x => new[] { x.Region, x.Confirmed.ToString(), x.Deaths.ToString() }).ToList<object>(); //Data for csv files
            exportFile.FileName = "Region Report";

            MakeExportFileDescriptor(type, exportFile);
            var fileSaveLocation = _options.Value.ExportPath;
            if (!Directory.Exists(fileSaveLocation))
            {
                Directory.CreateDirectory(fileSaveLocation);
            }

            var newFileFullPath = Path.Combine(fileSaveLocation, exportFile.FileName);
            exportFile.Name = exportFile.FileName;
            exportFile.FileName = newFileFullPath;

            switch (type)
            {
                case ExportType.XML:
                    dataToExport = ExportAsXml(listToExport);
                    break;
                case ExportType.JSON:
                    dataToExport = ExportAsJson(listToExport);
                    break;
                case ExportType.CSV:
                    dataToExport = ExportAsCsv(dataForCsv);
                    break;
            }
            return File(dataToExport, exportFile.FileType, exportFile.FileName);
        }



        [HttpGet]
        public async Task<FileResult> ExportDataProvince(ExportType type, string code)
        {
            var exportFile = new ExportFileDescriptor();
            var dataToExport = new byte[] { };
            var topProvince = await _reportService.GetProvincesByRegion(code);
            var listToExport = topProvince.Select(x => new ProvinceExport
            {
                Province = x.ProvinceName,
                Confirmed = x.Confirmed,
                Deaths = x.Deaths
            }).ToList();
            var dataForCsv = listToExport.Select(x => new[] {x.Province, x.Confirmed.ToString(), x.Deaths.ToString()})
                .ToList<object>(); //Data for csv files
            exportFile.FileName = "Province Report";

            MakeExportFileDescriptor(type, exportFile);
            var fileSaveLocation = _options.Value.ExportPath;
            if (!Directory.Exists(fileSaveLocation))
            {
                Directory.CreateDirectory(fileSaveLocation);
            }

            var newFileFullPath = Path.Combine(fileSaveLocation, exportFile.FileName);
            exportFile.Name = exportFile.FileName;
            exportFile.FileName = newFileFullPath;

            switch (type)
            {
                case ExportType.XML:
                    dataToExport = ExportAsXml(listToExport);
                    break;
                case ExportType.JSON:
                    dataToExport = ExportAsJson(listToExport);
                    break;
                case ExportType.CSV:
                    dataToExport = ExportAsCsv(dataForCsv, code);
                    break;
            }

            return File(dataToExport, exportFile.FileType, exportFile.FileName);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void MakeExportFileDescriptor(ExportType type, ExportFileDescriptor exportFile)
        {
            switch (type)
            {
                case ExportType.XML:
                    exportFile.FileName = $"{ exportFile.FileName}_{Guid.NewGuid()}.xml";
                    exportFile.FileType = "text/xml";
                    break;
                case ExportType.JSON:
                    exportFile.FileName = $"{ exportFile.FileName}_{Guid.NewGuid()}.json";
                    exportFile.FileType = "application/json";
                    break;
                case ExportType.CSV:
                    exportFile.FileName = $"{ exportFile.FileName}_{Guid.NewGuid()}.csv";
                    exportFile.FileType = "text/csv";
                    break;
            }
        }

        private byte[] ExportAsCsv(List<object> data, string code = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                data.Insert(0, new[] { "Region", "Cases", "Deaths" });
            }
            else
            {
                data.Insert(0, new[] { "Province", "Cases", "Deaths" });
            }
            
            var sb = new StringBuilder();
            for (var i = 0; i < data.Count; i++)
            {
                var value = (string[])data[i];
                foreach (var t in value)
                {
                    //Append data with separator.
                    sb.Append(t + ',');
                }

                //Append new line character.
                sb.Append("\r\n");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] ExportAsJson<T>(List<T> data)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data.ToArray()));
        }
        private byte[] ExportAsXml<T>(List<T> data)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            var stringifier = new StringWriter();
            serializer.Serialize(stringifier, data);
            return Encoding.UTF8.GetBytes(stringifier.ToString());
        }
    }
}
