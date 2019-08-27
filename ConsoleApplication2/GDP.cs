using System;
using System.Collections.Generic;
using System.Text;

namespace GSA_SERVER
{
   public class GDP
    {
       public ushort Playerid;
       public string AccessToken;
       public ushort Length;
       public byte Command;
       public byte[] Data;

       public static GDP Parse(byte[] data)
       {
           GDP p = new GDP();
           p.Playerid = BitConverter.ToUInt16(data, 0);
           p.AccessToken = Encoding.UTF8.GetString(data, 2, 30);
           p.Command = data[32];
           p.Length = BitConverter.ToUInt16(data, 33);
           p.Data = Utils.Extract(data, 35, p.Length);
           return p;
       }

       public byte[] ToPacket()
       {
           byte[] dt = Utils.Append(BitConverter.GetBytes(Playerid), Encoding.UTF8.GetBytes(AccessToken));
           dt = Utils.Append(dt, new byte[1] { Command });
           dt = Utils.Append(dt, BitConverter.GetBytes(Length));
           dt = Utils.Append(dt,Data);
           return dt;
       }
    }
}
