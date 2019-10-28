using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void test()
        {
            string nm = Console.ReadLine();
            Console.WriteLine(nm.ToUpper());
            Console.ReadKey();
        }
        static void OneShiGe()
        {
            Console.Write("白日依山尽");
            Console.Write("黄河入海流");
            Console.Write("欲穷千里目");
            Console.Write("更上一层楼");
            Console.WriteLine("\n");
            Console.WriteLine("锄禾日当午");
            Console.WriteLine("汗滴禾下土");
            Console.WriteLine("谁知盘中餐");
            Console.WriteLine("粒粒皆辛苦");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            //Console.Write("Hello C#");
            //Console.ReadKey();
            OneShiGe();


            test();
        }
    }
}
