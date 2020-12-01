# Modules

Modules represent a scoped hierarchy of common diagnostic utilities for:

1. Logging
2. Performance Metrics
3. Configuration

Modules represent the logical scope of a component (module) within an application. This is important as it ensures
that logs in the application aren't simply dumped to a massive log file that must be sifted manually for errors.
Modules represent a logical organization of diagnostic information, including not only logs, but performance
metrics and run-time configuration.

## Logging

Logging is designed specifically to separate the processes of **recording** and **analyzing** logs. The process
of **recording** diagnostic information should not impact the performance of the application. 

This library aims 
to provide high-performance logging functionality such that __all__ logs can be written at run-time with no need
to restrict logging levels in production. That's not to say you *cannot* or *should not* restrict the logging
levels in production - only that it's not a constraint of the underlying logging framework.

The process of log writing runs on a separate thread as to minimally impact the main execution thread. Strings
are **not** interpolated / formatted on the logging thread. In fact, values are dumped directly to the log, sometimes 
in their native format (i.e. integers are written as 32-bits and not converted to string first). It's not necessary
to format strings at log **recording** - this is a requirement for log **analysis**, and is thus deferred.

## Metrics

Performance metrics follow the same paradigm as logging: all performance metrics are **recorded** as discrete 
values with no conversion or aggregation at the time of recording. Any aggregations of the data are deferred 
to the time of **analysis**.

## Configuration

The configuration classes provide an interface for configuring modules of an application at run-time. The 
objective of the configuration classes is to provide a common interface for configuring a software 
module at run-time, with no regard to how the module is configured. That is, a module can be configured by a 
JSON file, XML File, or Web Service. The module doesn't care how it is configure - it only needs to expose the 
properties that are configurable.

Note that this is similar to the [System.Configuration](https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configuration)
functionality provided by Microsoft. This is interoperable with the Microsoft library and is designed to supplement
the functionality, not replace it.