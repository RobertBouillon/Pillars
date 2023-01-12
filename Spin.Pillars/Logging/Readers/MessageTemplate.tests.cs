using static System.Diagnostics.Debug;

namespace Spin.Pillars.Logging.Readers;

partial class MessageTemplate
{
  public static void TestMessage()
  {
    TestMessage("I am 12 years old", "I am {age} years old", 12);
    TestMessage("I am 12.0 years old", "I am {age:N1} years old", 12);
    TestMessage("Robert is 12.0 years old", "{Name} is {age:N1} years old", "Robert", 12);
  }

  private static void TestMessage(string expected, string template, params object[] args)
  {
    var actual = new MessageTemplate(template).Compose(args);
    Assert(expected == actual);
  }
}
