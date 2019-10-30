using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApplication1
{
    class egfile
    {
        string result;
        public void proc_file_dir()
        {
            //文件删除不成功
            bool b = System.IO.File.Exists(@"F:/abc.txt");
            if(b == true)
            {
                result = b.ToString();
                System.IO.File.Delete(@"F:/abc.txt");
            }
            Console.WriteLine("file exist:" + result);

            bool f = Directory.Exists(@"F:/VSTO");
            if(f == true)
            {
                result = f.ToString();
                string[] arr = Directory.GetFiles(@"F:/VSTO");
                for(int i = 0; i < arr.Length; i++)
                {
                    result += arr[i] + "\n";
                }
            }
            Console.WriteLine("last result: " + result);

        }
    }
}
