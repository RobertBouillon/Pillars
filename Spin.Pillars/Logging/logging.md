# Usage Guidelines & Examples

## 



# Concepts

Event logs record events taking place in the execution of a system in order to provide an audit trail that can be used to understand the activity of the system and to diagnose problems. 
-- [Wikipedia](https://en.wikipedia.org/wiki/Log_file)

Unlike most event logging systems that focus on text-based logging, this system stores events as log entries of arbitrary data, defined only be a timestamp created at the time the event occurred. 
Event data that is generated is binary and decoupled from any form of persistence. If a human-readable text file is the desired output of for an Event Log, a Log Writer can be configured to read
the data stream and generate a standard text-log. However, it is recommended to persist event logs in binary format to preserve the ability to analyze the logs at a later date.

## Text-Based Log Entries
Text-based log entries represent the lowest-common-denominator of log data: they are inefficient to write and to read. Paired with typed metadata, text-based log entries become more usable because they can be
filtered on data less ambiguous than plain text. Many text-based log entries are human-readable records of state transitions, such as "System Started" and "System Stopped".

### String Interpolation 
String interpolation is not performed at the time the log entry is written. String interpolation is necessary for _analyzing_ the logs, not _writing_ them, thus it is 
function that is deferred to a **Log Parser**. 

## Metadata 
Metadata can point to the source code that generated the log entry, identify related database entities, or include the detailed stack trace of a related exception.
This library includes facilities for these and other valuable metadata while leaving open the ability for developers to extend typed metadata for their particular application.


This system focuses on clearly dividing the act of recording a log from reading the log because both have different requirements. Recording a log of events must have a minimal impact on system performance. Log entries
should contain only the data necessary to perform analysis deemed necessary for the operation of the system.


# Design Objectives

1. Provide a high-performance logging module that doesn't discourage developers from writing trace log entries in fear of performance concerns
2. Provide a very abstract interface for recording data. Let the application figure out how to best use that data for analysis.

# Detailed Design

## Data

A log can contain the following information:

1. Text:      A human-readable text message, optionally interpolated with contextual data.
2. Labels:    Contextual meta-data as a string. Arbitrary information describing the context of the log entry.
3. Tags:      Contextual meta-data as a [String/Object] Key-Value pair. Arbitrary information describing the context of the log entry. This could contain something as simple as a blittable type, like and ID, or an Exception object.
4. States:    Contextual meta-data as a [String/String] Key-Value pair. 
5. Source:    The name of the type from which the log entry originates
6. Time:      The precise time of the log entry

## Recommended Lables
1. Error:     A condition has occurred that is likely a software defect requiring a code change to resolve.

## Severity

Log severity is handled using tags. Different applications may require different severity levels, by requirement, preference, corporate standard, etc. Rather than trying to create a "one-size-fits-all"
enumeration, log severity is left undefined, but supported by the use of Labels or Tags.

Common severities include:

1. Debug:         Used only during run-time debugging. Entries with this tag are excluded from persistence writers by-default.
2. Normal:        An ordinary log entry. All log entries are normal unless otherwise indicated.
3. Notification:  A log entry indicitive of abnormal activity that should be distinguished from normal trace logs
4. Alert:         A stateful condition has started that requires administrative attention
5. Critical:      A stateful condition has started that requires administrative IMMEDIATE attention

These can be implemented as tags, labels, or some combination of both.

## State

Logs often contain messages which indicate a stateful transition. For example, "Backup Started" and "Backup Stopped". 

State Transitions support the following use-cases:

1. View the time delta between subsequent or specific states
2. View current state at the time of a given log entry.
3. Navigating log entries while vieweing by jumping between state transitions
4. Provide a reliable means for determining state transitions that doesn't require text parsing


# Feature List
1. State Tracking - Track individial states, state transitions, and accurrate time between transitions
2. Stateful Time (Clock Deltas) - Logs are most useful for debugging when their correlates with some real-world event. Thus, the most useful metric in an event log is an accurate time.
   For servers, this time is most often UTC, but it can also be local time. For desktop applications, the time is often local time. The time provided by the operating system can change,
   and often does. This can be due to daylight savings time or typical drift - computers can provide high-precision clocks, but they're almost always synchronized with an external time
   provider, which can result in time adjustments. These adjustments can cause many issues, from making a log more difficult to read and filter, to corrupting time deltas (e.g. time differences
   between state transitions). 
   
   For this reason, all event logs are tracked to an internal timer that starts at the local system time (this clock can be changed at startup). Any time changes that may be associated with
   traditional time-keeping, such as UTC, are tracked by hooking change events from the external time source. In the case of tracking to the OS time, a notification from the OS is requested
   when the time changes. This event is tracked in the log and used by a log reader to provide an accurate time that reflects the clock of the machine at the time the event occurred without
   losing that event's temporal position relative to other events.

# Scratch Notes
An operation is uniquely identified by its DATA when it's created (assumption. Let's see if it works).  

- Category

Sync Client
- Session (Host:X, Mirror:X)
  - Downloading {File}

Sync Host
- Session (IP:X)
  - Uploading {File}


# Patterns

## Operation -> Success / Fail
"Doing SOMETHING..."
"Error occurred doing SOMETHING"
"SOMETHING succeeded in 00:00.000"

## Stateful Operation
"Doing SOMETHING..."
"SOMETHING started..."
"SOMETHING paused..."
"SOMETHING resumed..."
"SOMETHING finished"
"SOMETHING failed"

## Persistent State Changed
"SOMETHING Status: Offline"
"SOMETHING Status: Online"


# Use-Cases

# Design

## Problems
1. *Multi-threaded Log Spam* - Log is usable / unreadable when logging is multi-threaded. Hard to correlate log entries (e.g. Activity started / ended/ failed)
2. *Correlating errors to related log entries* - When a failure occurs, limit the log entries to those related to the failure.
3. *Text-based searches* Can only perform text-based searches on text entries.
3a. Want to Parse log for alert states, generating tickets, etc
4. Logs are very large, which can cause performance / storage problems that cause logs to be disabled although they're useful
5. It's difficult to track application performance in production, but we have event logs. Why can't we use those?
6. Culture-specific logs. Logs should be stored culture-agnostic, if posssible, with values rendered to the local culture during reading




## Solutions
1. *Log Contexts* [Committed] - Logs are associated with a common context, allowing unrelated log messages to be filtered out.
### Solves
  1. *Multi-threaded Log Spam* - Logs on different threads will have different contexts
  2. *Correlating errors to related log entries*
2. *Binary Log Files*

### Challenges
### Opportunities



- [Opportunity]
- Obtain statistics of events in production
  - Machine-readable data storage
- API & data structure conducive to common patterns (listed above)


# TO-DO

Custom formatters for interpolater. Use-Case: "Uploading {FileCount} files ({FileSize})" where FileSize is a decimal or double, and we want to use FileSize to format it.
Tag API: Currently always have to cast value to object. See if this can be cleaned up. Params ILogMetaData?
When starting and stopping (state transitions), there really needs to be a unique ID. (Pass back reference that can be triggered?)


