using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class Ucase
    {
        public int countCapital(string source)
        {
            int cnt = 0;
            foreach(char c in source)
            {
                if(c >='A' && c <= 'Z')
                {
                    cnt++;
                }
            }
            return cnt;
        }
    }
}
