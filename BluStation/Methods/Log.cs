using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace Blu
{
    public static partial class Method
    {
        /// <summary>
        /// Log method, prints to console and saves the log line to ./BluStation.log
        /// </summary>
        /// <param name="caption">caption of the log, e.g "error", "warn" or "ok"</param>
        /// <param name="content">log line</param>

        public static void Log(string caption, string content)
        {
            Logger.log(caption, content);
        }
    }
}
