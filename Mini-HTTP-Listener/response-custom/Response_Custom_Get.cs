using Mini_HTTP_Listener.configuration;
using Mini_HTTP_Listener.php;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;

namespace Mini_HTTP_Listener.response
{
    internal partial class Response
    {
        [CustomPath("/fabrizio/{param:int}")]
        public Dictionary<string, object> SetCustomResponse_GetFabrizio_int(int param, HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
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
            responseData += $"<br>Received the URL Param: {param}";
            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData}

            };
        }


        [CustomPath("/fabrizio")]
        public Dictionary<string, object> SetCustomResponse_GetFabrizio(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
        {

            //Another example to protect this Custom single URL Path
            //Note that in this case, you must return the dictionary to set the auth
            
            //bool auth = Authentication("digest", req_headers, ref resp, ref responseData, "GET", "Fabrizio", "Fabrizio");
            //if(!auth) return new Dictionary<string, object>(){{ "auth", auth }};

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


        [CustomPath("/fabrizio/test")]
        public Dictionary<string, object> SetCustomResponse_GetFabrizioTest(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
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

            responseData += $"Received: {QueryString.Count} params ({MethodBase.GetCurrentMethod().Name})";
            responseData += $"{body}";
            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData}

            };
        }


        [CustomPath("/fabrizio/example/ciao")]
        public Dictionary<string, object> SetCustomResponse_GetFabrizioExampleCiao(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
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


        [CustomPath("/fabrizio/example/{parola:string}")]
        public Dictionary<string, object> SetCustomResponse_GetFabrizioExample_string(string parola, HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
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
            responseData += $"<br>Received the URL Param: {parola}";
            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData}

            };
        }


        [CustomPath("/rewritephp")]
        public Dictionary<string, object> SetCustomResponse_rewritephp(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
        {
            //Call the PHP method to rewrite this Path to the script test.php
            bool inputFile = false;
            var t = new PHP();
            var myDict = t.GetType().GetMethod("Response").Invoke(t, new object[] { req, @"html\test.php", resp, responseData, inputFile, phconf });

            IDictionary dd = myDict as IDictionary;
            resp = (HttpListenerResponse)dd["HttpListenerResponse"];
            responseData = (string)dd["responseData"];
            if (dd.Contains("inputFile"))
                inputFile = (bool)dd["inputFile"];

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData},
                {"inputFile", inputFile}

            };
        }


        [CustomPath("/rewriteindex")]
        public Dictionary<string, object> SetCustomResponse_rewriteindex(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
        {
            //Rewrite for example the index.html
            bool inputFile = true;
            string filename = @"html\index.html";

            Stream myInputFile = new FileStream(filename, FileMode.Open);
            string mime;

            //Remember that is generic, you can't use in this case dependency injection for Mini_HTTP_StaticPath
            resp.ContentType = new Mini_HTTP_Listener_StaticPath()._mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
            resp.ContentLength64 = myInputFile.Length;
            resp.AddHeader("Date", DateTime.Now.ToString("r"));
            resp.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));

            byte[] buffer = new byte[1024 * 32];
            int nbytes;
            while ((nbytes = myInputFile.Read(buffer, 0, buffer.Length)) > 0)
                resp.OutputStream.Write(buffer, 0, nbytes);
            myInputFile.Close();
            resp.OutputStream.Flush();

            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData},
                {"inputFile", inputFile}

            };
        }

    }
}
