using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace OAuth.Core.HttpServer
{
    public class HttpProcessor
    {
        private readonly TcpClient _socket;
        private readonly HttpServer _server;

        private Stream _inputStream;
        public StreamWriter OutputStream;

        private string _httpMethod;
        public string HttpUrl;
        private readonly Hashtable _httpHeaders = new Hashtable();

        private static readonly int MaxPostSize = 10 * 1024 * 1024; // 10MB

        private const int BufSize = 4096;

        public HttpProcessor(TcpClient s, HttpServer server)
        {
            _socket = s;
            _server = server;
        }

        private string streamReadLine(Stream inputStream)
        {
            var data = "";
            while (true)
            {
                var nextChar = inputStream.ReadByte();
                if (nextChar == '\n') { break; }
                if (nextChar == '\r') { continue; }
                if (nextChar == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(nextChar);
            }
            return data;
        }

        public void Process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            _inputStream = new BufferedStream(_socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            OutputStream = new StreamWriter(new BufferedStream(_socket.GetStream()));
            try
            {
                ParseRequest();
                ReadHeaders();
                if (_httpMethod.Equals("GET"))
                {
                    HandleGetRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                WriteFailure();
            }
            OutputStream.Flush();
            // bs.Flush(); // flush any remaining output
            _inputStream = null; OutputStream = null; // bs = null;            
            _socket.Close();
        }

        private void ParseRequest()
        {
            var request = streamReadLine(_inputStream);
            var tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            _httpMethod = tokens[0].ToUpper();
            HttpUrl = tokens[1];
        }

        private void ReadHeaders()
        {
            string line;
            while ((line = streamReadLine(_inputStream)) != null)
            {
                if (line.Equals(""))
                    return;

                var separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                var name = line.Substring(0, separator);
                var pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                var value = line.Substring(pos, line.Length - pos);
                _httpHeaders[name] = value;
            }
        }

        private void HandleGetRequest()
        {
            _server.HandleGetRequest(this);
        }

        public void WriteSuccess(string contentType = "text/html")
        {
            // this is the successful HTTP response line
            OutputStream.WriteLine("HTTP/1.0 200 OK");
            // these are the HTTP headers...          
            OutputStream.WriteLine("Content-Type: " + contentType);
            OutputStream.WriteLine("Connection: close");
            // ..add your own headers here if you like

            OutputStream.WriteLine(""); // this terminates the HTTP headers.. everything after this is HTTP body..
        }

        private void WriteFailure()
        {
            // this is an http 404 failure response
            OutputStream.WriteLine("HTTP/1.0 404 File not found");
            // these are the HTTP headers
            OutputStream.WriteLine("Connection: close");
            // ..add your own headers here

            OutputStream.WriteLine(""); // this terminates the HTTP headers.
        }
    }
}
