using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem
{
  public struct FileSize
  {
    private long _value;
    private static string[] _abbreviations = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };

    public long Value => _value;

    public FileSize(long size) => _value = size;

    public override string ToString() => ToString(0);
    public string ToString(int roundToPlaces)
    {
      if (_value == 0)
        return "0 B";

      //int places = (int)Math.Log10((double)_size) / 3 * 3;
      int places = (int)Math.Log((double)_value, 8) / 3 * 3;
      var abbreviationKey = places / 3;
      if (abbreviationKey == 0)
        roundToPlaces = 0; //Bytes should never be displayed with a decimal
      var format = roundToPlaces == 0 ? "{0:0} {1}" : "{0:0." + new string('0',roundToPlaces) + "} {1}";
      return String.Format(format, _value / Math.Pow(1024, abbreviationKey), _abbreviations[abbreviationKey]);
    }

    public static explicit operator int(FileSize size) => (int)size._value;
    public static implicit operator FileSize(int size) => new FileSize(size);
    public static implicit operator long (FileSize size) => (long)size._value;
    public static implicit operator FileSize(long size) => new FileSize(size);
    public static explicit operator ulong (FileSize size) => (ulong)size._value;
    public static explicit operator FileSize(ulong size) => new FileSize((long)size);
    public static explicit operator decimal(FileSize size) => size._value;
    public static explicit operator FileSize(decimal size) => new FileSize((long)size);
  }
}
