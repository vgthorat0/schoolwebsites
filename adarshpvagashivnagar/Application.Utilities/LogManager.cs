using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities
{
    /// <summary>
    /// class used to generate loggers for components
    /// </summary>
    public static class LogManager
    {
        #region Constants

        //
        // UI LOGGERS NAMES
        //
        private const string AppLoggerName = "Application";
        
        //
        // SERVICES LOGGERS NAMES
        //
        private const string CacheLoggerName = "Cache";
        

        //
        // BUSINESS LOGGERS NAMES
        //
        private const string ComponentLoggerName = "Component";
       

        //
        // DAL LOGGERS NAMES
        //
        private const string DalLoggerName = "Dal";
      

        //
        // COMMON LOGGERS NAMES
        //
        private const string UtilitiesLoggerName = "Utilities";

        #endregion

        #region Static Constructor

        /// <summary>
        /// initialize all loggers
        /// </summary>
        static LogManager()
        {
            //InitializeLoggerConfiguration();
            XmlConfigurator.Configure();

            AppLogger = GetLogger(AppLoggerName);
           

          

            DalLogger = GetLogger(DalLoggerName);


            CacheLogger = GetLogger(CacheLoggerName);

            UtilitiesLogger = GetLogger(UtilitiesLoggerName);
            ComponentLogger = GetLogger(ComponentLoggerName);




        }



        #endregion

        #region Application Loggers

        /// <summary>
        /// get a logger instance for Send API
        /// </summary>
        public static ILog AppLogger { get; private set; }

        /// <summary>
        /// get a logger instance for Payer API 
        /// </summary>
        public static ILog DalLogger { get; private set; }

        /// <summary>
        /// get a logger instance for Mobile API 
        /// </summary>
        public static ILog CacheLogger { get; private set; }
        public static ILog ComponentLogger { get; private set; }
        #endregion

        #region Common Loggers


        ///// <summary>
        ///// get a logger instance for Security Manager
        ///// </summary>
        //public static log4net.ILog SecurityManagerLogger
        //{
        //    get
        //    {
        //        return SecurityComponentLogger;
        //    }
        //}

        /// <summary>
        /// get a logger instance for Utilities
        /// </summary>
        public static ILog UtilitiesLogger { get; private set; }

    

        #endregion

        #region Public Static Methods

        /// <summary>
        /// set session id for current thread
        /// </summary>
        /// <param name="sessionId">current session id</param>
        public static void SetSessionId(string sessionId)
        {
            ThreadContext.Properties["SessionId"] = sessionId;
        }

        /// <summary>
        /// get a logger for the specified name
        /// </summary>
        /// <param name="name">logger name</param>
        /// <returns>logger instance</returns>
        public static ILog GetLogger(string name)
        {
            return log4net.LogManager.GetLogger(name);
        }

        #endregion
    }
}