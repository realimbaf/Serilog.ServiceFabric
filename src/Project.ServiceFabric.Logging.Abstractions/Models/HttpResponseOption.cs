namespace Project.ServiceFabric.Logging.Abstractions.Models
{
    public class HttpResponseOption
    {
        public string Headers { get; set; }
        public string Body { get; set; }
        public int StatusCode { get; set; }

        public string ErrorResponse { get; set; }
    }
}
