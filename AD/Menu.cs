using System;
using System.Collections.Generic;
using System.Text;

namespace AD
{
    internal class Menu
    {
        internal static int MainMenu()
        {
            int answer = -1;
            while (true)
            {
                try
                {

                    Console.WriteLine("\nНапишите номер выполняемой задачи\n");
                    Console.WriteLine("1 - Получение списка пользователей домена");
                    Console.WriteLine("2 - Создание структуры файлов и папок пользователей");
                    Console.WriteLine("3 - Принудительная установка разрешений");
                    Console.WriteLine("4 - Создание ярлыков на папки");
                    Console.WriteLine("5 - Запуск приложений");
                    Console.WriteLine("0 - Справка по командам");
                    string val = Console.ReadLine();
                    answer = Convert.ToInt32(val);
                    return answer;
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nВыбор не разпознан\n");
                }
            }
            
        }
        internal static int HelpMenu()
        {
            int answer = -1;
            try
            {

                Console.WriteLine("Напишите номер задачи по которой нужна справка\n");
                Console.WriteLine("1 - Получение списка пользователей домена");
                Console.WriteLine("2 - Создание структуры файлов и папок пользователей с разрешениями");
                Console.WriteLine("3 - Принудительная установка разрешений");
                Console.WriteLine("4 - Создание ярлыков на папки");
                Console.WriteLine("5 - Запуск приложений");
                Console.WriteLine("0 - Возврат в главное меню");
                string val = Console.ReadLine();
                answer = Convert.ToInt32(val);
                return answer;

            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
                return answer;
            }
        }
    }
}
