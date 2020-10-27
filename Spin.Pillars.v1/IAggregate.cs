using System;
namespace System.Modules.v1_0
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
