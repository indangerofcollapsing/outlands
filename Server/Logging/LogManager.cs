// LogManager.cs
// compile with: /doc:LogManager.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Logging
{
    /// <summary>
    /// This class is responsible for handing incoming log messages
    /// </summary>
    /// <remarks>
    /// The LogManager class is used to store incoming log messages.  Messages
    /// are stored in the logEntries queue until the size of the queue passes a 
    /// threshold at which point the LogEntries are written to disk.  While the Logger
    /// class is writing the log entries to disk, any incoming messages are stored in the 
    /// overflow queue.
    /// </remarks>
    public class LogManager
    {

        protected static System.Collections.Generic.Queue<LogEntry> logEntries = new Queue<LogEntry>();
        protected static System.Collections.Generic.Queue<LogEntry> overFlow = new Queue<LogEntry>();
        protected static bool writingToDisk = false;

        /// <summary>
        /// If the Logger is in the process of writing the logEntries queue out to disk
        /// the LogEntry will be added to overflow, if not, the LogEntry is added to 
        /// logEntries and the size of the queue is checked.  If the logEntries queue size has
        /// passed the threshold (100 entries in this case), Logger.dumpLogToDisk is called
        /// TODO: Fix hardcoded threshold value
        /// </summary>
        /// <param name="entry">The LogEntry being added to the queue</param>
        /// <seealso cref="Logger.dumpLogToDisk(ref Queue<LogEntry>)"/>
        public static void addMessage(LogEntry entry)
        {

            /*if (!writingToDisk)
            {
                logEntries.Enqueue(entry);
                if (logEntries.Count > 100)
                {
                    writingToDisk = true;
                    Logger.dumpLogToDisk(ref logEntries);
                }              
            }
            else
            {
                overFlow.Enqueue(entry);
            }*/
        }

        /// <summary>
        /// This function is called when Logger finishes writing the log to disk.
        /// It sets writingToDisk = false and then moves any LogEntries in the overflow
        /// queue to the logEntries queue.  Upon completion it clears the overflow queue
        /// </summary>
        public static void setWritingFinished()
        {
            writingToDisk = false;
            IEnumerator<LogEntry> queueEnum = overFlow.GetEnumerator();
            while (queueEnum.MoveNext())
                logEntries.Enqueue(queueEnum.Current);
            overFlow.Clear();
        }

    }
}
