using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace GSA_SERVER
{
    internal class Game
    {
        internal int MaximumWaitDelay = 3;
        internal PasswordGenerator ATRegen = new PasswordGenerator();
        internal Dictionary<ushort, string> PlayerTable = new Dictionary<ushort, string>();
        internal Dictionary<ushort, string> AccessTok = new Dictionary<ushort, string>();
        internal Dictionary<ushort, GDPStore> PacketStore = new Dictionary<ushort, GDPStore>();
        internal Dictionary<ushort, GDP> Response = new Dictionary<ushort, GDP>();

        internal Regex GameDataSplitter = new Regex("<=>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        internal void AddPlayer(ushort playerid, string username)
        {

            if (PlayerTable.ContainsKey(playerid))
                PlayerTable[playerid] = username;
            else
                PlayerTable.Add(playerid, username);

        }
        internal void AddToken(ushort playerid, string token)
        {

            if (AccessTok.ContainsKey(playerid))
                AccessTok[playerid] = token;
            else
                AccessTok.Add(playerid, token);

        }
        internal void CreatePacketStore(ushort playerid)
        {

            if (PacketStore.ContainsKey(playerid))
                PacketStore[playerid].Clear();
            else
                PacketStore.Add(playerid, new GDPStore());

        }


        internal void AddPacket(ushort playerid, GDP packet)
        {
            if (PacketStore.ContainsKey(playerid))
                PacketStore[playerid].Push(packet);

        }
        internal GDP GetPacket(ushort playerid)
        {
            if (PacketStore.ContainsKey(playerid))
                return PacketStore[playerid].Pop();
            else
                return null;

        }
        internal string GetPlayerName(ushort playerid)
        {
            return PlayerTable[playerid];
        }
        internal ushort GetPlayerID(string username)
        {
            ushort pl = 999;
            foreach (KeyValuePair<ushort, string> pair in PlayerTable)
            {
                if (pair.Value == username)
                {
                    pl = pair.Key;
                    break;
                }
            }
            return pl;
        }
        internal string GenerateAccessToken()
        {
            ATRegen.Maximum = 30;
            ATRegen.Minimum = 29;
            ATRegen.RepeatCharacters = true;
         string x = ATRegen.Generate();
         if (x.Length == 29)
             return x + "9";
         else return x;
        }
        internal bool ValidToken(ushort playerid, string AccessToken)
        {
            if (AccessTok.ContainsKey(playerid))
                return (AccessTok[playerid] == AccessToken);
            else
                return false;
        }
        internal string PlayerToken(ushort playerid)
        {
            if (AccessTok.ContainsKey(playerid))
                return AccessTok[playerid];
            else return "000000000000000000000000000000";
        }

        internal GDP MakePacket(GDPCommand cmd, ushort playerid, string data)
        {
            GDP gdp = new GDP();
            gdp.Command = (byte)cmd;
            gdp.Playerid = playerid;
            gdp.AccessToken = PlayerToken(playerid);
        
            gdp.Data = Encoding.UTF8.GetBytes(data);
            gdp.Length = (ushort)gdp.Data.Length;
            return gdp;
        }

        internal GDP WaitForResponse(ushort playerid)
        {

            while (!Response.ContainsKey(playerid))
            {
                Thread.Sleep(50);
            }

                GDP gd = Response[playerid];
                Response.Remove(playerid);

                return gd;
   

        }

        internal bool ProcessGameData(string data, out string resp)
        {
            resp = "OK";
            string[] chunk = GameDataSplitter.Split(data);
            if (chunk.Length > 1)
            {
                string command = chunk[chunk.Length - 1];
                if (command == "READ_KEY")
                {
                    ushort playerid = ushort.Parse(chunk[0]);
                    GDP gdp = MakePacket(GDPCommand.READ_KEY , playerid, "");
                    AddPacket(playerid, gdp);
                    gdp = WaitForResponse(playerid);
                    if (gdp.Command == 14)
                        resp = Encoding.UTF8.GetString(gdp.Data);
                    else
                        resp = "000";
                    return true;
                }
             
         
                else if (command == "STREAM_P")
                {
                    ushort playerid = ushort.Parse(chunk[0]);

                    GDP gdp = MakePacket(GDPCommand.PLAY_STREAM, playerid, chunk[1]+";"+Streamer.GetVidTime(ushort.Parse(chunk[1])).ToString());
                    AddPacket(playerid, gdp);

                    return true;
                }
                else if (command == "STREAM_B")
                {
                    ushort playerid = ushort.Parse(chunk[0]);
                 // Start the stream from server
                    Streamer.StreamVID(ushort.Parse(chunk[0]));
                    return true;
                }
                else if (command == "LOGIN")
                {
                    ushort playerid = ushort.Parse(chunk[0]);
                    string username = chunk[1];
                    string AT = GenerateAccessToken();
                    AddToken(playerid, AT);
                    AddPlayer(playerid, username);
                    CreatePacketStore(playerid);
                    return true;

                }
                else if (command == "LOGOUT")
                {
                    ushort playerid = ushort.Parse(chunk[0]);
                    if (PlayerTable.ContainsKey(playerid))
                        PlayerTable.Remove(playerid);

                    if (AccessTok.ContainsKey(playerid))
                        AccessTok.Remove(playerid);

                    if (PacketStore.ContainsKey(playerid))
                        PacketStore.Remove(playerid);
                    return true;
                }
                else if (command == "SAY_TTS")
                {
                    ushort playerid = ushort.Parse(chunk[0]);
             
                    GDP gdp = MakePacket(GDPCommand.PLAY_VOICE, playerid, chunk[1]);
                    AddPacket(playerid, gdp);
                    return true;

                }
                else if (command == "STOP_TTS")
                {
                    ushort playerid = ushort.Parse(chunk[0]);

                    GDP gdp = MakePacket(GDPCommand.STOP_VOICE, playerid,"");
                    AddPacket(playerid, gdp);
                    return true;

                }
                else
                    return false;
            }
            else
                return false;
        }



    }
}
