using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Export
{
    public class ProvinceExport
    {
        public string Province { get; set; }

        public int Confirmed { get; set; }

        public int Deaths { get; set; }
    }
}
