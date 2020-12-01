using Spin.Pillars.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Modules.Logging.Writers
{
  public class CompositeWriter : LogWriter
  {
    public List<LogWriter> Writers { get; } = new List<LogWriter>();

    public CompositeWriter(IEnumerable<LogWriter> writers)
    {
      #region Validation
      if (writers is null)
        throw new ArgumentNullException(nameof(writers));
      #endregion
      Writers = new List<LogWriter>(writers);
    }

    public override void Write(IEnumerable<LogEntry> entries) => Writers.ForEach(x => x.Write(entries));
  }
}
