using System.IO;
using log4net.Appender;
using log4net.spi;
using Sitecore.Configuration;

namespace Sitecore.Playground.Logging
{
    public class CustomSitecoreLogFileAppender : SitecoreLogFileAppender
    {
        public Level Level
        {
            get
            {
                var filePath = Path.Combine(Settings.DataFolder, "log4net");
                if (System.IO.File.Exists(filePath))
                {
                    var levelStr = System.IO.File.ReadAllText(filePath);
                    return LogLevelMap.GetLogLevel(levelStr);
                }
                return null;
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (Level != null)
            {
                var currentLogLevel = loggingEvent.Level;
                if (currentLogLevel >= Level)
                {
                    base.Append(loggingEvent);
                }
            }
            else
            {
                base.Append(loggingEvent);
            }
        }
    }
}