using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace GSA_SERVER
{
    public class VStream
    {
        public ulong Duration;
        public ushort Vid;
        Stopwatch st = new Stopwatch();
        public ulong CurrentTime
        {
            get{
              return (ulong) st.ElapsedMilliseconds;
            }
        }
        public VStream(ushort vid,ulong duration)
        {
            Vid =vid;
            Duration = duration;
        }
        public bool Streaming = false;
        System.Timers.Timer aTimer; 
        public bool Stream()
        {
            if(!Streaming)
            {
             // Create a timer with a ten second interval.
        aTimer = new System.Timers.Timer(Duration);

        // Hook up the Elapsed event for the timer.
        aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

        // Set the Interval to 2 seconds (2000 milliseconds).
        aTimer.Interval = Duration;
        aTimer.Enabled = true;
                st.Start();
              
            Streaming = true;
            
      return true;
            }
            else{
                return false;
            }
        }
          public bool Stop()
        {
            if(Streaming)
            {
      
        aTimer.Enabled = false;
            Streaming = false;
                st.Stop();
            
      return true;
            }
            else{
                return false;
            }
        }
    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        aTimer.Enabled = false;
        try{
            st.Reset();
            st.Start();
        }
        catch{

        }
           aTimer.Enabled = true;
    }

    }
   public class Streamer
    {
       public static Dictionary<ushort, VStream> Streams = new Dictionary<ushort, VStream>();
       public static void Initialize()
       {
           try
           {
               foreach (string f in File.ReadAllLines(Application.StartupPath + @"\VS.txt"))
               {
                   string[] ln = f.Split(':');
                   Streams.Add(ushort.Parse(ln[0]), new VStream(ushort.Parse(ln[0]), ulong.Parse(ln[1])));
               }
           }
           catch
           {

           }
       }

       public static void StreamVID(ushort vid)
       {
           if (Streams.ContainsKey(vid))
           {
               Streams[vid].Stream();
           }
       }
       public static void StopVID(ushort vid)
       {
           if (Streams.ContainsKey(vid))
           {
               Streams[vid].Stop();
           }
       }
       public static ulong GetVidTime(ushort vid)
       {
           if (Streams.ContainsKey(vid))
           {
               return Streams[vid].CurrentTime;
           }
           else return 0;
       }
    }
}
