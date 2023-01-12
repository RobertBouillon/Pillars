using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars.FileSystem.OS;

partial class OsFile
{
  public static void TestParse()
  {
    var file = OsDirectory.CurrentExecuting.GetFile("local.bin");
  }
}
