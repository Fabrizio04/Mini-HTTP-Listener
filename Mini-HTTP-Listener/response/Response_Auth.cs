using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mini_HTTP_Listener.response
{
    internal partial class Response
    {
        private List<string> AuthType()
            => new List<string>(){

                "basic",
                "digest"
            };


        internal bool Authentication(

            string scheme,
            NameValueCollection req_headers,
            ref HttpListenerResponse resp,
            ref string responseData,
            string HTTP_Method,
            string username,
            string password

        ){

            // This try catch is made to prevent malformed client requests
            try
            {

                switch(scheme.ToLower())
                {

                    case "basic":

                        if (req_headers["Authorization"] != null)
                        {
                            //parse data
                            string base64_cred = req_headers["Authorization"].Split("Basic ")[1];
                            byte[] valueBytes = Convert.FromBase64String(base64_cred);
                            string us_pwd = Encoding.UTF8.GetString(valueBytes);

                            if ($"{us_pwd}" != $"{username}:{password}")
                            {
                                resp.Headers["WWW-Authenticate"] = "Basic realm=\"postmaster@localhost\"";
                                responseData = "<h1>401 Unauthorized</h1>";
                                resp.StatusCode = (int)HttpStatusCode.Unauthorized;
                                resp.StatusDescription = "Unauthorized";

                                byte[] buffer_er = Encoding.UTF8.GetBytes(responseData);
                                resp.ContentLength64 = buffer_er.Length;
                                using Stream ros_er = resp.OutputStream;
                                ros_er.Write(buffer_er, 0, buffer_er.Length);
                                ros_er.Close();
                                return false;
                            }

                            else return true;
                        }
                        else
                        {
                            resp.Headers["WWW-Authenticate"] = "Basic realm=\"postmaster@localhost\"";
                            responseData = "<h1>401 Unauthorized</h1>";
                            resp.StatusCode = (int)HttpStatusCode.Unauthorized;
                            resp.StatusDescription = "Unauthorized";

                            byte[] buffer_er = Encoding.UTF8.GetBytes(responseData);
                            resp.ContentLength64 = buffer_er.Length;
                            using Stream ros_er = resp.OutputStream;
                            ros_er.Write(buffer_er, 0, buffer_er.Length);
                            ros_er.Close();
                            return false;
                        }

                    case "digest":

                        if (req_headers["Authorization"] != null)
                        {
                            //parse data
                            string auth_header = req_headers["Authorization"];
                            auth_header = auth_header.Replace("Digest ", "").Replace(" ", "").Replace("cnonce", "id");

                            string[] digest_data = auth_header.Split(",");

                            string username_dig = "", realm_dig = "", nonce_dig = "", uri_dig = "", response_dig = "", opaque_dig = "", qop_dig = "", nc_dig = "", cnonce_dig = "";

                            foreach (string val in digest_data)
                            {
                                if (val.Contains("username=")) username_dig = val.Split("username=")[1].Replace("\"", "");
                                if (val.Contains("realm=")) realm_dig = val.Split("realm=")[1].Replace("\"", "");
                                if (val.Contains("nonce=")) nonce_dig = val.Split("nonce=")[1].Replace("\"", "");
                                if (val.Contains("uri=")) uri_dig = val.Split("uri=")[1].Replace("\"", "");
                                if (val.Contains("response=")) response_dig = val.Split("response=")[1].Replace("\"", "");
                                if (val.Contains("opaque=")) opaque_dig = val.Split("opaque=")[1].Replace("\"", "");
                                if (val.Contains("qop=")) qop_dig = val.Split("qop=")[1].Replace("\"", "");
                                if (val.Contains("nc=")) nc_dig = val.Split("nc=")[1].Replace("\"", "");
                                if (val.Contains("id=")) cnonce_dig = val.Split("id=")[1].Replace("\"", "");
                            }

                            //check received data
                            bool dig_auth_ok = true;

                            if (username_dig != username) dig_auth_ok = false;
                            if (realm_dig != "postmaster@localhost") dig_auth_ok = false;
                            if (nonce_dig != "cmFuZG9tbHlnZW5lcmF0ZWRub25jZQ") dig_auth_ok = false;
                            if (uri_dig == "") dig_auth_ok = false;
                            if (opaque_dig != "c29tZXJhbmRvbW9wYXF1ZXN0cmluZw") dig_auth_ok = false;

                            //get MD5 response
                            string HA1 = CreateMD5Hash($"{username_dig}:{realm_dig}:{password}");
                            string HA2 = CreateMD5Hash($"{HTTP_Method}:{uri_dig}");

                            string generate_response = CreateMD5Hash($"{HA1}:{nonce_dig}:{nc_dig}:{cnonce_dig}:{qop_dig}:{HA2}"); //for auth-int
                            //string generate_response = CreateMD5Hash($"{HA1}:{nonce_dig}:{HA2}"); //for auth

                            if (response_dig != generate_response) dig_auth_ok = false;

                            if (!dig_auth_ok)
                            {
                                resp.Headers["WWW-Authenticate"] = "Digest realm=\"postmaster@localhost\", qop=\"auth,auth-int\", algorithm=\"MD5\", nonce=\"cmFuZG9tbHlnZW5lcmF0ZWRub25jZQ\", opaque=\"c29tZXJhbmRvbW9wYXF1ZXN0cmluZw\"";
                                responseData = "<h1>401 Unauthorized</h1>";
                                resp.StatusCode = (int)HttpStatusCode.Unauthorized;
                                resp.StatusDescription = "Unauthorized";

                                byte[] buffer_er = Encoding.UTF8.GetBytes(responseData);
                                resp.ContentLength64 = buffer_er.Length;
                                using Stream ros_er = resp.OutputStream;
                                ros_er.Write(buffer_er, 0, buffer_er.Length);
                                ros_er.Close();
                                return false;
                            }

                            else return true;


                        }
                        else
                        {

                            resp.Headers["WWW-Authenticate"] = "Digest realm=\"postmaster@localhost\", qop=\"auth,auth-int\", algorithm=\"MD5\", nonce=\"cmFuZG9tbHlnZW5lcmF0ZWRub25jZQ\", opaque=\"c29tZXJhbmRvbW9wYXF1ZXN0cmluZw\"";
                            responseData = "<h1>401 Unauthorized</h1>";
                            resp.StatusCode = (int)HttpStatusCode.Unauthorized;
                            resp.StatusDescription = "Unauthorized";

                            byte[] buffer_er = Encoding.UTF8.GetBytes(responseData);
                            resp.ContentLength64 = buffer_er.Length;
                            using Stream ros_er = resp.OutputStream;
                            ros_er.Write(buffer_er, 0, buffer_er.Length);
                            ros_er.Close();
                            return false;

                        }

                    //Implements here your custom auth

                    default: return true;

                }

            }

            catch
            {

                responseData = "<h1>500 Internal Server Error</h1>";
                resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                resp.StatusDescription = "Internal Server Error";

                byte[] buffer_er = Encoding.UTF8.GetBytes(responseData);
                resp.ContentLength64 = buffer_er.Length;
                using Stream ros_er = resp.OutputStream;
                ros_er.Write(buffer_er, 0, buffer_er.Length);
                ros_er.Close();
                return false;

            }

            
        }

    }
}
