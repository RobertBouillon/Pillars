using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.v1_0
{
  public interface IAggregate<TValue, TAggregate> : IAggregate, IMeter
  {
    new TValue LastValue { get; }
    new TValue Max { get; }
    new TValue Min { get; }
    new TAggregate Average { get; }
    new TAggregate Sum { get; }
    new long Count { get; }

    void Record(TValue value);
  }
}
