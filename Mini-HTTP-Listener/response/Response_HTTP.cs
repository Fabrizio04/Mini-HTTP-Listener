using System.Web;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using HttpMultipartParser;
using System.IO;


namespace Mini_HTTP_Listener.response
{
    internal partial class Response
    {
        private void GetResponse(HttpListenerRequest req, NameValueCollection req_headers, ref HttpListenerResponse resp, bool HasEntityBody, Stream inputStream, Encoding encoding, string path, string filename, NameValueCollection QueryString, ref string responseData, ref bool inputFile, ref bool auth)
        {

            string body = "";

            switch (path)
            {

                case "/test":

                    //if you want to protect for example this single URL Path, simply set Authentication method to the auth and return if false
                    //auth = Authentication("digest", req_headers, ref resp, ref responseData, "GET", "FabryTest", "FabryTest");
                    //if (!auth) return;


                    //header
                    resp.Headers.Set("Content-Type", "text/html; charset=UTF-8");

                    //body
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

                    responseData += $"Received: {QueryString.Count} params<br>";
                    responseData += $"{body}<br>{Directory.GetCurrentDirectory()}";
                    resp.StatusCode = (int)HttpStatusCode.OK;
                    resp.StatusDescription = "Status OK";

                break;

                case "/getspecificparam":

                    //header
                    resp.Headers.Set("Content-Type", "text/html; charset=UTF-8");

                    //body
                    var param = QueryString["param"] != null ? QueryString["param"] : "";
                    //var param = QueryString["param[]"] != null ? QueryString["param[]"] : ""; -> for get an array

                    body += $"param=<strong>{param}</strong>";
                    responseData += $"{body}";
                    resp.StatusCode = (int)HttpStatusCode.OK;
                    resp.StatusDescription = "Status OK";

                break;

                default:
                    CustomResponse(req, req_headers, ref resp, path, filename, QueryString, ref responseData, ref inputFile, HasEntityBody, inputStream, encoding, "GET", ref auth);
                break;
            }
        }
       
