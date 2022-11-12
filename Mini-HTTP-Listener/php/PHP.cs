using Microsoft.Extensions.Options;
using Mini_HTTP_Listener.configuration;
using System.Diagnostics;
using System.Net;

namespace Mini_HTTP_Listener.php
{
    internal class PHP
    {
        public PHP() { }

        public Dictionary<string, object> Response(

            HttpListenerRequest req,
            string filename,
            HttpListenerResponse resp,
            string responseData,
            bool inputFile,
            string[] phconf

        )
        {

            //Console.WriteLine(filename);
            bool need2Stream = false;
            inputFile = false;
            string body = "";

            //Verify php parameters for the generics call
            if (phconf[1] == "" || !File.Exists(@phconf[1]) && phconf[2] == "" && !File.Exists(@phconf[2]))
            {
                responseData += "<h1>500 InternalServerError</h1>";
                resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                resp.StatusDescription = "InternalServerError";
                return new Dictionary<string, object>(){
                    {"HttpListenerResponse", resp},
                    {"responseData", responseData},
                    {"inputFile", inputFile}
                };
            }

            //Verify if enabled php for the generics call
            if (phconf[3] != "true")
                {
                    responseData += "<h1>500 InternalServerError</h1>";
                    resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                    resp.StatusDescription = "InternalServerError";
                    return new Dictionary<string, object>(){
                        {"HttpListenerResponse", resp},
                        {"responseData", responseData},
                        {"inputFile", inputFile}
                    };
            }

            // Get query string from URL
            var index = req.RawUrl.IndexOf("?");
            var queryString = index == -1 ? "" : req.RawUrl.Substring(index + 1);

            byte[] requestBody;
            using (var ms = new MemoryStream())
            {
                req.InputStream.CopyTo(ms);
                requestBody = ms.ToArray();
            }

            // Get paths for PHP
            //var documentRootPath = Directory.GetCurrentDirectory() + "\\" + options.Value.static_folder;
            var documentRootPath = @phconf[0];
            var scriptFilePath = Path.GetFullPath(filename);
            var scriptFileName = Path.GetFileName(filename);
            var scriptFolderPath = Path.GetDirectoryName(filename);
            var tempPath = Path.GetTempPath();

            //Console.WriteLine(documentRootPath);
            //Console.WriteLine(scriptFilePath);
            //Console.WriteLine(scriptFileName);
            //Console.WriteLine(scriptFolderPath);
            //Console.WriteLine(tempPath);
            // Console.WriteLine(context.Request.RawUrl);

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Dictionary<string, string> myHeaders = new Dictionary<string, string>();

            using (var process = new Process())
            {
                //process.StartInfo.FileName = @options.Value.php_cgi;
                process.StartInfo.FileName = @phconf[1];
                //process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                //process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

                process.StartInfo.EnvironmentVariables.Clear();

                process.StartInfo.EnvironmentVariables.Add("GATEWAY_INTERFACE", "CGI/1.1");
                process.StartInfo.EnvironmentVariables.Add("SERVER_PROTOCOL", "HTTP/1.1");
                process.StartInfo.EnvironmentVariables.Add("REDIRECT_STATUS", "200");
                process.StartInfo.EnvironmentVariables.Add("DOCUMENT_ROOT", documentRootPath);
                process.StartInfo.EnvironmentVariables.Add("SCRIPT_NAME", scriptFileName);
                process.StartInfo.EnvironmentVariables.Add("SCRIPT_FILENAME", scriptFilePath);
                process.StartInfo.EnvironmentVariables.Add("QUERY_STRING", queryString);
                process.StartInfo.EnvironmentVariables.Add("CONTENT_LENGTH", requestBody.Length.ToString());
                process.StartInfo.EnvironmentVariables.Add("CONTENT_TYPE", req.ContentType);
                process.StartInfo.EnvironmentVariables.Add("REQUEST_METHOD", req.HttpMethod);
                process.StartInfo.EnvironmentVariables.Add("USER_AGENT", req.UserAgent);
                process.StartInfo.EnvironmentVariables.Add("SERVER_ADDR", req.LocalEndPoint.Address.ToString());
                process.StartInfo.EnvironmentVariables.Add("REMOTE_ADDR", req.RemoteEndPoint.Address.ToString());
                process.StartInfo.EnvironmentVariables.Add("REMOTE_PORT", req.RemoteEndPoint.Port.ToString());
                process.StartInfo.EnvironmentVariables.Add("REFERER", req.UrlReferrer?.ToString() ?? "");
                process.StartInfo.EnvironmentVariables.Add("REQUEST_URI", req.RawUrl);
                process.StartInfo.EnvironmentVariables.Add("HTTP_COOKIE", req.Headers["Cookie"]);
                process.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT", req.Headers["Accept"]);
                process.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT_CHARSET", req.Headers["Accept-Charset"]);
                process.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT_ENCODING", req.Headers["Accept-Encoding"]);
                process.StartInfo.EnvironmentVariables.Add("HTTP_ACCEPT_LANGUAGE", req.Headers["Accept-Language"]);
                process.StartInfo.EnvironmentVariables.Add("TMPDIR", tempPath);
                process.StartInfo.EnvironmentVariables.Add("TEMP", tempPath);

                process.Start();

                // Write request body to standard input, for POST data
                using (var sw = process.StandardInput)
                    sw.BaseStream.Write(requestBody, 0, requestBody.Length);



                // Write headers and content to response stream
                var headersEnd = false;
                using (var sr = process.StandardOutput)
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!headersEnd)
                        {
                            if (line == "")
                            {
                                headersEnd = true;
                                continue;
                            }

                            // The first few lines are the headers, with a
                            // key and a value. Catch those, to write them
                            // into our response headers.
                            index = line.IndexOf(':');
                            var name = line.Substring(0, index);
                            var value = line.Substring(index + 2);

                            //resp.Headers[name] = value;
                            //resp.AddHeader(name, value);
                            //Console.WriteLine(name+" - "+value);
                            myHeaders.Add(name, value);

                            if (name.ToLower() == "content-type")
                            {
                                string tmp_value = value.Replace(" ", "");
                                var part_array = tmp_value.Split(";");

                                foreach (string part_string in part_array)
                                    if (Mini_HTTP_Listener_PHP_MimeType._mimeTypeMappings.Contains(part_string, StringComparer.OrdinalIgnoreCase))
                                        need2Stream = true;

                            }

                        }
                        else
                        {
                            if (need2Stream) break;

                            // Write non-header lines into the output as is.
                            body += line;
                        }
                    }

