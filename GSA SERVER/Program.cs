using System;
using System.Collections.Generic;
using System.Text;

namespace GSA_SERVER
{
    internal delegate void AsyncExec();
  internal  class Program
    {
      internal static Game GameInterface;
      internal static GCPS ServerInterface;
      internal static GCS GCSInterface;
        static void Main(string[] args)
        {
            Console.Title = "Mactown Server Communication Interface";
            Console.WriteLine("MSCI v1.0");
            GameInterface = new Game();
            ServerInterface = new GCPS();
            GCSInterface = new GCS();
            ServerInterface.OnGameDataArrived += ServerInterface_OnGameDataArrived;
            
           // test
            //GameInterface.AddPlayer(0, "Arsslen");
            //GameInterface.CreatePacketStore(0);
            //GameInterface.AddToken(0, "000000000000000000000000000000");
            // END
            Console.WriteLine("Starting Game Server...");
           AsyncExec a1 = new AsyncExec(ServerInterface.Start);
           a1.BeginInvoke(null, null);
            Console.WriteLine("Starting GCS Server...");
            AsyncExec a2 = new AsyncExec(GCSInterface.StartLocalServer);
            a2.BeginInvoke(null, null);
            Console.WriteLine("Monitoring...");
            Streamer.Initialize();
            while (Console.ReadLine() != "quit")
            {
                Console.WriteLine("To Exit type : quit");
            }

            GCSInterface.Disconnect();
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
        static void ServerInterface_OnGameDataArrived(ushort playerid, string type,string data)
        {
            Console.WriteLine(playerid.ToString() + "  SENT  " + type + "   DATA : " + data);
            
         //   GCSInterface.SendDataToGame("K"+IntToString(playerid) + data);
        }
    }
}
