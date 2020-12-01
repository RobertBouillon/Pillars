using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spin.Pillars.Configuration
{
  public abstract class SimpleConfiguration
  {
    private Dictionary<string, string> _data;
    protected Dictionary<string, string> Data => _data; 

    private static Regex _parser = new Regex(@"(([^:]+):([^#]+))?(#[^$]+)?$", RegexOptions.Compiled);
    private static Regex _ip = new Regex(@"([\d\.]+):(\d+)", RegexOptions.Compiled);

    protected abstract void Parse();

    public void Load(string shortFileName)
    {
      var executiondir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      Load(new FileInfo(Path.Combine(executiondir, shortFileName)));
    }

    public void Load(FileInfo config)
    {
      Dictionary<string, string> ret = new Dictionary<string, string>();
      foreach (var line in File.ReadAllLines(config.FullName))
      {
        var match = _parser.Match(line);
        if (!match.Success)
          continue;

        var param = match.Groups[2].Value.Trim();
        var val = match.Groups[3].Value.Trim();

        if (String.IsNullOrWhiteSpace(param) || String.IsNullOrWhiteSpace(val))
          continue;

        ret.Add(param, val);
      }

      _data = ret;

      Parse();
    }

    protected bool ReadBool(string key, bool required = true)
    {
      string val;
      var exists = _data.TryGetValue(key, out val);
      if (required && !exists)
        throw new Exception($"{key} is required but was not contained in the configuration");

      val = val.ToLower();
      return val == "yes" || val == "true";
    }

    protected int ReadInt32(string key, bool required = true)
    {
      string val;
      var exists = _data.TryGetValue(key, out val);
      if (required && !exists)
        throw new Exception($"{key} is required but was not contained in the configuration");

      val = val.ToLower();
      int ret;
      if (!Int32.TryParse(val, out ret))
        throw new Exception($"{val} is not a valid value for {key}. Value must be an integer");
      return ret;
    }

    protected string ReadString(string key, bool required = true)
    {
      string val;
      var exists = _data.TryGetValue(key, out val);
      if (required && !exists)
        throw new Exception($"{key} is required but was not contained in the configuration");

      return val;
    }

    protected IPEndPoint ReadIPEndPoint(string key, bool required = true)
    {
      string val;
      var exists = _data.TryGetValue(key, out val);
      if (required && !exists)
        throw new Exception($"{key} is required but was not contained in the configuration");

      var match = _ip.Match(val);
      if (!match.Success)
        throw new FormatException($"{val} is not recognized as an IP End Point");
      return new IPEndPoint(IPAddress.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value));
    }
  }
}
