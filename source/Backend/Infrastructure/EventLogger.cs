using System;
using System.Diagnostics;

namespace no.miles.at.Backend.Infrastructure
{
    public class EventLogger : ILog
    {
        private readonly EventLog _eventLogger;

        public EventLogger(string source, string log)
        {
            _eventLogger = new EventLog();
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(
                    source, log);
            }
            _eventLogger.Source = source;
            _eventLogger.Log = log;
        }

        public void Debug(string message)
        {
            var log = string.Format("Debug: {0}", message);
            _eventLogger.WriteEntry(log, EventLogEntryType.Information);
        }

        public void Debug(string message, Exception ex)
        {
            var log = string.Format("Debug: {0}. Exception: {1}", message, ex);
            _eventLogger.WriteEntry(log, EventLogEntryType.Information);
        }

        public void Error(string message)
        {
            var log = string.Format("Error: {0}", message);
            _eventLogger.WriteEntry(log, EventLogEntryType.Error);
        }

        public void Error(string message, Exception ex)
        {
            var log = string.Format("Error: {0}. Exception: {1}", message, ex);
            _eventLogger.WriteEntry(log, EventLogEntryType.Error);
        }

        public void Info(string message)
        {
            var log = string.Format("Info: {0}", message);
            _eventLogger.WriteEntry(log, EventLogEntryType.Information);
        }

        public void Info(string message, Exception ex)
        {
            var log = string.Format("Info: {0}. Exception: {1}", message, ex);
            _eventLogger.WriteEntry(log, EventLogEntryType.Information);
        }

        public void Warn(string message)
        {
            var log = string.Format("Warning: {0}", message);
            _eventLogger.WriteEntry(log, EventLogEntryType.Warning);
        }

        public void Warn(string message, Exception ex)
        {
            var log = string.Format("Warning: {0}. Exception: {1}", message, ex);
            _eventLogger.WriteEntry(log, EventLogEntryType.Warning);
        }
    }
}
