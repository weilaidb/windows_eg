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
        static void Main(string[] args)
        {
            Console.Write("Hello C#");
            Console.ReadKey();

            test();
        }
    }
}
