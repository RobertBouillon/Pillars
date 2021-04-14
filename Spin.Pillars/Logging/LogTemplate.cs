using Spin.Pillars.Logging.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.Logging
{
  public class LogTemplate
  {
    public static Tag CreateTypeTag(Type type) => new Tag("Type", type.FullName);
    public List<Object> Data { get; set; }
    public LogTemplate(Type type, params object[] data) : this(data) => Data = Data.Concat(CreateTypeTag(type)).ToList();
    public LogTemplate(params object[] data) => Data = new List<object>();


    public LogEntry Write(Exception ex) => Log.Write(Data.Concat(ex));
    public LogEntry Write(string message, IEnumerable<object> data) => Log.Write(Data.Concat(data).Concat(new Message(message)));
    public LogEntry Write(IEnumerable<object> data) => Log.Write(Data.Concat(data));
    public LogEntry Write(string message, params object[] data) => Log.Write(Data.Concat(data).Concat(new Message(message)));
    public LogEntry Transition(string state, string value) => Log.Write(Data.Concat(new State(state, value)));
    public LogEntry Start(string state, params object[] data) => Log.Write(Data.Concat(data).Concat(new State(state, true)));
    public LogEntry Finish(string state, params object[] data) => Log.Write(Data.Concat(data).Concat(new State(state, false)));

    public override string ToString() => Data.Join(',');
  }
}
