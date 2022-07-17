using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Data
{
  public partial struct Message
  {
    private static StringBuilder _interpolationBuffer = new StringBuilder(1024 * 16);
    public record EmbeddedTag(int Start, int Length, string Name, string Format);

    public string Text { get; set; }
    public Message(string text) => Text = text;
    public override string ToString() => Text;
  }
}
