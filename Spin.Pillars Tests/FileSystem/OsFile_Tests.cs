using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spin.Pillars.FileSystem.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars_Tests.FileSystem
{
  [TestClass]
  public class OsFile_Tests
  {
    [TestMethod]
    public void TestParse()
    {
      var file = OsDirectory.CurrentExecuting.GetFile("local.bin");
    }
  }
}
