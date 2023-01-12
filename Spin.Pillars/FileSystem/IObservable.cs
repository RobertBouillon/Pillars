using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem;

public interface IObservable
{
  Observer Observe(ChangeTypes types, string filter = null);
}
