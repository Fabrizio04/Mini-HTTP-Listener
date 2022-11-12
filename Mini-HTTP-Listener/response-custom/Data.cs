using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mini_HTTP_Listener.configuration;
using Mini_HTTP_Listener.response;
using NLog.Extensions.Logging;

namespace Mini_HTTP_Listener.response_custom
{
    /*
    
    You can also create you custom class, with all your personal methods.
    To Add some custom Path, simply create a Dictionary<string, object> method and add CustomPath attribute (as the Response_Custom partial class of Response)
    Remember that Reflection invoke the method on the class instance.

    */

    internal class Data
    {
        // if you want to use NLog logger with the custom exists settings in the json configuration file
        private readonly NLog.Logger logger;

        // some examples of personal attribute and method
        private string _token = "test-token";
        private string jsonData() => $"{{\"name\":\"Fabrizio\",\"lastName\":\"Amorelli\"}}";

        public Data()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            NLog.Config.LoggingConfiguration nlogConfig = new NLogLoggingConfiguration(config.GetSection("NLog"));
            logger = NLog.LogManager.GetCurrentClassLogger();
        }



        [CustomPath("/data")]
        public Dictionary<string, object> GetData(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
        {
            //Another example to protect this Custom single URL Path
            //Note that in this case, you must call the Authentication method on Response class instance
            //Otherways for example another solution, is simply extends Response Super Class

            //bool auth = new Response().Authentication("basic", req_headers, ref resp, ref responseData, "GET", "Fabrizio", "Fabrizio");
            //if(!auth) return new Dictionary<string, object>(){{ "auth", auth }};

            string body = "";
            bool json = false;

            if (QueryString.Count > 0)
            {
                var items = QueryString.AllKeys.SelectMany(QueryString.GetValues, (k, v) => new { key = k, value = v });
                foreach (var item in items)
                {
                    if (item.key == null && item.value == "json")
                    {
                        json = true;
                        responseData = jsonData();
                        break;
                    }

                    if (item.key == null)
                    {
                        body += $"<br>{item.value}=";
                    }
                    else
                    {
                        body += $"<br>{item.key}={item.value}";
                    }
                }
            }

            if (!json) // for normal get request with query string
            {
                resp.Headers.Set("Content-Type", "text/html; charset=UTF-8");
                responseData += $"Received: {QueryString.Count} params ({MethodBase.GetCurrentMethod().Name})<br>";
                responseData += $"{body}";
                responseData += $"<br>{_token}";
            }
            else // for /data?json
            {
                resp.Headers.Set("Content-Type", "application/json; charset=UTF-8");
            }


            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData}

            };
        }


        [CustomPath("/data", "POST")]
        public Dictionary<string, object> PostData(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, bool HasEntityBody, Stream inputStream, Encoding encoding, string[] phconf)
        {
            string body = "";
            string getparam = "";

            resp.Headers.Set("Content-Type", "application/json; charset=UTF-8");

            if (QueryString.Count > 0)
            {
                var items = QueryString.AllKeys.SelectMany(QueryString.GetValues, (k, v) => new { key = k, value = v });
                foreach (var item in items)
                {
                    if (item.key == null)
                    {
                        getparam += $"{item.value}=; ";
                    }
                    else
                    {
                        getparam += $"{item.key}={item.value}; ";
                    }
                }
            }

            if (HasEntityBody)
            {
                string inputPost = "";
                using (var reader = new StreamReader(inputStream, encoding)) inputPost = reader.ReadToEnd();

                try
                {
                    var tmpObj = JsonValue.Parse(inputPost);
                    body = $"{{\"Result\":\"Ok\", \"MethodName\":\"{MethodBase.GetCurrentMethod().Name}\", \"InputStream\":{inputPost}, \"QueryTotal\":{QueryString.Count}, \"QueryString\":\"{getparam}\"}}";
                }
                catch (Exception e)
                {
                    // In this case you must use NLog Methods.
                    logger.Error($"Error - Body input stream json data parser: {e}");
                    body = "{\"Result\":\"Error\"}";
                }


            }

            body = body == "" ? "{\"Result\":\"Empty input stream\"}" : body;
            responseData += $"{body}";
            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData}

            };
        }
    }
}
