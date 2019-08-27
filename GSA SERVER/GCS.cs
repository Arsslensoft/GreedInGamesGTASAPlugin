using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GSA_SERVER
{
   internal class GCS
    {

       public void StartLocalServer()
       {

           try
           {
               AsynchronousSocketListener s = new AsynchronousSocketListener();
               s.OnDataReceived += new GCQPDataHandler(DataReceived);
               s.SPort = 6667;
               s.StartListening();
           }
           catch (Exception ex)
           {

           }

       }
  
       byte[] DataReceived(byte[] data, int read)
       {
           try
           {
               string resp = "OK";
               bool ret = Program.GameInterface.ProcessGameData(Encoding.UTF8.GetString(data, 0, read), out resp);
               if (ret)
              return Encoding.UTF8.GetBytes(resp);
               else
                   return Encoding.UTF8.GetBytes("DE");

           }
           catch
           {

           }
           return Encoding.UTF8.GetBytes("ER");
       }

       NetworkStream stream;
       TcpClient client = null;
       public void Connect()
       {
           client = new TcpClient("127.0.0.1", 6668);
           stream = client.GetStream();
       }
       public void Disconnect()
       {
           stream.Close();
           client.Close();
       }
       public bool SendDataToGame(string gdata)
       {
           try
           {
               if (client == null)
                   Connect();
               // Create a TcpClient.
               // Note, for this client to work you need to have a TcpServer 
               // connected to the same address as specified by the server, port
               // combination.
               Int32 port = 13000;
           
               // Translate the passed message into ASCII and store it as a Byte array.
               Byte[] data = System.Text.Encoding.ASCII.GetBytes(gdata+"\n");

            

               // Send the message to the connected TcpServer. 
               stream.Write(data, 0, data.Length);



                   //// Buffer to store the response bytes.
                   //data = new Byte[2];

                   //// String to store the response ASCII representation.
                   //String responseData = String.Empty;

                   //// Read the first batch of the TcpServer response bytes.
                   //Int32 bytes = stream.Read(data, 0, data.Length);
                   //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
             
                   //if (responseData == "OK")
                   //    return true;

           }
           catch
           {

           }
           return false;
       }
    }
}
