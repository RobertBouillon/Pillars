using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Data
{
  public struct Message
  {
    public string Text { get; set; }
    public Message(string text) => Text = text;
    public override string ToString() => Text;
  }
}
