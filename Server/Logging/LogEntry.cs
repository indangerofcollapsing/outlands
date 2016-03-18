// LogEntry.cs
// compile with: /doc:LogEntry.xml

using System;
using System.Collections.Generic;
using System.Text;


namespace Server.Logging
{
    /// <summary>
    /// Simple data object for holding Log Entry information
    /// </summary>
    /// <remarks>
    /// Currently this object is only storing four properties but this class could be
    /// reworked and subclassed to handle a wider range of logging events.
    /// </remarks>
    public class LogEntry
    {
        protected string entryType;  
        protected string actorName; 
        protected string targetName;  
        protected DateTime time;
  

        /// <summary>
        /// Empty constructor for convenience purposes, currently unused
        /// </summary>
        public LogEntry() { }

        /// <summary>
        /// This simple constructor maps the arguments directly to the class member variables
        /// </summary>
        /// <param name="entryType">type of log entry</param>
        /// <param name="actorName">the mobile name peforming the action</param>
        /// <param name="targetName">the mobile on which the action is being performed.</param>
        /// <param name="time">the time the log event occured</param>
        public LogEntry(string entryType, string actorName, string targetName, DateTime time)
        {
            this.entryType = entryType;
            this.actorName = actorName;
            this.targetName = targetName;
            this.time = time;
        }

        #region Getters & Setters

        public string EntryType
        {
            get
            {
                return entryType;
            }
            set
            {
                entryType = value;
            }
        }

        public string Actor
        {
            get
            {
                return actorName;
            }
            set
            {
                actorName = value;
            }
        }

        public string Target
        {
            get
            {
                return targetName;
            }
            set
            {
                targetName = value;
            }
        }

        public DateTime Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }


        #endregion

        /// <summary>
        /// Overridden ToString is called during the logging process by Logger
        /// </summary>
        /// <returns>the string representation of this log entry duh</returns>
        /// <seealso cref="Logger"/>
        public override string  ToString()
        {
            return String.Format("Mobile: {0} performed action {1} on Mobile {2} at time: {3}", actorName, entryType, targetName, time.ToShortDateString() + " " + time.ToLongTimeString() + " " + time.Millisecond + "ms");

        }
    }
}
