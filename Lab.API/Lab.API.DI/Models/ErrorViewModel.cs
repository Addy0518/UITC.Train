namespace Lab.API.DI.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class Order
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string CustomerEmail { get; set; }
    }
}
