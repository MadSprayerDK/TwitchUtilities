using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Twitch.Api.HttpServer
{
    public abstract class HttpServer
    {

        protected int port;
        protected TcpListener listener;
        protected bool is_active = true;

        public HttpServer(int port)
        {
            this.port = port;
        }

        public void listen()
        {
            listener = new TcpListener(port);
            listener.Start();
            while (is_active)
            {
                try
                {
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    Thread thread = new Thread(new ThreadStart(processor.process));
                    thread.Start();
                    Thread.Sleep(1);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    } 
}
