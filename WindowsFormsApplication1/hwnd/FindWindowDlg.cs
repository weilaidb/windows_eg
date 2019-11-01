using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DesktopWndView
{
	/// <summary>
	/// FindWindowDlg 的摘要说明。
	/// </summary>
	public class FindWindowDlg : System.Windows.Forms.Form
	{
		#region 自定义变量
		public const string DescriptionInTextBox=
			"窗口类：向系统注册窗口类时指定的窗口类名称，例如Edit,Button,SysListView32等。\r\n"
			+"\r\n窗口名称：对一个窗口来说是窗口的标题，对一个控件来说是控件的文本。\r\n"
			+"\r\n注意：\r\n(1)在窗口类和窗口名称中可以输入空字符串，空字符串会被作为NULL处理。\r\n"
			+"\r\n(2)查找结果是符合条件的所有匹配窗口中的第一个。";
		//匹配标志定义
		public const int MF_NONE=0;
		public const int MF_FULLWORD=1;
		public const int MF_UPPERLOWER=2;

		//主窗口的指针
		private MainFrm m_MainFrm;
		//自定义光标
		private Cursor m_Cursor;
		//光标形状在图片框中占据的客户区矩形
		private static Rectangle RC_CURSORPOS=new Rectangle(7,7,18,18);
		//用查找工具找到的窗口
		private int m_HWND=0;
		//找到的前一个窗口
		private int m_PreHWND=0;
		//鼠标是否正在查找窗口的过程中（即鼠标是否在按下状态）
		private bool b_FindingWnd=false;
		//记录查找窗口的点
		private MainFrm.POINT m_POINT=new DesktopWndView.MainFrm.POINT(0,0);
		//换算成屏幕坐标的点
		private Point m_ScreenPoint;
		//找到的窗口的信息
		private WndInfo m_WndInfo;
		#endregion

		#region 窗体设计器变量

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbWndClass;
		private System.Windows.Forms.TextBox tbWindowName;
		private System.Windows.Forms.Button btOk;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btExpand;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbMapFullword;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox cbMapUpperLower;
		private System.Windows.Forms.CheckBox cbHideMainFrm;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.PictureBox pbFindWnd;

		#endregion
		private System.Windows.Forms.TextBox tbWndHandle;
		private System.Windows.Forms.Label LableMousePos;

		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FindWindowDlg()
		{
			InitializeComponent();
			//设置文本框的说明
			this.tbDescription.Text=DescriptionInTextBox;
			//加载光标
			this.LoadCursor();
			this.tbWindowName.Focus();
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FindWindowDlg));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cbWndClass = new System.Windows.Forms.ComboBox();
			this.tbWindowName = new System.Windows.Forms.TextBox();
			this.btOk = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btExpand = new System.Windows.Forms.Button();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.cbMapUpperLower = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cbMapFullword = new System.Windows.Forms.CheckBox();
			this.pbFindWnd = new System.Windows.Forms.PictureBox();
			this.cbHideMainFrm = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbWndHandle = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.LableMousePos = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "窗口类名：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "窗口标题：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbWndClass
			// 
			this.cbWndClass.Items.AddRange(new object[] {
															"#32770",
															"Notepad",
															"Progman",
															"Shell_TrayWnd",
															"MSPaintApp",
															"VBFloatingPalette"});
			this.cbWndClass.Location = new System.Drawing.Point(96, 24);
			this.cbWndClass.Name = "cbWndClass";
			this.cbWndClass.Size = new System.Drawing.Size(208, 20);
			this.cbWndClass.TabIndex = 2;
			// 
			// tbWindowName
			// 
			this.tbWindowName.Location = new System.Drawing.Point(96, 56);
			this.tbWindowName.Name = "tbWindowName";
			this.tbWindowName.Size = new System.Drawing.Size(208, 21);
			this.tbWindowName.TabIndex = 3;
			this.tbWindowName.Text = "";
			// 
			// btOk
			// 
			this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOk.Location = new System.Drawing.Point(168, 200);
			this.btOk.Name = "btOk";
			this.btOk.TabIndex = 4;
			this.btOk.Text = "确定";
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(248, 200);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 5;
			this.btCancel.Text = "取消";
			// 
			// btExpand
			// 
			this.btExpand.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btExpand.Location = new System.Drawing.Point(8, 200);
			this.btExpand.Name = "btExpand";
			this.btExpand.Size = new System.Drawing.Size(56, 23);
			this.btExpand.TabIndex = 6;
			this.btExpand.Text = "↓↓";
			this.btExpand.Click += new System.EventHandler(this.btExpand_Click);
			// 
			// tbDescription
			// 
			this.tbDescription.BackColor = System.Drawing.SystemColors.Control;
			this.tbDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbDescription.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.tbDescription.Location = new System.Drawing.Point(8, 248);
			this.tbDescription.Multiline = true;
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.ReadOnly = true;
			this.tbDescription.Size = new System.Drawing.Size(312, 168);
			this.tbDescription.TabIndex = 7;
			this.tbDescription.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.tbWndHandle);
			this.groupBox1.Controls.Add(this.cbMapUpperLower);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.cbMapFullword);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cbWndClass);
			this.groupBox1.Controls.Add(this.tbWindowName);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(312, 152);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// cbMapUpperLower
			// 
			this.cbMapUpperLower.Location = new System.Drawing.Point(200, 120);
			this.cbMapUpperLower.Name = "cbMapUpperLower";
			this.cbMapUpperLower.Size = new System.Drawing.Size(88, 24);
			this.cbMapUpperLower.TabIndex = 6;
			this.cbMapUpperLower.Text = "大小写匹配";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 23);
			this.label3.TabIndex = 5;
			this.label3.Text = "标题匹配：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbMapFullword
			// 
			this.cbMapFullword.Location = new System.Drawing.Point(96, 120);
			this.cbMapFullword.Name = "cbMapFullword";
			this.cbMapFullword.Size = new System.Drawing.Size(80, 24);
			this.cbMapFullword.TabIndex = 4;
			this.cbMapFullword.Text = "全字匹配";
			// 
			// pbFindWnd
			// 
			this.pbFindWnd.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.pbFindWnd.Image = ((System.Drawing.Image)(resources.GetObject("pbFindWnd.Image")));
			this.pbFindWnd.Location = new System.Drawing.Point(136, 160);
			this.pbFindWnd.Name = "pbFindWnd";
			this.pbFindWnd.Size = new System.Drawing.Size(32, 29);
			this.pbFindWnd.TabIndex = 7;
			this.pbFindWnd.TabStop = false;
			this.pbFindWnd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbFindWnd_MouseUp);
			this.pbFindWnd.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbFindWnd_MouseMove);
			this.pbFindWnd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbFindWnd_MouseDown);
			// 
			// cbHideMainFrm
			// 
			this.cbHideMainFrm.Location = new System.Drawing.Point(208, 160);
			this.cbHideMainFrm.Name = "cbHideMainFrm";
			this.cbHideMainFrm.Size = new System.Drawing.Size(88, 24);
			this.cbHideMainFrm.TabIndex = 8;
			this.cbHideMainFrm.Text = "隐藏主窗口";
			this.cbHideMainFrm.CheckedChanged += new System.EventHandler(this.cbHideMainFrm_CheckedChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 23);
			this.label4.TabIndex = 9;
			this.label4.Text = "窗口句柄：";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbWndHandle
			// 
			this.tbWndHandle.Location = new System.Drawing.Point(96, 88);
			this.tbWndHandle.Name = "tbWndHandle";
			this.tbWndHandle.ReadOnly = true;
			this.tbWndHandle.Size = new System.Drawing.Size(208, 21);
			this.tbWndHandle.TabIndex = 10;
			this.tbWndHandle.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(24, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 23);
			this.label5.TabIndex = 11;
			this.label5.Text = "查找窗口工具：";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// LableMousePos
			// 
			this.LableMousePos.Location = new System.Drawing.Point(72, 200);
			this.LableMousePos.Name = "LableMousePos";
			this.LableMousePos.Size = new System.Drawing.Size(88, 23);
			this.LableMousePos.TabIndex = 12;
			this.LableMousePos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FindWindowDlg
			// 
			this.AcceptButton = this.btOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(330, 231);
			this.Controls.Add(this.LableMousePos);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.tbDescription);
			this.Controls.Add(this.btExpand);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOk);
			this.Controls.Add(this.cbHideMainFrm);
			this.Controls.Add(this.pbFindWnd);
			this.Controls.Add(this.label5);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindWindowDlg";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "查找窗口";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		//--------------------------------
		//		外部接口
		//--------------------------------
		//窗口类名称
		public string WNDCLASS
		{
			get{return this.cbWndClass.Text.Length>0? this.cbWndClass.Text:null;}
		}
		//窗口名称（标题或者控件文本）
		public string WNDNAME
		{
			get{return this.tbWindowName.Text.Length>0? this.tbWindowName.Text:null;}
		}
		//标题是否全字匹配
		public bool MAP_FULLWORD
		{
			get{return this.cbMapFullword.Checked;}
		}
		//标题是否大小写匹配
		public bool MAP_UPPERLOWER
		{
			get{return this.cbMapUpperLower.Checked;}
		}
		//返回匹配标志
		public int MAP_FLAG
		{
			get
			{
				int fullword=this.cbMapFullword.Checked? MF_FULLWORD:MF_NONE;
				int upperlower=this.cbMapUpperLower.Checked? MF_UPPERLOWER:MF_NONE;
				return (fullword | upperlower);
			}
		}
		//设置主窗口指针
		public MainFrm MAINFRAME
		{
			set{this.m_MainFrm=value;}
		}

		//获取用查找工具找到的窗口
		public int HWND
		{
			get{return this.m_HWND;}
		}
		//获取窗口信息
		public WndInfo WNDINFO
		{
			get{return this.m_WndInfo;}
		}

		//-----------------------------------
		//			内部辅助函数
		//-----------------------------------
		private void LoadCursor()
		{
			System.Reflection.Assembly asm=System.Reflection.Assembly.GetExecutingAssembly();
			System.IO.Stream stream=asm.GetManifestResourceStream(asm.GetName().Name+".FINDWND.CUR");
			this.m_Cursor=new Cursor(stream);
		}

		//-----------------------------------
		//			控件事件的处理
		//-----------------------------------

		//扩展按钮的处理函数
		private void btExpand_Click(object sender, System.EventArgs e)
		{
			if(this.btExpand.Text=="↓↓")
			{
				//注意控件坐标没有包括标题栏的高度(大约20象素高度)，而Height中需要包含非客户区尺寸
				this.Height=this.tbDescription.Bottom+35;
				this.btExpand.Text="↑↑";
			}
			else
			{
				//注意控件坐标没有包括标题栏的高度(大约20象素高度)
				this.Height=this.tbDescription.Top+10;
				this.btExpand.Text="↓↓";
			}
		}

		private void pbFindWnd_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Graphics g=this.pbFindWnd.CreateGraphics();
			//擦除光标的形状
			g.FillRectangle(Brushes.White,RC_CURSORPOS);
			//设置光标
			Cursor.Current=this.m_Cursor;
			//捕获鼠标
			this.pbFindWnd.Capture=true;
			g.Dispose();
			//正在查找窗口
			this.b_FindingWnd=true;
		}

		//查找工具鼠标抬起
		private void pbFindWnd_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//不再查找窗口
			this.b_FindingWnd=false;
			//使PicureBox无效(重新显示出光标的形状)
            this.pbFindWnd.Invalidate(RC_CURSORPOS);
			//恢复光标
			Cursor.Current=Cursors.Default;
			this.pbFindWnd.Capture=false;
		}

		//查找工具鼠标移动
		private void pbFindWnd_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(!this.b_FindingWnd)
				return;
			this.m_ScreenPoint=this.pbFindWnd.PointToScreen(new Point(e.X,e.Y));
			this.m_POINT.X=this.m_ScreenPoint.X;
			this.m_POINT.Y=this.m_ScreenPoint.Y;
			this.LableMousePos.Text=string.Format("X:{0} Y:{1}",
				this.m_ScreenPoint.X,
				this.m_ScreenPoint.Y);
			this.m_HWND=MainFrm.WindowFromPoint(this.m_POINT);
			if(this.m_HWND!=this.m_PreHWND)
			{
				if(this.m_HWND==0)
				{
					//未找到窗口，清空显示内容
					this.cbWndClass.Text=string.Empty;
					this.tbWindowName.Clear();
					this.tbWndHandle.Clear();
				}
				else
				{
					//显示该窗口的信息
					this.m_WndInfo=this.m_MainFrm.GetWndInfo(this.m_HWND);
					this.cbWndClass.Text=this.m_WndInfo.WndClassName;
					this.tbWindowName.Text=this.m_WndInfo.Caption;
					this.tbWndHandle.Text=Convert.ToString(this.m_WndInfo.hWnd,16).PadLeft(8,'0');
					//高亮新找到的窗口
					this.m_MainFrm.HighLightWindow(this.m_HWND);
				}
				//再次高亮前一次的窗口（擦除效果）
				if(this.m_PreHWND>0)
					this.m_MainFrm.HighLightWindow(this.m_PreHWND);
				//更新前一窗口到当前窗口
				this.m_PreHWND=this.m_HWND;
			}
		}

		//隐藏主窗口的处理函数
		private void cbHideMainFrm_CheckedChanged(object sender, System.EventArgs e)
		{
			this.m_MainFrm.Visible=!this.cbHideMainFrm.Checked;
		}
	}
}
