# Mini-HTTP-Listener
## Based on [System.Net.HttpListener](https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener) (Microsoft-HTTPAPI/2.0), Mini-HTTP-Listener provides a simple implementation with some intersting functions with .NET

## Available Functions

- Console App / System Service
- Single / Multiple listeners inteface
- HTTPS Support
- Console / File System custom logger (with NLog)
- Server Authentication (Basic and Digest available)
- Custom URIs Paths
- Static File Stream Reader
- Basic support for PHP Server-Side Dynamic Pages

## Open Source Third parties Packets
A special thanks to these Open Source projects:
- [Http-Multipart-Data-Parser](https://github.com/Http-Multipart-Data-Parser/Http-Multipart-Data-Parser)
- [NLog](https://github.com/NLog/NLog)

# Documentation

## Summary
- [Requisites](#requisites)
  - [Developer Environment](#developer-environment)
  - [Runtime Environment](#runtime-environment)
- [Appsettings configuration file](#appsettings-configuration)
  - [Mini-HTTP-Listener Configuration](#mini_http_listener_configuration)
  - [NLog](#nlog)
    - [Examples](#nlog-examples)
- [HTTPS](#https)
  - [Netsh](#netsh)
  - [Reverse Proxy (recommended)](#reverse-proxy-recommended)
- [System Service](#system-service)
- [Response](#response)
  - [Priority](#priority)
  - [HTTP Method](#http-method)
  - [Custom Path](#custom-path)
    - [Absolute URL](#absolute-url)
    - [Special URL](#special-url)
    - [Custom Classes](#custom-classes)
  - [Authentication](#authentication)
    - [Global](#global)
    - [Single URI](#single-uri)
    - [Folder Path](#folder-path)
	- [Custom Authentication](#custom-authentication)
  - [PHP](#php)
- [Examples](#examples)
  - [CORS Policy](#cors-policy)
  - [Upload Files](#upload-files)
  - [WWW-Form-Urlencoded](#www-form-urlencoded)
  - [Custom Rewrite](#custom-rewrite)
    - [Static File](#static-file)
    - [PHP Script](#php-script)
    - [Binary File (through PHP Script)](#binary-file-(through-php-script))
  - [Miscellaneous and various](#miscellaneous-and-various)

## Requisites

### Developer Environment
  - .NET 6 SKD (latest) [https://dotnet.microsoft.com/en-us/download/dotnet/6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
  - Microsoft Visual Studio (any edition is fine) [https://visualstudio.microsoft.com/it/downloads/](https://visualstudio.microsoft.com/it/downloads/)
  - Alternatively, you can also use Visual Studio Code [https://code.visualstudio.com/](https://code.visualstudio.com/)

### Runtime Environment
  - .NET 6 Runtime for Console Apps (latest) [https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime)
  - Administrator user account

## Appsettings Configuration

List of all params on the [appsettings.json](Mini-HTTP-Listener/appsettings.json) configuration file

### Mini_HTTP_Listener_Configuration

| Attribute Name | Value | Description |
| :----------- | :------------------- | :-----------: |
| ip_address | array(string) | Specify a list of ip addresses or hosts to adds on listener's prefixes.<br>You can also adds all available interface on the host with the "&#42;" keyword<br>(require Administrators/System grants) |
| port_number | number | Specify the port for listener's prefixes.<br>The default ports 80 and 443 requires Administrators/System grants |
| secure | boolean(true &#124; false) | Enable/Disable [HTTPS](#https) protocol for SSL/TLS |
| title | string | Set the console title (only for Console App) |
| authentication_schemes | string(basic&nbsp;&#124;&nbsp;digest&nbsp;&#124;&nbsp;...) | Specify the Server Authentication Scheme.<br>See [Authentication](#authentication) chapter for more informations. |
| username | string | Default Server Username for Authentication |
| password | string | Default Server Password for Authentication |
| start_browser | boolean(true &#124; false) | Enable/Disable default Browser Startup when the Service is started<br>(only for Console App) |
| start_host | string | Specify the ip or host for the Browser Startup |
| enable_static | boolean(true &#124; false) | Enable/Disable the File System Stream Reading for Static Files.<br>(You must have reading grants) |
| static_folder | string | Specify the listener root path for static files.<br>You can set relative or absolute paths, remember to use escapes if needed. |
| enable_php | boolean(true &#124; false) | Enable/Disable [PHP](#php) for Dynamic Server Side Scripts. |
| php_cgi | string | Specify the PHP [Common Gateway Interface](https://en.wikipedia.org/wiki/Common_Gateway_Interface) executable file path.<br>You can set relative or absolute paths, remember to use escapes if needed. |
| php | string | Specify the PHP Interpreter executable file path.<br>You can set relative or absolute paths, remember to use escapes if needed. |

### NLog

Full Documentation is available at [https://github.com/NLog/NLog.Extensions.Logging/wiki](https://github.com/NLog/NLog.Extensions.Logging/wiki)

Default configuration targets presetted for logging on the [appsettings.json](Mini-HTTP-Listener/appsettings.json) configuration file:

- **logfile** (for all logging to File from Debug to up, excluded default Microsoft Host logging)
- **logconsole** (for Mini-HTTP-Listener Service Console Logging, from Info to up)
- **httpconsole** (for Mini-HTTP-Listener Response Console Logging, from Info to up)

#### NLog Examples

- To change colors of specific keywords, set the attributes of "wordHighlightingRules" array from preferred object in the targets.
- To change colors of lines, set the attributes of "rowHighlightingRules" array from preferred object in the targets.
- To change the log path and file name, set the attribute "fileName" from "logfile" object in the targets.
- To enable for example default Microsoft Host logging, set the correct target of "writeTo" attribute on the rules array.
- To disable for example Mini-HTTP-Listener Service or Response logging, leave the "writeTo" attribute empty on the rules array.
- To disable for example all logging to Fie, leave the "writeTo" attribute empty on the rules array.

### HTTPS

#### Netsh

Simple steps to enable SSL/TLS for HTTPS with netsh with self-signed certificate.<br>
Note: This is not recommended to environments production.

1. Generate for example certificate with OpenSSL
   ```bash
   openssl req -new -newkey rsa:4096 -x509 -sha256 -days 365 -addext "subjectAltName = IP:192.168.1.62,DNS:DESKTOP-08PU835" -nodes -out minihttplistener.crt -keyout minihttplistener.key
   ```

2. Generate the Personal Exchange Certificate with OpenSSL
   ```bash
   openssl pkcs12 -inkey minihttplistener.key -in minihttplistener.crt -export -out minihttplistener.pfx
   ```

3. Import the Personal Exchange Certificate (.pfx) into the Local Computer Personal Store<br>
   See full guide: [https://learn.microsoft.com/en-us/biztalk/adapters-and-accelerators/accelerator-swift/adding-certificates-to-the-certificates-store-on-the-client](https://learn.microsoft.com/en-us/biztalk/adapters-and-accelerators/accelerator-swift/adding-certificates-to-the-certificates-store-on-the-client)<br>
   Note: do not copy/paste the certificate password, write it.

4. Bind the certificate to host:port and application<br>
   See full guide: [https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-configure-a-port-with-an-ssl-certificate](https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-configure-a-port-with-an-ssl-certificate)

   Run CMD as Administrator and set for example:<br>
   ```bash
   netsh http add sslcert ipport=0.0.0.0:443 certhash=660480ce247f4f8944fb29730158632c99894313 appid={fb113f9c-41bf-4196-b4d5-a1e681910acd}
   ```

   Note: for appid, you can also generate a random GUID or use your custom available, remember to save it.

5. Verify the Certificate Binding
   ```bash
   netsh http show sslcert
   ```

   If you want to delete for example the Certificate Binding, run this command always on CMD as Administrator:
   ```bash
   netsh http delete sslcert ipport=0.0.0.0:443
   ```

#### Reverse Proxy (recommended)

Simple steps to enable SSL/TLS for HTTPS for example using NGINX Reverse Proxy with self-signed certificate.<br>
Note: This is not recommended to environments production.

1. Generate for example certificate with OpenSSL
   ```bash
   openssl req -new -newkey rsa:4096 -x509 -sha256 -days 365 -addext "subjectAltName = IP:192.168.1.62,DNS:DESKTOP-08PU835" -nodes -out nginx-certificate.crt -keyout nginx.key
   ```

2. Stop NGINX service, edit the nginx configuration file (path depends from OS version).<br>
   Note: In this example, Mini-HTTP-Listener is configured to 127.0.0.1:10000 (always in the appsettings.json).

    ```bash
    #user  nobody;
    worker_processes  1;

    #error_log  logs/error.log;
    #error_log  logs/error.log  notice;
    #error_log  logs/error.log  info;

    #pid        logs/nginx.pid;


    events {
        worker_connections  1024;
    }


    http {
        include       mime.types;
        default_type  application/octet-stream;

        #log_format  main  '$remote_addr - $remote_user [$time_local] "$request" '
        #                  '$status $body_bytes_sent "$http_referer" '
        #                  '"$http_user_agent" "$http_x_forwarded_for"';

        #access_log  logs/access.log  main;

        sendfile        on;
        #tcp_nopush     on;

        #keepalive_timeout  0;
        keepalive_timeout  65;

        #gzip  on;
        
        server {
            listen 80;
            server_name  127.0.0.1;
            
            location / {
                return 301 https://$host$request_uri;
            }
        }
        
        server {
            listen [::]:80;
            
            location / {
                return 301 https://$host$request_uri;
            }
        }

        server {
            #listen       80;
            #server_name  localhost;
            listen 443 ssl default_server;
            listen [::]:443 ssl default_server;
            ssl_certificate C:/Users/Fabrizio/Desktop/nginx-certificate.crt;
            ssl_certificate_key C:/Users/Fabrizio/Desktop/nginx.key;
            ssl_protocols TLSv1 TLSv1.1 TLSv1.2;
            ssl_ciphers HIGH:!aNULL:!MD5;

            #charset koi8-r;

            #access_log  logs/host.access.log  main;

            #location / {
            #    root   html;
            #    index  index.html index.htm;
            #}
            
            location / {
                proxy_pass   http://127.0.0.1:10000;
            }
        }
   }
   ```
   
### System Service

You can easily install Mini-HTTP-Listener as System Service through simple tools like:

- [NSSM (recommended)](https://nssm.cc/)
- [SrvStart](https://github.com/rozanski/srvstart)

Example:

Download and unzip latest NSSM. Launch with command:
```bash
nssm.exe install
```

Set the full path of Mini-HTTP-Listener.exe file, Service Name and other desidered settings and just click Install service.<br>
With this simple procedure, you can easily install other programs as System Service on Windows such as NGINX, described in the previous chapter for the Reverse Proxy.

### Response

#### Priority

The Response Priority for the received Requests, is the following:

1. Switch the Response Absolute URL Path in Response_HTTP
2. Search an Absolute CustomPath URL
3. Search a Special CustomPath URL
4. Search a Static File (if enabled)

In the next chapters, You can see how to implements the HTTP Methods and Responses.

#### HTTP Method

The Request HTTP Methods are configured on the Response Partial Class ([Response.cs](Mini-HTTP-Listener/response/Response.cs))<br>
In the *SetResponse(object o)* Method, you can edit the *switch(HTTP_Method)* to add for example other HTTP Methos that you need.<br>
On this implementation the HTTP Methods already available are:

- GET
- POST
- PUT
- DELETE
- OPTIONS (for the CORS, see examples chapter)

This available HTTP Methods are implemented always on the Response Partial Class ([Response_HTTP.cs](Mini-HTTP-Listener/response/Response_HTTP.cs)).<br>
In the methods you can set for example the Absolute URL Response Paths and the implementations (see examples chapter).<br>
Note: It is not mandatory to implement the responses in these methods.<br>
To continue with next priority step (Custom Path), simple remember to call *CustomResponse* method.

#### Custom Path

##### Absolute URL

To set an Absolute CustomPath URL, first extend the Response Partial Class.<br>
Add a new generic method of type *Dictionary<string, object>* with your personal custom name, with the *CustomPath* attribute.<br>
On the *CustomPath* set the URL Path and the HTTP Method (if the HTTP Method is not specified, by default It will be GET).<br>
Implements the new method added and return the Dictionary with HttpListenerResponse, responseData and inputFile (optional).<br>
Note: These methods for CustomPath are Generics, You can't use Dependency Injection.<br>
To call for example NLog you can manually reload the settings.<br>
You can find some example on the *response-custom* folder.

##### Special URL

The procedure for implements a Special CustomPath URL is the same as previous chapter.<br>
To specify a parameter, set the URL Path On the *CustomPath* attribute with the following sintax:

```csharp
[CustomPath("/mypath/{myname:datatype}")]
```

Note: on this implementation, you can specify only 1 parameter for each URL.<br>
If you want to pass multiple parameters, you can use for example the QueryString.<br>
On this implementation the Data Type already available for Special CustomPath URL are:

- int
- string

You can add for example your custom Data Type on the *switch(dataType)* of *CustomResponse* method, in the Response Partial Class ([Response_Custom.cs](Mini-HTTP-Listener/response/Response_Custom.cs)).<br>
You can find some example on the *response-custom* folder.

##### Custom Classes

You can also create your custom classes without extends the Response Partial Class.<br>
Add generic methods with CustomPath attribute in the same manner of previous chapters.<br>
You can find an example on the *response-custom* folder.

### Authentication

The Authentication method is available on the Response Partial Class ([Response_Auth.cs](Mini-HTTP-Listener/response/Response_Auth.cs)).<br>
As mentioned at the beginning, 2 authentication types are available on this implementation (basic and digest).<br>
To add for example your custom authentication method, simply add your case on the *switch(scheme.ToLower())* and implements It.<br>
To enable the authentication method for static paths, remember to add name also on the *List<string> AuthType()*.<br>
To require the authentication, there are some differents mode:

1. Global Authentication for all Requests.
   Simply set the 3 parameters (authentication_schemes, username and password) correctly in the appsettings.json configuration file.
   
2. Custom Authentication for Specified Requests.
   There are some different mode:
   
   - For Absolute URL Path in Response_HTTP, set for example the *auth* reference and check if the authentication is correct:
     ```csharp
     auth = Authentication("digest", req_headers, ref resp, ref responseData, "GET", "myUsername", "myPassword");
     if (!auth) return;
     ```
   
   - For Absolute and Special CustomPath URL, check for example if the authentication is not correct so return the Dictionary:
     ```csharp
     bool auth = Authentication("digest", req_headers, ref resp, ref responseData, "GET", "myUsername", "myPassword");
     if(!auth) return new Dictionary<string, object>(){{ "auth", auth }};
     ```

3. Static File Paths
   For Static File Paths, create a new text file *.mini* on the desiderated folder and set the 3 parameters:
   
   ```bash
   method
   username
   password
   ```
   
   Username and Password are optionals, if not specificated will be the default credentials on appsettings configuration file.<br>
   Note: on this implementation, the files path for all the subfolder will not requires the authentication, it requires only for current path.<br>
   To requires authentication also for subpaths you must copy the *.mini* file in the subfolders.

You can find some example on the *response-custom* folder and on ([Response_HTTP.cs](Mini-HTTP-Listener/response/Response_HTTP.cs)).

### PHP

A basic support for PHP is available (tested with PHP 8.1.10 ZTS Visual C++ 2019 x64).<br>
On php folder there is PHP Class with a basic CGI implementation in the *Response* generic method.<br>
To use PHP scripts, you can insert for example the source files on the static folder path.<br>
You can also rewrite for example your Custom Paths to PHP scripts, in this case you must invoke response generic method of PHP Class.<br>
For more info about PHP, see examples chapter.<br>
Note: the BaseStream max size for the Input Request Data used on the PHP class is 2 GB.<br>
If you want for example to POST / Upload big requests / files, please considered to use for example Http-Multipart-Data-Parser.<br>
Remeber to set also the *post_max_size*, *upload_max_filesize*  and *max_file_uploads* in the *php.ini* configuration file.

## Examples

### CORS Policy

**Server**

```csharp
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
```

**Client**

```HTML
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Document</title>
</head>
<body>
    <h1>Login Example</h1>
    <form id="testloginsession">
        <label for="name">Username</label>
        <input type="text" id="username" required>
        <br>
        <label for="password">Password&nbsp;</label>
        <input type="password" id="password" required>
        <br>
        <input type="submit" value="Login">
        <br>
        <span id="message"></span>
    </form>
    <script type="text/javascript" src="login.js"></script>
</body>
</html>
```

```JavaScript
const form = document.getElementById("testloginsession");

form.addEventListener("submit", (e) => {
    e.preventDefault();

    const request = {
        method: 'POST',
        headers: {
            'Accept': 'application/json, text/plain, */*',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            username: document.getElementById("username").value,
            password: document.getElementById("password").value
        })
    };

    //console.log(request.body);

    fetch('/login', request).then(res => res.json())
    .then(res => {
        if(res["Result"] == "ok"){
            document.getElementById("message").textContent = 'SUCCESS!';
            document.getElementById("message").style.color = "green";
        }
        else{
            document.getElementById("message").textContent = 'Username or Password wrong!';
            document.getElementById("message").style.color = "red";
        }
            
        console.log(res);
    });

});
```

### Upload Files

**Small File Size (2 GB)**

```csharp
var parser = MultipartFormDataParser.Parse(inputStream);

foreach (var field in parser.Parameters)
{
	body += $"{field.Name} = {field.Data}<br>";
}

if (parser.Files.Count != 0)
{
	foreach (var file in parser.Files)
	{
		body += $"{file.Name} = {file.FileName} - {file.ContentType}<br>";
		var fileStream = File.Create(@$"tmp/{file.FileName}");
		file.Data.Seek(0, SeekOrigin.Begin);
		file.Data.CopyTo(fileStream);
		fileStream.Close();
	}
}
```

**Big File Size**

```csharp
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
```

### WWW-Form-Urlencoded

```csharp
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
```

### Custom Rewrite

#### Static File

```csharp
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
```

#### PHP Script

```csharp
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
```

#### Binary File (through PHP Script)

```csharp
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
```

### Miscellaneous and various

**CustomPath POST Request**

```csharp
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
```

**CustomPath Special POST Request**

```csharp
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
```

**CustomPath Special GET Request**

```csharp
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
```

**CustomPath GET Request**

```csharp
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
```
