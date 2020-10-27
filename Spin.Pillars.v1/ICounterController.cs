using System;
namespace System.Modules.v1_0
{
  public interface ICounterController
  {
    IAggregate GetCounter<T>(string name);
  }
}
