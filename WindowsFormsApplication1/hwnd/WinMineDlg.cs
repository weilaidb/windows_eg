using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;	//������
using System.Runtime.InteropServices;	//P-Invoke

namespace DesktopWndView
{
	/// <summary>
	/// WinMineDlg ��ժҪ˵����
	/// </summary>
	public class WinMineDlg : System.Windows.Forms.Form
	{
		#region �Զ������

		private Process m_WinMineProcess;//���̶���
		private int m_MainWndHandle;	//�����ھ��
		//�ֱ���ɨ�׽����е��ڴ��ַ
		private int iWidthAddress;		//��ȣ���λ����Ԫ��
		private int iHeightAddress;		//��ȣ���λ����Ԫ��
		private int iMinesAddress;		//������
		private int iCellBaseAddress;	//��Ԫ��������ʼ��ַ

		//�ڲ�����
		private int m_Width;			//��ȣ���
		private int m_Height;			//�߶ȣ���
		private int m_Mines;
		private byte[] m_Bytes=new byte[512];			//�������ն�ȡ�ֽڵĻ�����

		//�����ֲ�������λͼ
		private Bitmap m_OffScrBitmap;
		private Graphics m_GraphicsOffScr;
		//��ʾ��һ������
		private const int MINE= 0x8f;

		#endregion
		private System.Windows.Forms.Button btStartWinMine;
		private System.Windows.Forms.Button btDrawMineBitmap;
		private System.Windows.Forms.ImageList images;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.Button btClickAll;
		private System.Windows.Forms.Button btClickPart;
		private System.Windows.Forms.Button btMarkMine;
		private System.ComponentModel.IContainer components;

		//���캯��
		public WinMineDlg()
		{
			InitializeComponent();
			if(!this.InitAddresses())
			{
				MessageBox.Show("Sorry, only winXP and win2K are supported!");
				this.Close();
			}
			//��ʼ������λͼ
			this.InitOffScrBitmap();
		}

		#region P/Invoke KERNEL32
		// constants information can be found in <winnt.h>
		public const uint PROCESS_VM_READ = (0x0010);
		// function declarations are found in the MSDN and in <winbase.h> 
		
		//		HANDLE OpenProcess(
		//			DWORD dwDesiredAccess,  // access flag
		//			BOOL bInheritHandle,    // handle inheritance option
		//			DWORD dwProcessId       // process identifier
		//			);
		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(
			uint dwDesiredAccess, 
			int bInheritHandle, 
			int dwProcessId
			);

		//		BOOL CloseHandle(
		//			HANDLE hObject   // handle to object
		//			);
		[DllImport("kernel32.dll")]
		public static extern Int32 CloseHandle(
			IntPtr hObject
			);

