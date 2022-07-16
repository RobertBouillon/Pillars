using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spin.Pillars.Logging.Data;
using Spin.Pillars.Logging.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spin.Pillars_Tests
{
  [TestClass]
  public class Logging
  {
    [TestMethod]
    public void TestMessage()
    {
      TestMessage("I am 12 years old", "I am {age} years old", 12);
      TestMessage("I am 12.0 years old", "I am {age:N1} years old", 12);
      TestMessage("Robert is 12.0 years old", "{Name} is {age:N1} years old", "Robert", 12);
    }

    private void TestMessage(string expected, string template, params object[] args)
    {
      var actual = new MessageTemplate(template).Compose(args);
      Assert.AreEqual(expected, actual);
    }
  }
}
