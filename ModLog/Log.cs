using System;
using System.Collections.Concurrent;

namespace DarkLog
{
    public class ModLog
    {
        private string modName;
        private long realtimeEpoch;
        private ConcurrentQueue<string> messages = new ConcurrentQueue<string>();

        public ModLog(string modName)
        {
            realtimeEpoch = DateTime.UtcNow.Ticks - (long)(UnityEngine.Time.realtimeSinceStartup * TimeSpan.TicksPerSecond);
            this.modName = modName;
        }

        public void Log(string message)
        {
            UnityEngine.Debug.Log(PrependText(message));
        }

        public void LogThread(string message)
        {
            messages.Enqueue(PrependText(message));
        }

        /// <summary>
        /// If using threads, this will flush the message cache to log. Call from Update and FixedUpdate.
        /// </summary>
        public void Update()
        {
            while (messages.TryDequeue(out string message))
            {
                UnityEngine.Debug.Log(PrependText(message));
            }
        }

        private string PrependText(string message)
        {
            return $"{GetTime()} [{modName}] {message}";
        }

        //RealTimeSinceStartup is not thread safe... somehow.
        private float GetTime()
        {
            return (DateTime.UtcNow.Ticks - realtimeEpoch) / (float)TimeSpan.TicksPerSecond;
        }
    }
}
