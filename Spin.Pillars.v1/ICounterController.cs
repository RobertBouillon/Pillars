using System;
namespace Spin.Pillars.v1
{
  public interface ICounterController
  {
    IAggregate GetCounter<T>(string name);
  }
}
