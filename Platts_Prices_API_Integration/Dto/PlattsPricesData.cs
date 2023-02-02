using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Danaos.TRD.App.Dto
{
    public class Datum
    {
        public string bate { get; set; }
        public DateTime assessDate { get; set; }
        public double value { get; set; }
        public string isCorrected { get; set; }
        public DateTime modDate { get; set; }
    }

    public class Metadata
    {
        public int count { get; set; }
        public int pageSize { get; set; }
        public int page { get; set; }
        public int totalPages { get; set; }
        public string queryTime { get; set; }
    }

    public class Result
    {
        public string symbol { get; set; }
        public List<Datum> data { get; set; }
    }

    public class PlattsPricesData
    {
        public Metadata metadata { get; set; }
        public List<Result> results { get; set; }
    }

}