		//		BOOL ReadProcessMemory(
		//			HANDLE hProcess,              // handle to the process
		//			LPCVOID lpBaseAddress,        // base of memory area
		//			LPVOID lpBuffer,              // data buffer
		//			SIZE_T nSize,                 // number of bytes to read
		//			SIZE_T * lpNumberOfBytesRead  // number of bytes read
		//			);
		[DllImport("kernel32.dll")]
		public static extern Int32 ReadProcessMemory(
			IntPtr hProcess, 
			int lpBaseAddress,
			[In, Out] byte[] buffer, 
			UInt32 size, 
			ref int lpNumberOfBytesRead
			);
		#endregion

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WinMineDlg));
			this.btStartWinMine = new System.Windows.Forms.Button();
			this.btDrawMineBitmap = new System.Windows.Forms.Button();
			this.btClickAll = new System.Windows.Forms.Button();
			this.btClickPart = new System.Windows.Forms.Button();
			this.images = new System.Windows.Forms.ImageList(this.components);
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.btMarkMine = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btStartWinMine
			// 
			this.btStartWinMine.Location = new System.Drawing.Point(0, 0);
			this.btStartWinMine.Name = "btStartWinMine";
			this.btStartWinMine.TabIndex = 0;
			this.btStartWinMine.Text = "����ɨ��";
			this.btStartWinMine.Click += new System.EventHandler(this.btStartWinMine_Click);
			// 
			// btDrawMineBitmap
			// 
			this.btDrawMineBitmap.Location = new System.Drawing.Point(75, 0);
			this.btDrawMineBitmap.Name = "btDrawMineBitmap";
			this.btDrawMineBitmap.TabIndex = 1;
			this.btDrawMineBitmap.Text = "������ͼ";
			this.btDrawMineBitmap.Click += new System.EventHandler(this.btDrawMineBitmap_Click);
			// 
			// btClickAll
			// 
			this.btClickAll.Location = new System.Drawing.Point(150, 0);
			this.btClickAll.Name = "btClickAll";
			this.btClickAll.TabIndex = 2;
			this.btClickAll.Text = "ȫ�����";
			this.btClickAll.Click += new System.EventHandler(this.btClickAll_Click);
			// 
			// btClickPart
			// 
			this.btClickPart.Location = new System.Drawing.Point(225, 0);
			this.btClickPart.Name = "btClickPart";
			this.btClickPart.TabIndex = 3;
			this.btClickPart.Text = "�������";
			this.btClickPart.Click += new System.EventHandler(this.btClickPart_Click);
			// 
			// images
			// 
			this.images.ImageSize = new System.Drawing.Size(13, 13);
			this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
			this.images.TransparentColor = System.Drawing.Color.Red;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 294);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(507, 22);
			this.statusBar1.TabIndex = 4;
			this.statusBar1.Text = "����";
			// 
			// btMarkMine
			// 
			this.btMarkMine.Location = new System.Drawing.Point(300, 0);
			this.btMarkMine.Name = "btMarkMine";
			this.btMarkMine.TabIndex = 5;
			this.btMarkMine.Text = "��ǵ���";
			this.btMarkMine.Click += new System.EventHandler(this.btMarkMine_Click);
			// 
			// WinMineDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(507, 316);
			this.Controls.Add(this.btMarkMine);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.btClickPart);
			this.Controls.Add(this.btClickAll);
			this.Controls.Add(this.btDrawMineBitmap);
			this.Controls.Add(this.btStartWinMine);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WinMineDlg";
			this.Text = "ɨ��������";
			this.ResumeLayout(false);

		}
		#endregion

		#region �ڲ���������

		//��ʼ��ɨ�׽��̵��ڲ����ݵ�ַ
		private bool InitAddresses()
		{
			// check if version is win2k or winXP
			if (Environment.OSVersion.Version.Major == 5)
			{
				if (Environment.OSVersion.Version.Minor == 0)	// win2K
				{
					// Thanks goes to Ryan Schreiber for discovering these addresses.
					iWidthAddress = 0x10056f8;
					iHeightAddress = 0x1005a68;
					iMinesAddress = 0x1005a6c;
					iCellBaseAddress = 0x1005700;
				}
				else	// winXP
				{
					iWidthAddress = 0x1005334;
					iHeightAddress = 0x1005338;
					iMinesAddress = 0x1005330;
					iCellBaseAddress = 0x1005340;
				}
				return true;
			}
			else
			{
				//MessageBox.Show("Sorry, only winXP and win2K are supported!");
				return false;
			}
		}

		//��ʼ������λͼ
		private void InitOffScrBitmap()
		{
			this.m_OffScrBitmap=new Bitmap(481,257);
			this.m_GraphicsOffScr=Graphics.FromImage(this.m_OffScrBitmap);
			//ˢ����
			this.m_GraphicsOffScr.FillRectangle(
				new SolidBrush(SystemColors.Control),
				0,0,481,257);
		}

		private void GetWinMineData()
		{
			int bytesRead=0;
			//�򿪽���
			OpenProcess(PROCESS_VM_READ,1,this.m_WinMineProcess.Id);
			//��ʼ��ȡ��������
			//��ȡ���
			ReadProcessMemory(this.m_WinMineProcess.Handle,
				this.iWidthAddress,
				this.m_Bytes,(uint)1,ref bytesRead);
			this.m_Width=this.m_Bytes[0];

			//�߶�
			ReadProcessMemory(this.m_WinMineProcess.Handle,
				this.iHeightAddress,
				this.m_Bytes,(uint)1,ref bytesRead);
			this.m_Height=this.m_Bytes[0];

			//������
			ReadProcessMemory(this.m_WinMineProcess.Handle,
				this.iMinesAddress,
				this.m_Bytes,(uint)1,ref bytesRead);
			this.m_Mines=this.m_Bytes[0];

			//��ȡ�ڴ��еĵ��׷ֲ�
			ReadProcessMemory(this.m_WinMineProcess.Handle,
				this.iCellBaseAddress+33,
				this.m_Bytes,(uint)512,ref bytesRead);

			//�رս���
			CloseHandle(this.m_WinMineProcess.Handle);
			//��ʾ״̬��Ϣ
			 this.statusBar1.Text=string.Format("{0}*{1} ����:{2}",
				 this.m_Width,this.m_Height,this.m_Mines);
		}

		//��������λͼ
		private void DrawOffScrBitmap()
		{
			//ˢ����
			this.m_GraphicsOffScr.FillRectangle(
				new SolidBrush(SystemColors.Control),
				0,0,481,257);
			
			//������
			for(int i=0;i<=this.m_Width;i++)
				this.m_GraphicsOffScr.DrawLine(Pens.Black,
					i*16,0,
					i*16,this.m_Height*16);

			//����
			for(int j=0;j<=this.m_Height;j++)
				this.m_GraphicsOffScr.DrawLine(Pens.Black,
					0,j*16,
					this.m_Width*16,j*16);

			//���Ƶ��׷ֲ�
			for(int j=0;j<this.m_Height;j++)
			{
				for(int i=0;i<this.m_Width;i++)
				{
					if(this.m_Bytes[i+j*32]==MINE)
					{
						//�ǵ��ף�����Ƶ���ͼƬ
						this.m_GraphicsOffScr.DrawImage(this.images.Images[0],
							i*16+2,j*16+2);
					}
				}
			}
		}

		//On Paint
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			e.Graphics.DrawImage(
				this.m_OffScrBitmap,
				10,
				this.btStartWinMine.Bottom+10);
		}

		#endregion

		private void btStartWinMine_Click(object sender, System.EventArgs e)
		{
            this.m_WinMineProcess=Process.Start("WinMine");	
			this.m_MainWndHandle=(int)this.m_WinMineProcess.MainWindowHandle;
		}

		private void btDrawMineBitmap_Click(object sender, System.EventArgs e)
		{
			//����ɨ����Ϸ�����ƣ���ȡ��������
			Process[] processes = Process.GetProcessesByName("winmine");
			// take first instance of minesweeper you find
			if (processes.Length == 0)
			{
				MessageBox.Show("û�з���ɨ�׽���!");
				return;
			}
			else
			{
				this.m_WinMineProcess=processes[0];
				this.m_MainWndHandle=(int)this.m_WinMineProcess.MainWindowHandle;
			}
			//��ȡ��Ϣ
			this.GetWinMineData();
			this.DrawOffScrBitmap();
			this.Invalidate(new Rectangle(10,this.btStartWinMine.Bottom+10,
				481,
				257));
		}

		//ȫ�����
		private void btClickAll_Click(object sender, System.EventArgs e)
		{
			//����ɨ����Ϸ�����ƣ���ȡ��������
			Process[] processes = Process.GetProcessesByName("winmine");
			// take first instance of minesweeper you find
			if (processes.Length == 0)
			{
				MessageBox.Show("û�з���ɨ�׽���!");
				return;
			}
			else
			{
				this.m_WinMineProcess=processes[0];
				this.m_MainWndHandle=(int)this.m_WinMineProcess.MainWindowHandle;
			}
			//��ȡȫ����Ϣ
			this.GetWinMineData();
			int lParam;
			//�������ǵ��׵�Ԫ��
			for(int j=0;j<this.m_Height;j++)
			{
				for(int i=0;i<this.m_Width;i++)
				{
					//�ж��Ƿ��ǵ���
					if(this.m_Bytes[i+j*32]!=MINE)
					{
						//������ǵ��ף������Ӧ�õ���Ŀͻ�������
						lParam=(i*16+19)+((j*16+62)<<16);
						MainFrm.SendMessage(this.m_MainWndHandle,MainFrm.WM_LBUTTONDOWN,0,lParam);
						MainFrm.SendMessage(this.m_MainWndHandle,MainFrm.WM_LBUTTONUP,0,lParam);
					}
				}
			}
		}

		//���һ����
		private void btClickPart_Click(object sender, System.EventArgs e)
		{
			//����ɨ����Ϸ�����ƣ���ȡ��������
			Process[] processes = Process.GetProcessesByName("winmine");
			// take first instance of minesweeper you find
			if (processes.Length == 0)
			{
				MessageBox.Show("û�з���ɨ�׽���!");
				return;
			}
			else
			{
				this.m_WinMineProcess=processes[0];
				this.m_MainWndHandle=(int)this.m_WinMineProcess.MainWindowHandle;
			}
			//��ȡȫ����Ϣ
			this.GetWinMineData();
			int lParam;
			//�������ǵ��׵�Ԫ��
			for(int j=0;j<this.m_Height;j++)
			{
				//��j��2ȡ����
				for(int i=j%2;i<this.m_Width;i+=2)
				{
					//�ж��Ƿ��ǵ���
					if(this.m_Bytes[i+j*32]!=MINE)
					{
						//������ǵ��ף������Ӧ�õ���Ŀͻ�������
						lParam=(i*16+19)+((j*16+62)<<16);
						MainFrm.SendMessage(this.m_MainWndHandle,MainFrm.WM_LBUTTONDOWN,0,lParam);
						MainFrm.SendMessage(this.m_MainWndHandle,MainFrm.WM_LBUTTONUP,0,lParam);
					}
				}
			}
		
		}

		//��ɨ�״����б�ǵ���
		private void btMarkMine_Click(object sender, System.EventArgs e)
		{
			//����ɨ����Ϸ�����ƣ���ȡ��������
			Process[] processes = Process.GetProcessesByName("winmine");
			// take first instance of minesweeper you find
			if (processes.Length == 0)
			{
				MessageBox.Show("û�з���ɨ�׽���!");
				return;
			}
			else
			{
				this.m_WinMineProcess=processes[0];
				this.m_MainWndHandle=(int)this.m_WinMineProcess.MainWindowHandle;
			}
			//��ȡȫ����Ϣ
			this.GetWinMineData();
			int lParam;
			//�������ǵ��׵�Ԫ��
			for(int j=0;j<this.m_Height;j++)
			{
				//��j��2ȡ����
				for(int i=0;i<this.m_Width;i++)
				{
					if(this.m_Bytes[i+j*32]==MINE)
					{
						//����ǵ��ף�������Ҽ���ǵ��ף���Ǻ󲻻�����
						lParam=(i*16+19)+((j*16+62)<<16);
						MainFrm.SendMessage(this.m_MainWndHandle,MainFrm.WM_RBUTTONDOWN,0,lParam);
						MainFrm.SendMessage(this.m_MainWndHandle,MainFrm.WM_RBUTTONUP,0,lParam);
					}
				}
			}		
		}
		/////////////////////////////////////////////////////
	}
}
