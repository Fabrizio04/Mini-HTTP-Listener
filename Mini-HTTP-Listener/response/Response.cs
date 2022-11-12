using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mini_HTTP_Listener.configuration;
using System.Net;
using System.Text;

namespace Mini_HTTP_Listener.response
{
    internal partial class Response
    {
        private readonly ILogger<Response> logger;
        private readonly IOptions<Mini_HTTP_Listener_Configuration> options;
        private readonly Mini_HTTP_Listener_StaticPath staticPath;

        public Response() { } // enable Response-Custom for Invoke

        public Response(ILogger<Response> _logger, IOptions<Mini_HTTP_Listener_Configuration> _options, Mini_HTTP_Listener_StaticPath _staticPath)
        {
            logger = _logger;
            options = _options;
            staticPath = _staticPath;
        }


        public void SetResponse(object o)
        {
            var context = o as HttpListenerContext;
            var req = context.Request;

            string HTTP_Method = req.HttpMethod;
            var queryString = req.QueryString;
            string path = "";

            if (options.Value.port_number == "80" || options.Value.port_number == "443")
            {
                var host = req.Url.ToString().Split("//")[1].Split("/")[0];

                if(options.Value.secure == "true")
                    path = req.Url.ToString().Split($"https://{host}")[1].Split("?")[0].ToLower();

                if (options.Value.secure == "false")
                    path = req.Url.ToString().Split($"http://{host}")[1].Split("?")[0].ToLower();
            }
            else
                path = req.Url.ToString().Split(options.Value.port_number)[1].Split("?")[0].ToLower();

            string filename = context.Request.Url.AbsolutePath;
            bool inputFile = false;
            bool auth = true;

            logger.LogInformation($"{HTTP_Method} - {req.Url}");

            //string inputPost = "";
            //using (var reader = new StreamReader(req.InputStream, req.ContentEncoding)) inputPost = reader.ReadToEnd();
            //string bodyLog = inputPost != "" ? " - " + inputPost : "";
            // Console.WriteLine(inputPost);

            string responseData = "";
            HttpListenerResponse resp = context.Response;

            resp.Headers["Server"] = "Mini-HTTP-Listener by Fabrizio Amorelli based on ";

            //Global Authentication
            if (options.Value.authentication_schemes != "" && options.Value.username != "" && options.Value.password != "")
                if (!Authentication(options.Value.authentication_schemes, req.Headers, ref resp, ref responseData, HTTP_Method, options.Value.username, options.Value.password))
                    return;

            // Get the response
            switch(HTTP_Method)
            {
                case "GET":
                    GetResponse(req, req.Headers, ref resp, req.HasEntityBody, req.InputStream, req.ContentEncoding, path, filename, queryString, ref responseData, ref inputFile, ref auth);
                    break;

                case "POST":
                    PostResponse(req, req.Headers, ref resp, req.HasEntityBody, req.InputStream, req.ContentEncoding, path, filename, queryString, ref responseData, ref inputFile, ref auth);
                    break;

                case "PUT":
                    PutResponse(req, req.Headers, ref resp, req.HasEntityBody, req.InputStream, req.ContentEncoding, path, filename, queryString, ref responseData, ref inputFile, ref auth);
                    break;

                case "DELETE":
                    DeleteResponse(req, req.Headers, ref resp, req.HasEntityBody, req.InputStream, req.ContentEncoding, path, filename, queryString, ref responseData, ref inputFile, ref auth);
                    break;

                case "OPTIONS":
                    OptionsResponse(req, req.Headers, ref resp, req.HasEntityBody, req.InputStream, req.ContentEncoding, path, filename, queryString, ref responseData, ref inputFile, ref auth);
                    break;

                    //implements here other HTTP Methods
            }

            // Other Authentication check
            if (!auth) return;

            // Final Closing
            try
            {
                if (inputFile)
                {
                    resp.OutputStream.Close();
                }
                else
                {

                    byte[] buffer = Encoding.UTF8.GetBytes(responseData);
                    resp.ContentLength64 = buffer.Length;

                    using Stream ros = resp.OutputStream;
                    ros.Write(buffer, 0, buffer.Length);
                    ros.Close();
                }
            }
            catch {}

            //using (StreamWriter writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
            //writer.WriteLine("File Uploaded");
        }

    }
}
