using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace webform.practice
{
    public class TestContentAPI
    {
        public string contentType { get; set; }

        public bool isImage { get; set; }

        public int size { get; set; }

        public Data[] data { get; set; }
    }

    public class Data
    {
        public int Seq { get; set; }

        public string 項目別 { get; set; }

        [JsonProperty("本年累計至當期底人數[計]")]
        public string 本年累計至當期底人數_計 { get; set; }

        [JsonProperty("本年累計至當期底人數[男]")]
        public string 本年累計至當期底人數_男 { get; set; }

        [JsonProperty("本年累計至當期底人數[女]")]
        public string 本年累計至當期底人數_女 { get; set; }

        [JsonProperty("期底人數[計]")]
        public string 期底人數_計 { get; set; }

        [JsonProperty("期底人數[男]")]
        public string 期底人數_男 { get; set; }

        [JsonProperty("期底人數[女]")]
        public string 期底人數_女 { get; set; }

        public string 本期人次 { get; set; }

        public string 本期金額 { get; set; }
    }
}
