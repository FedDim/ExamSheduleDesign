using System;
using System.IO;

namespace ExamSheduleDesign.Services.Logging
{
    public class FileLogger : IAppLogger
    {
        private readonly string _logPath;

        public FileLogger()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string logFolder = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logFolder);
            _logPath = Path.Combine(logFolder, "app.log");
        }

        public void Info(string message) => Log("INFO", message);
        public void Warning(string message) => Log("WARN", message);
        public void Error(string message, Exception? ex = null)
        {
            Log("ERROR", message);
            if (ex != null)
                Log("ERROR", ex.ToString());
        }

        private void Log(string level, string message)
        {
            string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            File.AppendAllText(_logPath, line + Environment.NewLine);
        }
    }
}
