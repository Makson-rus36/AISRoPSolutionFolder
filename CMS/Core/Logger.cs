using System;
using System.IO;
using System.Windows;

namespace CMS.Core
{
    public class LogPath
    {
        public static string path =
            (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\log_" +
             DateTime.Now.ToString().Replace(":", "_") + ".txt").Replace(" ", "_");
    }

    public class LoggerHelper
    {
        public static Logger logger = new Logger();
    }

    public class Logger
    {
        public void startLog(object value)
        {
            try
            {
                var logText = DateTime.Now + " | " + value + "\n";
                File.AppendAllText(LogPath.path, logText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}