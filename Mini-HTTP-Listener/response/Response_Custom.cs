using Mini_HTTP_Listener.configuration;
using Mini_HTTP_Listener.php;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace Mini_HTTP_Listener.response
{
    internal partial class Response
    {

        /*
            Here are the configuration for the custom response
            Remember that the priority is:

            1. Switch the Response Absolute URL Path in Response_HTTP
            2. Search an Absolute CustomPath URL
            3. Search a Special CustomPath URL
            4. Search a Static File (if enabled)
        */

        private void CustomResponse(

            HttpListenerRequest req,
            NameValueCollection req_headers,

            ref HttpListenerResponse resp,
            string path,
            string filename,
            NameValueCollection QueryString,
            ref string responseData,
            ref bool inputFile,

            bool HasEntityBody,
            Stream inputStream,
            Encoding encoding,

            string http_method,

            ref bool http_auth

        )
        {
            string[] phconf = new string[]{
                Directory.GetCurrentDirectory() + "\\" + options.Value.static_folder,
                options.Value.php_cgi,
                options.Value.php,
                options.Value.enable_php
            };

            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(typeof(CustomPath), false)
                .FirstOrDefault() != null);

            bool customResponse = false;

            foreach (var method in methods) // absolute URL
            {
                var m = method.GetCustomAttribute<CustomPath>();
                if (m != null)
                {
                    if (m.Method == http_method)
                    {
                        if (path == m.URL)
                        {
                            customResponse = true;
                            object dict;

                            var obj = Activator.CreateInstance(method.DeclaringType);
                            //dict = method.Invoke(obj, new object[] { resp, QueryString, responseData });

                            switch (http_method)
                            {
                                case "GET": dict = method.Invoke(obj, new object[] { req, req_headers, resp, QueryString, responseData, phconf }); break;
                                case "POST": dict = method.Invoke(obj, new object[] { req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                case "PUT": dict = method.Invoke(obj, new object[] { req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                case "DELETE": dict = method.Invoke(obj, new object[] { req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                case "OPTIONS": dict = method.Invoke(obj, new object[] { req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                default:

                                    responseData += $"<h1>500 Internal Server Error</h1>";
                                    resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    resp.StatusDescription = "Internal Server Error";

                                    dict = new Dictionary<string, object>(){
                                        {"HttpListenerResponse", resp},
                                        {"responseData", responseData}
                                    };

                                    break;
                            }

                            IDictionary d = dict as IDictionary;

                            //check the auth
                            if (d.Contains("auth"))
                            {
                                http_auth = (bool)d["auth"];
                                if (!http_auth) return;
                            }
                                

                            resp = (HttpListenerResponse)d["HttpListenerResponse"];
                            responseData = (string)d["responseData"];
                            if(d.Contains("inputFile"))
                                inputFile = (bool)d["inputFile"];
                            //try { inputFile = (bool)d["inputFile"]; } catch(NullReferenceException e){ Console.WriteLine(e.Message); }

                            break;
                        }
                    }
                }
            }

            if (!customResponse) // special custom URL
            {
                foreach (var method in methods)
                {
                    var m = method.GetCustomAttribute<CustomPath>();
                    if (m != null)
                    {
                        if (m.Method == http_method)
                        {
                            //Console.WriteLine(m.URL);

                            if (m.URL.Contains("{"))
                            {
                                //Console.WriteLine(m.URL);

                                var a = path.Split("/");
                                var url_param = a.Last();
                                //Console.WriteLine(url_param);

                                if (url_param != "")
                                {
                                    var raw_path = path.Replace(url_param, "");
                                    //Console.WriteLine(raw_path);
                                    //Console.WriteLine(m.URL);

                                    if (raw_path == m.URL.Split("{")[0])
                                    {
                                        //Console.WriteLine("ok");
                                        customResponse = true;
                                        object dict;

                                        var obj = Activator.CreateInstance(method.DeclaringType);

                                        string dataType = m.URL.Split("{")[1].Replace("}", "").Split(":")[1];

                                        switch(dataType)
                                        {
                                            case "int":
                                                try
                                                {
                                                    int url_param_int = Convert.ToInt32(url_param);
                                                    //dict = method.Invoke(obj, new object[] { url_param_int, resp, QueryString, responseData });
                                                    switch (http_method)
                                                    {
                                                        case "GET": dict = method.Invoke(obj, new object[] { url_param_int, req, req_headers, resp, QueryString, responseData, phconf }); break;
                                                        case "POST": dict = method.Invoke(obj, new object[] { url_param_int, req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                                        case "PUT": dict = method.Invoke(obj, new object[] { url_param_int, req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                                        case "DELETE": dict = method.Invoke(obj, new object[] { url_param_int, req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                                        case "OPTIONS": dict = method.Invoke(obj, new object[] { url_param_int, req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                                        default:

                                                            responseData += $"<h1>500 Internal Server Error</h1>";
                                                            resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                                            resp.StatusDescription = "Internal Server Error";

                                                            dict = new Dictionary<string, object>(){
                                                                {"HttpListenerResponse", resp},
                                                                {"responseData", responseData}
                                                            };

                                                            break;
                                                    }

                                                }
                                                catch
                                                {
                                                    responseData = "<h1>DataType Error</h1>";
                                                    resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                                    resp.StatusDescription = "InternalServerError";

                                                    dict = new Dictionary<string, object>(){
                                                        {"HttpListenerResponse", resp},
                                                        {"responseData", responseData}
                                                    };
                                                }


                                                break;

                                            case "string":
                                                //dict = method.Invoke(obj, new object[] { url_param, resp, QueryString, responseData });
                                                switch (http_method)
                                                {
                                                    case "GET": dict = method.Invoke(obj, new object[] { url_param, req, req_headers, resp, QueryString, responseData, phconf }); break;
                                                    case "POST": dict = method.Invoke(obj, new object[] { url_param, req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                                    case "PUT": dict = method.Invoke(obj, new object[] { url_param, req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                                    case "DELETE": dict = method.Invoke(obj, new object[] { url_param, req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                                    case "OPTIONS": dict = method.Invoke(obj, new object[] { url_param, req, req_headers, resp, QueryString, responseData, HasEntityBody, inputStream, encoding, phconf }); break;
                                                    default:

                                                        responseData += $"<h1>500 Internal Server Error</h1>";
                                                        resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                                        resp.StatusDescription = "Internal Server Error";

                                                        dict = new Dictionary<string, object>(){
                                                            {"HttpListenerResponse", resp},
                                                            {"responseData", responseData}
                                                        };

                                                        break;
                                                }
                                                break;

                                            // Implements here your custom DataType

                                            default: // DataType Not Exist / Implemented

                                                //dict = method.Invoke(obj, new object[] { url_param, resp, QueryString, responseData });
                                                responseData += $"<h1>500 Internal Server Error</h1>";
                                                resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                                resp.StatusDescription = "Internal Server Error";

                                                dict = new Dictionary<string, object>(){
                                                    {"HttpListenerResponse", resp},
                                                    {"responseData", responseData}
                                                };

                                                break;
                                        }

                                        IDictionary d = dict as IDictionary;

                                        //check the auth
                                        if (d.Contains("auth"))
                                        {
                                            http_auth = (bool)d["auth"];
                                            if (!http_auth) return;
                                        }

                                        resp = (HttpListenerResponse)d["HttpListenerResponse"];
                                        responseData = (string)d["responseData"];
                                        if (d.Contains("inputFile"))
                                            inputFile = (bool)d["inputFile"];
                                        //try { inputFile = (bool)d["inputFile"]; } catch(Exception e) { }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!customResponse) // static path
            {

                if (options.Value.enable_static == "true")
                {
                    //Console.WriteLine(filename);
                    filename = filename.Substring(1);

                    if (Path.GetFileName(filename) == ".mini") //special file
                    {
                        responseData = "<h1>403 Forbidden</h1>";
                        resp.StatusCode = (int)HttpStatusCode.Forbidden;
                        resp.StatusDescription = "Forbidden";
                        return;
                    }

                    var _rootDirectory = options.Value.static_folder;

                    if (string.IsNullOrEmpty(filename))
                    {
                        foreach (string indexFile in staticPath.DefaultDocuments)
                        {
                            if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                            {
                                filename = indexFile;
                                break;
                            }
                        }
                    }

                    filename = Path.Combine(_rootDirectory, HttpUtility.UrlDecode(filename));
                    //Console.WriteLine(filename);

                    //Check subfolders
                    var u = filename[filename.Length - 1];

                    if (u == '/')
                    {
                        filename = filename.Replace("/", "\\");
                        //Console.WriteLine(filename);

                        foreach (string indexFile in staticPath.DefaultDocuments)
                        {
                            if (File.Exists(Path.Combine(filename, indexFile)))
                            {
                                filename = Path.Combine(filename, indexFile);
                                break;
                            }
                        }
                    }

                    //Console.WriteLine(filename);

                    if (File.Exists(filename))
                    {

                        //Check the .mini script for authentication
                        var mini = filename.Replace(Path.GetFileName(filename), ".mini");

                        if (File.Exists(@mini))
                        {
                            int lineCount = File.ReadLines(@mini).Count();

                            if (lineCount == 1)
                            {
                                string auth = File.ReadLines(@mini).First();

                                if (AuthType().Contains(auth.ToLower()))
                                {
                                    http_auth = Authentication(auth, req_headers, ref resp, ref responseData, http_method, options.Value.username, options.Value.password);
                                    if (!http_auth) return;
                                }
                                else
                                {
                                    responseData += $"<h1>500 Internal Server Error</h1>";
                                    resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    resp.StatusDescription = "Internal Server Error";
                                    return;
                                }

                            }
                            else if (lineCount == 2)
                            {
                                responseData += $"<h1>500 Internal Server Error</h1>";
                                resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                resp.StatusDescription = "Internal Server Error";
                                return;
                            }
                            else if (lineCount == 3)
                            {
                                string[] auth_data = File.ReadLines(@mini).ToArray();

                                if (AuthType().Contains(auth_data[0].ToLower()))
                                {
                                    http_auth = Authentication(auth_data[0], req_headers, ref resp, ref responseData, http_method, auth_data[1], auth_data[2]);
                                    if (!http_auth) return;
                                }
                                else
                                {
                                    responseData += $"<h1>500 Internal Server Error</h1>";
                                    resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    resp.StatusDescription = "Internal Server Error";
                                    return;
                                }
                            }
                            else if (lineCount >= 4)
                            {
                                responseData += $"<h1>500 Internal Server Error</h1>";
                                resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                                resp.StatusDescription = "Internal Server Error";
                                return;
                            }

                        }

                        //Start read file
                        try
                        {
                            var ext = Path.GetExtension(filename);
                            //Console.WriteLine(ext);

                            if (ext == ".php" && options.Value.enable_php == "true")
                            {

                                var t = new PHP();
                                var myDict = t.GetType().GetMethod("Response").Invoke(t, new object[] { req, filename, resp, responseData, inputFile, phconf });

                                IDictionary dd = myDict as IDictionary;
                                resp = (HttpListenerResponse)dd["HttpListenerResponse"];
                                responseData = (string)dd["responseData"];
                                if (dd.Contains("inputFile"))
                                    inputFile = (bool)dd["inputFile"];
                                //try { inputFile = (bool)dd["inputFile"]; } catch { }

                                //PHP(req, filename, ref resp, ref responseData, ref inputFile, phconf);
                                //responseData += "PHP!";
                                //resp.StatusCode = (int)HttpStatusCode.OK;
                                //resp.StatusDescription = "Status OK";
                            }

                            else
                            {
                                inputFile = true;
                                Stream myInputFile = new FileStream(filename, FileMode.Open);
                                string mime;
                                resp.ContentType = staticPath._mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
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
                            }

                        }
                        catch
                        {
                            inputFile = false;
                            responseData += "<h1>500 InternalServerError</h1>";
                            resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                            resp.StatusDescription = "InternalServerError";
                        }
                    }
                    else
                    {

                        //Folder exist?
                        if (Directory.Exists(filename))
                        {

                            string new_filename = "";

                            foreach (string indexFile in staticPath.DefaultDocuments)
                            {
                                if (File.Exists(Path.Combine(filename, indexFile)))
                                {
                                    new_filename = Path.Combine(filename, indexFile);
                                    break;
                                }
                            }

                            //Console.WriteLine(filename);
                            //Console.WriteLine(new_filename);

                            if (new_filename != "")
                            {
                                //Redirect to folder
                                resp.StatusCode = (int)HttpStatusCode.Redirect;
                                resp.StatusDescription = "Redirect";
                                var req_host = req_headers["Host"];
                                resp.Redirect(@$"{filename.Replace("html", "").Replace(@"\", "/")}/");

                            }
                            else
                            {
                                // File on fs not found
                                responseData = "<h1>404 Not Found</h1>";
                                resp.StatusCode = (int)HttpStatusCode.NotFound;
                                resp.StatusDescription = "Not Found";
                            }

                        }
                        else
                        {
                            // File on fs not found
                            responseData = "<h1>404 Not Found</h1>";
                            resp.StatusCode = (int)HttpStatusCode.NotFound;
                            resp.StatusDescription = "Not Found";
                        }
                    }
                }
                else
                {
                    // Static pages disabled
                    responseData = "<h1>404 Not Found</h1>";
                    resp.StatusCode = (int)HttpStatusCode.NotFound;
                    resp.StatusDescription = "Not Found";
                }

            }

        }

    }
}
