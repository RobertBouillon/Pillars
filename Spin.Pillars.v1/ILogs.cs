﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.v1_0
{
  public interface ILogs : IList<ILog>
  {
    ILog this[string name] { get; }
  }
}
