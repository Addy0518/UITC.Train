using Microsoft.AspNetCore.Mvc;

namespace Lab.API.Serilog___Seq
{
    public class MyProblemDetails : ProblemDetails
    {
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        public string? TraceId { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
    }
}
