using GSA_SERVER;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            //try
            //{
            //    GDP gdp = new GDP();
            //    gdp.Playerid = 0;
            //    gdp.Command = 6;
            //    gdp.Data = Encoding.UTF8.GetBytes("123");
            //    gdp.Length = 3;
            //    gdp.AccessToken = "000000000000000000000000000000";


            //    // Create a TcpClient.
            //    // Note, for this client to work you need to have a TcpServer 
            //    // connected to the same address as specified by the server, port
            //    // combination.
            //    Int32 port = 13000;
            //    TcpClient client = new TcpClient("192.168.1.11", 9685);

            //    // Translate the passed message into ASCII and store it as a Byte array.
            //    Byte[] data = gdp.ToPacket();

            //    // Get a client stream for reading and writing.
            //    //  Stream stream = client.GetStream();

            //    NetworkStream stream = client.GetStream();

            //    // Send the message to the connected TcpServer. 
            //    stream.Write(data, 0, data.Length);

            //    Console.WriteLine("Sent: {0}", "K>0>12\n");

            //    while (true)
            //    {
            //        // Receive the TcpServer.response.

            //        // Buffer to store the response bytes.
            //        data = new Byte[256];

            //        // String to store the response ASCII representation.
            //        String responseData = String.Empty;

            //        // Read the first batch of the TcpServer response bytes.
            //        Int32 bytes = stream.Read(data, 0, data.Length);
            //        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //        Console.WriteLine("Received: {0}", responseData);
            //    }




            //    // Close everything.
            //    stream.Close();
            //    client.Close();
            //}
            //catch (ArgumentNullException e)
            //{
            //    Console.WriteLine("ArgumentNullException: {0}", e);
            //}
            //catch (SocketException e)
            //{
            //    Console.WriteLine("SocketException: {0}", e);
            //}

            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 6667;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
            //PasswordGenerator pass = new PasswordGenerator();
            //pass.Maximum = 30;
            //pass.Minimum = 30;

            //Console.WriteLine(pass.Generate());
            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
        internal static string IntToString(ushort x)
        {
            string str = x.ToString();
            if (str.Length < 3)
            {
                for (int i = 0; i <= 3 - str.Length; i++)
                    str = "0" + str;

                return str;
            }
            else
                return str.Substring(0, 3);

        }
    }
}
