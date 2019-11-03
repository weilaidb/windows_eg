using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace UDF20191102
{
    [Guid("13791DD9-C1CE-448B-9FBE-F08746CD6EBA")]
    //工具/创建GUID
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class ClassLeap
    {
        #region COM related
        [ComRegisterFunction]
        public static void RegisterFunction(Type type)
        {
            Registry.ClassesRoot.CreateSubKey(GetSubKeyName(type,
                "Programmable"));
            var key = Registry.ClassesRoot.OpenSubKey(GetSubKeyName(type,
                "InprocServer32"), true);
            key.SetValue("", Environment.SystemDirectory + @"\mscoree.dll",
                RegistryValueKind.String);
        }

        [ComUnregisterFunction]
        public static void UnregisterFunction(Type type)
        {
            Registry.ClassesRoot.DeleteSubKey(GetSubKeyName(type,
                "Programmable"), false);
        }

        private static string GetSubKeyName(Type type, string subKeyName)
        {
            var s = new System.Text.StringBuilder();
            s.Append(@"CLSID\{");
            s.Append(type.GUID.ToString().ToUpper());
            s.Append(@"}\");
            s.Append(subKeyName);
            return s.ToString();
        }

        ///<summary>
        ///将Object类的4个公共方法隐藏
        ///否则将会出现在Excel的UDF函数中
        ///</summary>
        ///<returns></returns>
        [ComVisible(false)]
        public override string ToString()
        {
            return base.ToString();
        }

        [ComVisible(false)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [ComVisible(false)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        //以下为自定义Excel函数
        //项目属性设置:生成/勾选"为COM互操作注册"
        public bool IsLeap(int year)
        {
            if (year % 4 == 0 && year % 100 != 0)
                return true;
            else if (year % 400 == 0)
            {
                return true;
            }
            else
                return false;
        }


        public int issonicethings(int num)
        {
            return num;
        }
    }
}

