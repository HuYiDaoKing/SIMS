using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMS.Common
{
    public class LogHelper
    {
        private static string logFolderPath = AppDomain.CurrentDomain.BaseDirectory + "WorkLog/";
        private static object privateObjectLock = new object();

        /// <summary>
        /// 验证是否需要打日志
        /// </summary>
        /// <param name="logType"></param>
        /// <returns></returns>
        private static bool ValidateNeedWriteLog(string logType)
        {
            string configKey = string.Format("Log{0}", logType);
            try
            {
                if (ConfigurationManager.AppSettings[configKey] == "0")
                    return false;
                else
                    return true;
            }
            catch
            {
                return true;//默认需要打日志
            }
        }


        /// <summary>
        /// 写日志方法
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logMessage">日志信息</param>
        public static void Log(string logType, string logMessage)
        {
            if (logType != LogType.Error && logType != LogType.Info && logType != LogType.Warn)
                return;
            if (!ValidateNeedWriteLog(logType)) return;//如果不需要打日志 直接返回
            string logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string writeInfo = string.Format("[{0}]:{1} ----{2}\r\n", logType, logMessage, logTime);
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            string filePath = string.Format("{0}{1}Log{2}.log", logFolderPath, logType, currentDate);
            if (!Directory.Exists(logFolderPath))
                Directory.CreateDirectory(logFolderPath);
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            try
            {
                lock (privateObjectLock)
                {
                    StreamWriter sw = new StreamWriter(filePath, true);
                    sw.WriteLine(writeInfo);
                    sw.Close();
                }

            }
            catch (Exception)
            {

            }
        }
    }

    public class LogType
    {
        public static string Info = "Info";
        public static string Warn = "Warn";
        public static string Error = "Error";
    }
}
