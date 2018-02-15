using System;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        TcpListener Listener;

        public async Task StartAsync(int port)
        {
            Listener = new TcpListener(IPAddress.Any, port);
            Listener.Start();
            while (true)
            {
                var client = await Listener.AcceptTcpClientAsync();
                new Thread(() => HandleClient(client)).Start();
            }
        }

        void HandleMessage(TcpClient client, object message)
        {
            Console.WriteLine(message);
        }

        public void HandleClient(TcpClient client)
        {
            byte[] messageBuffer = new byte[4];
            int messageLength = 4;
            int messagePosition = 0;
            bool headerDone = false;

            while (client.Connected) {
                while (client.Available <= 0) {
                    Thread.Sleep(100);
                }

                var available = client.Available;
                var responseBuffer = new byte[available];
                var responsePosition = 0;

                available = client.GetStream().Read(responseBuffer, 0, available);

                while (responsePosition < available)
                {
                    var copied = responseBuffer.CopyTo(messageBuffer, responsePosition, messagePosition, available);
                    responsePosition += copied;
                    messagePosition += copied;

                    if (messagePosition == messageLength)
                    {
                        if (!headerDone)
                        {
                            messageLength = (messageBuffer[0] >> 24) + (messageBuffer[1] >> 16) + (messageBuffer[2] >> 8) + messageBuffer[3];
                            messageBuffer = new byte[messageLength];
                            headerDone = true;
                            Console.WriteLine($"Expecting message with length of {messageLength}");
                        } 
                        else 
                        {
                            var message = JsonConvert.DeserializeObject(new string(messageBuffer.Select(x => (char)x).ToArray()));
                            HandleMessage(client, message);
                            messageBuffer = new byte[4];
                            messageLength = 4;
                            headerDone = false;
                        }
                        messagePosition = 0;
                    }
                }
            }
        }
    }
}
