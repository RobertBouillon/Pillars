using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spin.Pillars.Workers;
using System.Net.Sockets;
using System.Threading;

namespace Spin.Pillars_Tests
{
  [TestClass]
  public class Workers
  {
    [TestMethod]
    public void TestTcpWorker()
    {
      var ep = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 5050);
      var worker = new TcpConnectionWorker(ep);
      worker.Start();

      //Thread.Sleep(10000);
      Connect(ep);
      Connect(ep);
      Connect(ep);

      worker.Stop();
    }

    private static void Connect(System.Net.IPEndPoint ep)
    {
      using var client = new TcpClient();
      client.Connect(ep);
      client.Close();
    }
  }
}