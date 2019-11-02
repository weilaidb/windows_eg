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
using 获取主机所有的硬件信息;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        Excel.Application ExcelApp;
        string result;
        public Form1()
        {
            InitializeComponent();
            if (null == ExcelApp)
            {
                //MessageBox.Show("请打开Excel创建");
                //return;
                if (-1 == InstanceExcel())
                {
                    return;
                }
            }

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
            if (-1 == InstanceExcel())
            {
                return;
            }

            //获取正在运行的Excel
            try
            {
                MessageBox.Show("Excel的版本是：" + ExcelApp.ActiveSheet + "");

                //button2_Click(sender, e);

                if(ExcelApp.ActiveWorkbook == null)
                {
                    return;
                }



                try
                {
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
                }

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
            try
            {            
                wbk = ExcelApp.ActiveWorkbook;
                if(null == wbk)
                {
                    return;
                }
                //wbk = ExcelApp.Workbooks[1];
                //wbk = ExcelApp.Workbooks["hello.xls"];

                result = wbk.Worksheets.Count + "";
                result = wbk.Sheets.Count + "";

                //foreach(Excel.Workbook)
            }
            catch(SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //示例化Excel对象
        private int InstanceExcel()
        {
            try
            {
                ExcelApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
            }
            catch(SystemException ex)
            {
                ShowBox("no excel opened");
                return -1;
            }
            
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
            try
            {            
                wbk = ExcelApp.Workbooks.Open(@"C:\Users\Administrator\Desktop\compare\a.xls");
                result = wbk.FileFormat.ToString();

                wbk = ExcelApp.Workbooks.Add();

                wbk.SaveAs(@"C:\Users\Administrator\Desktop\compare\aa.xls");
                wbk.Close(false, Type.Missing, Type.Missing);
            }
            catch(SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            string filePath = System.Environment.CurrentDirectory;
            //MessageBox.Show("当前路径是:" + filePath);
            System.Diagnostics.Process.Start("Explorer.exe", filePath);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("taskmgr.exe");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (null == ExcelApp)
            {
                MessageBox.Show("请打开Excel创建");
                //return;
                if(-1 == InstanceExcel())
                {
                    return;
                }
            }
            Excel.Workbook wbk = ExcelApp.ActiveWorkbook;
            if(null == wbk)
            {
                MessageBox.Show("请打开Excel并创建");
                return;
            }
            foreach(Excel.Worksheet wst in wbk.Worksheets)
            {
                result += wst.Name + "\n";
            }

            foreach(Excel.Window w in wbk.Windows)
            {
                w.Caption = "Change Caption";
                //改变工作簿各窗口的标题文字
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (null == ExcelApp)
            {
                MessageBox.Show("请打开Excel创建");
                //return;
                if (-1 == InstanceExcel())
                {
                    return;
                }
            }
            Excel.Workbook wbk = ExcelApp.ActiveWorkbook;
            if (null == wbk)
            {
                MessageBox.Show("请打开Excel并创建");
                return;
            }

            Excel.Worksheet wst;
            wst = (Excel.Worksheet)ExcelApp.ActiveSheet;
            if(null == wst)
            {
                MessageBox.Show("没有活动的表");
                return;
            }
            wst = (Excel.Worksheet)ExcelApp.ActiveWorkbook.Worksheets[1];
            //wst = (Excel.Worksheet)ExcelApp.ActiveWorkbook.Worksheets["Sheet3"];
            MessageBox.Show(wst.Shapes.Count + "");
            //wst.UsedRange.Select();//选择使用区域
            long r = wst.Rows.Count;
            long c = wst.Columns.Count;
            MessageBox.Show(r + "/" + c + "");
            wst.Visible = Excel.XlSheetVisibility.xlSheetHidden;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Excel.Worksheet wst;
            wst = (Excel.Worksheet)ExcelApp.ActiveSheet;
            if(null == wst)
            {
                PleaseOpenExcel();
                return;
            }
            ClearResult();
            //wst.Select();//选择工作表
            //遍历工作表
            foreach(Excel.Worksheet w in ExcelApp.ActiveWorkbook.Worksheets)
            {
                result += (w.Index + "\t" + w.Name + "\n");
            }
            ShowBox(result);

        }

        private void ShowBox(string str)
        {
            MessageBox.Show(str);
        }

        private void ClearResult()
        {
            result = "";
        }

        private void PleaseOpenExcel()
        {
            ShowBox("请打开Excel");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Explorer.exe", @"C:\Users\Administrator\Downloads\office2016_4in1\Office2016\Office16\EXCEL.EXE");

        }

        private void button16_Click(object sender, EventArgs e)
        {
            Excel.Worksheet wst2;
            wst2 = (Excel.Worksheet)ExcelApp.ActiveSheet;
            if (null == wst2)
            {
                PleaseOpenExcel();
                return;
            }
            ClearResult();
            wst2.SelectionChange += new Excel.DocEvents_SelectionChangeEventHandler(wst2_SelectionChange);

            Excel.Worksheet wst3;

            if(ExcelApp.ActiveWorkbook.Worksheets.Count < 3)
            {
                ShowBox("当前活动表的表数量" + ExcelApp.ActiveWorkbook.Worksheets.Count + ",请创建到3");
                return;
            }

            wst3 = (Excel.Worksheet)ExcelApp.ActiveWorkbook.Worksheets[3];
            if (null == wst3)
            {
                PleaseOpenExcel();
                return;
            }
            //wst3.Activate();
            //wst3.BeforeDoubleClick += new Excel.DocEvents_BeforeDoubleClickEventHandler(wst3_BeforeDoubleClick);
        }

        public void wst2_SelectionChange(Excel.Range Target)
        {
            Target.Interior.Color = System.Drawing.Color.Yellow;
        }

        public void wst3_BeforeDoubleClick(Excel.Range Target,  bool Cancel)
        {
            ShowBox("双击了工作表");
        }

        private void button17_Click(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            MachineInfoManage info = new MachineInfoManage();
            ShowBox(MachineInfoManage.GetMachineInfoPub());
        }




    }
}
