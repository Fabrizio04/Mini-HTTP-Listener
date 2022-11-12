using System.Net;
using System.Collections;
using System.Collections.Specialized;
using Mini_HTTP_Listener.configuration;
using Mini_HTTP_Listener.php;

namespace Mini_HTTP_Listener.response
{
    internal partial class Response
    {
        [CustomPath("/phpimage")]
        public Dictionary<string, object> SetCustomResponse_PHP_Image(HttpListenerRequest req, NameValueCollection req_headers, HttpListenerResponse resp, NameValueCollection QueryString, string responseData, string[] phconf)
        {

            //Call the PHP method to rewrite this Path to the script image.php as image/jpg
            bool inputFile = false;
            var t = new PHP();
            var myDict = t.GetType().GetMethod("Response").Invoke(t, new object[] { req, @"html\image.php", resp, responseData, inputFile, phconf });

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
    }

}
