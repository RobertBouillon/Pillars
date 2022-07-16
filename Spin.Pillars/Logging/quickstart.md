# Quick Start

Logging should be simple. The most basic form of logging is acheived with the `Log` class.

```csharp
Log.Write("Hello World!")
```

String interpolation is an important part of logging.

```csharp
int age = 30;
Log.Write("I am {age} years old!", age);
```

The `Log.Writer` is called any time a log is written. By default, this writes to the `Console`.
