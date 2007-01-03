using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging.Simple;
using Common.Logging;
using System.Collections.Specialized;
namespace EmcasterTest
{
    public class Startup
    {
        public static void Init()
        {
            ConfigureLogging();
        }

        public byte[] ToBytes(string data)
        {
            return new UTF8Encoding().GetBytes(data);
        }


        public static void ConfigureLogging()
        {
            NameValueCollection vals = new NameValueCollection();
            vals["level"] = "info";
            ConsoleOutLoggerFactoryAdapter logger = new ConsoleOutLoggerFactoryAdapter(vals);
            LogManager.Adapter = logger;
            ILog log = LogManager.GetLogger(typeof(Startup));
            System.AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs exc)
            {
                log.Error(exc.ExceptionObject);
            };
        }

     
    }
}
