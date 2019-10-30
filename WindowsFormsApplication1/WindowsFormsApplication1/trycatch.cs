using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class trycatch
    {
        public void procerror()
        {
            string result = "";
            try
            {
                int [] i = {1,3,5};
                result = i[3].ToString();
            }
            catch(SystemException ex)
            {
                result = ex.Message;
            }
            finally
            {
                result +="\nfinally xxx";
            }

        }
    }
}
