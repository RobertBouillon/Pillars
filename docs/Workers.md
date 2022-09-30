# Workers

A worker class represents an operation that runs continuously until stopped. `Work` is called continuously, depending on the type of worker, until `Stop` is called on the worker.

Workers themselves are designed to operate on a single logical thread. While each invokation of `Work` may spawn additional threads or tasks, the worker itself operates serially.

## Concepts
Each worker must know:
1. How to wait for work
2. What to do for work
3. How to cancel work
4. How to gracefully shut down

Each of these is defined by a combination of Worker Type and concrete implementation.

### How to wait for work
A worker should be idle when no work is ready to be performed. Signaling the worker that work is ready can be tricky, especially if the worker needs to be stopped while waiting for work.

### What to do for work
This is almost always defined by the concrete implementation, though _how_ the work is performed can be determined by the underlying worker (for exmaple, synchronously or within a **Task**).

### How to cancel work
When a worker is stopped, it can optionally complete its current work or cancel it. How the work is cancelled depends on both the implementation and the type of worker. Th

### How to gracefully shut down
A worker my not respond to requests to shut down (the other thread cannot be directly controlled or guaranteed to be killed in .NET). This situation may require special handling depending on the type of worker or implementation.



## Differences from Tasks
The Task class represents a single operation that does not return a value and that usually executes asynchronously. ([reference](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task))
A worker class represents an operation that runs continuously until stopped.

A worker is complementary to a Task, and can be executed in a synchronous or asynchronous context.


### Errors
- When an Exception occurs in a Task
  - The Exception can be swallowed by the framework unless a closure has been specified.
  - The Task will terminate
- When an Exception occurs in a Worker
  - The Exception is raised to the `Error` event
  - The Worker continues operating

# Worker Types
An abstract worker class.

## Timed Worker
`Work` is performed at specific intervals.

## Queue Worker
`Work` is performed once (serially & sequentially) for every item added to the queue. The internal queue is a `ConcurrentQueue`.

## Circular Queue Worker
`Work` is performed once (serially & sequentially) for every item added to the queue. The internal queue is a fixed-size Circular Queue.

# Worker Implementations

## TCP Connection Worker
Raises `ClientConnected` by working a single instance of `TcpListener`. This example demonstrates a Worker which does not manage its own thread or task, but relies on an asynchronous callback to raise the event.

NOTE: This was implemented using both Task and IAsyncResult patterns, and the latter was superior in that it was simpler and didn't rely on Exceptions for messaging. The Task implementation is excluded from the project,
but still available as a file named `TcpConnectionWorker_Task.cs`.

## Directory Worker
Watches a directory for new files and performs `Work` on any file that matches the `Filter`.
