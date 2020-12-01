using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spin.Pillars.v1
{ 
  public interface ISimpleMeter<T> : ISimpleMeter, IMeter<T>
  {
    new T Increment();
    new T Decrement();
    T Increment(T value);
    T Decrement(T value);

    new T Value { get; }
  }
}
