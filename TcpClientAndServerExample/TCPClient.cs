using System;
using System.Net.Sockets;
using System.Text;

namespace TcpClientAndServerExample
{
    internal class TCPClient
    {
        private readonly TcpClient _client;
        public TCPClient()
        {
            // Set the IP address and port number for the server
            string serverIP = "127.0.0.1";
            int serverPort = 8888;

            try
            {
                this.LogMessage("Starting connection to TCP Server");

                // Create a TCP client instance
                _client = new TcpClient(serverIP, serverPort);
                this.ConnectToServer();
            }
            catch (Exception ex)
            {
                this.LogMessage("Error: " + ex);
            }

        }

        private void ConnectToServer()
        {


            try
            {
                // Create a TCP client instance

                this.LogMessage("Connected to server.");

                // Get the network stream for reading and writing data
                NetworkStream networkStream = _client.GetStream();


                // Start a separate thread for sending heartbeat messages
                Task.Run(() => SendHeartbeat(_client));

                while (true)
                {
                    // Read input from the user
                    Console.Write("Enter a message to send to the server (or 'exit' to quit): ");
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input)) { continue; }

                    if (input.ToLower() == "exit")
                    {
                        // Send termination request to the server
                        byte[] terminationBytes = Encoding.ASCII.GetBytes("END_CONNECTION");
                        networkStream.Write(terminationBytes, 0, terminationBytes.Length);
                        break;
                    }
                        
                    // Send the message to the server
                    byte[] data = Encoding.ASCII.GetBytes(input);
                    networkStream.Write(data, 0, data.Length);

                    // Read the response from the server
                    byte[] buffer = new byte[_client.ReceiveBufferSize];
                    int bytesRead = networkStream.Read(buffer, 0, _client.ReceiveBufferSize);
                    string responseData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    this.LogMessage("Server response: " + responseData);
                }

                // Close the connection
                _client.Close();
            }
            catch (Exception ex)
            {
                this.LogMessage("Error: " + ex.Message);
            }
        }

        private void SendHeartbeat(TcpClient client)
        {
            try
            {
                NetworkStream networkStream = client.GetStream();

                // Heartbeat interval
                int heartbeatInterval = 5000; // 5 seconds

                while (true)
                {
                    // Wait for the specified interval
                    Thread.Sleep(heartbeatInterval);

                    if(networkStream != null && client.Connected)
                    {
                        // Send a heartbeat message to the server
                        byte[] heartbeatBytes = Encoding.ASCII.GetBytes("heartbeat");
                        networkStream.Write(heartbeatBytes, 0, heartbeatBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending heartbeat: " + ex.Message);
            }
        }


        private void LogMessage(string message)
        {
            Console.WriteLine($"{nameof(TCPClient)}: {message}");
        }

    }
}