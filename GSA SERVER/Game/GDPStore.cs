using System;
using System.Collections.Generic;
using System.Text;

namespace GSA_SERVER
{
  public class GDPStore : List<GDP>
    {
      public void Push(GDP packet)
      {
          this.Add(packet);
      }
      public GDP Pop()
      {
          GDP gdp = this[this.Count - 1];
          this.Remove(gdp);
          return gdp;
      }

    }
}
