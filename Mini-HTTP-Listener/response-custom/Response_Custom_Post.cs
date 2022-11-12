using System.Net;
using System.Text;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using Mini_HTTP_Listener.configuration;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;

namespace Mini_HTTP_Listener.response
{
    internal partial class Response
    {
        [CustomPath("/fabrizio", "POST")]
        public Dictionary<string, object> SetCustomResponse_PostFabrizio(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, bool HasEntityBody, Stream inputStream, Encoding encoding, string[] phconf)
        {
            string body = "";

            resp.Headers.Set("Content-Type", "text/html; charset=UTF-8");

            if (QueryString.Count > 0)
            {
                var items = QueryString.AllKeys.SelectMany(QueryString.GetValues, (k, v) => new { key = k, value = v });
                foreach (var item in items)
                {
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

            responseData += $"Received: {QueryString.Count} params ({MethodBase.GetCurrentMethod().Name})<br>";
            responseData += $"{body}";
            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData}

            };
        }


        [CustomPath("/myjson", "POST")]
        public Dictionary<string, object> SetCustomResponse_PostMyjson(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, bool HasEntityBody, Stream inputStream, Encoding encoding, string[] phconf)
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
                    //This method is generic, you can't use DI
                    //Set for example NLog manually
                    //In this case you must use NLog Methods.
                    IConfigurationRoot config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

                    NLog.Config.LoggingConfiguration nlogConfig = new NLogLoggingConfiguration(config.GetSection("NLog"));
                    var logger = NLog.LogManager.GetCurrentClassLogger();

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


        [CustomPath("/myjson/{id:int}", "POST")]
        public Dictionary<string, object> SetCustomResponse_PostMyjson_int(int id, HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, bool HasEntityBody, Stream inputStream, Encoding encoding, string[] phconf)
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
                    body = $"{{\"Result\":\"Ok\", \"MethodName\":\"{MethodBase.GetCurrentMethod().Name}\", \"Id\":{id}, \"InputStream\":{inputPost}, \"QueryTotal\":{QueryString.Count}, \"QueryString\":\"{getparam}\"}}";
                }
                catch (Exception e)
                {
                    //This method is generic, you can't use DI
                    //Set for example NLog manually
                    //In this case you must use NLog Methods.
                    IConfigurationRoot config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

                    NLog.Config.LoggingConfiguration nlogConfig = new NLogLoggingConfiguration(config.GetSection("NLog"));
                    var logger = NLog.LogManager.GetCurrentClassLogger();

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


        [CustomPath("/myjson/string/value/{value:string}", "POST")]
        public Dictionary<string, object> SetCustomResponse_PostMyjson_string(string value, HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, bool HasEntityBody, Stream inputStream, Encoding encoding, string[] phconf)
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
                    body = $"{{\"Result\":\"Ok\", \"MethodName\":\"{MethodBase.GetCurrentMethod().Name}\", \"Value\":\"{value}\", \"InputStream\":{inputPost}, \"QueryTotal\":{QueryString.Count}, \"QueryString\":\"{getparam}\"}}";
                }
                catch (Exception e)
                {
                    //This method is generic, you can't use DI
                    //Set for example NLog manually
                    //In this case you must use NLog Methods.
                    IConfigurationRoot config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

                    NLog.Config.LoggingConfiguration nlogConfig = new NLogLoggingConfiguration(config.GetSection("NLog"));
                    var logger = NLog.LogManager.GetCurrentClassLogger();

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


        [CustomPath("/login", "OPTIONS")]
        public Dictionary<string, object> SetCustomResponse_PostLoginOptions(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, bool HasEntityBody, Stream inputStream, Encoding encoding, string[] phconf)
        {

            // Here configure the CORS Policy for exmaple for /login POST
            resp.Headers.Add("Access-Control-Allow-Origin", "http://192.168.1.57");
            resp.Headers.Add("Access-Control-Allow-Methods", "POST");
            resp.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            responseData += "";
            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData}

            };
        }


        [CustomPath("/login", "POST")]
        public Dictionary<string, object> SetCustomResponse_PostLogin(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, bool HasEntityBody, Stream inputStream, Encoding encoding, string[] phconf)
        {
            string body = "";

            resp.Headers.Add("Content-Type", "application/json; charset=UTF-8");
            resp.Headers.Add("Access-Control-Allow-Origin", "http://192.168.1.57"); // In case you call only from local, CORS are optional
            resp.Headers.Add("Access-Control-Allow-Methods", "POST");
            resp.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            if (HasEntityBody)
            {
                string inputPost = "";
                using (var reader = new StreamReader(inputStream, encoding)) inputPost = reader.ReadToEnd();

                try
                {
                    string result = "Error";
                    var tmpObj = JsonValue.Parse(inputPost);
                    if (tmpObj["username"].ToString() == "Fabrizio" && tmpObj["password"].ToString() == "Fabrizio123")
                        result = "ok";

                    body = $"{{\"Result\":\"{result}\"}}";
                }
                catch
                {
                    body = "{\"Result\":\"Error\"}";
                }


            }
            else
            {
                body = "{\"Result\":\"Error\"}";
            }

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
