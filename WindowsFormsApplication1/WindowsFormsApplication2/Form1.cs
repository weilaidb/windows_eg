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

        private void button2_Click(object sender, EventArgs e)
        {
            Excel.Application newApp = new Excel.Application();
            newApp.Visible = true;
            newApp.Caption = "new app";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //获取正在运行的Excel
            try
            {
                ExcelApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
                if(((Excel.Worksheet)ExcelApp.ActiveSheet) == null)
                {
                    return;
                }
                MessageBox.Show("Excel的版本是：" + ExcelApp.ActiveSheet + "");

                //button2_Click(sender, e);

                ((Excel.Worksheet)ExcelApp.ActiveSheet).Name = "MysSheet";//重命名活动工作表

                Excel.Range rng = (Excel.Range)ExcelApp.Selection;//Excel对象赋值给对象变量rng
                ExcelApp.StatusBar = "My Excel Application"; //改变Excel状态栏文字
                MessageBox.Show(ExcelApp.Workbooks.Count + "");//显示工作簿个数
                MessageBox.Show(ExcelApp.UserName + "");//显示用户名
                ExcelApp.DisplayFormulaBar = false;//隐藏公式编辑栏


            }
            catch(SystemException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("请打开Excel文件");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            //获取正在运行的Excel
            try
            {
                ExcelApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
                //MessageBox.Show("Excel的版本是：" + ExcelApp.Version + "");
                ExcelApp.Undo();//Excel撤销，相当于按下Ctrl+Z
                ExcelApp.Workbooks.Close();
                ExcelApp.Quit();

            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("请打开Excel文件");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
