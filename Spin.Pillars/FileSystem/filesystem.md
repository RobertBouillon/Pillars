# Concepts
Files are inhernetly disconected entities - a File object cannot guarantee that the file it represents actually exists. Because files are not structly controlled by the application, the state 
of a file may change between calls. For this reason, every method and property
of a File induces the file system to search for the File again to fetch the latest metadata, if the file still exists. Any race conditions that may result from multiple system accessing the same
File or Directory are outside of the scope of this interface. Any values by which the file or directory is identified are also cached by default. These values are typically Name and Path.

In the cases where this pattern is inefficient, the Cache method is provided to load a handle to the file and cache the results. The Cache can be cleared by setting IsCached = false. Exists() never
returns a cached value: cache will return true if the file exists to indicate that the file exists and the metadata has been successfully cached.


# file system 

Data must be persisted by some means, and the persistence is provided concepturally by a file system.

## file system vs DriveInfo
https://github.com/dotnet/runtime/issues/34243
https://github.com/dotnet/runtime/issues/26081



## Comparison to System.IO
The API is very similar to System.IO except that it can be extended beyond the Operating System's File System.

File and Directory are analogous to FileInfo and DirectoryInfo, except the latter are always cached and the former are not cached by default, but can be.
Paths are now described by structs rather strings to provide a context for path-related operations.
Static members in System.IO (Directory, File) are integrated into the respective file system class.
`FullName` has been replaced with `PathedName`. `RootedName` was a close second. `FullName` seems too ambiguous

## Key Features
Most operations are available via the file system class by simply providing a Path parameter. This is conducive to high-performance operations. File and Directory classes are available when 
a more robust API is desired and GC pressure is not a concern.
Path provides a delimiter-agnostic representation of a path.

# TO-DO
Pick-Up Notes

General Cleanup
- Rename InMemoryh to Virtual
- Indexer on FileSystem and Directory
- Rename File & Directory to XPointer, XReference, etc. Specifically for static members.
- Add CreateTempFile and CreateTempDirectory to Osfile system
  - Go through existing API and carry over functionality
- Replace Path(string) with Path.Create

Os File System compatability for Linux. Directory structure a little tricky since the mounts are all part of a virtual tree
Heterogeneous operations - MoveTo and CopyTo when File or Directory belongs to another file system.
Return a File/Directory from FileSystem.CreateX? How does this affect File.Create and Directory.Create?
Move DriveInfo stuff to FileSystemfile system class
AlwaysCache file system property
IPath instead of Path?
Robustness design for the API
Replace File.Extension with File.Extensions.
CreateDirectory/CreateFile with option to supporting folder structure first?
Allow locking: ILockable - Lock, Unlock, IsLocked
  - Allow Faux Locking?
  - Interface or abstract?
Rename InMemory file system to Virtual File system. Retain InMemory files and add virtual files that allow other file types to be children (loose-typed > wrapper, if possible)


PERFORMANCE TUNE LAST!
Consider using a List<> instead of Array for path. Would this mean fewer allocations for traversal (MoveUp, etc?)
Intern strings for OsFileSystem.GetFiles()
Optimize GetFiles and GetDirectories for OS

# .NET Integration Notes
Might want to rename `File` and `Directory`. Recommendation is something like `FilePointer` of `FileReference`, reflecting that the classes are disconnected views of the resources they manage.


# Design Decisions
Static factory methods exist for any constructors that might need to do work beyond initializing state.