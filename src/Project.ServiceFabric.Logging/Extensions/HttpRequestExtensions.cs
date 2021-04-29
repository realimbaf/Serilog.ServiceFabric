using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Project.Common.Extensions;
using Project.ServiceFabric.Logging.Abstractions.Constants;

namespace Project.ServiceFabric.Logging.Extensions
{
    internal static class HttpRequestExtensions
    {
        public static async Task<string> ReadBodyAsStringAsync(this HttpRequest request, RecyclableMemoryStreamManager streamManager = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                string requestBodyAsText = "";

                if (!HttpMethods.IsGet(request.Method)
                    && !HttpMethods.IsHead(request.Method)
                    && !HttpMethods.IsDelete(request.Method)
                    && !HttpMethods.IsTrace(request.Method)
                    && request.ContentLength > 0
                    && request.ContentType == ContentTypes.Json
                    )
                {
                    requestBodyAsText = await BodyToString(request.Body, streamManager);
                }
                return requestBodyAsText?.Truncate(255);
            }
            catch(Exception ex)
            {
                return $"Unable to read request due to {ex.GetMessage()}";
            }
        }
        public static async Task<string> ReadBodyAsStringAsync(this HttpResponse response, RecyclableMemoryStreamManager streamManager = null)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            try
            {
                string responseBodyAsText = "";

                if (response.ContentLength > 0)
                {
                    responseBodyAsText = await BodyToString(response.Body, streamManager);
                }
                return responseBodyAsText?.Truncate(255);
            }
            catch(Exception ex)
            {
                return $"Unable to read response due to {ex.GetMessage()}";
            }
        }
        public static string ReadHeadersAsString(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return HeadersToString(request.Headers);
        }
        public static string ReadHeadersAsString(this HttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return HeadersToString(response.Headers);
        }

        private static string HeadersToString(IHeaderDictionary headerDictionary)
        {
            if (headerDictionary == null || !headerDictionary.Any())
            {
                return String.Empty;
            }

            var headers = new StringBuilder();
            foreach (string key in headerDictionary.Keys)
            {
                string value = headerDictionary[key];
                if (!String.IsNullOrEmpty(value))
                {
                    headers.AppendLine($"{key}={value}");
                }
            }
            return headers.ToString();
        }

        private static async Task<string> BodyToString(Stream inputStream, RecyclableMemoryStreamManager outputStreamManager = null)
        {
            if (!inputStream.CanRead || !inputStream.CanSeek)
            {
                return $"Unable to read body due to invalid stream state: CanRead={inputStream.CanRead}, CanSeek={inputStream.CanSeek}";
            }

            inputStream.Seek(0, SeekOrigin.Begin);

            using MemoryStream tempStream = outputStreamManager?.GetStream() ?? new MemoryStream();
            using var reader = new StreamReader(tempStream);

            await inputStream.CopyToAsync(tempStream);
            tempStream.Seek(0, SeekOrigin.Begin);
            inputStream.Seek(0, SeekOrigin.Begin);

            return await reader.ReadToEndAsync();
        }
    }
}
