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
        string result;
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
                if (-1 == InstanceExcel())
                {
                    return;
                }
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
                if (-1 == InstanceExcel())
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
                //ExcelApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
                if(-1 == InstanceExcel())
                {
                    return;
                }
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

        private void button6_Click(object sender, EventArgs e)
        {
            //ExcelApp.WorkbookBeforeClose += new Excel.AppEvents_SinkHelper
        }

        public void ApplicatoinObject()
        {
            //ExcelApp.WorkbookBeforeClose += new Excel.AppEvents_WorkbookBeforeCloseEventHandler();
        }

        public void ExcelApp_WorkbookBeforeClose(Excel.Workbook wbk, ref bool Cancel)
        {
            MessageBox.Show("即将关闭:" + wbk.FullName);
            Cancel = true;//取消关闭
        }

        public void ExcelApp_SheetSelectionChange(object sh, Excel.Range Target)
        {
            Target.Interior.Color = System.Drawing.Color.Green;//改变鼠标所选区域底纹颜色
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(ExcelApp.Dialogs.Count + "");//Excel所有内置对话框个数
            //foreach (Excel.AddIn adn in ExcelApp.AddIns)
            //{
            //    result += (adn.Name + "\t" + adn.Installed + "\n");
            //}
            //MessageBox.Show(ExcelApp.CommandBars.Count + "");
            //foreach(Office cmb in ExcelApp.CommandBars)
            //{
            //    result += (cmb.Name)
            //}
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Excel.Workbook wbk;
            if (-1 == InstanceExcel())
            {
                return;
            }
            wbk = ExcelApp.ActiveWorkbook;
            //wbk = ExcelApp.Workbooks[1];
            //wbk = ExcelApp.Workbooks["hello.xls"];

            result = wbk.Worksheets.Count + "";
            result = wbk.Sheets.Count + "";

            //foreach(Excel.Workbook)


        }

        //示例化Excel对象
        private int InstanceExcel()
        {
            ExcelApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
            if (null == ExcelApp)
            {
                ExcelApp = new Excel.Application();
            }

            if (null == ExcelApp)
            {
                return -1;
            }
            return 0;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Excel.Workbook wbk;
            //打开已存在的工作簿，没有对象时创建一个对象
            if( -1 == InstanceExcel())
            {
                return;
            }

            wbk = ExcelApp.Workbooks.Open(@"C:\Users\Administrator\Desktop\compare\a.xls");
            result = wbk.FileFormat.ToString();

            wbk = ExcelApp.Workbooks.Add();

            wbk.SaveAs(@"C:\Users\Administrator\Desktop\compare\aa.xls");
            wbk.Close(false, Type.Missing, Type.Missing);

        }

        private void button10_Click(object sender, EventArgs e)
        {
            string filePath = System.Environment.CurrentDirectory;
            //MessageBox.Show("当前路径是:" + filePath);
            System.Diagnostics.Process.Start("Explorer.exe", filePath);
        }






    }
}
