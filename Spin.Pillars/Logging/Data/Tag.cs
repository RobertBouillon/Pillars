using Spin.Pillars.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging.Data;

public struct Tag : ILogMetaData
{
  public string Text { get; }
  public object Value { get; }

  public Tag(string text, object value) => (Text, Value) = (text, value);
  public override string ToString()
  {
    var value =
      (Value is Path path) ? path.ToString('\\') :
      Value.ToString();

    return $"{Text}: {value}";
  }
}
