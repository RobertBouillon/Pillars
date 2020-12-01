using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.v1
{
  public interface ILogEntryStream
  {
    bool CanRead { get; }
    bool CanWrite { get; }
    bool CanReadFromStart { get; }

    long Position { get; }
    long Length { get; }

    ILogEntryStream ReadFromStart();
    void Write(ILogEntry logEntry);
    ILogEntry Read();
  }
}
