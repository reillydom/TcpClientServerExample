# What is this project?
This project is an example of how to use the 'TcpClient' and 'TcpListener' classes in C# to send data back and forth between a client and server via TCP. 
There are two types of servers in this project, the first('TcpServer') will only accept one client at a time, whereas the second ('MultipleConnectionTcpServer') class will support multiple connections. 

The 'TcpServer' hasn't really had as much development as the 'MultipleConnectionTcpServer', so it may not function as expected with the 'TcpClient' class due to it not supporting commands such as the "heartbeat" or the "END_CONNECTION" that the Client sends. 

# Why?
I was struggling to find a reliable example of how I could easily implement a TCP Client and server using C#. I've published it here in the hopes that it might help someone else in the future. 


