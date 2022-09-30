# Logging

There are N things to consider when designing a logging system

- Creation
  - Performance
  - Configuration
    - Persistence
  - Maintenance
    - Archival
- Reading
  - Streaming
  - Searching
- Management
  - Alerts
  - States
- Counters



# Problems to solve

- Every piece of code can potentially write a log. How to categorize without being onerus? 
- How to categorize logs from third parties into a hierarchical system?