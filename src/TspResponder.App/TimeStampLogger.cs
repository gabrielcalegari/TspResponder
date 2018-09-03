using Microsoft.Extensions.Logging;
using TspResponder.Core;

namespace TspResponder.App
{
    public class TimeStampLogger : ITimeStampLogger
    {
        public void Debug(string message)
        {
            Logger.LogDebug(message);
        }

        public void Error(string message)
        {
            Logger.LogError(message);
        }

        public void Warn(string message)
        {
            Logger.LogWarning(message);
        }

        private ILogger<TimeStampLogger> Logger { get; }

        public TimeStampLogger(ILogger<TimeStampLogger> logger)
        {
            Logger = logger;
        }
    }
}
