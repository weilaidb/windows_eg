using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            for (int i =  0; i < n.Length; i++)
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
            for(int r = 0; r < 9; r++)
            {
                result += "\n";
                R = r + 1;
                for(int c = 0; c <= r; c++)
                {
                    C = c + 1;
                    result += C + "*" + R + "=" + (R * C) + " ";
                }
            }
            MessageBox.Show(result);
        }
    }
}
