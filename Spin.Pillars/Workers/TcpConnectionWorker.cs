using System;
using System.Net;
using System.Net.Sockets;

namespace Spin.Pillars.Workers;

public partial class TcpConnectionWorker
{
  private TcpListener _tcpListener;

  public TcpConnectionWorker(IPEndPoint ep)
  {
    #region Validation
    if (ep == null)
      throw new ArgumentNullException(nameof(ep));
    #endregion
    _tcpListener = new TcpListener(ep);
  }

  public void Start() => AcceptNext();

  private void AcceptNext()
  {
    _tcpListener.Start();
    _tcpListener.BeginAcceptTcpClient(AcceptNext, this);
  }

  private void AcceptNext(IAsyncResult result)
  {
    if (_isStopped)
      return;
    OnClientConnected(_tcpListener.EndAcceptTcpClient(result));
    AcceptNext();
  }

  private bool _isStopped;
  public void Stop()
  {
    _tcpListener.Stop();
    _isStopped = true;
  }

  #region ClientConnectedEventArgs Subclass
  public class ClientConnectedEventArgs : EventArgs
  {
    #region Properties
    public TcpClient Client { get; private set; }
    #endregion
    #region Constructors
    internal ClientConnectedEventArgs(TcpClient client)
    {
      #region Validation
      if (client == null)
        throw new ArgumentNullException(nameof(client));
      #endregion
      Client = client;
    }
    #endregion
  }
  #endregion

  public event EventHandler<ClientConnectedEventArgs> ClientConnected;
  protected void OnClientConnected(TcpClient client) => OnClientConnected(new ClientConnectedEventArgs(client));
  protected virtual void OnClientConnected(ClientConnectedEventArgs e) => ClientConnected?.Invoke(this, e);
}
