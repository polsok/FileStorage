using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AD
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //загружаем Config
                Config.Load("config.ini");
                //запуск с ключами
                if(args.Length!=0)
                    Tasks.tasks(Convert.ToInt32(args[0]));
                Console.WriteLine("Приложение можно запустить с ключом, просто указав его номер (только один)\n");
                while (true)
                {
                    //запуск меню
                    int answer = Menu.MainMenu();
                    //запуск задачи
                    Tasks.tasks(answer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Проверьте что есть файл config.ini и он без ошибок");
                Thread.Sleep(3000);
                Log.Exception(e);
            }
            
        }
    }
}
