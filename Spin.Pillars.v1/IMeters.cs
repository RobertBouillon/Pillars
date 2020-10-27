using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Modules.v1_0
{
  public interface IMeters : IList<IMeter>
  {
    IModule Component { get; }
    IMeter this[string name] { get; }

    ISimpleMeter<T> Add<T>(string name);
    ISimpleMeter<T> Add<T>(string name, ISimpleMeter<T> parent);
    IFlow<T> Add<T>(string name, IMeter<T> source, TimeSpan span, TimeSpan precision);
    //ISimpleCounter<T> AddCounter<T>(ISimpleCounter<T> counter);

    IAggregate<TValue, TAggregate> Add<TValue, TAggregate>(string name);
    IAggregate<TValue, TAggregate> Add<TValue, TAggregate>(string name, IAggregate<TValue, TAggregate> parent);
    ITimeMeter AddTime(string name);
    //IAggregate<TValue, TAggregate> AddAggregate<TValue, TAggregate>(IAggregate<TValue, TAggregate> aggregate);
  }
}
