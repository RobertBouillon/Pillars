using System;
using io = System.IO;
using System.Text;
using Spin.Pillars.Hierarchy;

namespace Spin.Pillars.FileSystem.Assembly
{
  public class AssemblyFile : File
  {
    public override AssemblyProvider Provider => base.Provider as AssemblyProvider;

    public override Directory Parent => new AssemblyDirectory(Provider, Path.MoveUp());
    public override DateTime GetDate(DateStamp stamp, DateTimeKind kind) => throw new NotImplementedException(stamp.ToString());

    public AssemblyFile(AssemblyProvider provider, string path)
    {
      #region Validation
      if (provider is null)
        throw new ArgumentNullException(nameof(provider));
      #endregion
      Provider = provider;
      Path = new Path(path, Provider.PathSeparator);
    }

    public AssemblyFile(AssemblyProvider provider, Path path) : base(provider, path) { }

    public override io.Stream OpenRead() => Provider.Assembly.GetManifestResourceStream(FullName);
    public override io.Stream OpenWrite() => throw new NotSupportedException();
    public override void Delete() => throw new NotSupportedException();
    public override bool Exists() => Provider.FileIndex.Contains(FullName);

    public override void Write(string text, bool overwrite = true, Encoding encoding = null) => throw new NotSupportedException();
    public override void SetDate(DateStamp stamp, DateTime date, DateTimeKind kind = DateTimeKind.Utc) => throw new NotSupportedException();
  }
}
