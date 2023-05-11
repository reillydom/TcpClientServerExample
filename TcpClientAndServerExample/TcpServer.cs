using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace TcpClientAndServerExample
{

    internal class TCPServer
    {
        TcpListener _listener;
        public TCPServer()
        {
            this.LogMessage("Starting server");

            // Set the IP address and port number for the server
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8888;

            // Create a TCPListener instance
            _listener = new TcpListener(ipAddress, port);

            this.StartListeningForRequests();
        }

        private void StartListeningForRequests()
        {
            try
            {
                // Start listening for client connections
                _listener.Start();
                this.LogMessage("Server started. Waiting for clients...");

                // Accept the client connection
                TcpClient client = _listener.AcceptTcpClient();
                this.LogMessage("Client connected.");

                // Get the network stream for reading and writing data
                NetworkStream networkStream = client.GetStream();

                while (true)
                {
                    // Read data from the client
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    this.LogMessage("Received: " + dataReceived);

                    // Process the received data
                    string responseData = dataReceived.ToUpper();

                    // Send the response back to the client
                    byte[] responseBytes = Encoding.ASCII.GetBytes(responseData);
                    networkStream.Write(responseBytes, 0, responseBytes.Length);
                    this.LogMessage("Sent: " + responseData);
                }
            }
            catch (Exception ex)
            {
                this.LogMessage("Error: " + ex.Message);
            }
            finally
            {
                // Stop listening and clean up resources
                _listener.Stop();
            }
        }




        private void LogMessage(string message)
        {
            Console.WriteLine($"{nameof(TCPServer)}: {message}");
        }
    }
}
