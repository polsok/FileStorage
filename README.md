# FileStorage
Хранение файлов на предприятии
# Функции приложения
1. Получение списка пользователей домена
2. Создание структуры файлов и папок пользователей
3. Принудительная установка разрешений
4. Создание ярлыков на папки
5. Запуск приложений
# Работа приложения
Приложение можно запускать в интерактивном режиме либо с ключами. Ключом является номер функции. За раз можно задавать только один ключ. Все настройки приложения находятся в файле config.ini, который должен находится в той же папке где и исполняемый файл. При работе приложения создаются логи в папке AD_log. Все логи кроме LogMain.log очищаются при достижении размера 10 мб
1. logMain.log              Главный лог (ведутся записи запуска приложений и очистки логов)
2. LogJob.log               Лог для записи сообщений во время работы приложения
3. LogError.log             Лог для записи ошибок
4. LogErrorExstension.log   Лог для записи критических ошибок
# Пример заполнения файла config.ini
```yaml
#1 Получение списка пользователей домена
# путь в каталог LDAP где ищем пользователей
SearchUsersIn=LDAP://OU=HEAD,OU=Users,OU=MyFirma,DC=office,DC=local
SearchUsersIn=LDAP://OU=NORD,OU=Users,OU=MyFirma,DC=office,DC=local
SearchUsersIn=LDAP://OU=SOUTH,OU=Users,OU=MyFirma,DC=office,DC=local
SearchUsersIn=LDAP://OU=WEST,OU=Users,OU=MyFirma,DC=office,DC=local
SearchUsersIn=LDAP://OU=EAST,OU=Users,OU=MyFirma,DC=office,DC=local
#название файла где сохраняем найденных пользователей
UsersList=UsersList.txt
#название файла где сохраняем пользователей не найденных в UsersList
NewUsersList=NewUsersList.txt
#пользователи которые не были найдены в LDAP но есть в UsersList
LastUsersList=LastUsersList.txt
#2 Создание структуры файлов и папок пользователей с разрешениями
#Путь к каталогу структуру которого надо повторить для каждого пользователя
PathTemplateDirectory=MyKatalog
#Путь к каталогу где будем сохранять получившиеся каталоги
PathUsersDirectory=Users
#Права на папки через = указываем папку с путем, где корень это создаваемые папки
#Пользователь или группа на которую распространяются данные права %user% - это пользователь папки
#Сами права RO - права на чтение, RW - права на чтение и запись, FC - полные права.
Permission=/=%user%=RW
Permission=/=admin=RW
Permission=/=FULL_FC=RW
Permission=/=FULL_RO=RO
Permission=/SCAN 00=FULL_RO=RW
#3 Принудительная запись разрешений
#4 Создание ярлыков на папки
#Ссылка на папку где находятся подпапки которым нужно создать ярлыки (обязательно указываем полный путь)
PathDirectoryForLinks=c:\MyCode\AD\bin\Debug\Users\
#Ссылка на папку где нужно разместить сделанные ярлыки 
LinkSDirectory=Links
#5 Запуск приложения
Run=Notepad.exe
Run=ADExplorer.exe
```
# Некоторые полезности
1. При запуске приложения через планировщик заданий нужно указывать рабочую папку с приложением.
2. Для подключения пользовательского диска просто запустите данную строчку в командной строке: `net use x: \\fileserver\Users\%username%`

# Изменение версии 1.1
Теперь логи записываются также в журнал Application Windows
1. logMain.log              код события 900
2. LogJob.log               код события 901
3. LogError.log             код события 902
4. LogErrorExstension.log   код события 903
5. решена проблема с добавлением более 1000 пользователей
6. теперь есть три вида прав на чтение, на запись, полные права
7. добавлен счетчик при добавление папок
