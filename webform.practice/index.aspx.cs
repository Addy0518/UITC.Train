using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace webform.practice
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{

            //}
            string content = GetJsonContent(
                "https://soa.tainan.gov.tw/Api/Service/Get/285a4c94-f9cf-40c4-bcd2-eb473262893c"
            );

            TestContentAPI data = JsonConvert.DeserializeObject<TestContentAPI>(content);
            message.InnerHtml += $"<p>size: {data.size}</p>";
            foreach (Data item in data.data)
            {
                message.InnerHtml +=
                    $"<tr><td>"
                    + $"Seq: {item.Seq}"
                    + "</td>"
                    + "<td>"
                    + $"本期金額: {item.本期金額}"
                    + "</td>"
                    + "<td>"
                    + $"本期人次: {item.本期人次}"
                    + "</td><tr>";
            }
        }

        private static string GetJsonContent(string Url)
        {
            string targeturl = Url;
            var request = System.Net.WebRequest.Create(targeturl);
            request.ContentType = "application/json; charset=utf-8";

            var response = request.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            return text;
        }
    }
}
