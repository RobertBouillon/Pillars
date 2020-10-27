﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Modules.v1_0;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Modules.Meters.Flow
{
  public class UIntFlow : IFlow<UInt32>
  {
    #region Fields
    private bool _enabled;
    private string _name;
    private IClock _clock;
    private uint _maximumRate;
    private TimeSpan _per;
    private TimeSpan _precision;
    private IHeartbeat _heartbeat;
    private IHeartbeatSubscription _subscription;
    //private TimeSpan _refreshRate;
    //private IHeartbeatSubscription _subscription;
    private TimeSpan _cooldownPeriod;

    private uint _rate;
    //private uint _total;
    private uint _lastValue;
    private Queue<KeyValuePair<DateTime, uint>> _records = new Queue<KeyValuePair<DateTime, uint>>();
    private IMeter<UInt32> _source;
    #endregion

    #region Properties

    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        if (_enabled == value)
          return;

        if (!value)
          Disable();
        else
          Enable();

        _enabled = value;
      }
    }

    public uint Value
    {
      get { return _rate; }
      set { _rate = value; }
    }

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public TimeSpan Precision
    {
      get { return _precision; }
    }

    public TimeSpan Span
    {
      get { return _per; }
    }

    object IMeter.Value
    {
      get { return _rate; }
      set { _rate = (uint)value; }
    }
    #endregion

    #region Constructors
    public UIntFlow(string name, IMeter<UInt32> source, TimeSpan span, TimeSpan precision, IHeartbeat heartbeat)
    {
      #region Validation
      if (name == null)
        throw new ArgumentNullException("name");
      if (source == null)
        throw new ArgumentNullException("source");
      if (heartbeat == null)
        throw new ArgumentNullException("heartbeat");
      if (span == TimeSpan.Zero)
        throw new ArgumentException("span cannot be Zero", "span");
      if (span < TimeSpan.Zero)
        throw new ArgumentException("span cannot be less than Zero", "span");
      if (precision == TimeSpan.Zero)
        throw new ArgumentException("precision cannot be Zero", "precision");
      if (precision < TimeSpan.Zero)
        throw new ArgumentException("precision cannot be less than Zero", "precision");
      #endregion

      _name = name;
      _source = source;
      _heartbeat = heartbeat;
      _clock = heartbeat.Clock;
      _precision = precision;
      _per = span;
    }


    #endregion

    #region Methods
    private void Enable()
    {
      Reset();
      throw new NotImplementedException();
      //_subscription = _heartbeat.Subscribe(_precision, Calculate, true);
      _enabled = true;
    }

    private void Disable()
    {
      _subscription.Cancel();
      _enabled = false;
    }

    private void Calculate()
    {
      lock (this)
      {
        var now = _clock.Time;
        var value = _source.Value;
        var delta = value - _lastValue;
        _lastValue = value;
        
        _rate += delta;

        _records.Enqueue(new KeyValuePair<DateTime, uint>(now, delta));

        var limit = now - _per;
        while (_records.Peek().Key < limit)
          _rate -= _records.Dequeue().Value;
      }
    }

    public void Reset()
    {
      lock (this)
      {
        _rate = 0;
        _lastValue = _source.Value;
        _records.Clear();
      }
    }
    #endregion

  }
}
