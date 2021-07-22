using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AD
{
    /// <summary>
    /// работа с файлом config
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// словарь конфигурации
        /// </summary>
        static Dictionary<string, List<string>> ConfigDict = new Dictionary<string, List<string>>();
        /// <summary>
        /// загрузка config файла открытого в режиме чтения
        /// </summary>
        /// <param name="path">путь к файлу</param>
        static internal void Load(string path)
        {
            try
            {
                FileInfo log = new FileInfo(path);
                string text;
                using (var streamReader = new StreamReader(log.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    text = streamReader.ReadToEnd();
                }
                text = text.Replace("\r", "");
                char[] chars = { '\n' };
                string[] Data = text.Split(chars);
                //загрузка получившегося массива в словарь
                foreach (var V in Data)
                {
                    //пропускаем пустые строки
                    if(V.Length == 0)
                        continue;
                    //пропускаем примечания
                    if(V.Substring(0,1)=="#")
                        continue;
                    //делим его на =
                    string[] t = V.Split(new char[]{'='},2);
                    //если ключ в словарь еще не добавлен
                    if (!ConfigDict.ContainsKey(t[0]))
                    {
                        ConfigDict[t[0]] = new List<string>();
                        ConfigDict[t[0]].Add(t[1]);
                    }
                    else
                    {
                        ConfigDict[t[0]].Add(t[1]);
                    }
                }
                
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
        /// <summary>
        /// получаем список значений ключа key
        /// </summary>
        internal static List<string> GetValue(string key)
        {
            try
            {
                List<string> Value = ConfigDict[key];
                return Value;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
                return null;
            }
        }
    }
}
