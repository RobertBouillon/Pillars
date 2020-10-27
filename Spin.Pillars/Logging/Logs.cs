using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Modules.v1_0;

namespace System.Modules.Logging
{
  public class Logs : CollectionBase<ILog>, ILogs
  {
    private Dictionary<string,ILog> _index = new Dictionary<string,ILog>();

    public Logs()
    {
        
    }

    public Logs(IEnumerable<ILog> source) : base(source)
    {

    }

    public ILog this[string name]
    {
      get
      {
        //return this.FirstOrDefault(x => x.Name == name);
        ILog ret;
        if (!_index.TryGetValue(name, out ret))
          return null;
        return ret;
      }
    }

    public override bool Contains(ILog item)
    {
      return _index.ContainsKey(item.FullPath);
    }

    protected override void OnInserted(int index, ILog value)
    {
      _index[value.Name] = value;
      base.OnInserted(index, value);
    }

    protected override void OnRemoved(int index, ILog value)
    {
      _index.Remove(value.Name);
      base.OnRemoved(index, value);
    }


    //public ILog Add(string name)
    //{
    //  Log ret = new Log { Name = name };
    //  Add(ret);
    //  return ret;
    //}
  }
}
