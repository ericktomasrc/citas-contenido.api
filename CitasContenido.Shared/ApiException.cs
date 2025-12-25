namespace Smr.Backend.Shared
{
    public class ApiException : Exception
    {
        public string Url { get; }
        public string Header { get; }
        public string RequestBody { get; }
        public string ResponseBody { get; }

        public ApiException(string message, string url, string header, string requestBody, string responseBody)
            : base(message)
        {
            Url = url;
            Header = header;
            RequestBody = requestBody;
            ResponseBody = responseBody;
        }

        public override string ToString()
        {
            string aditionalMessage = string.Format("Url: {0}, Header: {1}, RequestBody: {2}, ResponseBody: {3}", Url, Header, RequestBody, ResponseBody);
            return aditionalMessage + base.ToString();
        }
    }
}
