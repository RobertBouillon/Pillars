//DO NOT DELETE: This is excluded from the project, but kept in souce control as a demonstration
//               of the difference between IAsyncResult and Task pattern implementations of this class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spin.Pillars.Workers
{
  internal class TcpConnectionWorker_Task : Worker
  {
    private TcpListener _tcpListener;
    private Task _task;
    private CancellationTokenSource _cancellationTokenSource = new();

    public TcpConnectionWorker_Task(string name, IPEndPoint ep)
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
      _task = _tcpListener
        .AcceptTcpClientAsync(_cancellationTokenSource.Token)
        .AsTask()
        .ContinueWith(OnClientConnected);
    }

    public void Stop()
    {
      _cancellationTokenSource.Cancel();
      _tcpListener.Stop();
    }

    private void OnClientConnected(Task<TcpClient> client)
    {
      if (client.IsCompletedSuccessfully)
      {
        OnClientConnected(client.Result);
        _tcpListener.Start();
        AcceptNext();
      }
      else if (client.IsCanceled)
      {
        return;
      }
      else
        OnError(client.Exception);
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
}
