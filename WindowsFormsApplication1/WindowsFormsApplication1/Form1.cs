using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FM = System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using VB = Microsoft.VisualBasic;


//[DllImport("user32.dll",EntryPoint = "FindWindowA")]
//static extern int FindWindow(string lpClassName, string lpWindowName);

//DllImport("user32.dll", EntryPoint = "SetWindowTextA")
//static extern int SetWindowText(int hwnd, string lpString);


namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            loadtext();
            eg1();
            eg2();
            eg3();
            eg4();
            eg5_replace();
            eg6_parse();
            eg7_convert();
            eg8_sum();
            eg9_multiply();
            eg10_exception();
            eg11_file_wr();
            this.richTextBox1.Text = libai.Visual_Studio_2017_常用快捷键;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("the first button");
            this.Text = System.DateTime.Now.ToString();
            MessageBox.Show(@"Hello\Form");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Hello, Form");
        }
        private void loadtext()
        {
            string result = "";
            result += "山东省\t济南\r";
            result += "山东省\t济南\r";
            result += "山东省\t济南\r";
            result += "山东省\t济南\r";
            result += "山东省\t济南\r";
            result += "山东省\t济南\r";
            this.maskedTextBox1.Text = result;

        }
        private void eg1()
        {
            string s1 = "micro";
            string s2 = s1 + "off";

            Console.WriteLine(s2);

            string s3 = (2.232 + 3.4434).ToString();
            string s4 = "" + 2.7832 + 323;
            Console.WriteLine(s4);
        }

        private void eg2()
        {
            string s1 = "micro, test for it ";
            string s2 = s1.Substring(3, 4);
            //Substring提取字符s1，从第三个字符起连续提取4个。
        }

        private void eg3()
        {
            string s1 = "officew wwwww ";
            int p = s1.IndexOf("go");
            Console.WriteLine("index is " + p);
        }

        private void eg4()
        {
            string s0 = "format", s1 = "s1", s2 = "s2", s3 = "s3", s4 = "s4", s5 = "s5";
            string result = "";
            result = String.Format("result is {0} {3} {4}", s0, s1, s2, s3, s4, s5);
            Console.WriteLine("result is " + result);

        }

        private void eg5_replace()
        {
            string s = "microsoft office";
            string t = s.Replace('o', '%');
            string u = s.Replace("oso", "db");
            Console.WriteLine("s is " + s);
            Console.WriteLine("t is " + t);
            Console.WriteLine("u is " + u);
        }

        private void eg6_parse()
        {
            string temp = "1";
            int i = int.Parse(temp);
            float f = float.Parse(temp);
            char c = char.Parse(temp);

            string s = "false";
            bool b = bool.Parse(s);

            string d = "2016-8-8";
            DateTime dt = DateTime.Parse(d);


            Console.WriteLine(i);
            Console.WriteLine(f);
            Console.WriteLine(c);
            Console.WriteLine(b);
            Console.WriteLine(dt);

        }

        private void eg7_convert()
        {
            double p = System.Convert.ToDouble("3.14");
            Console.WriteLine(p);
        }

        private void eg8_sum()
        {
            int[] n = new int[10];
            for (int i = 0; i < n.Length; i++)
            {
                n[i] = i + 1;
            }

            int total = 0;
            for (int i = 0; i < n.Length; i++)
            {
                total += n[i];
            }
            //MessageBox.Show(total.ToString());
            Console.WriteLine("total is " + total.ToString());
        }

        private void eg9_multiply()
        {
            string[,] t = new string[9, 9];
            string result = "";
            int R, C;
            for (int r = 0; r < 9; r++)
            {
                result += "\n";
                R = r + 1;
                for (int c = 0; c <= r; c++)
                {
                    C = c + 1;
                    result += C + "*" + R + "=" + (R * C) + " ";
                }
            }
            //MessageBox.Show(result);
            //FM.MessageBox.Show(result);
            Console.WriteLine(result);
        }

        private void eg10_exception()
        {
            string result = "";
            try
            {
                int[] i = { 1, 3, 4, 5, };
                result = i[3].ToString();
            }
            catch (SystemException ex)
            {
                result = ex.Message;
            }
            finally
            {
                result += "\nfinally";
            }
            Console.WriteLine(result);
        }

        private void eg11_file_wr()
        {
            string path = @"F:\a.txt";
            try
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                //开始写入
                sw.WriteLine("Hello VSTO");
                sw.WriteLine("Second line");
                sw.WriteLine("3th line");

                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                fs.Close();
            } 
            catch (SystemException e)
            {

            }


            try { 
                //读入内容 
                StreamReader sr = new StreamReader(path, Encoding.Default);
                String line;
                String result = "";
                while((line = sr.ReadLine()) != null)
                {
                    result = line + "\n";
                }
                Console.WriteLine(result);
            }
            catch(SystemException e)
            {

            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            //MessageBox.Show("加载窗体中");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string Source = "12345678901";
            Regex obj = new Regex(@"[\d]{11}", RegexOptions.None);
            bool result = obj.IsMatch(Source);
            if(true == result)
            {
                Console.WriteLine(Source + "\n包含一个手机号码");
            }
            else
            {
                Console.WriteLine(Source + "\n不包含一个手机号码");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string Source = "this.textBox1.Text.China.WorLD";
            Regex obj = new Regex(@"China", RegexOptions.None);
            this.textBox1.Text = obj.Replace(Source, "中国");

            obj = new Regex(@"world", RegexOptions.IgnoreCase);
            this.textBox1.Text = obj.Replace(this.textBox1.Text, "世界");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string Source = "中国国土面积960万平方公里，56个民族，13亿人口。";
            Regex obj = new Regex(@"\d+", RegexOptions.IgnoreCase);
            MatchCollection col = obj.Matches(Source);
            foreach (Match m in col)
            {
                MessageBox.Show("\n位置：" + m.Index + "\n长度:" + m.Length
                    + "\n值:" + m.Value);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string result = "";
            string Source = "胡萝卜35公斤土豆231公斤西红柿25公斤白菜12345公斤";
            Regex obj = new Regex(@"\d+公斤", RegexOptions.IgnoreCase);
            string[] arr = obj.Split(Source);
            foreach(string s in arr)
            {
                result += s + "\n";
            }
            MessageBox.Show(result);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Dictionary<string, int> population = new System.Collections.Generic.Dictionary<string, int>();
            population.Clear();
            population.Add("Russia", 1707);
            population.Add("Candana", 233);
            population.Add("China", 960);
            population.Add("America", 122);
            population.Add("Brazil", 854);

            population.Remove("Candana");

            int cnt = population.Count;

            bool hasKey = population.ContainsKey("China");

            bool hasValue = population.ContainsValue(850);

            foreach (KeyValuePair<string, int> kvp in population)
            {
                MessageBox.Show(kvp.Key + kvp.Value);
            }

            //遍历所有键名
            foreach(string key in population.Keys)
            {
                MessageBox.Show(key);
            }

            //遍历所有值
            foreach(int val in population.Values)
            {
                MessageBox.Show(val.ToString());
            }



        }

        private void button7_Click(object sender, EventArgs e)
        {
            //去除重复
            string[] arr = { "李白", "杜甫", "白居易", "杜甫", "李商隐", "贺知章", "白居易", "孟浩然", "李商隐" };
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach(string s in arr)
            {
                if(dic.ContainsKey(s) == false)
                {
                    dic.Add(s, "");
                }
            }

            foreach(string k in dic.Keys)
            {
                MessageBox.Show(k);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form1 fm2 = new Form1();
            fm2.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form1 fm2 = new Form1();
            fm2.Show(this);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form1 fm2 = new Form1();
            fm2.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Form1 fm2 = new Form1();

            fm2.Show();
            Thread.Sleep(10000);            
            fm2.Dispose();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Text = this.Width + "  " + this.Height;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            DialogResult v;
            v = MessageBox.Show("是否提交试卷?", "询问", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if(v == DialogResult.Yes)
            {
                this.Text = "提交";
            }
            else if (v == DialogResult.No)
            {
                this.Text = "不提交";
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string result = VB.Interaction.InputBox("请输入您的姓名:", "输入框");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Ucase U = new Ucase();
            MessageBox.Show(U.countCapital("Microsoft Office PPT").ToString());
        }





    }


}
