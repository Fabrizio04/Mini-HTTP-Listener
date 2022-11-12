using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mini_HTTP_Listener.configuration;
using Mini_HTTP_Listener.response;

namespace Mini_HTTP_Listener.service
{
    internal partial class Mini_HTTP_Listener_Service : IHostedService
    {
        private readonly ILogger<Mini_HTTP_Listener_Service> logger;
        private readonly IOptions<Mini_HTTP_Listener_Configuration> options;
        private readonly IHostApplicationLifetime appLifetime;
        private readonly Response response;
        private HttpListener listener = new HttpListener();
        private Thread ThreadingServer;

        public Mini_HTTP_Listener_Service() { }

        public Mini_HTTP_Listener_Service(ILogger<Mini_HTTP_Listener_Service> _logger, IOptions<Mini_HTTP_Listener_Configuration> _options, IHostApplicationLifetime _appLifetime, Response _response)
        {
            logger = _logger;
            options = _options;
            appLifetime = _appLifetime;
            response = _response;
        }
        private void OnStarted()
        {
            // start listener
            try{ Console.Title = options.Value.title; }
            catch { }

            if (options.Value.ip_address.Length == 0)
            {
                logger.LogCritical("The ip_address value is not correct. Please set correct ip value to appsettings.json");
                appLifetime.StopApplication();
                return;
            }

            if (options.Value.port_number == "" || options.Value.port_number == "0")
            {
                logger.LogCritical("The port_number value is not correct. Please set correct port value to appsettings.json");
                appLifetime.StopApplication();
                return;
            }

            if (options.Value.secure == "" || (options.Value.secure != "false" && options.Value.secure != "true"))
            {
                logger.LogCritical("The secure value is not correct. Please set correct secure value to appsettings.json");
                appLifetime.StopApplication();
                return;
            }

            string protocol = "";
            if (options.Value.secure == "true") protocol = "https://";
            else protocol = "http://";

            try
            {

                foreach (var host in options.Value.ip_address)
                    listener.Prefixes.Add($"{protocol}{host}:{options.Value.port_number}/");

                /*
                    In This case, I implements Basic and Digest Authentication manually
                    AuthenticationSchemes aren't needed, so It's fixed to Anonymous
                    You can also implements your favourite AuthenticationSchemes
                */

                // switch(options.Value.authentication_schemes.ToLower())
                // {
                //     case "none": listener.AuthenticationSchemes = AuthenticationSchemes.None; break;
                //     case "digest": listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous; break;
                //     case "negotiate": listener.AuthenticationSchemes = AuthenticationSchemes.Negotiate; break;
                //     case "ntlm": listener.AuthenticationSchemes = AuthenticationSchemes.Ntlm; break;
                //     case "integratedwindowsauthentication": listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication; break;
                //     case "basic": listener.AuthenticationSchemes = AuthenticationSchemes.Basic; break;
                //     case "anonymous": listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous; break;
                //     default: listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous; break;
                // }

                //listener.Realm = "";

                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                listener.Start();
                var hosts = options.Value.ip_address.Select(i => $"{protocol}{i}:{options.Value.port_number}/").ToArray();
                logger.LogInformation($"Listening started on {string.Join(" - ", hosts)}");
            }
            catch
            {
                var hosts = options.Value.ip_address.Select(i => $"{protocol}{i}:{options.Value.port_number}/").ToArray();
                logger.LogCritical($"Error to try listener start on {string.Join(" - ", hosts)}");
                appLifetime.StopApplication();
                return;
            }

            //Open browser
            if (options.Value.start_browser == "true" && options.Value.start_host != "")
            {
                try
                {
                    //NOTE: This not work if run as System Service, only if start program from the Console App File
                    Process.Start(new ProcessStartInfo($"{protocol}{options.Value.start_host}:{options.Value.port_number}/") { UseShellExecute = true });
                }

                catch
                {
                    logger.LogError($"Fail to start Default Web Browser");
                }
            }


            // Start Threading Server
            ThreadingServer = new Thread(ServerResponse);
            ThreadingServer.Name = "ServerResponse";
            ThreadingServer.Start();
        }
        private void OnStopping()
        {
            //Thread.Sleep(3000);
            if(ThreadingServer != null)
                ThreadingServer.Interrupt();

            if (listener.IsListening)
            {
                listener.Stop();
                logger.LogInformation($"Listener stopped");
            }
        }

        private void OnStopped()
        {
            logger.LogInformation("Bye!");
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping application...");
            return Task.CompletedTask;
        }
    }
}
