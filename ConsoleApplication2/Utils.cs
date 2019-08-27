using System;
using System.Collections.Generic;
using System.Text;

namespace GSA_SERVER
{
   public class Utils
    {
       public static byte[] ConvertString(string data, int size)
       {

           if (data.Length == size)
               return Encoding.UTF8.GetBytes(data);
           else
           {
               byte[] dest = new byte[size];
               byte[] dt = Encoding.UTF8.GetBytes(data);
               for (int i = 0; i < size; i++)
               {
                   if (i < dt.Length)
                       dest[i] = dt[i];
                   else
                       dest[i] = 0;
               }
               return dest;
           }

       }
        public static byte[] Extract(byte[] data, int offset, int length)
        {
            if (data.Length > length && offset >= 0 && offset + length <= data.Length)
            {
                byte[] d = new byte[length];
                for (int i = offset; i < offset + length; i++)
                    d[i - offset] = data[i];

                return d;
            }
            else
                return null;
        }

        public static byte[] Append(byte[] data, byte[] arg)
        {
            byte[] a = new byte[data.Length + arg.Length];
            for (int i = 0; i < data.Length; i++)
                a[i] = data[i];

            for (int j = 0; j < arg.Length; j++)
                a[data.Length + j] = arg[j];

            return a;
        }
    }
}
