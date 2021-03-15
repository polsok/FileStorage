using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AD
{
    internal class Permission
    {
        /// <summary>
        /// установка наследуемых полных прав
        /// </summary>
        internal static bool Add_FC_Inheritage(string path, string name)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                DirectorySecurity U_M = dirInfo.GetAccessControl(AccessControlSections.Access);
                //Только для папок
                U_M.AddAccessRule(new FileSystemAccessRule(name,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit,
                    PropagationFlags.InheritOnly,
                    AccessControlType.Allow));
                //Только для файлов
                U_M.AddAccessRule(new FileSystemAccessRule(name,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ObjectInherit,
                    PropagationFlags.InheritOnly,
                    AccessControlType.Allow));
                //Только для этой папки
                U_M.AddAccessRule(new FileSystemAccessRule(name,
                    FileSystemRights.FullControl,
                    InheritanceFlags.None,
                    PropagationFlags.InheritOnly,
                    AccessControlType.Allow));
                dirInfo.SetAccessControl(U_M);
                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
                return false;
            }
        }

        /// <summary>
        /// установка наследуемых прав на чтение
        /// </summary>
        internal static bool Add_RO_Inheritage(string path, string name)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                DirectorySecurity U_M = dirInfo.GetAccessControl(AccessControlSections.Access);
                //Только для папок
                U_M.AddAccessRule(new FileSystemAccessRule(name,
                    FileSystemRights.ReadAndExecute,
                    InheritanceFlags.ContainerInherit,
                    PropagationFlags.InheritOnly,
                    AccessControlType.Allow));
                //Только для файлов
                U_M.AddAccessRule(new FileSystemAccessRule(name,
                    FileSystemRights.ReadAndExecute,
                    InheritanceFlags.ObjectInherit,
                    PropagationFlags.InheritOnly,
                    AccessControlType.Allow));
                //Только для этой папки
                U_M.AddAccessRule(new FileSystemAccessRule(name,
                    FileSystemRights.ReadAndExecute,
                    InheritanceFlags.None,
                    PropagationFlags.InheritOnly,
                    AccessControlType.Allow));
                dirInfo.SetAccessControl(U_M);
                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
                return false;
            }
        }
    }
}
