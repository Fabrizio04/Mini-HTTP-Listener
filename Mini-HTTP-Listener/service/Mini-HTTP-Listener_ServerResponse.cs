using System.Diagnostics;
using System.Net;

namespace Mini_HTTP_Listener.service
{
    internal partial class Mini_HTTP_Listener_Service
    {
        private void ServerResponse()
        {
            while (listener.IsListening)
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(response.SetResponse, listener.GetContext());
                }
                catch { }

            }
        }
    }
}
