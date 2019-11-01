using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace DesktopWndView
{
	/// <summary>
	/// QQMsgtailDlg ��ժҪ˵����
	/// </summary>
	public class QQMsgtailDlg : System.Windows.Forms.Form
	{
		#region �Զ������
		private StringBuilder m_StringBuilder=new StringBuilder(256);
		//�Է����ֵ�����ؼ���
		private string m_UserNameMask="��USER��";
		//��Ϣβ��
		private string m_Msgtail;
		//���ʹ�����0��ʾ���޴�)
		private int m_SendTimes;
		//���������������������ʹ���ʱֹͣ��ʱ��
		private int m_life=0;
		//�ͻ�������
		public const int CT_QQ=0;
		public const int CT_TM=1;
		string[] btSendName={"����(S)","����(&S)"};	//���Ͱ�ť�����֣�0��QQ��1��TM��
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
			//��ʼ������combobox��ѡ����
			this.cbInterval.Text="10";
			this.cbSendTimes.SelectedIndex=0;
			//����tooltip��ʾ��Ϣ
			this.toolTip1.SetToolTip(this.cbAutoSend,"�Ƿ񴥷��Է��Ͱ�ť�ĵ�����������Ϣ���ᷢ�ͳ�ȥ");
			this.toolTip1.SetToolTip(this.btInsertUsername,"�Զ�����Ϣβ��������в�����������QQ�ǳ�");
			this.toolTip1.SetToolTip(this.cbInterval,"ѡ��ÿ���������Ⲣ����һ��");
			this.toolTip1.SetToolTip(this.btStartStop,"��������ֹͣģ��");
			this.toolTip1.SetToolTip(this.btClose,"ֹͣģ�Ⲣ�رմ˶Ի���");
			this.toolTip1.SetToolTip(this.cbSendTimes,"ѡ���Զ�������Ϣ�������������ʹ����ﵽ�Ժ�ֹͣ");
		}

		/// <summary>
		/// ������������ʹ�õ���Դ��
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

		#region Windows ������������ɵĴ���
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
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
			this.label1.Text = "ע��:\r\n��1���������ģ��QQ����������������κ�Σ������Ϊ�������ġ�\r\n��2��ģ��ּ����ʾ���ƣ����������ڲ�����;��\r\n��3������ʱ�����ٴ�һ��QQ���촰" +
				"�ڣ���Ϊ����ģʽ��\r\n��4����ʹ���ַ���\"��USER��\"����Է���QQ�ǳơ�";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(232, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "��������Ҫ������QQ�������ݺ����Ϣ:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 208);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "���ͼ��(��)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btStartStop
			// 
			this.btStartStop.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(255)), ((System.Byte)(192)));
			this.btStartStop.Location = new System.Drawing.Point(264, 248);
			this.btStartStop.Name = "btStartStop";
			this.btStartStop.TabIndex = 6;
			this.btStartStop.Text = "����";
			this.btStartStop.Click += new System.EventHandler(this.btStartStop_Click);
			// 
			// btClose
			// 
			this.btClose.Location = new System.Drawing.Point(339, 248);
			this.btClose.Name = "btClose";
			this.btClose.TabIndex = 7;
			this.btClose.Text = "�ر�";
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
			this.btInsertUsername.Text = "����Է��ǳ�";
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
			this.cbAutoSend.Text = "�Զ�����";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(168, 208);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 23);
			this.label4.TabIndex = 11;
			this.label4.Text = "���ʹ���";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbSendTimes
			// 
			this.cbSendTimes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSendTimes.Items.AddRange(new object[] {
															 "����",
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
			this.Text = "ģ��QQ��Ϣβ��";
			this.ResumeLayout(false);

		}
		#endregion

		//-----------------------------------
		//	Button Handler
		//-----------------------------------
		//������������ǳ�
		private void btInsertUsername_Click(object sender, System.EventArgs e)
		{
			int index=this.tbMsgtail.SelectionStart;
			this.tbMsgtail.Text=this.tbMsgtail.Text.Insert(index,this.m_UserNameMask);
			this.tbMsgtail.Focus();				//�ǵ����ý��㣬�����û���������
			this.tbMsgtail.SelectionStart=index+this.m_UserNameMask.Length;	//���²����
			this.tbMsgtail.SelectionLength=0;
		}

		//����/ֹͣ���ð�ť
		private void btStartStop_Click(object sender, System.EventArgs e)
		{
			switch(this.btStartStop.Text)
			{
				case "����":
					//��ȡ�û���д��β��
					this.m_Msgtail=this.tbMsgtail.Text;
					//��ֹcontrols
					this.tbMsgtail.Enabled=false;
					this.cbInterval.Enabled=false;
					this.btInsertUsername.Enabled=false;
					this.cbAutoSend.Enabled=false;
					this.cbSendTimes.Enabled=false;
					//�������������ʹ���
					this.m_life=0;
					this.m_SendTimes=this.cbSendTimes.SelectedIndex;
					//������ʱ��
					this.timer1.Interval=Convert.ToInt32(this.cbInterval.Text)*1000;
					this.timer1.Start();
					this.btStartStop.Text="ֹͣ";
					this.btStartStop.BackColor=Color.FromArgb(255,192,192);	//����ǳ��ɫ����
					break;
				case "ֹͣ":
					//ֹͣ��ʱ��
					this.timer1.Stop();
					//�ָ�ʹ��
					this.tbMsgtail.Enabled=true;
					this.cbInterval.Enabled=true;
					this.btInsertUsername.Enabled=true;
					this.cbAutoSend.Enabled=true;
					this.cbSendTimes.Enabled=true;
					this.btStartStop.Text="����";
					this.btStartStop.BackColor=Color.FromArgb(192,255,192);	//����ǳ��ɫ����
					break;
			}
		}

		//�رհ�ť
		private void btClose_Click(object sender, System.EventArgs e)
		{
            if(this.timer1.Enabled)
				this.timer1.Stop();
			this.Close();
		}

		//�ڶ�ʱ������ɶ�qqβ�Ͳ�����Ϊ��ģ�⣨ע�⣺�������ڲ�����;��������
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			int hwnd=0;
			string caption,username,tail;		//���ڱ��⣬�û�����,��Ҫ��ӵ���Ϣβ��
			int hBtSend=0,hRichEdit=0;			//�ؼ��ľ��,���Ͱ�ť��������Ϣ����ؼ�
			int charcode;						//ANSI������ַ�
			username=string.Empty;
            int clienttype=CT_QQ;						//�ͻ�������
			while((hwnd=MainFrm.FindWindowEx(0,hwnd,"#32770",null))>0)
			{
				//��ȡ���ڱ���
				MainFrm.GetWindowText(hwnd,this.m_StringBuilder,256);
                //�жϴ��ڱ����Ƿ���"�� **** ������"��ʽ
				caption=this.m_StringBuilder.ToString();
				//�������̫�̣��򲻿������κ����촰��
				if(caption.Length<5)
					continue;
				if(caption.StartsWith("�� ") && caption.EndsWith(" ������"))
				{
					//QQ��ĳ�����촰��
					clienttype=CT_QQ;
					username=caption.Substring(2,caption.Length-6);
					break;
				}
				else if(caption.StartsWith("�� ") && caption.EndsWith(" ��̸��"))
				{
					//TM��ĳ�����촰��
					clienttype=CT_TM;
					username=caption.Substring(2,caption.Length-6);
					break;	
				}
				else if(caption.EndsWith(" �� Ⱥ"))
				{
					//QQȺ����
					clienttype=CT_QQ;
					username=caption.Substring(0,caption.Length-4);
					break;
				}
				else if(caption.EndsWith("  �� Ⱥ "))
				{
					//TMȺ����
					clienttype=CT_TM;
					username=caption.Substring(0,caption.Length-6);
					break;
				}
				else if(caption.StartsWith("�������� �� "))
				{
					//QQ�����鴰��
					clienttype=CT_QQ;
					username=caption.Substring(7,caption.Length-7);
					break;
				}
				else if(caption.EndsWith(" �� ������ "))
				{
					//TM�����鴰��
					clienttype=CT_TM;
					username=caption.Substring(0,caption.Length-7);
					break;
				}
			}
			//���������жԻ�����û���ҵ�QQ���촰�ڣ����˳�
			if(hwnd<=0)
				return;
			//��ȡ�ͻ������
			int hclient=MainFrm.FindWindowEx(hwnd,0,"#32770",null);
			//��ȡ���Ͱ�ť���
			hBtSend=MainFrm.FindWindowEx(hclient,0,"Button",btSendName[clienttype]);
			//��ȡ���������ĸ�,�������͵Ĵ����кü��������Ա���ע�ⴰ��˳��֤��ȷ��ȡ
			int hAfxWnd42_0=MainFrm.FindWindowEx(hclient,0,"AfxWnd42",null);
			//��ȡ�����������
			hRichEdit=MainFrm.FindWindowEx(hAfxWnd42_0,0,"RICHEDIT",null);
			//��β���е��û��������滻Ϊ�������û�����,ע��m_Msgtail���ݲ���ı�
			tail=this.m_Msgtail.Replace(this.m_UserNameMask,username);
			//��ȡĬ�ϱ��루ANSI������)���ַ�����
			byte[] bytes;
			//ģ���������β������ (WM_CHAR��
			for(int i=0;i<tail.Length;i++)
			{
				bytes=Encoding.Default.GetBytes(tail[i].ToString());
				if(bytes.Length>1)
					charcode=bytes[1]*256 + bytes[0];		//�����ֽڱ�ʾ���ַ�
				else
					charcode=bytes[0];						//һ���ֽڱ�ʾ���ַ�
                MainFrm.SendMessage(hRichEdit,MainFrm.WM_CHAR,charcode,1);
			}
			//������Ͱ�ť���ͳ�ȥ��wParam,lParamδ�ã�����Ϊ0
			if(this.cbAutoSend.Checked)
				MainFrm.SendMessage(hBtSend,MainFrm.BM_CLICK,0,0);
			//�Ѿ�������һ�Σ��ʱ����������
			this.m_life++;
			//�ж������Ƿ���Ҫ������ʱ��
			if(this.m_SendTimes>0 && this.m_life>=this.m_SendTimes)
			{
				//���ֹͣ��ť��ֹͣ��ʱ��
				this.btStartStop.PerformClick();
			}
		}

	}
}
