using System;
namespace Spin.Pillars.v1
{
  public interface IAggregate : IMeter
  {
    object LastValue { get; }
    object Max { get; }
    object Min { get; }
    object Average { get; }
    object Sum { get; }
    long Count { get; }

    void Record(object value);
  }
}
