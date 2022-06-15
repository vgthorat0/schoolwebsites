using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities
{
    using System;
    using System.Globalization;
    using System.IO;

    using log4net.Appender;
    using log4net.Core;
    using log4net.Util;

    /// <summary>
    /// Custom appender that creates one file each day
    /// </summary>    
    public class DailyAppender : FileAppender
    {
        #region Static Members

        /// <summary>
        /// The 1st of January 1970 in UTC
        /// </summary>
        private static readonly DateTime SDate1970 = new DateTime(1970, 1, 1);

        #endregion

        #region Private Instance Fields

        /// <summary>
        /// This object supplies the current date/time. 
        /// </summary>
        private readonly IDateTime _dateTime;

        /// <summary>
        /// Extension for the log file
        /// </summary>
        private string _extension = @".log";

        /// <summary>
        /// Prefix for the log file
        /// </summary>
        private string _prefix = "LOG";

        /// <summary>
        /// The date pattern. By default, the pattern is set to <c>".yyyyMMdd"</c> 
        /// meaning daily rollover.
        /// </summary>
        private string _datePattern = "yyyyMMdd";

        /// <summary>
        /// The actual formatted filename that is currently being written to
        /// or will be the file transferred to on roll over.
        /// </summary>
        private string _scheduledFilename;

        /// <summary>
        /// The timestamp when we shall next re-compute the filename.
        /// </summary>
        private DateTime _nextCheck = DateTime.MaxValue;

        /// <summary>
        /// Holds date of last roll over
        /// </summary>
        private DateTime _now;

        /// <summary>
        /// The type of rolling done
        /// </summary>
        private RollPoint _rollPoint;

        /// <summary>
        /// FileName provided in configuration(the root).  Used for rolling properly
        /// </summary>
        private string _baseFileName;

        #endregion Private Instance Fields

        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyAppender"/> class.
        /// </summary>
        public DailyAppender()
        {
            _dateTime = new DefaultDateTime();
        }

        #endregion Public Instance Constructors

        #region Protected Enums

        /// <summary>
        /// How to roll the log
        /// </summary>
        protected enum RollPoint
        {
            /// <summary>
            /// Invalid roll
            /// </summary>
            InvalidRollPoint = 0,

            /// <summary>
            /// Roll the log each day (midnight)
            /// </summary>
            TopOfDay = 1
        }

        #endregion Protected Enums

        #region DateTime Interface

        /// <summary>
        /// This interface is used to supply Date/Time information to the <see cref="RollingFileAppender"/>.
        /// </summary>
        private interface IDateTime
        {
            /// <summary>
            /// Gets the <i>current</i> time.
            /// </summary>
            /// <value>The <i>current</i> time.</value>
            /// <remarks>
            /// <para>
            /// Gets the <i>current</i> time.
            /// </para>
            /// </remarks>
            DateTime Now { get; }
        }

        #endregion

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the date pattern
        /// (by default is set to 'yyyyMMdd' meaning daily rollover).
        /// </summary>
        public string DatePattern
        {
            get { return _datePattern; }
            set { _datePattern = value; }
        }

        /// <summary>
        /// Gets or sets the extension for log file
        /// </summary>
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        /// <summary>
        /// Gets or sets the prefix for log file
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        #endregion Public Instance Properties

        #region Static methods

        /// <summary>
        /// Change the appender file
        /// </summary>
        /// <param name="appenderName">Name of appender to change file</param>
        /// <param name="newFileName">New file name</param>
        /// <returns>Returns true if the appender was found and name changed to newFileName/>.
        /// Return false if the appender was not found</returns>
        public static bool ChangeLogFileName(string appenderName, string newFileName)
        {
            var rootRep = log4net.LogManager.GetRepository();

            foreach (var fileAppender in from app in rootRep.GetAppenders() let fileAppender = app as FileAppender where string.Compare(app.Name, appenderName, StringComparison.Ordinal) == 0 && (fileAppender != null) select fileAppender)
            {
                fileAppender.File = newFileName;

                fileAppender.ActivateOptions();

                return true;  // appender found and name changed to newFilename
            }

            return false; // appender not found
        }

        #endregion

        #region Public Override implementation of FileAppender

        /// <summary>
        /// Initialize the appender based on the options set
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// <para>
        /// Sets initial conditions including date/time roll over information, first check,
        /// scheduledFilename, and calls <see cref="ExistingInit"/> to initialize
        /// the current number of backups.
        /// </para>
        /// </remarks>
        public override void ActivateOptions()
        {
            if (_datePattern != null)
            {
                _now = _dateTime.Now;
                _rollPoint = ComputeCheckPeriod(_datePattern + _extension);

                if (_rollPoint == RollPoint.InvalidRollPoint)
                {
                    throw new ArgumentException("Invalid RollPoint, unable to parse [" + _prefix + _datePattern +
                                                _extension + "]");
                }

                // next line added as this removes the name check in rollOver
                _nextCheck = NextCheckDate(_now, _rollPoint);
            }
            else
            {
                ErrorHandler.Error("Either DatePattern or rollingStyle options are not set for [" + Name + "].");
            }

            if (SecurityContext == null)
            {
                SecurityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
            }

            using (SecurityContext.Impersonate(this))
            {
                // Must convert the FileAppender's mFilePath to an absolute path before we
                // call ExistingInit(). This will be done by the base.ActivateOptions() but
                // we need to duplicate that functionality here first.
                File = ConvertToFullPath(File.Trim());

                // Store fully qualified base file name
                _baseFileName = File;
            }

            if (File != null && _scheduledFilename == null)
            {
                var uniqueIdWithExtension = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}",
                    _now.ToString(_datePattern, DateTimeFormatInfo.InvariantInfo),
                    _extension);
                _scheduledFilename = string.Concat(File, uniqueIdWithExtension);
            }

            ExistingInit();

            base.ActivateOptions();
        }

        #endregion

        #region NextCheckDate

        /// <summary>
        /// Get the start time of the next window for the current roll point
        /// </summary>
        /// <param name="currentDateTime">the current date</param>
        /// <param name="rollPoint">the type of roll point we are working with</param>
        /// <returns>the start time for the next roll point an interval after the currentDateTime date</returns>
        /// <remarks>
        /// <para>
        /// Returns the date of the next roll point after the currentDateTime date passed to the method.
        /// </para>
        /// <para>
        /// The basic strategy is to subtract the time parts that are less significant
        /// than the roll point from the current time. This should roll the time back to
        /// the start of the time window for the current roll point. Then we add 1 window
        /// worth of time and get the start time of the next window for the roll point.
        /// </para>
        /// </remarks>
        protected static DateTime NextCheckDate(DateTime currentDateTime, RollPoint rollPoint)
        {
            // Local variable to work on (this does not look very efficient)
            DateTime current = currentDateTime;

            // Do slightly different things depending on what the type of roll point we want.
            if (rollPoint == RollPoint.TopOfDay)
            {
                current = current.AddMilliseconds(-current.Millisecond);
                current = current.AddSeconds(-current.Second);
                current = current.AddMinutes(-current.Minute);
                current = current.AddHours(-current.Hour);
                current = current.AddDays(1);
            }

            return current;
        }

        #endregion

        #region Protected Override implementation of FileAppender

        /// <summary>
        /// Sets the quiet writer being used.
        /// </summary>
        /// <param name="writer">the writer to set</param>
        protected override void SetQWForFiles(TextWriter writer)
        {
            QuietWriter = new CountingQuietTextWriter(writer, ErrorHandler);
        }

        /// <summary>
        /// Write out a logging event.
        /// </summary>
        /// <param name="loggingEvent">the event to write to file.</param>
        /// <remarks>
        /// <para>
        /// Handles append time behavior for RollingFileAppender.  This checks
        /// if a roll over by date 
        /// is need and then appends to the file last.
        /// </para>
        /// </remarks>
        protected override void Append(LoggingEvent loggingEvent)
        {
            AdjustFileBeforeAppend();
            base.Append(loggingEvent);
        }

        /// <summary>
        /// Write out an array of logging events.
        /// </summary>
        /// <param name="loggingEvents">the events to write to file.</param>
        /// <remarks>
        /// <para>
        /// Handles append time behavior for RollingFileAppender.  This checks
        /// if a roll over by date 
        /// is need and then appends to the file last.
        /// </para>
        /// </remarks>
        protected override void Append(LoggingEvent[] loggingEvents)
        {
            AdjustFileBeforeAppend();
            base.Append(loggingEvents);
        }

        /// <summary>
        /// Creates and opens the file for logging. 
        /// </summary>
        /// <param name="fileName">the name of the file to open</param>
        /// <param name="append">true to append to existing file</param>
        /// <remarks>
        /// <para>This method will ensure that the directory structure
        /// for the <paramref name="fileName"/> specified exists.</para>
        /// </remarks>
        protected override void OpenFile(string fileName, bool append)
        {
            lock (this)
            {
                fileName = GetNextOutputFileName(fileName);

                if (!append)
                {
                    if (LogLog.IsErrorEnabled)
                    {
                        // Internal check that the file is not being overwritten
                        // If not Appending to an existing file we should have rolled the file out of the
                        // way. Therefore we should not be over-writing an existing file.
                        // The only exception is if we are not allowed to roll the existing file away.
                        if (FileExists(fileName))
                        {
                            LogLog.Error(
                                typeof(DailyAppender),
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "RollingFileAppender: INTERNAL ERROR. Append is False but OutputFile [{0}] already exists.",
                                    fileName));
                        }
                    }
                }

                _scheduledFilename = fileName;

                // Open the file (call the base class to do it)
                base.OpenFile(fileName, append);
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Performs any required rolling before outputting the next event
        /// </summary>
        /// <remarks>
        /// <para>
        /// Handles append time behavior for RollingFileAppender.  This checks
        /// if a roll over by date
        /// is need and then appends to the file last.
        /// </para>
        /// </remarks>
        protected virtual void AdjustFileBeforeAppend()
        {
            DateTime currentDateAndTime = _dateTime.Now;
            if (currentDateAndTime >= _nextCheck)
            {
                _now = currentDateAndTime;
                _nextCheck = NextCheckDate(_now, _rollPoint);

                RolloverTime(true);
            }
        }

        /// <summary>
        /// Get the current output file name
        /// </summary>
        /// <param name="fileName">the base file name</param>
        /// <returns>the output file name</returns>
        /// <remarks>
        /// The output file name depends on the date pattern.
        /// </remarks>
        protected string GetNextOutputFileName(string fileName)
        {
            fileName = fileName.Trim();
            var prefixWithUniqueIdAndExtension = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                _prefix,
                _now.ToString(_datePattern, DateTimeFormatInfo.InvariantInfo),
                _extension);
            fileName = Path.Combine(fileName, prefixWithUniqueIdAndExtension);

            return fileName;
        }

        #region Roll File

        /// <summary>
        /// Rollover the file(s) to date/time tagged file(s).
        /// </summary>
        /// <param name="fileIsOpen">set to true if the file to be rolled is currently open</param>
        /// <remarks>
        /// <para>
        /// Rollover the file(s) to date/time tagged file(s).
        /// If fileIsOpen is set then the new file is opened (through SafeOpenFile).
        /// </para>
        /// </remarks>
        protected void RolloverTime(bool fileIsOpen)
        {
            // new scheduled name
            var prefixWithUniqueIdAndExtension = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                _prefix,
                _now.ToString(_datePattern, DateTimeFormatInfo.InvariantInfo),
                _extension);
            _scheduledFilename = string.Concat(File, prefixWithUniqueIdAndExtension);

            if (fileIsOpen)
            {
                // This will also close the file. This is OK since multiple close operations are safe.
                SafeOpenFile(_baseFileName, false);
            }
        }

        /// <summary>
        /// Test if a file exists at a specified path
        /// </summary>
        /// <param name="path">the path to the file</param>
        /// <returns>true if the file exists</returns>
        /// <remarks>
        /// <para>
        /// Test if a file exists at a specified path
        /// </para>
        /// </remarks>
        protected bool FileExists(string path)
        {
            using (SecurityContext.Impersonate(this))
            {
                return System.IO.File.Exists(path);
            }
        }

        #endregion

        #endregion

        #region Initialize Options

        /// <summary>
        /// Initializes based on existing conditions at time of <see cref="ActivateOptions"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Initializes based on existing conditions at time of <see cref="ActivateOptions"/>.
        /// The following is done
        /// <list type="bullet">
        /// <item>determine curSizeRollBackups (only within the current roll point)</item>
        /// <item>initiates a roll over if needed for crossing a date boundary since the last run.</item>
        /// </list>
        /// </para>
        /// </remarks>
        protected void ExistingInit()
        {
            // If file exists and we are not appending then roll it out of the way
            if (AppendToFile == false)
            {
                // bool fileExists = false;
                string fileName = GetNextOutputFileName(_baseFileName);

                using (SecurityContext.Impersonate(this))
                {
                    // fileExists = 
                    System.IO.File.Exists(fileName);
                }
            }
        }

        /// <summary>
        /// Calculates the RollPoint for the datePattern supplied.
        /// </summary>
        /// <param name="datePattern">the date pattern to calculate the check period for</param>
        /// <returns>The RollPoint that is most accurate for the date pattern supplied</returns>
        /// <remarks>
        /// Essentially the date pattern is examined to determine what the
        /// most suitable roll point is. The roll point chosen is the roll point
        /// with the smallest period that can be detected using the date pattern
        /// supplied. i.e. if the date pattern only outputs the year, month, day 
        /// and hour then the smallest roll point that can be detected would be
        /// and hourly roll point as minutes could not be detected.
        /// </remarks>
        private static RollPoint ComputeCheckPeriod(string datePattern)
        {
            // s_date1970 is 1970-01-01 00:00:00 this is UniversalSortableDateTimePattern 
            // (based on ISO 8601) using universal time. This date is used for reference
            // purposes to calculate the resolution of the date pattern.

            // Get string representation of base line date
            string r0 = SDate1970.ToString(datePattern, DateTimeFormatInfo.InvariantInfo);

            // Check each type of rolling mode starting with the smallest increment.
            string r1 = NextCheckDate(SDate1970, RollPoint.TopOfDay).ToString(
                datePattern, DateTimeFormatInfo.InvariantInfo);

            // Check if the string representations are different
            if (!r0.Equals(r1))
            {
                // Found highest precision roll point
                return RollPoint.TopOfDay;
            }

            return RollPoint.InvalidRollPoint; // Deliberately head for trouble...
        }

        #endregion

        #region DateTime

        /// <summary>
        /// Default implementation of <see cref="IDateTime"/> that returns the current time.
        /// </summary>
        private class DefaultDateTime : IDateTime
        {
            /// <summary>
            /// Gets the <b>current</b> time.
            /// </summary>
            /// <value>The <b>current</b> time.</value>
            /// <remarks>
            /// <para>
            /// Gets the <b>current</b> time.
            /// </para>
            /// </remarks>
            public DateTime Now
            {
                get { return DateTime.Now; }
            }
        }

        #endregion DateTime
    }
}
