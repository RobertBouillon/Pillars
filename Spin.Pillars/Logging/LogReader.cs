﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Modules.Logging
{
  public abstract class LogReader
  {
    public abstract bool Read();
    public abstract LogEntry Current { get; }
  }
}
