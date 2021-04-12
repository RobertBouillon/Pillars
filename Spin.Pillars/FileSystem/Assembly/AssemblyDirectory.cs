using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using System.Text;
using Spin.Pillars.Hierarchy;
using System.Reflection;

namespace Spin.Pillars.FileSystem.Assembly
{
  public class AssemblyDirectory : Directory
  {
    public override AssemblyProvider Provider => base.Provider as AssemblyProvider;

    public override AssemblyDirectory Parent => Path.Count == 0 ? null : new AssemblyDirectory(Provider, Path.MoveUp());

    public AssemblyDirectory(AssemblyProvider provider, string path) : this(provider, new Path(path, provider.PathSeparator)) { }
    public AssemblyDirectory(AssemblyProvider provider, Path path)
    {
      #region Validation
      if (provider is null)
        throw new ArgumentNullException(nameof(provider));
      #endregion
      Provider = provider;
      Path = path;
    }

    public override IEnumerable<File> GetFiles() => Provider.Files.Where(x => x.StartsWith(FullName)).Select(x => new AssemblyFile(Provider, x));
    public override IEnumerable<Directory> GetDirectories() => Provider.Directories.Where(x => x.StartsWith(FullName)).Select(x => new AssemblyDirectory(Provider, x));

    public override void Create() => io.Directory.CreateDirectory(FullName);
    public override bool Exists() => io.Directory.Exists(FullName);
  }
}