        private void PostResponse(HttpListenerRequest req, NameValueCollection req_headers, ref HttpListenerResponse resp, bool HasEntityBody, Stream inputStream, Encoding encoding, string path, string filename, NameValueCollection QueryString, ref string responseData, ref bool inputFile, ref bool auth)
        {
            string body = "";

            switch (path)
            {
                case "/uploadfile":

                    resp.Headers.Set("Content-Type", "text/html; charset=UTF-8");

                    if (HasEntityBody)
                    {
                        Dictionary<string, FileStream> filestreamsByName = new Dictionary<string, FileStream>();
                        Dictionary<string, string> myFiles = new Dictionary<string, string>();
                        Dictionary<string, string> myTypes = new Dictionary<string, string>();
                        int i = 0;

                        try
                        {
                            // Small File Size (2 GB)

                            //var parser = MultipartFormDataParser.Parse(inputStream);

                            //foreach (var field in parser.Parameters)
                            //{
                            //    body += $"{field.Name} = {field.Data}<br>";
                            //}

                            //if (parser.Files.Count != 0)
                            //{
                            //    foreach (var file in parser.Files)
                            //    {
                            //        body += $"{file.Name} = {file.FileName} - {file.ContentType}<br>";
                            //        var fileStream = File.Create(@$"tmp/{file.FileName}");
                            //        file.Data.Seek(0, SeekOrigin.Begin);
                            //        file.Data.CopyTo(fileStream);
                            //        fileStream.Close();
                            //    }
                            //}


                            // Big File Size
                            var parser = new StreamingMultipartFormDataParser(inputStream);

                            parser.ParameterHandler += parameter => body += $"{parameter.Name} = {parameter.Data}<br>";

                            parser.FileHandler += (name, fileName, type, disposition, buffer, bytes, partNumber, additionalProperties) =>
                            {
                                if (!filestreamsByName.ContainsKey(fileName))
                                {
                                    var filepath = "tmp" + "/" + fileName;
                                    
                                    try
                                    {
                                        var filestream = File.Create(filepath);
                                        filestreamsByName.Add(fileName, filestream);

                                        if (!myFiles.ContainsKey(name))
                                            myFiles.Add(name, fileName);
                                        else 
                                        {
                                            i += 1;
                                            myFiles.Add(name + $"[{i}]", fileName);
                                        }
                                            
                                        myTypes.Add(fileName, type);
                                    }
                                    catch (Exception e)
                                    {
                                        logger.LogError("Error - Creating new file in temp folder");

                                    }

                                }

                                try
                                {
                                    filestreamsByName[fileName].Write(buffer, 0, bytes);
                                    
                                    //Console.WriteLine(partNumber);
                                    
                                    //if (partNumber == -1) // the last chunk/part for this file -> Not working I don't know why
                                    //{
                                    //    filestreamsByName[fileName].Dispose();
                                    //    filestreamsByName[fileName].Close();
                                    //}
                                }
                                catch (Exception e)
                                {
                                    logger.LogError("Error - Using filestreamsByName write");
                                    throw e;
                                }

                            };

                            parser.Run();
                            //Console.WriteLine("finish");

                            foreach (var item in filestreamsByName.Values)
                            {
                                try
                                {
                                    item.Dispose();
                                    item.Close();
                                }
                                catch { }
                                
                            }

                            foreach (var file in myFiles)
                            {
                                body += $"{file.Key} = {file.Value} - {myTypes[file.Value]}<br>";
                                
                            }

                        }
                        catch (Exception e)
                        {
                            logger.LogError($"Error - Body input stream form data parser: {e}");
                            responseData += $"<h1>500 Internal Server Error</h1>";
                            resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                            resp.StatusDescription = "Internal Server Error";

                            foreach (var item in filestreamsByName.Values)
                            {
                                try
                                {
                                    item.Dispose();
                                    item.Close();
                                }
                                catch { }

                            }

                            foreach (var item in filestreamsByName.Keys)
                            {
                                if (File.Exists(Path.Combine("tmp", item)))
                                    File.Delete(Path.Combine("tmp", item));
                                
                            }

                            break;
                        }

                    }

                    responseData += $"{body}";
                    resp.StatusCode = (int)HttpStatusCode.OK;
                    resp.StatusDescription = "Status OK";

                    break;

                case "/json":

                    resp.Headers.Set("Content-Type", "application/json; charset=UTF-8");

                    if (HasEntityBody)
                    {
                        string inputPost = "";
                        using (var reader = new StreamReader(inputStream, encoding)) inputPost = reader.ReadToEnd();

                        try
                        {
                            var tmpObj = JsonValue.Parse(inputPost);
                            body = $"{{\"Result\":\"Ok\", \"InputStream\":{inputPost}}}";
                        }
                        catch (Exception e)
                        {
                            logger.LogError($"Error - Body input stream json data parser: {e}");
                            body = "{\"Result\":\"Error\"}";
                        }


                    }

                    body = body == "" ? "{\"Result\":\"Empty input stream\"}" : body;
                    responseData += $"{body}";
                    resp.StatusCode = (int)HttpStatusCode.OK;
                    resp.StatusDescription = "Status OK";

                    break;

                case "/www-form-urlencoded":

                    resp.Headers.Set("Content-Type", "text/html; charset=UTF-8");

                    if (HasEntityBody)
                    {
                        string inputPost = "";
                        using (var reader = new StreamReader(inputStream, encoding)) inputPost = reader.ReadToEnd();

                        int l = inputPost.Split("&").Length;
                        body += $"<h1>Received {l} parameters</h1>";

                        body += @"<table style='font-family: arial, sans-serif; border-collapse: collapse; width: 25%;'>
                    <thead>
                    <tr style='background-color: #dddddd;'>
                        <th style='border: 1px solid black; text-align: left; padding: 8px;'>Key</th>
                        <th style='border: 1px solid black; text-align: right; padding: 8px;'>Value</th>
                    </tr>
                    </thead>
                    <tbody>
                    ";

                        foreach (var field in inputPost.Split("&"))
                        {
                            var s = field.Split("=");
                            body += $@"<tr>
                        <td style='border: 1px solid black; text-align: left; padding: 8px;'>{HttpUtility.UrlDecode(s[0])}</td>
                        <td style='border: 1px solid black; text-align: right; padding: 8px;'>{HttpUtility.UrlDecode(s[1])}<td>
                        </tr>";
                        }

                        body += "</tbody></table>";


                    }

                    body = body == "" ? "<h1>No Data</h1>" : body;
                    responseData += $"{body}";
                    resp.StatusCode = (int)HttpStatusCode.OK;
                    resp.StatusDescription = "Status OK";

                    break;

                default:
                    CustomResponse(req, req_headers, ref resp, path, filename, QueryString, ref responseData, ref inputFile, HasEntityBody, inputStream, encoding, "POST", ref auth);
                    break;
            }
        }

        private void PutResponse(HttpListenerRequest req, NameValueCollection req_headers, ref HttpListenerResponse resp, bool HasEntityBody, Stream inputStream, Encoding encoding, string path, string filename, NameValueCollection QueryString, ref string responseData, ref bool inputFile, ref bool auth)
        {

            switch (path)
            {
                default:
                    CustomResponse(req, req_headers, ref resp, path, filename, QueryString, ref responseData, ref inputFile, HasEntityBody, inputStream, encoding, "PUT", ref auth);
                break;
            }
        }

        private void DeleteResponse(HttpListenerRequest req, NameValueCollection req_headers, ref HttpListenerResponse resp, bool HasEntityBody, Stream inputStream, Encoding encoding, string path, string filename, NameValueCollection QueryString, ref string responseData, ref bool inputFile, ref bool auth)
        {

            switch (path)
            {
                default:
                    CustomResponse(req, req_headers, ref resp, path, filename, QueryString, ref responseData, ref inputFile, HasEntityBody, inputStream, encoding, "DELETE", ref auth);
                break;
            }
        }

        private void OptionsResponse(HttpListenerRequest req, NameValueCollection req_headers, ref HttpListenerResponse resp, bool HasEntityBody, Stream inputStream, Encoding encoding, string path, string filename, NameValueCollection QueryString, ref string responseData, ref bool inputFile, ref bool auth)
        {

            switch (path)
            {
                default:
                    CustomResponse(req, req_headers, ref resp, path, filename, QueryString, ref responseData, ref inputFile, HasEntityBody, inputStream, encoding, "OPTIONS", ref auth);
                break;
            }
        }


    }
}
