using System.Net.Sockets;

namespace Spin.Pillars.Workers
{
  partial class TcpConnectionWorker
  {
    public static void TestTcpWorker()
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
