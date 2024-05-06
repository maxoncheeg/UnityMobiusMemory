using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityModel
{
    public class ServerService : IDisposable
    {
        private static ServerService _service = null;

        private readonly Socket _server;

        public Socket Server => _server;
        public bool AcceptingClient { get; set; } = false;
        public bool DisposeRequired { get; set; } = true;
        
        public List<Socket> Clients { get; }

        private ServerService()
        {
            _server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Clients = new();
            Debug.Log("lol");
        }

        public static ServerService Instantiate()
        {
            return _service ??= new();
        }

        public async Task StartAccept(IPAddress ip, int port)
        {
            await Task.Run(async () =>
            {
                _service.Server.Bind(new IPEndPoint(ip, port));
                _service.Server.Listen(10);

                Debug.Log("started");
                _service.AcceptingClient = true;
                while (_service.AcceptingClient)
                {
                    try
                    {
                        Socket client = await _service.Server.AcceptAsync();

                        Debug.Log("accepted client " + client.RemoteEndPoint.ToString());

                        _service.Clients.Add(client);
                    }
                    catch
                    {
                        Debug.Log("не удалось поймать человека");
                        
                        if (!_service.AcceptingClient)
                        {
                            Debug.Log("yes");
                            break;
                        }

                        //break;
                    }
                }

            });
        }

        public void Dispose()
        {
            _service.AcceptingClient = false;
            Debug.Log("DISPOSE");
            foreach (var client in Clients)
            {
                client.Dispose();
            }
            _server.Dispose();
            _service = null;
            GC.SuppressFinalize(this);
        }

        ~ServerService()
        {
            _service.AcceptingClient = false;
            Debug.Log("FINALIZER");
            foreach (var client in Clients)
            {
                client.Dispose();
            }
            _server.Dispose();
            _service = null;
        }
    }
}