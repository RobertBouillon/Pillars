using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Data;

public struct Label : ILogMetaData
{
  public string Text { get; }

  public Label(string text) => Text = text;
  public override string ToString() => Text;
}
