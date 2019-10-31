using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Interop;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        Excel.Application ExcelApp;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //获取正在运行的Excel
            try
            {
                ExcelApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
                MessageBox.Show("Excel的版本是：" + ExcelApp.Version + "");
            }
            catch(SystemException ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("请打开Excel文件");
            }

        }
    }
}
