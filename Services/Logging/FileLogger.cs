using System;
using System.IO;
using System.Threading;

namespace ExamSheduleDesign.Services.Logging
{
    public class FileLogger : IAppLogger
    {
        private readonly string _logPath;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private const long MaxLogSize = 5 * 1024 * 1024; // 5 MB

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
            _semaphore.Wait();
            try
            {
                RotateLogIfNeeded();
                string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                File.AppendAllText(_logPath, line + Environment.NewLine);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void RotateLogIfNeeded()
        {
            if (File.Exists(_logPath) && new FileInfo(_logPath).Length > MaxLogSize)
            {
                string archivePath = _logPath + ".1";
                if (File.Exists(archivePath))
                    File.Delete(archivePath);
                File.Move(_logPath, archivePath);
            }
        }
    }
}