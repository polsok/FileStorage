using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace AD
{
    public enum Logs
    {
        /// <summary>
        /// Главный лог (ведутся записи запуска приложений и очистки логов)
        /// </summary>
        LogMain,
        /// <summary>
        /// Лог для записи сообщений
        /// </summary>
        LogJob,
        /// <summary>
        /// лог ошибок
        /// </summary>
        LogError,
        /// <summary>
        /// лог ошибок расширенный
        /// </summary>
        LogErrorExpanded
    }
    /// <summary>
    /// Работа с логами
    /// </summary>
    public class Log
    {
        /// <summary>
        /// путь к папке логов
        /// </summary>
        public static string PathToLog = Environment.CurrentDirectory + "\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "_log";

        /// <summary>
        /// лог ошибок
        /// </summary>
        public static string LogError = PathToLog + "\\LogError.log";
        /// <summary>
        /// лог ошибок расширенный
        /// </summary>
        public static string LogErrorExpanded = PathToLog + "\\LogErrorExpanded.log";
        /// <summary>
        /// Главный лог (ведутся записи запуска приложений и очистки логов)
        /// </summary>
        public static string LogMain = PathToLog + "\\LogMain.log";
        /// <summary>
        /// Лог для записи сообщений
        /// </summary>
        public static string LogJob = PathToLog + "\\LogJob.log";
        /// <summary>
        /// максимальный размер логов (кроме LogMain) по умолчанию 10 мб
        /// </summary>
        public static int MaxLength = 10000000;

        /// <summary>
        /// добавление записи в лог
        /// для добавления записи в LogErrorExtension использовать Ex.ex
        /// </summary>
        /// <param name="logs">в перечислении указать имя лог файла</param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static bool AddLog(Logs logs, string Message)
        {
            try
            {
                //проверяем что папка с логами существует
                if (!Directory.Exists(PathToLog))
                {
                    Directory.CreateDirectory(PathToLog);
                }

                FileInfo file;
                switch (logs)
                {
                    case Logs.LogJob:
                        file = new FileInfo(Log.LogJob);
                        if (file.Exists && file.Length > MaxLength)
                        {
                            File.AppendAllText(Log.LogMain,
                                DateTime.Now.ToString("U") + "\t" + Environment.MachineName + "\t" +
                                Environment.UserName + "\tClear LogJob\n");
                            file.Delete();
                        }

                        if (!file.Exists)
                        {
                            File.AppendAllText(Log.LogJob, "Date\t\t\t\t\t\tMashine\tUsername\t\tMessage\n");
                        }

                        File.AppendAllText(Log.LogJob,
                            DateTime.Now.ToString("U") + "\t" + Environment.MachineName + "\t" + Environment.UserName +
                            "\t" + Message + "\n");
                        Console.WriteLine(Message);
                        break;
                    case Logs.LogMain:
                        file = new FileInfo(Log.LogMain);
                        if (!file.Exists)
                        {
                            File.AppendAllText(Log.LogMain, "Date\t\t\t\t\t\tMashine\tUsername\t\tMessage\n");
                        }
                        File.AppendAllText(Log.LogMain,
                            DateTime.Now.ToString("U") + "\t" + Environment.MachineName + "\t" + Environment.UserName +
                            "\t" + Message + "\n");
                        Console.WriteLine(Message);
                        break;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
                return false;
            }
        }

        /// <summary>
        /// обработка исключений
        /// </summary>
        public static void Exception(string Message)
        {
            //проверяем что папка с логами существует
            if (!Directory.Exists(PathToLog))
            {
                Directory.CreateDirectory(PathToLog);
            }
            FileInfo file = new FileInfo(Log.LogError);
            if (file.Exists && file.Length > MaxLength)
            {
                File.AppendAllText(Log.LogMain,
                    DateTime.Now.ToString("U") + "\t" + Environment.MachineName + "\t" +
                    Environment.UserName + "\tClear LogError\n");
                file.Delete();
            }
            if (!file.Exists)
            {
                File.AppendAllText(Log.LogError, "Date\t\t\t\t\t\tMashine\tUsername\t\tMessage\n");
            }
            File.AppendAllText(Log.LogError, DateTime.Now.ToString("U") + "\t" + Environment.MachineName + "\t" + Environment.UserName + "\t" + Message + "\n");
            Console.WriteLine(Message);
        }

        /// <summary>
        /// обработка исключений расширенная
        /// </summary>
        public static void Exception(Exception e)
        {
            //проверяем что папка с логами существует
            if (!Directory.Exists(PathToLog))
            {
                Directory.CreateDirectory(PathToLog);
            }
            FileInfo file = new FileInfo(Log.LogError);
            if (file.Exists && file.Length > MaxLength)
            {
                File.AppendAllText(Log.LogMain,
                    DateTime.Now.ToString("U") + "\t" + Environment.MachineName + "\t" +
                    Environment.UserName + "\tClear LogErrorExpanded\n");
                file.Delete();
            }
            if (!file.Exists)
            {
                File.AppendAllText(Log.LogErrorExpanded, "Date\t\t\t\t\t\tMashine\tUsername\t\tNessage\n");
            }
            File.AppendAllText(Log.LogErrorExpanded,
                DateTime.Now.ToString("U") + "\t" + Environment.MachineName + "\t" + Environment.UserName + "\t" +
                e.Message + "\t" + e.StackTrace + "\n");
            Console.WriteLine("Приложение вызвало ошибку и будет закрыто");
            Console.WriteLine(e.Message);
            Console.WriteLine("Подробнее смотрите в LogErrorExpanded.log");
            Thread.Sleep(5000);
            Environment.Exit(1);
        }
    }
}
