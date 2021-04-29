namespace Project.ServiceFabric.Logging.Abstractions.Models
{
    public class HttpRequestOption
    {
        public string Method { get; set; }
        public string Query { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }

        public string Headers { get; set; }
        public string Body { get; set; }

    }
}
