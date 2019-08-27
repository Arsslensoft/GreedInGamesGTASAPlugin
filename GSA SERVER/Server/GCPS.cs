using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GSA_SERVER
{
  internal   delegate void GameDataArrived(ushort playerid, string type,string data);
   public class GCPS
    {

    internal event GameDataArrived OnGameDataArrived;
       //void OnGameDataArrived(ushort playerid, string type, string data)
       //{
       //    Console.WriteLine(playerid.ToString()+"  SENT  "+ type+ "   DATA : " +data);
       //}
       public void Start()
       {
           try
           {
               AsynchronousSocketListener s = new AsynchronousSocketListener();
               s.OnDataReceived += new GCQPDataHandler(DataReceived);
               s.StartListening();
           }
           catch (Exception ex)
           {
             
           }

       }
       bool Login(string gtaun, string mac)
       {
           return true;
       }
       byte[] DataReceived(byte[] data, int read)
       {
           try
           {
               GDP gdp = GDP.Parse(Utils.Extract(data, 0, read));
               switch ((GDPCommand)gdp.Command)
               {
                   case GDPCommand.LOGIN:
                       string dat = Encoding.UTF8.GetString(gdp.Data);
                       // GTA UN;mac
                       string[] ld = dat.Split(';');
                       if (Login(ld[0], ld[1]) == true)
                       {
                           gdp.Command = (byte)GDPCommand.LOGGED;
                           ushort playerid = Program.GameInterface.GetPlayerID(ld[0]);
                           gdp.AccessToken = Program.GameInterface.AccessTok[playerid];
                           gdp.Length = 30;
                           gdp.Playerid = playerid;
                           gdp.Data = Encoding.UTF8.GetBytes(gdp.AccessToken);

                           return gdp.ToPacket();
                       }
                       else
                       {
                           gdp.Command = (byte)GDPCommand.LOGIN_ERROR;
                           gdp.Length = 3;
                           gdp.Playerid = 0;
                           gdp.Data = Encoding.UTF8.GetBytes("DEN");

                           return gdp.ToPacket();
                       }
                       break;
                   case GDPCommand.KEYDATA:
                       // KEY_CODE
                       if (Program.GameInterface.ValidToken(gdp.Playerid, gdp.AccessToken))
                       {

                           OnGameDataArrived(gdp.Playerid, "KEY", Encoding.UTF8.GetString(gdp.Data));
                           gdp.Command = (byte)GDPCommand.OK;
                           gdp.Length = 2;
                           gdp.Playerid = 0;
                           gdp.Data = Encoding.UTF8.GetBytes("OK");

                           return gdp.ToPacket();
                       }
                       else
                       {
                           gdp.Command = (byte)GDPCommand.ACCESS_TOKEN_ERROR;
                           gdp.Length = 3;
                           gdp.Playerid = 0;
                           gdp.Data = Encoding.UTF8.GetBytes("DEN");

                           return gdp.ToPacket();
                       }
                       break;

                   case GDPCommand.STATUS:
                       // STATUS
                       if (Program.GameInterface.ValidToken(gdp.Playerid, gdp.AccessToken))
                       {
                           string at = gdp.AccessToken;
                        gdp = Program.GameInterface.GetPacket(gdp.Playerid);
                        if (gdp != null)
                            return gdp.ToPacket();
                        else
                        {
                            gdp = new GDP();
                            gdp.AccessToken = at;
                            gdp.Command = (byte)GDPCommand.ERROR;
                            gdp.Length = 3;
                            gdp.Playerid = 0;
                            gdp.Data = Encoding.UTF8.GetBytes("ERR");

                            return gdp.ToPacket();
                        }

                       }
                       else
                       {
                           gdp.Command = (byte)GDPCommand.ACCESS_TOKEN_ERROR;
                           gdp.Length = 3;
                           gdp.Playerid = 0;
                           gdp.Data = Encoding.UTF8.GetBytes("DEN");

                           return gdp.ToPacket();
                       }
                       break;
                   case GDPCommand.RESPONSE:
                       // RESPONSE
                       if (Program.GameInterface.ValidToken(gdp.Playerid, gdp.AccessToken))
                       {
                           string at = gdp.AccessToken;
                          if(!Program.GameInterface.Response.ContainsKey(gdp.Playerid))
                                                      Program.GameInterface.Response.Add(gdp.Playerid,gdp);
                        
                               gdp = new GDP();
                               gdp.AccessToken = at;
                               gdp.Command = (byte)GDPCommand.OK;
                               gdp.Length = 2;
                               gdp.Playerid = 0;
                               gdp.Data = Encoding.UTF8.GetBytes("OK");

                               return gdp.ToPacket();
                        

                       }
                       else
                       {
                           gdp.Command = (byte)GDPCommand.ACCESS_TOKEN_ERROR;
                           gdp.Length = 3;
                           gdp.Playerid = 0;
                           gdp.Data = Encoding.UTF8.GetBytes("DEN");

                           return gdp.ToPacket();
                       }
                       break;

                   case GDPCommand.PLAYER_CONNECTED:
                       // STATUS
                       string player = Encoding.UTF8.GetString(gdp.Data);
                       if (Program.GameInterface.GetPlayerID(player) != 999)
                       {
                           gdp.Data = Encoding.UTF8.GetBytes("CON");
                           gdp.Length = 3;

                           return gdp.ToPacket();


                       }
                       else
                       {
                           gdp.Data = Encoding.UTF8.GetBytes("DIS");
                           gdp.Length = 3;

                           return gdp.ToPacket();
                       }
                       break;
               }
            
           }
           catch
           {
           }
           return Encoding.UTF8.GetBytes("ERROR");
       }
    }
}
