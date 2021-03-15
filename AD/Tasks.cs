using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Threading;

namespace AD
{
    /// <summary>
    /// сдесь запускаются все задачи
    /// </summary>
    class Tasks
    {
        private static SearchResult result;
        private static SearchResultCollection resultCol;

        /// <summary>
        /// обработка инструкций
        /// </summary>
        internal static void tasks(int task)
        {
            try
            {
                switch (task)
                {
                    case 0:
                        int answer = Menu.HelpMenu();
                        Help.help(answer);
                        break;
                    case 1:
                        Log.AddLog(Logs.LogMain, "Получение списка пользователей домена");
                        GetListUser();
                        break;
                    case 2:
                        Log.AddLog(Logs.LogMain, "Создание структуры файлов и папок пользователей с разрешениями");
                        CreateUserDirectory();
                        break;
                    case 3:
                        Log.AddLog(Logs.LogMain, "Принудительная установка разрешений");
                        AddPermission();
                        break;
                    case 4:
                        Log.AddLog(Logs.LogMain, "Создание ярлыков на папки");
                        CreateShortcut();
                        break;
                    case 5:
                        Log.AddLog(Logs.LogMain, "Запуск приложений");
                        RunApplication();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
            }

        }

        #region Получение списка пользователей домена

        /// <summary>
        /// Получение списка пользователей домена
        /// </summary>
        private static void GetListUser()
        {
            try
            {
                // список пользователей
                List<string> UsersList = new List<string>();
                //список пользователей из файла
                List<string> OldUsersList = new List<string>();
                //список новых пользователей
                List<string> NewUsersList = new List<string>();
                //список пользователей не найденных в LDAP
                List<string> LastUsersList = new List<string>();
                //получаем список пользователей
                
                foreach (var VALUE in Config.GetValue("SearchUsersIn"))
                {
                    DirectoryEntry searchRoot = new DirectoryEntry(VALUE);
                    DirectorySearcher search = new DirectorySearcher(searchRoot);
                    search.Filter = "(&(objectClass=user)(objectCategory=person))";
                    search.PropertiesToLoad.Add("samaccountname");
                    resultCol = search.FindAll();
                    if (resultCol != null)
                    {
                        for (int counter = 0; counter < resultCol.Count; counter++)
                        {
                            result = resultCol[counter];
                            if (result.Properties.Contains("samaccountname"))
                            {
                                string UserName = (String) result.Properties["samaccountname"][0];
                                UsersList.Add(UserName);
                            }
                        }
                    }
                }

                //сравниваем его с предыдущим списком
                //вначале загружаем файл в список, если он существует

                //List<string> KeyUsersList = Config.GetValue("UsersList");
                if (File.Exists(Config.GetValue("UsersList")[0]))
                {
                    Log.AddLog(Logs.LogJob, "Загружаем файл в список");
                    string[] OldUsers = File.ReadAllLines(Config.GetValue("UsersList")[0]);

                    foreach (var oldUser in OldUsers)
                    {
                        OldUsersList.Add(oldUser);
                    }

                    //заполняем списки NewUsersList и LastUsersList если есть чем
                    bool NewUsersLabel;
                    bool LastUsersLabel;
                    //ищем новых пользователей
                    Log.AddLog(Logs.LogJob, "ищем новых пользователей");
                    foreach (var User in UsersList)
                    {
                        NewUsersLabel = true;
                        foreach (var OldUser in OldUsersList)
                        {
                            if (User == OldUser)
                                NewUsersLabel = false;
                        }

                        if (NewUsersLabel)
                        {
                            NewUsersList.Add(User);
                            Log.AddLog(Logs.LogJob, "Новый пользователь: " + User);
                        }
                    }

                    //ищем пользователей не найденных в LDAP
                    Log.AddLog(Logs.LogJob, "ищем пользователей не найденных в LDAP");
                    foreach (var OldUser in OldUsersList)
                    {
                        LastUsersLabel = true;
                        foreach (var User in UsersList)
                        {
                            if (User == OldUser)
                                LastUsersLabel = false;
                        }

                        if (LastUsersLabel)
                        {
                            LastUsersList.Add(OldUser);
                            Log.AddLog(Logs.LogJob, "Не найден в LDAP: " + OldUser);
                        }
                    }
                }

                //заполняем файлы
                if (NewUsersList.Count > 0)
                {
                    Log.AddLog(Logs.LogJob, "Дописываем в файл " + Config.GetValue("NewUsersList")[0]);
                    File.AppendAllLines(Config.GetValue("NewUsersList")[0], NewUsersList);
                }

                if (LastUsersList.Count > 0)
                {
                    Log.AddLog(Logs.LogJob, "Дописываем в файл " + Config.GetValue("LastUsersList")[0]);
                    File.AppendAllLines(Config.GetValue("LastUsersList")[0], LastUsersList);
                }

                if (NewUsersList.Count > 0 || LastUsersList.Count > 0 || OldUsersList.Count == 0)
                {
                    if (File.Exists(Config.GetValue("UsersList")[0]))
                    {
                        File.Delete(Config.GetValue("UsersList")[0]);
                    }

                    File.AppendAllLines(Config.GetValue("UsersList")[0], UsersList);
                    Log.AddLog(Logs.LogJob, "Заполняем файл " + Config.GetValue("UsersList")[0]);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
            }
        }

        #endregion

        #region Создание структуры файлов и папок пользователей

        /// <summary>
        /// Создание структуры файлов и папок пользователей
        /// </summary>
        private static void CreateUserDirectory()
        {
            //если файла с пользователями не существует запускаем задачу Получение списка пользователей домена
            if (!File.Exists(Config.GetValue("UsersList")[0]))
            {
                GetListUser();
            }

            //загружаем файл в массив  Users
            string[] Users = File.ReadAllLines(Config.GetValue("UsersList")[0]);
            string TemplateDirectory = Config.GetValue("PathTemplateDirectory")[0];
            //если каталога нет создаем его и копируем туда структуру и файлы из шаблона
            foreach (var user in Users)
            {
                Log.AddLog(Logs.LogJob, "Создаем каталог пользователю "+ user);
                // метка о существовании директории до запуска приложения
                bool ExistDirectory = true;
                try
                {
                    string UserDirectory = Config.GetValue("PathUsersDirectory")[0] + "\\" + user;
                    if (!Directory.Exists(UserDirectory))
                    {
                        Directory.CreateDirectory(UserDirectory);
                        ExistDirectory = false;
                    }

                    //Создать идентичное дерево каталогов
                    foreach (string dirPath in Directory.GetDirectories(TemplateDirectory, "*",
                        SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(TemplateDirectory, UserDirectory));

                    //Скопировать все файлы. И перезаписать(если такие существуют)
                    foreach (string newPath in Directory.GetFiles(TemplateDirectory, "*.*", SearchOption.AllDirectories)
                    )
                        File.Copy(newPath, newPath.Replace(TemplateDirectory, UserDirectory), true);
                    //установка прав если директория не существовала
                    if (!ExistDirectory)
                    {
                        List<string> permList = Config.GetValue("Permission");
                        foreach (var perm in permList)
                        {
                            string[] t = perm.Split(new char[] {'='});
                            if (t[1] == "%user%")
                            {
                                t[1] = user;
                            }
                            if (t[2] == "RW")
                            {
                                Permission.Add_FC_Inheritage(UserDirectory + t[0], t[1]);
                            }
                            if (t[2] == "RO")
                            {
                                Permission.Add_RO_Inheritage(UserDirectory + t[0], t[1]);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Exception(e.Message);
                }
            }
        }

        #endregion

        #region Принудительная установка разрешений
        /// <summary>
        /// Принудительная установка разрешений
        /// </summary>
        private static void AddPermission()
        {
            //если файла с пользователями не существует запускаем задачу Получение списка пользователей домена
            if (!File.Exists(Config.GetValue("UsersList")[0]))
            {
                GetListUser();
            }

            //загружаем файл в массив  Users
            string[] Users = File.ReadAllLines(Config.GetValue("UsersList")[0]);
            //если каталога нет создаем его и копируем туда структуру и файлы из шаблона
            foreach (var user in Users)
            {
                try
                {
                    string UserDirectory = Config.GetValue("PathUsersDirectory")[0] + "\\" + user;
                    //установка прав если директория существует
                    if (Directory.Exists(UserDirectory))
                    {
                        Log.AddLog(Logs.LogJob, "Устанавливаем принудительные права на папку "+ UserDirectory);
                        List<string> permList = Config.GetValue("Permission");
                        foreach (var perm in permList)
                        {
                            string[] t = perm.Split(new char[] { '=' });
                            if (t[1] == "%user%")
                            {
                                t[1] = user;
                            }
                            if (t[2] == "RW")
                            {
                                Permission.Add_FC_Inheritage(UserDirectory + t[0], t[1]);
                            }
                            if (t[2] == "RO")
                            {
                                Permission.Add_RO_Inheritage(UserDirectory + t[0], t[1]);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Exception(e.Message);
                }
            }
        }

        #endregion

        #region Создание ярлыков

        /// <summary>
        /// создание ярлыков
        /// </summary>
        internal static void CreateShortcut()
        {
            //Ссылка на папку где находятся подпапки которым нужно создать ярлыки
            string PathDirectoryForLinks = Config.GetValue("PathDirectoryForLinks")[0];
            //Ссылка на папку где нужно разместить сделанные ярлыки
            string LinksDirectory = Config.GetValue("LinkSDirectory")[0];
            foreach (string dir in Directory.GetDirectories(PathDirectoryForLinks, "*.*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    Log.AddLog(Logs.LogJob, "Создаем ярлык на папку " + dir);
                    CreateLinks.createShortcut(dir, LinksDirectory);
                }
                catch (Exception e)
                {
                    Log.Exception(e.Message);
                }
            }
            
        }

        #endregion

        #region запуск приложений
        /// <summary>
        /// запуск приложений
        /// </summary>
        private static void RunApplication()
        {
            try
            {
                //килим процесс если есть
                foreach (var proc in Config.GetValue("Run"))
                {
                    foreach (var process in Process.GetProcessesByName(proc))
                    {
                        process.Kill();
                    }
                    //даем 5 сек
                    Thread.Sleep(5000);
                    //запускаем его
                    Process.Start(proc);
                    Log.AddLog(Logs.LogJob, "Запущен процесс " + proc);

                }


            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
            }
        }

        #endregion

    }

}
