using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace DesktopWndView
{
	/// <summary>
	/// QQMsgtailDlg 的摘要说明。
	/// </summary>
	public class QQMsgtailDlg : System.Windows.Forms.Form
	{
		#region 自定义变量
		private StringBuilder m_StringBuilder=new StringBuilder(256);
		//对方名字的替代关键词
		private string m_UserNameMask="【USER】";
		//消息尾巴
		private string m_Msgtail;
		//发送次数（0表示无限次)
		private int m_SendTimes;
		//发送寿命计数，超过发送次数时停止计时器
		private int m_life=0;
		//客户端类型
		public const int CT_QQ=0;
		public const int CT_TM=1;
		string[] btSendName={"发送(S)","发送(&S)"};	//发送按钮的名字（0－QQ，1－TM）
		#endregion

		private System.Windows.Forms.TextBox tbMsgtail;
		private System.Windows.Forms.ComboBox cbInterval;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btStartStop;
		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button btInsertUsername;
		private System.Windows.Forms.CheckBox cbAutoSend;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbSendTimes;
		private System.ComponentModel.IContainer components;

		public QQMsgtailDlg()
		{
			InitializeComponent();
			//初始化两个combobox的选择项
			this.cbInterval.Text="10";
			this.cbSendTimes.SelectedIndex=0;
			//设置tooltip提示信息
			this.toolTip1.SetToolTip(this.cbAutoSend,"是否触发对发送按钮的点击，如果否，消息不会发送出去");
			this.toolTip1.SetToolTip(this.btInsertUsername,"自动在消息尾巴输入框中插入聊天对象的QQ昵称");
			this.toolTip1.SetToolTip(this.cbInterval,"选择每隔多少秒检测并触发一次");
			this.toolTip1.SetToolTip(this.btStartStop,"启动或者停止模拟");
			this.toolTip1.SetToolTip(this.btClose,"停止模拟并关闭此对话框");
			this.toolTip1.SetToolTip(this.cbSendTimes,"选择自动发送消息的最大次数，发送次数达到以后停止");
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
			this.components = new System.ComponentModel.Container();
			this.tbMsgtail = new System.Windows.Forms.TextBox();
			this.cbInterval = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btStartStop = new System.Windows.Forms.Button();
			this.btClose = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.btInsertUsername = new System.Windows.Forms.Button();
			this.cbAutoSend = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label4 = new System.Windows.Forms.Label();
			this.cbSendTimes = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// tbMsgtail
			// 
			this.tbMsgtail.Location = new System.Drawing.Point(8, 120);
			this.tbMsgtail.Multiline = true;
			this.tbMsgtail.Name = "tbMsgtail";
			this.tbMsgtail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbMsgtail.Size = new System.Drawing.Size(400, 80);
			this.tbMsgtail.TabIndex = 0;
			this.tbMsgtail.Text = "";
			// 
			// cbInterval
			// 
			this.cbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbInterval.Items.AddRange(new object[] {
															"5",
															"10",
															"20",
															"30",
															"40",
															"60"});
			this.cbInterval.Location = new System.Drawing.Point(88, 208);
			this.cbInterval.Name = "cbInterval";
			this.cbInterval.Size = new System.Drawing.Size(56, 20);
			this.cbInterval.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.Info;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(432, 80);
			this.label1.TabIndex = 2;
			this.label1.Text = "注意:\r\n（1）本程序仅模仿QQ病毒，但不会产生任何危害性行为，请勿担心。\r\n（2）模拟旨在演示机制，请切勿用于不良用途。\r\n（3）测试时请至少打开一个QQ聊天窗" +
				"口，设为聊天模式。\r\n（4）可使用字符串\"【USER】\"代表对方的QQ昵称。";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(232, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "请输入需要附加在QQ聊天内容后的信息:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 208);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "发送间隔(秒)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btStartStop
			// 
			this.btStartStop.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.btStartStop.Location = new System.Drawing.Point(264, 248);
			this.btStartStop.Name = "btStartStop";
			this.btStartStop.TabIndex = 6;
			this.btStartStop.Text = "启动";
			this.btStartStop.Click += new System.EventHandler(this.btStartStop_Click);
			// 
			// btClose
			// 
			this.btClose.Location = new System.Drawing.Point(339, 248);
			this.btClose.Name = "btClose";
			this.btClose.TabIndex = 7;
			this.btClose.Text = "关闭";
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 20;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// btInsertUsername
			// 
			this.btInsertUsername.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btInsertUsername.Location = new System.Drawing.Point(312, 88);
			this.btInsertUsername.Name = "btInsertUsername";
			this.btInsertUsername.Size = new System.Drawing.Size(96, 23);
			this.btInsertUsername.TabIndex = 8;
			this.btInsertUsername.Text = "插入对方昵称";
			this.btInsertUsername.Click += new System.EventHandler(this.btInsertUsername_Click);
			// 
			// cbAutoSend
			// 
			this.cbAutoSend.Checked = true;
			this.cbAutoSend.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbAutoSend.Location = new System.Drawing.Point(320, 208);
			this.cbAutoSend.Name = "cbAutoSend";
			this.cbAutoSend.Size = new System.Drawing.Size(80, 24);
			this.cbAutoSend.TabIndex = 9;
			this.cbAutoSend.Text = "自动发送";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(168, 208);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 23);
			this.label4.TabIndex = 11;
			this.label4.Text = "发送次数";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbSendTimes
			// 
			this.cbSendTimes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSendTimes.Items.AddRange(new object[] {
															 "无限",
															 "1",
															 "2",
															 "3",
															 "4",
															 "5"});
			this.cbSendTimes.Location = new System.Drawing.Point(224, 208);
			this.cbSendTimes.Name = "cbSendTimes";
			this.cbSendTimes.Size = new System.Drawing.Size(64, 20);
			this.cbSendTimes.TabIndex = 12;
			// 
			// QQMsgtailDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(430, 283);
			this.Controls.Add(this.cbSendTimes);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbAutoSend);
			this.Controls.Add(this.btInsertUsername);
			this.Controls.Add(this.btClose);
			this.Controls.Add(this.btStartStop);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbInterval);
			this.Controls.Add(this.tbMsgtail);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "QQMsgtailDlg";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "模拟QQ消息尾巴";
			this.ResumeLayout(false);

		}
		#endregion

		//-----------------------------------
		//	Button Handler
		//-----------------------------------
		//插入聊天对象昵称
		private void btInsertUsername_Click(object sender, System.EventArgs e)
		{
			int index=this.tbMsgtail.SelectionStart;
			this.tbMsgtail.Text=this.tbMsgtail.Text.Insert(index,this.m_UserNameMask);
			this.tbMsgtail.Focus();				//记得设置焦点，方便用户继续输入
			this.tbMsgtail.SelectionStart=index+this.m_UserNameMask.Length;	//更新插入点
			this.tbMsgtail.SelectionLength=0;
		}

		//启动/停止公用按钮
		private void btStartStop_Click(object sender, System.EventArgs e)
		{
			switch(this.btStartStop.Text)
			{
				case "启动":
					//获取用户填写的尾巴
					this.m_Msgtail=this.tbMsgtail.Text;
					//禁止controls
					this.tbMsgtail.Enabled=false;
					this.cbInterval.Enabled=false;
					this.btInsertUsername.Enabled=false;
					this.cbAutoSend.Enabled=false;
					this.cbSendTimes.Enabled=false;
					//发送年龄和最大发送次数
					this.m_life=0;
					this.m_SendTimes=this.cbSendTimes.SelectedIndex;
					//启动定时器
					this.timer1.Interval=Convert.ToInt32(this.cbInterval.Text)*1000;
					this.timer1.Start();
					this.btStartStop.Text="停止";
					this.btStartStop.BackColor=Color.FromArgb(255,192,192);	//设置浅红色背景
					break;
				case "停止":
					//停止计时器
					this.timer1.Stop();
					//恢复使能
					this.tbMsgtail.Enabled=true;
					this.cbInterval.Enabled=true;
					this.btInsertUsername.Enabled=true;
					this.cbAutoSend.Enabled=true;
					this.cbSendTimes.Enabled=true;
					this.btStartStop.Text="启动";
					this.btStartStop.BackColor=Color.FromArgb(192,255,192);	//设置浅绿色背景
					break;
			}
		}

		//关闭按钮
		private void btClose_Click(object sender, System.EventArgs e)
		{
            if(this.timer1.Enabled)
				this.timer1.Stop();
			this.Close();
		}

		//在定时器中完成对qq尾巴病毒行为的模拟（注意：请勿用于不良用途！！！）
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			int hwnd=0;
			string caption,username,tail;		//窗口标题，用户名称,需要添加的消息尾巴
			int hBtSend=0,hRichEdit=0;			//关键的句柄,发送按钮，聊天信息输入控件
			int charcode;						//ANSI编码的字符
			username=string.Empty;
            int clienttype=CT_QQ;						//客户端类型
			while((hwnd=MainFrm.FindWindowEx(0,hwnd,"#32770",null))>0)
			{
				//获取窗口标题
				MainFrm.GetWindowText(hwnd,this.m_StringBuilder,256);
                //判断窗口标题是否是"与 **** 聊天中"形式
				caption=this.m_StringBuilder.ToString();
				//如果标题太短，则不可能是任何聊天窗口
				if(caption.Length<5)
					continue;
				if(caption.StartsWith("与 ") && caption.EndsWith(" 聊天中"))
				{
					//QQ与某人聊天窗口
					clienttype=CT_QQ;
					username=caption.Substring(2,caption.Length-6);
					break;
				}
				else if(caption.StartsWith("与 ") && caption.EndsWith(" 交谈中"))
				{
					//TM与某人聊天窗口
					clienttype=CT_TM;
					username=caption.Substring(2,caption.Length-6);
					break;	
				}
				else if(caption.EndsWith(" － 群"))
				{
					//QQ群窗口
					clienttype=CT_QQ;
					username=caption.Substring(0,caption.Length-4);
					break;
				}
				else if(caption.EndsWith("  － 群 "))
				{
					//TM群窗口
					clienttype=CT_TM;
					username=caption.Substring(0,caption.Length-6);
					break;
				}
				else if(caption.StartsWith("多人聊天 － "))
				{
					//QQ讨论组窗口
					clienttype=CT_QQ;
					username=caption.Substring(7,caption.Length-7);
					break;
				}
				else if(caption.EndsWith(" － 讨论组 "))
				{
					//TM讨论组窗口
					clienttype=CT_TM;
					username=caption.Substring(0,caption.Length-7);
					break;
				}
			}
			//搜索了所有对话框还是没能找到QQ聊天窗口，则退出
			if(hwnd<=0)
				return;
			//获取客户区框架
			int hclient=MainFrm.FindWindowEx(hwnd,0,"#32770",null);
			//获取发送按钮句柄
			hBtSend=MainFrm.FindWindowEx(hclient,0,"Button",btSendName[clienttype]);
			//获取聊天输入框的父,这种类型的窗口有好几个，所以必须注意窗口顺序保证正确获取
			int hAfxWnd42_0=MainFrm.FindWindowEx(hclient,0,"AfxWnd42",null);
			//获取聊天输入框句柄
			hRichEdit=MainFrm.FindWindowEx(hAfxWnd42_0,0,"RICHEDIT",null);
			//将尾巴中的用户名掩码替换为真正的用户名称,注意m_Msgtail内容不会改变
			tail=this.m_Msgtail.Replace(this.m_UserNameMask,username);
			//获取默认编码（ANSI）编码)的字符数组
			byte[] bytes;
			//模拟键盘输入尾巴内容 (WM_CHAR）
			for(int i=0;i<tail.Length;i++)
			{
				bytes=Encoding.Default.GetBytes(tail[i].ToString());
				if(bytes.Length>1)
					charcode=bytes[1]*256 + bytes[0];		//两个字节表示的字符
				else
					charcode=bytes[0];						//一个字节表示的字符
                MainFrm.SendMessage(hRichEdit,MainFrm.WM_CHAR,charcode,1);
			}
			//点击发送按钮发送出去，wParam,lParam未用，必须为0
			if(this.cbAutoSend.Checked)
				MainFrm.SendMessage(hBtSend,MainFrm.BM_CLICK,0,0);
			//已经发送了一次，令定时器寿命增加
			this.m_life++;
			//判断寿命是否需要结束定时器
			if(this.m_SendTimes>0 && this.m_life>=this.m_SendTimes)
			{
				//点击停止按钮，停止计时器
				this.btStartStop.PerformClick();
			}
		}

	}
}
