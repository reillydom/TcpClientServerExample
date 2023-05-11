using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace TcpClientAndServerExample
{
    internal class MultipleConnectionTcpServer
    {
        private readonly List<TcpClient> _connectedClients;
        private readonly TcpListener _listener;

        public MultipleConnectionTcpServer()
        {
            _connectedClients = new List<TcpClient>();


            // Set the IP address and port number for the server
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8888;

            // Create a TCPListener instance
            _listener = new TcpListener(ipAddress, port);
            this.StartServer();
        }
        private void StartServer()
        {

            try
            {
                // Start listening for client connections
                _listener.Start();
                this.LogMessage("Server started. Waiting for clients...");

                while (true)
                {
                    // Accept the client connection
                    TcpClient client = _listener.AcceptTcpClient();
                    _connectedClients.Add(client);
                    this.LogMessage("Client connected.");

                    // Start a new thread to handle the client communication
                    Thread clientThread = new Thread(HandleClientCommunication);
                    clientThread.Start(client);
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

        private void HandleClientCommunication(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream networkStream = client.GetStream();

            // Heartbeat interval and timeout
            int heartbeatInterval = 5000; // 5 seconds
            int heartbeatTimeout = 15000; // 15 seconds

            DateTime lastHeartbeatReceived = DateTime.Now;


            try
            {
                // Set a timeout value for reading from the client
                //int receiveTimeout = 5000; // 5 seconds
                //networkStream.ReadTimeout = receiveTimeout;


                while (true)
                {
                    // Read data from the client
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);

                    if (bytesRead == 0)
                    {
                        // Zero bytes read indicates that the client has closed the connection
                        break;
                    }


                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    this.LogMessage("Received from client: " + dataReceived);

                    if (dataReceived.Trim().Equals("END_CONNECTION", StringComparison.OrdinalIgnoreCase))
                    {
                        // Client requested termination
                        break;
                    }
                    else if (dataReceived.Trim().Equals("heartbeat", StringComparison.OrdinalIgnoreCase))
                    {
                        // Received heartbeat message, update last heartbeat received time
                        lastHeartbeatReceived = DateTime.Now;
                        continue; // Skip further processing for heartbeat messages
                    }


                    // Send the response back to the client -- Essentially send back what they sent us.
                    byte[] responseBytes = Encoding.ASCII.GetBytes(dataReceived);
                    networkStream.Write(responseBytes, 0, responseBytes.Length);
                    this.LogMessage("Sent to client: " + dataReceived);


                    // Check for unresponsive client
                    if ((DateTime.Now - lastHeartbeatReceived).TotalMilliseconds > heartbeatTimeout)
                    {
                        this.LogMessage("Client connection is unresponsive. Closing connection.");
                        break;
                    }
                }
            }
            catch (IOException ex)
            {
                // Handle IOException caused by abrupt connection closure
                this.LogMessage("Client connection closed abruptly: " + ex.Message);
            }
            catch (Exception ex)
            {
                this.LogMessage("Error: " + ex.Message);
            }
            finally
            {
                // Close the client connection
                client.Close();
                _connectedClients.Remove(client);
                this.LogMessage("Client connection closed.");
            }

        }


        private void LogMessage(string message)
        {
            Console.WriteLine($"{nameof(MultipleConnectionTcpServer)}: {message}");
        }
    }
}
