// Logger.cs
// compile with: /doc:Logger.xml

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Server.Logging
{
    /// <summary>
    /// This class handles writing the logEntries queue out to disk asychronously using a delegate
    /// </summary>
    class Logger
    {
        /// <summary>
        /// The Asych delegate used to call asychLogPersist
        /// </summary>
        /// <param name="entries">This delegate takes a function as a parameter that takes a 
        /// reference to a generic queue
        /// </param>
        protected delegate void PersistLogDelegate(ref Queue<LogEntry> entries);

        /// <summary>
        /// dumpLogToDisk creates the deletgate and class BeginInvoke as 
        /// fire & forget
        /// </summary>
        /// <param name="entries">A reference to the static logEntries queue</param>
        public static void dumpLogToDisk(ref Queue<LogEntry> entries)
        {
            PersistLogDelegate pld = new PersistLogDelegate(asychLogPersist);
            IAsyncResult result = pld.BeginInvoke(ref entries, null, null);

        }

        /// <summary>
        /// This function writes the logEntries out to disk
        /// LogManager.setWritingFinished is called in the finally clause
        /// TODO: Fix hardcoded log file name
        /// </summary>
        /// <param name="entries">A reference to the static logEntries queue</param>
        /// <seealso cref="LogManger.setWritingFinished()"/>
        private static void asychLogPersist(ref Queue<LogEntry> entries)
        {
            IEnumerator<LogEntry> logEnum = null;
            StreamWriter writer = null;
            String logFile = "";

            try
            {
                logFile = Path.Combine(Core.BaseDirectory, "Logs") + "\\targetLog.txt";
                writer = new StreamWriter(logFile, true);
                logEnum = entries.GetEnumerator();
                while(logEnum.MoveNext())
                    writer.WriteLine(logEnum.Current.ToString());
                entries.Clear();
            }
            finally
            {
                if(writer != null)
                    writer.Close();
                LogManager.setWritingFinished();
            }
        }
    }
}