                    //sr.BaseStream.Flush();
                }
            }

            responseData += body;

            if (need2Stream)
            {
                inputFile = true;
                var php = new Process();

                Console.OutputEncoding = System.Text.Encoding.UTF8;
                string script = @filename;
                string WorkingDirectory = Path.GetFullPath(script).Replace(Path.GetFileName(script), "");

                php.StartInfo.FileName = @phconf[2];
                php.StartInfo.UseShellExecute = false;
                php.StartInfo.RedirectStandardOutput = true;
                php.StartInfo.RedirectStandardInput = true;
                php.StartInfo.CreateNoWindow = true;
                php.StartInfo.Arguments = Path.GetFullPath(script);
                php.StartInfo.WorkingDirectory = @WorkingDirectory;
                php.Start();

                var reader = php.StandardOutput;
                Stream myStream = reader.BaseStream;

                byte[] buffer = new byte[1024 * 32];
                int nbytes;
                long total = 0;

                foreach (var item in myHeaders)
                {
                    if (item.Key == "Content-Length")
                        resp.ContentLength64 = long.Parse(item.Value);
                    else
                        resp.AddHeader(item.Key, item.Value);
                }

                while ((nbytes = myStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    resp.OutputStream.Write(buffer, 0, nbytes);
                    total += nbytes;
                }

                //Console.WriteLine(resp.ContentLength64);
                resp.OutputStream.Flush();
                php.WaitForExit();
            }
            else
            {
                foreach (var item in myHeaders)
                {
                    if (item.Key == "Content-Length")
                        resp.ContentLength64 = long.Parse(item.Value);
                    else
                        resp.AddHeader(item.Key, item.Value);
                }
            }


            if (resp.Headers.Get("Status") != null)
            {
                //Console.WriteLine(resp.Headers["Status"]);
                int StatusCode = Convert.ToInt32(resp.Headers["Status"].Split(" ")[0]);
                string StatusDescription = resp.Headers["Status"].Replace(resp.Headers["Status"].Split(" ")[0] + "", "");
                resp.StatusCode = StatusCode;
                resp.StatusDescription = StatusDescription;
            }
            else
            {
                resp.StatusCode = (int)HttpStatusCode.OK;
                resp.StatusDescription = "Status OK";
            }

            return new Dictionary<string, object>(){

                {"HttpListenerResponse", resp},
                {"responseData", responseData},
                {"inputFile", inputFile}
            };
        }
    }
}
