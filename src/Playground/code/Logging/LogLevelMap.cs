using System.Reflection;
using log4net.spi;

namespace Sitecore.Playground.Logging
{
    public static class LogLevelMap
    {
        static readonly LevelMap LevelMap = new LevelMap();

        static LogLevelMap()
        {
            foreach (FieldInfo fieldInfo in typeof(Level).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (fieldInfo.FieldType == typeof(Level))
                {
                    LevelMap.Add((Level)fieldInfo.GetValue(null));
                }
            }
        }

        public static Level GetLogLevel(string logLevel)
        {
            return string.IsNullOrWhiteSpace(logLevel) ? null : LevelMap[logLevel];
        }
    }
}