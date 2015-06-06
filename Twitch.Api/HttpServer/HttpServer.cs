using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Twitch.Api.HttpServer
{
    public abstract class HttpServer
    {
        private readonly int _port;
        protected TcpListener Listener;
        protected bool IsActive = true;

        protected HttpServer(int port)
        {
            _port = port;
        }

        public void Listen()
        {
            Listener = new TcpListener(IPAddress.Any, _port);
            Listener.Start();
            while (IsActive)
            {
                try
                {
                    TcpClient s = Listener.AcceptTcpClient();
                    var processor = new HttpProcessor(s, this);
                    var thread = new Thread(processor.Process);
                    thread.Start();
                    Thread.Sleep(1);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public abstract void HandleGetRequest(HttpProcessor p);
    } 
}
