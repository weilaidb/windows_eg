using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;	//p/invoke
using System.Text;
using System.Diagnostics;
using System.Threading;	//for sleep

namespace DesktopWndView
{
	/// <summary>
	/// 分析桌面窗口的层次成分
	/// </summary>
	public class MainFrm : System.Windows.Forms.Form
	{
		#region 自定义变量
		//用于API函数的缓冲区，因为频繁使用，所以设为类成员变量
		private StringBuilder m_StringBuilder=new StringBuilder(256);
		//API回调函数（在c＃中，一个委托实例代表一个函数的指针（入口地址））
		public delegate bool WNDENUMPROC(int hwnd,int lParam);
		//枚举子窗口回调函数指针
		private WNDENUMPROC lpEnumChildFunc;
		//枚举线程窗口回调函数指针
		private WNDENUMPROC lpEnumThreadFunc;
		//用于判断线程下是否有窗口的回调函数，如果有，则添加一个临时节点
		private WNDENUMPROC lpAddTempChild2ThreadNode;
		//枚举桌面顶级窗口的回调函数指针
		private WNDENUMPROC lpEnumDesktopFunc;
		//保存当前窗口的矩形
		private RECT m_Rect=new RECT(0,0,0,0);
		//定时器寿命（也就是突出显示的绘制次数，注意为了复原，寿命应为偶数
		private int m_TimerLife;
		//缓存进程窗口的指针
        private ProcessDetailDlg m_ProcessDetailDlg;
		//缓存图片对话框的指针
		private BitmapBoxDlg m_BitmapBoxDlg;
		//是否需要在特定操作之前提醒用户
		private bool b_AskForConfirm;
		//应用程序名称
		static string MyAppName="桌面窗口查看器";
		#endregion

		#region API define
        
		/*
		 * Message ID Constants
		 */
		public const int WM_PAINT=0x000F;
		public const int WM_SETFOCUS = 0x0007;
		public const int WM_KILLFOCUS = 0x0008;
		public const int WM_ENABLE = 0x000A;
		public const int WM_SETTEXT = 0x000C;
		public const int WM_GETTEXT = 0x000D;
		public const int WM_GETTEXTLENGTH = 0x000E;
		public const int WM_CLOSE = 0x0010;
		public const int WM_QUIT = 0x0012;

		//获取和设置图标
		public const int WM_GETICON = 0x007F;
		public const int WM_SETICON = 0x0080;

		public const int ICON_SMALL = 0;
		public const int ICON_BIG   = 1;
		public const int ICON_SMALL2= 2;	//XP系统中使用
		/*
		 * Button Control Messages
		 */
		public const int BM_GETCHECK       = 0x00F0;
		public const int BM_SETCHECK       = 0x00F1;
		public const int BM_GETSTATE       = 0x00F2;
		public const int BM_SETSTATE       = 0x00F3;
		public const int BM_SETSTYLE       = 0x00F4;
		public const int BM_CLICK          = 0x00F5;	//单击按钮
		public const int BM_GETIMAGE       = 0x00F6;
		public const int BM_SETIMAGE       = 0x00F7;
		public const int BST_UNCHECKED     = 0x0000;
		public const int BST_CHECKED       = 0x0001;
		public const int BST_INDETERMINATE = 0x0002;
		public const int BST_PUSHED        = 0x0004;
		public const int BST_FOCUS         = 0x0008;
		
		/*
		 * 键盘消息
		 */
		public const int WM_KEYFIRST      =               0x0100;
		public const int WM_KEYDOWN       =               0x0100;
		public const int WM_KEYUP         =               0x0101;
		public const int WM_CHAR          =               0x0102;
		public const int WM_DEADCHAR      =               0x0103;
		public const int WM_SYSKEYDOWN    =               0x0104;
		public const int WM_SYSKEYUP      =               0x0105;
		public const int WM_SYSCHAR       =               0x0106;
		public const int WM_SYSDEADCHAR   =               0x0107;
		public const int WM_UNICHAR       =               0x0109;			//XP系统
		public const int WM_KEYLAST       =               0x0109;
		public const int UNICODE_NOCHAR   =               0xFFFF;

		/*
		 * 鼠标消息ID
		 */
		public const int WM_MOUSEFIRST      = 0x0200;
		public const int WM_MOUSEMOVE       = 0x0200;
		public const int WM_LBUTTONDOWN     = 0x0201;
		public const int WM_LBUTTONUP       = 0x0202;
		public const int WM_LBUTTONDBLCLK   = 0x0203;
		public const int WM_RBUTTONDOWN     = 0x0204;
		public const int WM_RBUTTONUP       = 0x0205;
		public const int WM_RBUTTONDBLCLK   = 0x0206;
		public const int WM_MBUTTONDOWN     = 0x0207;
		public const int WM_MBUTTONUP       = 0x0208;
		public const int WM_MBUTTONDBLCLK   = 0x0209;
		public const int WM_MOUSEWHEEL      = 0x020A;	//滚轮消息
		public const int WM_XBUTTONDOWN     = 0x020B;
		public const int WM_XBUTTONUP       = 0x020C;
		public const int WM_XBUTTONDBLCLK   = 0x020D;

		/*
		 * Key State Masks for Mouse Messages
		 */
		public const int MK_LBUTTON        =  0x0001;
		public const int MK_RBUTTON        =  0x0002;
		public const int MK_SHIFT          =  0x0004;
		public const int MK_CONTROL        =  0x0008;
		public const int MK_MBUTTON        =  0x0010;
		public const int MK_XBUTTON1       =  0x0020;
		public const int MK_XBUTTON2       =  0x0040;
		/*
		 * ShowWindow() Constants
		 */
		public const int SW_HIDE           = 0;
		public const int SW_SHOWNORMAL     = 1;
		public const int SW_NORMAL         = 1;
		public const int SW_SHOWMINIMIZED  = 2;
		public const int SW_SHOWMAXIMIZED  = 3;
		public const int SW_MAXIMIZE       = 3;
		public const int SW_SHOWNOACTIVATE = 4;		//显示但不激活窗口，一个关键参数
		public const int SW_SHOW           = 5;
		public const int SW_MINIMIZE       = 6;
		public const int SW_SHOWMINNOACTIVE= 7;
		public const int SW_SHOWNA         = 8;
		public const int SW_RESTORE        = 9;
		public const int SW_SHOWDEFAULT    = 10;
		public const int SW_FORCEMINIMIZE  = 11;
		public const int SW_MAX            = 11;

		/*
		 * GetWindow() Constants
		*/
		public const uint GW_HWNDFIRST      =0;
		public const uint GW_HWNDLAST       =1;
		public const uint GW_HWNDNEXT       =2;
		public const uint GW_HWNDPREV       =3;
		public const uint GW_OWNER          =4;
		public const uint GW_CHILD          =5;
		public const uint GW_ENABLEDPOPUP   =6;
		public const uint GW_MAX            =6;	//(低版本系统中定义为5)

		/* Binary raster ops */
		public const int R2_BLACK           =1;   /*  0       */
		public const int R2_NOTMERGEPEN     =2;   /* DPon     */
		public const int R2_MASKNOTPEN      =3;   /* DPna     */
		public const int R2_NOTCOPYPEN      =4;   /* PN       */
		public const int R2_MASKPENNOT      =5;   /* PDna     */
		public const int R2_NOT             =6;   /* Dn       */
		public const int R2_XORPEN          =7;   /* DPx      */
		public const int R2_NOTMASKPEN      =8;   /* DPan     */
		public const int R2_MASKPEN         =9;   /* DPa      */
		public const int R2_NOTXORPEN       =10;  /* DPxn     */
		public const int R2_NOP             =11;  /* D        */
		public const int R2_MERGENOTPEN     =12;  /* DPno     */
		public const int R2_COPYPEN         =13;  /* P        */
		public const int R2_MERGEPENNOT     =14;  /* PDno     */
		public const int R2_MERGEPEN        =15;  /* DPo      */
		public const int R2_WHITE           =16;  /*  1       */
		public const int R2_LAST            =16;

		/* Ternary raster operations 光栅操作码，BitBlt函数的参数 */
		public const int SRCCOPY             =0x00CC0020; /* dest = source                   */
		public const int SRCPAINT            =0x00EE0086; /* dest = source OR dest           */
		public const int SRCAND              =0x008800C6; /* dest = source AND dest          */
		public const int SRCINVERT           =0x00660046; /* dest = source XOR dest          */
		public const int SRCERASE            =0x00440328; /* dest = source AND (NOT dest )   */
		public const int NOTSRCCOPY          =0x00330008; /* dest = (NOT source)             */
		public const int NOTSRCERASE         =0x001100A6; /* dest = (NOT src) AND (NOT dest) */
		public const int MERGECOPY           =0x00C000CA; /* dest = (source AND pattern)     */
		public const int MERGEPAINT          =0x00BB0226; /* dest = (NOT source) OR dest     */
		public const int PATCOPY             =0x00F00021; /* dest = pattern                  */
		public const int PATPAINT            =0x00FB0A09; /* dest = DPSnoo                   */
		public const int PATINVERT           =0x005A0049; /* dest = pattern XOR dest         */
		public const int DSTINVERT           =0x00550009; /* dest = (NOT dest)               */
		public const int BLACKNESS           =0x00000042; /* dest = BLACK                    */
		public const int WHITENESS           =0x00FF0062; /* dest = WHITE                    */

		/* Pen Styles */
		public const int PS_SOLID          =0;
		public const int PS_DASH           =1;       /* -------  */
		public const int PS_DOT            =2;       /* .......  */
		public const int PS_DASHDOT        =3;       /* _._._._  */
		public const int PS_DASHDOTDOT     =4;       /* _.._.._  */
		public const int PS_NULL           =5;
		public const int PS_INSIDEFRAME    =6;
		public const int PS_USERSTYLE      =7;
		public const int PS_ALTERNATE      =8;
		public const int PS_STYLE_MASK     =0x0000000F;

		public const int PS_ENDCAP_ROUND   =0x00000000;
		public const int PS_ENDCAP_SQUARE  =0x00000100;
		public const int PS_ENDCAP_FLAT    =0x00000200;
		public const int PS_ENDCAP_MASK    =0x00000F00;

		public const int PS_JOIN_ROUND     =0x00000000;
		public const int PS_JOIN_BEVEL     =0x00001000;
		public const int PS_JOIN_MITER     =0x00002000;
		public const int PS_JOIN_MASK      =0x0000F000;

		public const int PS_COSMETIC       =0x00000000;
		public const int PS_GEOMETRIC      =0x00010000;
		public const int PS_TYPE_MASK      =0x000F0000;

		#endregion

		#region 窗体设计器变量

		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.Button btUpdate;
		private System.Windows.Forms.Button btMoveLeft;
		private System.Windows.Forms.Button btMoveRight;
		private System.Windows.Forms.TreeView tvWnd;
		private System.Windows.Forms.Button btMoveUp;
		private System.Windows.Forms.Button btMoveDown;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ImageList imagesOfTreeView;
		private System.Windows.Forms.MenuItem mnShowWnd;
		private System.Windows.Forms.MenuItem mnHideWnd;
		private System.Windows.Forms.MenuItem mnHighLightWnd;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnRedrawWnd;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioBtOnlyEnumExplorer;
		private System.Windows.Forms.RadioButton radioBtEnumAllWnd;
		private System.Windows.Forms.ContextMenu cMenuWnd;
		private System.Windows.Forms.Button btShowWnd;
		private System.Windows.Forms.Button btHideWnd;
		private System.Windows.Forms.Button btHighLightWnd;
		private System.Windows.Forms.Button btFindWindow;
		private System.Windows.Forms.Button btDisableWnd;
		private System.Windows.Forms.Button btEnableWnd;
		private System.Windows.Forms.MenuItem mnEnableWnd;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem mnDisableWnd;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.RadioButton radioBtEnumAllProcess;
		private System.Windows.Forms.RadioButton radioBtEnumOneProcess;
		private System.Windows.Forms.ContextMenu cMenuProcess;
		private System.Windows.Forms.MenuItem mnViewProcessDetail;
		private System.Windows.Forms.CheckBox cbAskForConfirm;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mnAbout;
		private System.Windows.Forms.MenuItem mnSnapWnd;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem mnUpdateInfo;
		private System.Windows.Forms.MenuItem mnMaximizeWnd;
		private System.Windows.Forms.MenuItem mnMinimizeWnd;
		private System.Windows.Forms.MenuItem mnRestoreWnd;
		private System.Windows.Forms.MenuItem mnShowDefaultWnd;
		private System.Windows.Forms.MenuItem mnClickBtn;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem mnGetText;
		private System.Windows.Forms.MenuItem mnSendCharA;
		private System.Windows.Forms.MenuItem mnSendString;
		private System.Windows.Forms.MenuItem mnQQMsgtail;
		private System.Windows.Forms.ContextMenu cMenuThread;
		private System.Windows.Forms.MenuItem mnReloadChild;
		private System.Windows.Forms.CheckBox cbAutoHighlightWnd;
		private System.Windows.Forms.MenuItem mnIEMan;
		private System.Windows.Forms.MenuItem mnWinMine;
		private System.Windows.Forms.MenuItem mnlCloseWnd;
		private System.Windows.Forms.PictureBox pbIcon;
		private System.Windows.Forms.MenuItem mnPEView;
		private System.ComponentModel.IContainer components;

		
		#endregion

		#region MainFrm Creator and Handler （构造函数等）
		public MainFrm()
		{
			InitializeComponent();
			//枚举窗口子窗口的函数指针
			this.lpEnumChildFunc=new WNDENUMPROC(this.EnumChildWndProc);
			//枚举线程窗口的函数指针
			this.lpEnumThreadFunc=new WNDENUMPROC(this.EnumThreadWndProc);
			//为线程节点添加临时子节点的回调函数指针
			this.lpAddTempChild2ThreadNode=new WNDENUMPROC(this.AddTempChild2ThreadNode);
			//枚举桌面窗口的函数指针
			this.lpEnumDesktopFunc=new WNDENUMPROC(this.EnumDesktopWndFunc);
			//默认为重要操作询问用户
			this.b_AskForConfirm=true;
		}
		private void MainFrm_SizeChanged(object sender, System.EventArgs e)
		{
			this.SuspendLayout();
			//组合框
			this.groupBox1.Width=this.Width-20;
			this.btFindWindow.Left=this.groupBox1.Right-this.btFindWindow.Width-20;
			this.btUpdate.Left=this.btFindWindow.Left-this.btUpdate.Width;
			this.tvWnd.Height=this.statusBar1.Top-this.tvWnd.Top;
			this.tbDescription.Height=this.statusBar1.Top-this.tbDescription.Top;
			this.ResumeLayout();
		}
		#endregion

		#region P/Invoke USER32
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT 
		{
			public RECT(int _left,int _top,int _right,int _bottom)
			{
				Left=_left;
				Top=_top;
				Right=_right;
				Bottom=_bottom;
			}
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		//Declare wrapper managed POINT class.
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT 
		{
			public POINT(long _x,long _y)
			{
				X=_x;
				Y=_y;
			}
			public long X;
			public long Y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PAINTSTRUCT
		{ 
			public int  hdc; 
			public bool fErase; 
			public RECT rcPaint; 
			public bool fRestore; 
			public bool fIncUpdate; 
			public byte[] rgbReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct LOGPEN
		{
			public uint lopnStyle;
			POINT lopnWidth;
			int lopnColor;
		}

		[DllImport("user32", EntryPoint="SetParent")]
		public static extern int SetParent (
			IntPtr hwndChild,
			int hwndNewParent
			);
		[DllImport("user32", EntryPoint="FindWindowA")]
		public static extern int FindWindow(
			string lpClassName,
			string lpWindowName
			);
		[DllImport("user32", EntryPoint="FindWindowExA")]
		public static extern int FindWindowEx (
			int hwndParent,
			int hwndChildAfter,
			string lpszClass,		//窗口类
			string lpszWindow		//窗口标题
			);

		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		public static extern int SendMessage(
			int hWnd, 
			int wMsg, 
			int wParam, 
			IntPtr lParam
			);
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		public static extern int SendMessage(
			int hWnd, 
			int wMsg, 
			int wParam, 
			int lParam
			);

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern int SendMessage(
			int hWnd, 
			int wMsg, 
			int wParam, 
			string lParam
			);

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern int SendMessage(
			int hWnd, 
			int wMsg, 
			int wParam, 
			StringBuilder lParam
			);

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(
			int hWnd,
			ref int lpdwProcessId);

		[DllImport("user32")]
		public static extern int Sleep(
			int dwMilliseconds
			);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(
			int hWnd,
			ref RECT lpRect
			);

		[DllImport("user32")]
		public static extern int GetWindowText(
			int hWnd,
			StringBuilder lpString,
			int nMaxCount
			);


		[DllImport("user32.dll")]
		public static extern bool ShowWindow(
			int hWnd,
			int nCmdShow
			);

		[DllImport("user32", EntryPoint="SetWindowLong")]
		public static extern uint SetWindowLong (
			IntPtr hwnd,
			int nIndex,
			uint dwNewLong
			);
		[DllImport("user32", EntryPoint="GetWindowLong")]
		public static extern uint GetWindowLong (
			IntPtr hwnd,
			int nIndex
			);
		[DllImport("user32", EntryPoint="SetLayeredWindowAttributes")]
		public static extern int SetLayeredWindowAttributes (
			IntPtr hwnd,			//目标窗口句柄
			int ColorRefKey,		//透明色
			int bAlpha,				//不透明度
			int dwFlags
			);

		[DllImport("user32")]
		public static extern bool MoveWindow(
			int hWnd,
			int X,
			int Y,
			int nWidth,
			int nHeight,
			bool bRepaint
			);

		//获得窗口类名称，返回值为字符串的字符数量
		[DllImport("user32")]
		public static extern uint RealGetWindowClass(
			int hWnd,
			StringBuilder pszType,		//缓冲区
			uint cchType);				//缓冲区长度

		//枚举屏幕上所有顶级窗口（不会枚举子窗口，除了一些有WS_CHILD的顶级窗口）
		[DllImport("user32")]
		public static extern bool EnumWindows(
			WNDENUMPROC lpEnumFunc,
			int lParam
			);

		[DllImport("user32")]
		public static extern bool EnumChildWindows(
			int hWndParent,
			WNDENUMPROC lpEnumFunc,
			int lParam
			);

		[DllImport("user32")]
		public static extern bool EnumThreadWindows(		//枚举线程窗口
			int dwThreadId,
			WNDENUMPROC lpEnumFunc,
			int lParam
			);

		[DllImport("user32")]
		public static extern int GetParent(
			int hWnd
			);

		[DllImport("user32")]
		public static extern int GetWindow(
			int hWnd,	//基础窗口
			uint uCmd	//关系
			);

		[DllImport("user32")]
		public static extern int GetDC(
			int hWnd
			);

		[DllImport("user32")]
		public static extern int GetWindowDC(
			int hWnd
			);

		[DllImport("user32")]
		public static extern int ReleaseDC(
			int hWnd,
			int hDC
			);

		[DllImport("user32")]
		public static extern int FillRect(
			int hDC,
			RECT lprc,
			int hBrush
			);

		[DllImport("user32")]
		public static extern bool InvalidateRect(
			int hwnd,
			ref RECT lpRect,
			bool bErase
			);

		//判断一个窗口是否是可见的
		[DllImport("user32")]
		public static extern bool IsWindowVisible(
			int hwnd
			);

		//绘制焦点举行
		[DllImport("user32")]
		public static extern bool DrawFocusRect(
			int hDC,
			ref RECT lprc
			);

		[DllImport("user32")]
		public static extern bool UpdateWindow(
			int hwnd
			);

		[DllImport("user32")]
		public static extern bool EnableWindow(
			int hwnd,
			bool bEnable
			);

		//设置前景窗口，强制其线程成为前台，并激活窗口
		[DllImport("user32")]
		public static extern bool SetForegroundWindow(
			int hwnd
			);

		//获取拥有焦点窗口（唯一拥有键盘输入的窗口）
		[DllImport("user32")]
		public static extern int GetFocus(
			);

		//设置焦点窗口（返回值是前一个焦点窗口）
		[DllImport("user32")]
		public static extern int SetFocus(
			int hwnd
			);

		//根据点查找窗口
		[DllImport("user32")]
		public static extern int WindowFromPoint(
			POINT Point
			);

		[DllImport("user32")]
		public static extern int ChildWindowFromPoint(
			int hWndParent,
			POINT Point
			);

		[DllImport("user32")]
		public static extern bool DestroyIcon(
			int hIcon
			);


		#endregion

		#region P/Invoke GDI32
		[DllImport("Gdi32")]
		public static extern int SetROP2(
			int hDC,
			int fnDrawMode
			);

		[DllImport("Gdi32")]
		public static extern int GetROP2(
			int hDC
			);

        [DllImport("Gdi32")]
		public static extern bool ValidateRect(
			int hWnd,
			ref RECT lpRect
			);

		[DllImport("Gdi32")]
		public static extern int CreateSolidBrush(
			int crColor
			);

		[DllImport("Gdi32")]
		public static extern int CreateDC(
			string lpszDriver,
			string lpszDevice,
			string lpszOutput,
			int lpInitData		//这个参数实际是一个 DEVMODE 结构的指针
			);

		[DllImport("Gdi32")]
		public static extern int SelectObject(
			int hDC,
			int hGdiObj
			);

		[DllImport("Gdi32")]
		public static extern int DeleteObject(
			int hObject
			);

		[DllImport("Gdi32")]
		public static extern int CreatePen(
			int fnPenStyle,
			int nWidth,
			int crColor
			);

		[DllImport("Gdi32")]
		public static extern int CreatePenIndirect(
			ref LOGPEN lplogpen
			);

		[DllImport("Gdi32")]
		public static extern bool MoveToEx(
			int hDC,
			int X,
			int Y,
			ref POINT lpPoint
			);

		[DllImport("Gdi32")]
		public static extern bool LineTo(
			int hDC,
			int nXEnd,
			int nYEnd
			);

		//无伸展位图传送
		[DllImport("Gdi32")]
		public static extern bool BitBlt(
			int hdcDest,
			int nXDest,
			int nYDest,
			int nWidth,
			int nHeight,
			int hdcSrc,
			int nXsrc,
			int nYsrc,
			int dwRop		//光栅操作码
			);

		//获取特定设备的特定信息，例如屏幕象素高度，宽度
		[DllImport("Gdi32")]
		public static extern bool GetDeviceCaps(
			int hdc,
			int nIndex
			);

		//创建一个匹配的内存DC，返回其句柄
		[DllImport("Gdi32")]
		public static extern int CreateCompatibleDC(
			int hDC		//如果此参数null，则创建于屏幕匹配的内存dc
			);

		//创建一个与某dc匹配的内存位图，返回句柄
		[DllImport("Gdi32")]
		public static extern int CreateCompatibleBitmap(
			int hDC,
			int nWidth,
			int nHeight
			);


		#endregion

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainFrm));
			this.tvWnd = new System.Windows.Forms.TreeView();
			this.imagesOfTreeView = new System.Windows.Forms.ImageList(this.components);
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.btShowWnd = new System.Windows.Forms.Button();
			this.btHideWnd = new System.Windows.Forms.Button();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.btUpdate = new System.Windows.Forms.Button();
			this.btMoveLeft = new System.Windows.Forms.Button();
			this.btMoveRight = new System.Windows.Forms.Button();
			this.btMoveUp = new System.Windows.Forms.Button();
			this.btMoveDown = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.cMenuWnd = new System.Windows.Forms.ContextMenu();
			this.mnShowWnd = new System.Windows.Forms.MenuItem();
			this.mnHideWnd = new System.Windows.Forms.MenuItem();
			this.mnMinimizeWnd = new System.Windows.Forms.MenuItem();
			this.mnMaximizeWnd = new System.Windows.Forms.MenuItem();
			this.mnRestoreWnd = new System.Windows.Forms.MenuItem();
			this.mnShowDefaultWnd = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.mnEnableWnd = new System.Windows.Forms.MenuItem();
			this.mnDisableWnd = new System.Windows.Forms.MenuItem();
			this.mnlCloseWnd = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnHighLightWnd = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mnClickBtn = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.mnRedrawWnd = new System.Windows.Forms.MenuItem();
			this.mnSnapWnd = new System.Windows.Forms.MenuItem();
			this.mnGetText = new System.Windows.Forms.MenuItem();
			this.mnSendCharA = new System.Windows.Forms.MenuItem();
			this.mnSendString = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mnUpdateInfo = new System.Windows.Forms.MenuItem();
			this.btHighLightWnd = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioBtEnumAllProcess = new System.Windows.Forms.RadioButton();
			this.btFindWindow = new System.Windows.Forms.Button();
			this.radioBtEnumOneProcess = new System.Windows.Forms.RadioButton();
			this.radioBtEnumAllWnd = new System.Windows.Forms.RadioButton();
			this.radioBtOnlyEnumExplorer = new System.Windows.Forms.RadioButton();
			this.btDisableWnd = new System.Windows.Forms.Button();
			this.btEnableWnd = new System.Windows.Forms.Button();
			this.cMenuProcess = new System.Windows.Forms.ContextMenu();
			this.mnViewProcessDetail = new System.Windows.Forms.MenuItem();
			this.cbAskForConfirm = new System.Windows.Forms.CheckBox();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mnIEMan = new System.Windows.Forms.MenuItem();
			this.mnWinMine = new System.Windows.Forms.MenuItem();
			this.mnQQMsgtail = new System.Windows.Forms.MenuItem();
			this.mnPEView = new System.Windows.Forms.MenuItem();
			this.mnAbout = new System.Windows.Forms.MenuItem();
			this.cMenuThread = new System.Windows.Forms.ContextMenu();
			this.mnReloadChild = new System.Windows.Forms.MenuItem();
			this.cbAutoHighlightWnd = new System.Windows.Forms.CheckBox();
			this.pbIcon = new System.Windows.Forms.PictureBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvWnd
			// 
			this.tvWnd.HideSelection = false;
			this.tvWnd.ImageList = this.imagesOfTreeView;
			this.tvWnd.Location = new System.Drawing.Point(8, 64);
			this.tvWnd.Name = "tvWnd";
			this.tvWnd.Size = new System.Drawing.Size(360, 352);
			this.tvWnd.TabIndex = 0;
			this.tvWnd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvWnd_MouseDown);
			this.tvWnd.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvWnd_AfterSelect);
			this.tvWnd.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvWnd_BeforeExpand);
			// 
			// imagesOfTreeView
			// 
			this.imagesOfTreeView.ImageSize = new System.Drawing.Size(16, 16);
			this.imagesOfTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagesOfTreeView.ImageStream")));
			this.imagesOfTreeView.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 423);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(656, 22);
			this.statusBar1.TabIndex = 2;
			this.statusBar1.Text = "Author: Jinfd";
			// 
			// btShowWnd
			// 
			this.btShowWnd.Location = new System.Drawing.Point(376, 88);
			this.btShowWnd.Name = "btShowWnd";
			this.btShowWnd.Size = new System.Drawing.Size(80, 23);
			this.btShowWnd.TabIndex = 3;
			this.btShowWnd.Text = "显示窗口";
			this.btShowWnd.Click += new System.EventHandler(this.btShow_Click);
			// 
			// btHideWnd
			// 
			this.btHideWnd.Location = new System.Drawing.Point(456, 88);
			this.btHideWnd.Name = "btHideWnd";
			this.btHideWnd.Size = new System.Drawing.Size(80, 23);
			this.btHideWnd.TabIndex = 4;
			this.btHideWnd.Text = "隐藏窗口";
			this.btHideWnd.Click += new System.EventHandler(this.btHide_Click);
			// 
			// tbDescription
			// 
			this.tbDescription.HideSelection = false;
			this.tbDescription.Location = new System.Drawing.Point(376, 192);
			this.tbDescription.Multiline = true;
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.ReadOnly = true;
			this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbDescription.Size = new System.Drawing.Size(256, 224);
			this.tbDescription.TabIndex = 5;
			this.tbDescription.Text = "";
			// 
			// btUpdate
			// 
			this.btUpdate.Location = new System.Drawing.Point(464, 16);
			this.btUpdate.Name = "btUpdate";
			this.btUpdate.Size = new System.Drawing.Size(88, 24);
			this.btUpdate.TabIndex = 6;
			this.btUpdate.Text = "加载数据";
			this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
			// 
			// btMoveLeft
			// 
			this.btMoveLeft.Location = new System.Drawing.Point(456, 136);
			this.btMoveLeft.Name = "btMoveLeft";
			this.btMoveLeft.Size = new System.Drawing.Size(80, 23);
			this.btMoveLeft.TabIndex = 8;
			this.btMoveLeft.Text = "左移50";
			this.btMoveLeft.Click += new System.EventHandler(this.btMoveLeft_Click);
			// 
			// btMoveRight
			// 
			this.btMoveRight.Location = new System.Drawing.Point(376, 136);
			this.btMoveRight.Name = "btMoveRight";
			this.btMoveRight.Size = new System.Drawing.Size(80, 23);
			this.btMoveRight.TabIndex = 7;
			this.btMoveRight.Text = "右移50";
			this.btMoveRight.Click += new System.EventHandler(this.btMoveRight_Click);
			// 
			// btMoveUp
			// 
			this.btMoveUp.Location = new System.Drawing.Point(456, 160);
			this.btMoveUp.Name = "btMoveUp";
			this.btMoveUp.Size = new System.Drawing.Size(80, 23);
			this.btMoveUp.TabIndex = 10;
			this.btMoveUp.Text = "上移50";
			this.btMoveUp.Click += new System.EventHandler(this.btMoveUp_Click);
			// 
			// btMoveDown
			// 
			this.btMoveDown.Location = new System.Drawing.Point(376, 160);
			this.btMoveDown.Name = "btMoveDown";
			this.btMoveDown.Size = new System.Drawing.Size(80, 23);
			this.btMoveDown.TabIndex = 9;
			this.btMoveDown.Text = "下移50";
			this.btMoveDown.Click += new System.EventHandler(this.btMoveDown_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 200;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// cMenuWnd
			// 
			this.cMenuWnd.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnShowWnd,
																					 this.mnHideWnd,
																					 this.mnMinimizeWnd,
																					 this.mnMaximizeWnd,
																					 this.mnRestoreWnd,
																					 this.mnShowDefaultWnd,
																					 this.menuItem2,
																					 this.mnEnableWnd,
																					 this.mnDisableWnd,
																					 this.mnlCloseWnd,
																					 this.menuItem3,
																					 this.mnHighLightWnd,
																					 this.menuItem1,
																					 this.mnClickBtn,
																					 this.menuItem7,
																					 this.mnRedrawWnd,
																					 this.mnSnapWnd,
																					 this.mnGetText,
																					 this.mnSendCharA,
																					 this.mnSendString,
																					 this.menuItem5,
																					 this.mnUpdateInfo});
			this.cMenuWnd.Popup += new System.EventHandler(this.cMenuWnd_Popup);
			// 
			// mnShowWnd
			// 
			this.mnShowWnd.Index = 0;
			this.mnShowWnd.Text = "显示窗口";
			this.mnShowWnd.Click += new System.EventHandler(this.mnShowWnd_Click);
			// 
			// mnHideWnd
			// 
			this.mnHideWnd.Index = 1;
			this.mnHideWnd.Text = "隐藏窗口";
			this.mnHideWnd.Click += new System.EventHandler(this.mnHideWnd_Click);
			// 
			// mnMinimizeWnd
			// 
			this.mnMinimizeWnd.Index = 2;
			this.mnMinimizeWnd.Text = "最小化窗口";
			this.mnMinimizeWnd.Click += new System.EventHandler(this.mnMinimizeWnd_Click);
			// 
			// mnMaximizeWnd
			// 
			this.mnMaximizeWnd.Index = 3;
			this.mnMaximizeWnd.Text = "最大化窗口";
			this.mnMaximizeWnd.Click += new System.EventHandler(this.mnMaximizeWnd_Click);
			// 
			// mnRestoreWnd
			// 
			this.mnRestoreWnd.Index = 4;
			this.mnRestoreWnd.Text = "恢复窗口";
			this.mnRestoreWnd.Click += new System.EventHandler(this.mnRestoreWnd_Click);
			// 
			// mnShowDefaultWnd
			// 
			this.mnShowDefaultWnd.Index = 5;
			this.mnShowDefaultWnd.Text = "复位到默认";
			this.mnShowDefaultWnd.Click += new System.EventHandler(this.mnShowDefaultWnd_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 6;
			this.menuItem2.Text = "-";
			// 
			// mnEnableWnd
			// 
			this.mnEnableWnd.Index = 7;
			this.mnEnableWnd.Text = "使能窗口";
			this.mnEnableWnd.Click += new System.EventHandler(this.mnEnableWnd_Click);
			// 
			// mnDisableWnd
			// 
			this.mnDisableWnd.Index = 8;
			this.mnDisableWnd.Text = "禁止窗口";
			this.mnDisableWnd.Click += new System.EventHandler(this.mnDisableWnd_Click);
			// 
			// mnlCloseWnd
			// 
			this.mnlCloseWnd.Index = 9;
			this.mnlCloseWnd.Text = "关闭窗口！";
			this.mnlCloseWnd.Click += new System.EventHandler(this.mnlCloseWnd_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 10;
			this.menuItem3.Text = "-";
			// 
			// mnHighLightWnd
			// 
			this.mnHighLightWnd.Index = 11;
			this.mnHighLightWnd.Text = "突出显示";
			this.mnHighLightWnd.Click += new System.EventHandler(this.mnHighLightWnd_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 12;
			this.menuItem1.Text = "-";
			// 
			// mnClickBtn
			// 
			this.mnClickBtn.Index = 13;
			this.mnClickBtn.Text = "点击（按钮）";
			this.mnClickBtn.Click += new System.EventHandler(this.mnClickBtn_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 14;
			this.menuItem7.Text = "-";
			// 
			// mnRedrawWnd
			// 
			this.mnRedrawWnd.Index = 15;
			this.mnRedrawWnd.Text = "重绘窗口";
			this.mnRedrawWnd.Click += new System.EventHandler(this.mnRedrawWnd_Click);
			// 
			// mnSnapWnd
			// 
			this.mnSnapWnd.Index = 16;
			this.mnSnapWnd.Text = "窗口截图";
			this.mnSnapWnd.Click += new System.EventHandler(this.mnSnapWnd_Click);
			// 
			// mnGetText
			// 
			this.mnGetText.Index = 17;
			this.mnGetText.Text = "获取文本";
			this.mnGetText.Click += new System.EventHandler(this.mnGetText_Click);
			// 
			// mnSendCharA
			// 
			this.mnSendCharA.Index = 18;
			this.mnSendCharA.Text = "发送字符A";
			this.mnSendCharA.Click += new System.EventHandler(this.mnSendCharA_Click);
			// 
			// mnSendString
			// 
			this.mnSendString.Index = 19;
			this.mnSendString.Text = "发送字符串";
			this.mnSendString.Click += new System.EventHandler(this.mnSendString_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 20;
			this.menuItem5.Text = "-";
			// 
			// mnUpdateInfo
			// 
			this.mnUpdateInfo.Index = 21;
			this.mnUpdateInfo.Text = "刷新信息";
			this.mnUpdateInfo.Click += new System.EventHandler(this.mnUpdateInfo_Click);
			// 
			// btHighLightWnd
			// 
			this.btHighLightWnd.Location = new System.Drawing.Point(536, 88);
			this.btHighLightWnd.Name = "btHighLightWnd";
			this.btHighLightWnd.Size = new System.Drawing.Size(80, 23);
			this.btHighLightWnd.TabIndex = 11;
			this.btHighLightWnd.Text = "突出显示";
			this.btHighLightWnd.Click += new System.EventHandler(this.btHighLight_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioBtEnumAllProcess);
			this.groupBox1.Controls.Add(this.btFindWindow);
			this.groupBox1.Controls.Add(this.radioBtEnumOneProcess);
			this.groupBox1.Controls.Add(this.radioBtEnumAllWnd);
			this.groupBox1.Controls.Add(this.radioBtOnlyEnumExplorer);
			this.groupBox1.Controls.Add(this.btUpdate);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(632, 48);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			// 
			// radioBtEnumAllProcess
			// 
			this.radioBtEnumAllProcess.Location = new System.Drawing.Point(320, 16);
			this.radioBtEnumAllProcess.Name = "radioBtEnumAllProcess";
			this.radioBtEnumAllProcess.Size = new System.Drawing.Size(72, 24);
			this.radioBtEnumAllProcess.TabIndex = 12;
			this.radioBtEnumAllProcess.Text = "全部进程";
			// 
			// btFindWindow
			// 
			this.btFindWindow.Location = new System.Drawing.Point(552, 16);
			this.btFindWindow.Name = "btFindWindow";
			this.btFindWindow.Size = new System.Drawing.Size(72, 24);
			this.btFindWindow.TabIndex = 11;
			this.btFindWindow.Text = "查找窗口";
			this.btFindWindow.Click += new System.EventHandler(this.btFindWindow_Click);
			// 
			// radioBtEnumOneProcess
			// 
			this.radioBtEnumOneProcess.Location = new System.Drawing.Point(232, 16);
			this.radioBtEnumOneProcess.Name = "radioBtEnumOneProcess";
			this.radioBtEnumOneProcess.Size = new System.Drawing.Size(72, 24);
			this.radioBtEnumOneProcess.TabIndex = 10;
			this.radioBtEnumOneProcess.Text = "某个进程";
			// 
			// radioBtEnumAllWnd
			// 
			this.radioBtEnumAllWnd.Checked = true;
			this.radioBtEnumAllWnd.Location = new System.Drawing.Point(128, 16);
			this.radioBtEnumAllWnd.Name = "radioBtEnumAllWnd";
			this.radioBtEnumAllWnd.Size = new System.Drawing.Size(72, 24);
			this.radioBtEnumAllWnd.TabIndex = 8;
			this.radioBtEnumAllWnd.TabStop = true;
			this.radioBtEnumAllWnd.Text = "全部窗口";
			// 
			// radioBtOnlyEnumExplorer
			// 
			this.radioBtOnlyEnumExplorer.Location = new System.Drawing.Point(8, 17);
			this.radioBtOnlyEnumExplorer.Name = "radioBtOnlyEnumExplorer";
			this.radioBtOnlyEnumExplorer.TabIndex = 7;
			this.radioBtOnlyEnumExplorer.Text = "桌面有关窗口";
			// 
			// btDisableWnd
			// 
			this.btDisableWnd.Location = new System.Drawing.Point(456, 112);
			this.btDisableWnd.Name = "btDisableWnd";
			this.btDisableWnd.Size = new System.Drawing.Size(80, 23);
			this.btDisableWnd.TabIndex = 14;
			this.btDisableWnd.Text = "禁止窗口";
			this.btDisableWnd.Click += new System.EventHandler(this.btDisableWnd_Click);
			// 
			// btEnableWnd
			// 
			this.btEnableWnd.Location = new System.Drawing.Point(376, 112);
			this.btEnableWnd.Name = "btEnableWnd";
			this.btEnableWnd.Size = new System.Drawing.Size(80, 23);
			this.btEnableWnd.TabIndex = 13;
			this.btEnableWnd.Text = "使能窗口";
			this.btEnableWnd.Click += new System.EventHandler(this.btEnableWnd_Click);
			// 
			// cMenuProcess
			// 
			this.cMenuProcess.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.mnViewProcessDetail});
			// 
			// mnViewProcessDetail
			// 
			this.mnViewProcessDetail.Index = 0;
			this.mnViewProcessDetail.Text = "查看进程信息";
			this.mnViewProcessDetail.Click += new System.EventHandler(this.mnViewProcessDetail_Click);
			// 
			// cbAskForConfirm
			// 
			this.cbAskForConfirm.Checked = true;
			this.cbAskForConfirm.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbAskForConfirm.Location = new System.Drawing.Point(376, 64);
			this.cbAskForConfirm.Name = "cbAskForConfirm";
			this.cbAskForConfirm.Size = new System.Drawing.Size(136, 24);
			this.cbAskForConfirm.TabIndex = 15;
			this.cbAskForConfirm.Text = "重要操作时询问我";
			this.cbAskForConfirm.CheckedChanged += new System.EventHandler(this.cbAskForConfirm_CheckedChanged);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem4});
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 0;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnIEMan,
																					  this.mnWinMine,
																					  this.mnQQMsgtail,
																					  this.mnPEView,
																					  this.mnAbout});
			this.menuItem4.Text = "主菜单";
			// 
			// mnIEMan
			// 
			this.mnIEMan.Index = 0;
			this.mnIEMan.Text = "IE管理器";
			this.mnIEMan.Click += new System.EventHandler(this.mnIEMan_Click);
			// 
			// mnWinMine
			// 
			this.mnWinMine.Index = 1;
			this.mnWinMine.Text = "扫雷作弊器";
			this.mnWinMine.Click += new System.EventHandler(this.mnWinMine_Click);
			// 
			// mnQQMsgtail
			// 
			this.mnQQMsgtail.Index = 2;
			this.mnQQMsgtail.Text = "模拟QQ尾巴";
			this.mnQQMsgtail.Click += new System.EventHandler(this.mnQQMsgtail_Click);
			// 
			// mnPEView
			// 
			this.mnPEView.Index = 3;
			this.mnPEView.Text = "PE文件头查看器";
			this.mnPEView.Click += new System.EventHandler(this.mnPEView_Click);
			// 
			// mnAbout
			// 
			this.mnAbout.Index = 4;
			this.mnAbout.Text = "关于...";
			this.mnAbout.Click += new System.EventHandler(this.mnAbout_Click);
			// 
			// cMenuThread
			// 
			this.cMenuThread.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.mnReloadChild});
			// 
			// mnReloadChild
			// 
			this.mnReloadChild.Index = 0;
			this.mnReloadChild.Text = "重新加载子节点";
			this.mnReloadChild.Click += new System.EventHandler(this.mnReloadChild_Click);
			// 
			// cbAutoHighlightWnd
			// 
			this.cbAutoHighlightWnd.Location = new System.Drawing.Point(512, 64);
			this.cbAutoHighlightWnd.Name = "cbAutoHighlightWnd";
			this.cbAutoHighlightWnd.Size = new System.Drawing.Size(128, 24);
			this.cbAutoHighlightWnd.TabIndex = 18;
			this.cbAutoHighlightWnd.Text = "自动突出显示窗口";
			// 
			// pbIcon
			// 
			this.pbIcon.Location = new System.Drawing.Point(600, 152);
			this.pbIcon.Name = "pbIcon";
			this.pbIcon.Size = new System.Drawing.Size(34, 34);
			this.pbIcon.TabIndex = 19;
			this.pbIcon.TabStop = false;
			// 
			// MainFrm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(656, 445);
			this.Controls.Add(this.pbIcon);
			this.Controls.Add(this.cbAutoHighlightWnd);
			this.Controls.Add(this.cbAskForConfirm);
			this.Controls.Add(this.btDisableWnd);
			this.Controls.Add(this.btEnableWnd);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btHighLightWnd);
			this.Controls.Add(this.btMoveUp);
			this.Controls.Add(this.btMoveDown);
			this.Controls.Add(this.btMoveLeft);
			this.Controls.Add(this.btMoveRight);
			this.Controls.Add(this.tbDescription);
			this.Controls.Add(this.btHideWnd);
			this.Controls.Add(this.btShowWnd);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.tvWnd);
			this.Menu = this.mainMenu1;
			this.Name = "MainFrm";
			this.Text = "桌面窗口查看器";
			this.SizeChanged += new System.EventHandler(this.MainFrm_SizeChanged);
			this.Load += new System.EventHandler(this.MainFrm_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainFrm());
		}

		#region 对外公开的静态或实例方法—获取节点信息，高亮窗口

		//根据窗口句柄获取窗口信息
		public WndInfo GetWndInfo(int hwnd)
		{
			WndInfo info=new WndInfo();
			info.hWnd=hwnd;
			//获得窗口的标题
			GetWindowText(hwnd,this.m_StringBuilder,256);
			info.Caption=this.m_StringBuilder.ToString();
			
			//获得窗口的类名
			RealGetWindowClass(hwnd,this.m_StringBuilder,256);
			info.WndClassName=this.m_StringBuilder.ToString();

			//获取窗口矩形
			GetWindowRect(hwnd,ref this.m_Rect);
			info.X=this.m_Rect.Left;
			info.Y=this.m_Rect.Top;
			info.Width=this.m_Rect.Right-this.m_Rect.Left;
			info.Height=this.m_Rect.Bottom-this.m_Rect.Top;
			info.IsVisible=IsWindowVisible(hwnd);
			return info;
		}

		//更新节点的信息
		public void UpdateWndNode(TreeNode node)
		{
			if(node==null)
			{
				this.tbDescription.Clear();
				return;
			}
			WndInfo info=(WndInfo)node.Tag;
			//获得窗口的标题
			GetWindowText(info.hWnd,this.m_StringBuilder,256);
			info.Caption=this.m_StringBuilder.ToString();

			//获取窗口矩形
			GetWindowRect(info.hWnd,ref this.m_Rect);
			info.X=this.m_Rect.Left;
			info.Y=this.m_Rect.Top;
			info.Width=this.m_Rect.Right-this.m_Rect.Left;
			info.Height=this.m_Rect.Bottom-this.m_Rect.Top;
			info.IsVisible=IsWindowVisible(info.hWnd);
			node.SelectedImageIndex=(info.IsVisible)? 1:0;
			node.ImageIndex=(info.IsVisible)? 1:0;
			//更新tag
			node.Tag=info;
			//更新显示
			this.ShowNodeInfo((TreeNodeEx)node);
		}

		/// <summary>
		/// 根据Process返回一个进程信息结构
		/// </summary>
		/// <param name="pro"></param>
		/// <returns></returns>
		public static ProcessInfo GetProcessInfo(Process pro)
		{
			ProcessInfo info=new ProcessInfo();
            //pro.StartInfo.Arguments = "";
            //pro.StartInfo.WorkingDirectory = "";
            //pro.StartInfo.UseShellExecute = false;
            //pro.StartInfo.RedirectStandardInput = true;
            //pro.StartInfo.RedirectStandardOutput = true;
            //pro.StartInfo.RedirectStandardError = true;
            //pro.StartInfo.ErrorDialog = false;
            //pro.StartInfo.CreateNoWindow = true;

			info.ID=pro.Id;
			info.BasePriority=pro.BasePriority;
			info.IsResponding=pro.Responding;
			info.ProcessName=pro.ProcessName;
			info.MainWindowHandle=(int)pro.MainWindowHandle;
			info.MainWindowTitle=pro.MainWindowTitle;
            //info.StartTime=pro.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            //info.StartTime = pro.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
			info.VirtualMemorySize=pro.VirtualMemorySize;
			return info;
		}

		public static ThreadInfo GetThreadInfo(ProcessThread thread)
		{
			ThreadInfo info=new ThreadInfo();
			info.ID=thread.Id;
			info.BasePriority=thread.BasePriority;
			info.StartTime=thread.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
			return info;
		}

		//高亮显示某个窗口，在窗口外部发色绘制边框，绘制偶数次会擦除反色边框
		public void HighLightWindow(int hwnd)
		{
			GetWindowRect(hwnd,ref this.m_Rect);
			//获得窗口dc
			int width=this.m_Rect.Right-this.m_Rect.Left;
			int height=this.m_Rect.Bottom-this.m_Rect.Top;
			int hdc=GetWindowDC(hwnd);
			//创建黑色的，4象素宽度的画笔
			int hNewPen=CreatePen(PS_SOLID,4,0);
			int hOldPen=SelectObject(hdc,hNewPen);
			//设置为反色模式
			SetROP2(hdc,R2_NOT);
			POINT lpPoint=new POINT();

			//注意LineTo函数不会绘制最后一个点
			MoveToEx(hdc,0,2,ref lpPoint);
			LineTo(hdc,width-2,2);
			LineTo(hdc,width-2,height-2);
			LineTo(hdc,2,height-2);
			LineTo(hdc,2,2);

			//释放对象
			SelectObject(hdc,hOldPen);
			DeleteObject(hNewPen);
			//释放dc
			ReleaseDC(hwnd,hdc);
		}
		#endregion

		#region 内部辅助函数-加载数据 & 显示节点信息
		/// <summary>
		/// 在Treeview的根节点上添加一个进程节点，注意位置是根结点
		/// </summary>
		/// <param name="pro"></param>
		private void AddOneProcessNode(Process pro)
		{
			ProcessInfo info=GetProcessInfo(pro);
			TreeNodeEx node=new TreeNodeEx(NodeType.Process,info);
			this.tvWnd.Nodes.Add(node);
		}

		/// <summary>
		/// 在Treeview的根节点上添加一个窗口节点，注意位置是根结点
		/// </summary>
		/// <param name="pro"></param>
		private void AddOneWndNode(int hwnd)
		{
			WndInfo info=this.GetWndInfo(hwnd);
			TreeNodeEx node=new TreeNodeEx(NodeType.Window,info,(FindWindowEx(hwnd,0,null,null)>0));
			
			this.tvWnd.Nodes.Add(node);
		}

		//展开进程节点以后，添加所有线程节点
		private void AddThreadNodes(TreeNodeEx parentnode,Process pro)
		{
			foreach(ProcessThread thread in pro.Threads)
			{
				ThreadInfo info=GetThreadInfo(thread);
				TreeNodeEx node=new TreeNodeEx(NodeType.Thread,info,false);
				if (node.Text.IndexOf("指定发送")>=0)
				{
					MessageBox.Show(node.Text);
				}
                parentnode.Nodes.Add(node);
				//判断线程子节点下是否有窗口（确定是否需要加临时子节点！）
                EnumThreadWindows(info.ID,this.lpAddTempChild2ThreadNode,(int)node.Handle);
			}
		}

		/// <summary>
		/// 初始化TreeView，添加explorer创建的窗口，会导致已有节点全部清空，重新加载节点
		/// </summary>
		private void AddExplorerWnd2TreeView()
		{
			this.tbDescription.Clear();
			//清空树中的所有节点
			this.tvWnd.Nodes.Clear();
			//添加Progman
			int hwnd=FindWindow("Progman",null);
			if(hwnd==0)
			{
				DialogResult dr=MessageBox.Show(
					"未找到桌面窗口，可能是explorer.exe进程意外终止。\n是否现在启动explorer.exe进程？",
					MainFrm.MyAppName,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1);
				if(dr==DialogResult.Yes)
				{
					System.Diagnostics.Process.Start("explorer.exe");
					//休眠一段时间，等待桌面进程启动
					System.Threading.Thread.Sleep(500);
					hwnd=FindWindow("Progman",null);
				}
				else
					return;					
			}
			WndInfo info=this.GetWndInfo(hwnd);
			TreeNodeEx node=new TreeNodeEx(NodeType.Window,info);
			this.tvWnd.Nodes.Add(node);

			//添加shell tray
			hwnd=FindWindow("Shell_TrayWnd",null);
			info=this.GetWndInfo(hwnd);
			node=new TreeNodeEx(NodeType.Window,info);
			this.tvWnd.Nodes.Add(node);
		}

		/// <summary>
		/// 枚举桌面下的所有任何窗口
		/// </summary>
		private void AddDesktopWnd2TreeView()
		{
			this.tbDescription.Clear();
			//清空树中的所有节点
			this.tvWnd.Nodes.Clear();
			//开始查找桌面窗口，注意桌面窗口的类名是#32769
            int hwnd=0;
			while((hwnd=FindWindowEx(0,hwnd,null,null))>0)
			{
				WndInfo info=this.GetWndInfo(hwnd);
				TreeNodeEx node=new TreeNodeEx(NodeType.Window,info,(FindWindowEx(hwnd,0,null,null)>0));
				this.tvWnd.Nodes.Add(node);
			}
		}

		/// <summary>
		/// 添加所有进程的节点到Treeview
		/// </summary>
		private void AddAllProcess2TreeView()
		{
			this.tbDescription.Clear();
			//清空树中的所有节点
			this.tvWnd.Nodes.Clear();
			Process[] allpro=Process.GetProcesses();
			for(int i=0;i<allpro.Length;i++)
				this.AddOneProcessNode(allpro[i]);
		}

		/// <summary>
		/// 显示树节点的信息，在右侧只读textBox中
		/// </summary>
		/// <param name="node"></param>
		private void ShowNodeInfo(TreeNodeEx node)
		{
			if(node!=null)
			{
				switch(node.NODETYPE)
				{
					case NodeType.Window:
						WndInfo wndinfo=(WndInfo)node.Tag;
						this.tbDescription.Text=string.Format("节点类型:{0}\r\n句柄[10进制]: {1}\r\n窗口类: {2}\r\n文本: {3}\r\n坐标: ({4},{5})\r\n尺寸: {6}*{7}",
							node.NODETYPE,
							wndinfo.hWnd,
							wndinfo.WndClassName,
							wndinfo.Caption,
							wndinfo.X,
							wndinfo.Y,
							wndinfo.Width,
							wndinfo.Height);
						//获取窗口的大图标
						int hicon=SendMessage(wndinfo.hWnd,WM_GETICON,ICON_BIG,0);
						//显示图标
						this.pbIcon.Image=hicon>1? Icon.FromHandle((IntPtr)hicon).ToBitmap() : null;
						if(hicon>0)
						{
							//销毁图标
							MainFrm.DestroyIcon(hicon);
						}

						if(this.cbAutoHighlightWnd.Checked)
						{
							//画两次（也就是完整闪烁一次）
							this.m_TimerLife=2;
							this.timer1.Start();
						}
						break;

					case NodeType.Process:
						ProcessInfo proinfo=(ProcessInfo)node.Tag;
						this.tbDescription.Text=string.Format("节点类型:{0}\r\nPID[10进制]:{1}\r\n进程名:{2}\r\n优先级:{3}\r\n主窗口句柄:{4}\r\n主窗口标题:{5}\r\n启动时间:{6}\r\n虚拟内存:{7}\r\n是否响应:{8}",
							node.NODETYPE,
							proinfo.ID,
							proinfo.ProcessName,
							proinfo.BasePriority,
							proinfo.MainWindowHandle,
							proinfo.MainWindowTitle,
							proinfo.StartTime,
							proinfo.VirtualMemorySize,
							proinfo.IsResponding);
						//清除图标显示
						this.pbIcon.Image=null;
						break;

					case NodeType.Thread:
						ThreadInfo thdinfo=(ThreadInfo)node.Tag;
						this.tbDescription.Text=string.Format("节点类型:{0}\r\nTID[10进制]:{1}\r\n优先级:{2}\r\n启动时间:{3}",
							node.NODETYPE,
							thdinfo.ID,
							thdinfo.BasePriority,
							thdinfo.StartTime);
						//清除图标显示
						this.pbIcon.Image=null;
						break;
				}
			}
			else
				this.tbDescription.Clear();
		}
		#endregion

		#region CallBack Functions (EnumWindows and Timer)
		/// <summary>
		/// 枚举后代窗口的回调函数，注意应该使用GetWindow检查每个子窗口的父窗口，
		/// 只添加直接的子窗口到树节点上，忽略孙辈及以后的后代
		/// </summary>
		/// <param name="hwnd"></param>
		/// <param name="lParam">父窗口的句柄，来自于调用EnumChildWindows中的输入参数</param>
		/// <returns></returns>
		private bool EnumChildWndProc(int hwnd,int lParam)
		{
			int parent=GetParent(hwnd);
			if(parent==lParam)
			{
				//添加该节点
				WndInfo info=this.GetWndInfo(hwnd);
				TreeNodeEx node=new TreeNodeEx(NodeType.Window,info,(FindWindowEx(hwnd,0,null,null)>0));
				
				this.tvWnd.SelectedNode.Nodes.Add(node);
			}
			//返回true表示要求系统继续枚举窗口，返回false停止枚举
			return true;
		}

		/// <summary>
		/// 枚举线程窗口的回调函数
		/// </summary>
		/// <param name="hwnd"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private bool EnumThreadWndProc(int hwnd,int lParam)
		{
			//添加该节点到treeview的当前选中节点
			WndInfo info=this.GetWndInfo(hwnd);
			TreeNodeEx node=new TreeNodeEx(NodeType.Window,info,(FindWindowEx(hwnd,0,null,null)>0));
			
			this.tvWnd.SelectedNode.Nodes.Add(node);
			return true;
		}

		/// <summary>
		/// 回调函数，用于为线程节点添加临时子节点（如果没有窗口，则不会添加子节点）
		/// </summary>
		/// <param name="hwnd">系统传递来的被枚举的子窗口句柄</param>
		/// <param name="NodeHandle">lParam: 线程节点的Handle</param>
		/// <returns></returns>
		private bool AddTempChild2ThreadNode(int hwnd,int NodeHandle)
		{
			TreeNode node=TreeNode.FromHandle(this.tvWnd,(IntPtr)NodeHandle);
			
			node.Nodes.Add("TempChild");
			//仅枚举一次，因为只是为了添加一个临时子节点而已
			return false;
		}

		/// <summary>
		/// 枚举桌面所有顶级窗口的回调函数，系统不会枚举子窗口
		/// </summary>
		/// <param name="hwnd">从操作系统接收到的子窗口句柄</param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private bool EnumDesktopWndFunc(int hwnd,int lParam)
		{
			//添加该节点到treeview的当前选中节点
			WndInfo info=this.GetWndInfo(hwnd);
			
			TreeNodeEx node=new TreeNodeEx(NodeType.Window,info,(FindWindowEx(hwnd,0,null,null)>0));

			this.tvWnd.Nodes.Add(node);
			return true;
		}

		/// <summary>
		/// 定时器回调函数，为了突出显示窗口！
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(--this.m_TimerLife<=0)
			{
				this.timer1.Stop();
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node==null || node.NODETYPE!=NodeType.Window)
				return;
			WndInfo info=(WndInfo)node.Tag;
			//高亮窗口（绘制反色边框）
			this.HighLightWindow(info.hWnd);			
		}

		#endregion

		#region TreeView EventHandler
		/// <summary>
		/// 选中树的节点
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tvWnd_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			this.ShowNodeInfo((TreeNodeEx)e.Node);
		}

		/// <summary>
		/// TreeView按下了鼠标键，如果是右键，则弹出上下文菜单！
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tvWnd_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button==MouseButtons.Right)
			{
				Point pt=new Point(e.X,e.Y);
				TreeNodeEx node=(TreeNodeEx)this.tvWnd.GetNodeAt(pt);
				if(node!=null)
				{
					this.tvWnd.SelectedNode=node;
					switch(node.NODETYPE)
					{
						case NodeType.Window:
							this.cMenuWnd.Show(this.tvWnd,pt);
							break;
						case NodeType.Process:
							this.cMenuProcess.Show(this.tvWnd,pt);
							break;
						case NodeType.Thread:
							this.cMenuThread.Show(this.tvWnd,pt);
							break;
					}
				}
				else
					this.ShowNodeInfo(null);
			}
		}

		/// <summary>
		/// 树节点展开之前的事件，此时将添加所有子节点
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tvWnd_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			TreeNodeEx node=(TreeNodeEx)e.Node;
			//如果已经添加过子节点，则返回
			if(node.b_ChildAdded)
				return;
			if(this.tvWnd.SelectedNode!=node)
				this.tvWnd.SelectedNode=node;
			//删除临时子节点
			node.Nodes.Clear();
			switch(node.NODETYPE)
			{
				case NodeType.Window:
					WndInfo wndinfo=(WndInfo)node.Tag;
					//枚举子窗口，在回调函数中添加子节点
					EnumChildWindows(wndinfo.hWnd,this.lpEnumChildFunc,wndinfo.hWnd);
					break;

				case NodeType.Process:
					ProcessInfo proinfo=(ProcessInfo)node.Tag;
					Process pro=Process.GetProcessById(proinfo.ID);
					//添加所有子线程节点
					this.AddThreadNodes(node,pro);
					break;

				case NodeType.Thread:
					ThreadInfo thdinfo=(ThreadInfo)node.Tag;
					EnumThreadWindows(thdinfo.ID,this.lpEnumThreadFunc,thdinfo.ID);
					break;
			}
			//设置添加过节点的标志
			node.b_ChildAdded=true;
		}

		#endregion

		//-----------------------------------------------------------------
		//			一组和上下文菜单功能一样的按钮处理
		//-----------------------------------------------------------------
		#region 按钮事件的处理
		// “更新数据”按钮的处理
		private void btUpdate_Click(object sender, System.EventArgs e)
		{
			//枚举Progman和ShellTrat
			if(this.radioBtOnlyEnumExplorer.Checked)
			{
				this.AddExplorerWnd2TreeView();
				return;
			}

			//枚举所有窗口
			if(this.radioBtEnumAllWnd.Checked)
			{
				//this.AddDesktopWnd2TreeView();	//用FindWindowEx循环完成枚举桌面窗口
				//清空所有节点
				this.tvWnd.Nodes.Clear();
				//清空右侧的节点信息
				this.tbDescription.Clear();
				EnumWindows(this.lpEnumDesktopFunc,0);	//用WINAPI的枚举窗口函数
				return;
			}

			//枚举某一个进程
			if(this.radioBtEnumOneProcess.Checked)
			{
				//加载进程列表对话框时需要较长时间
				Cursor.Current=Cursors.WaitCursor;
				ProcessSelDlg dlg=new ProcessSelDlg();
				Cursor.Current=Cursors.Default;
				if(dlg.ShowDialog()==DialogResult.OK)
				{
					this.tbDescription.Clear();
					//清空树中的所有节点
					this.tvWnd.Nodes.Clear();
					this.AddOneProcessNode(dlg.SelectedProcess);					
				}
				return;
			}

			//枚举所有进程
			if(this.radioBtEnumAllProcess.Checked)
			{
				this.AddAllProcess2TreeView();
				return;
			}
		}

		/// <summary>
		/// “查找窗口”按钮的处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btFindWindow_Click(object sender, System.EventArgs e)
		{
			FindWindowDlg dlg=new FindWindowDlg();
			//设置主窗口指针
			dlg.MAINFRAME=this;
			if(dlg.ShowDialog()==DialogResult.OK)
			{
				int hwnd=0;
				string wndclass=dlg.WNDCLASS;
				string wndname=dlg.WNDNAME;	//窗口名称
				string lowername= (wndname==null)? string.Empty : wndname.ToLower();	//小写的窗口名称
				switch(dlg.MAP_FLAG)
				{
					case (FindWindowDlg.MF_FULLWORD | FindWindowDlg.MF_UPPERLOWER):
						//标题完全精确匹配：全字匹配，大小写匹配
						hwnd=FindWindow(wndclass,wndname);
						break;
					case (FindWindowDlg.MF_FULLWORD):
						//全字匹配
						while((hwnd=FindWindowEx(0,hwnd,wndclass,null))>0)
						{
							GetWindowText(hwnd,this.m_StringBuilder,256);
							if(this.m_StringBuilder.ToString().ToLower()==lowername)
								break;
						}
						break;
					case (FindWindowDlg.MF_UPPERLOWER):
						//大小写严格匹配
						while((hwnd=FindWindowEx(0,hwnd,wndclass,null))>0)
						{
							GetWindowText(hwnd,this.m_StringBuilder,256);
							if(this.m_StringBuilder.ToString().IndexOf(wndname)>=0)
								break;
						}
						break;
					case (FindWindowDlg.MF_NONE):
						//不全字匹配，也不大小写匹配
						while((hwnd=FindWindowEx(0,hwnd,wndclass,null))>0)
						{
							GetWindowText(hwnd,this.m_StringBuilder,256);
							if(this.m_StringBuilder.ToString().ToLower().IndexOf(lowername)>=0)
								break;
						}
						break;

				}
				if(hwnd>0)
				{
					this.tvWnd.Nodes.Clear();
					this.tbDescription.Clear();
					this.AddOneWndNode(hwnd);
				}
				else
				{
					MessageBox.Show("未找到窗口!");
					return;
				}
			}
		}

		/// <summary>
		/// 是否在重要操作时提醒我
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbAskForConfirm_CheckedChanged(object sender, System.EventArgs e)
		{
			this.b_AskForConfirm=this.cbAskForConfirm.Checked;
		}

		private void btShow_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			bool pre=ShowWindow(info.hWnd,SW_SHOW);
			this.statusBar1.Text="此前可见="+pre.ToString();
		}

		private void btHide_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			bool pre=ShowWindow(info.hWnd,SW_HIDE);
			this.statusBar1.Text="此前可见="+pre.ToString();
		}
		private void btEnableWnd_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			bool pre=EnableWindow(info.hWnd,true);
			this.statusBar1.Text="此前使能="+pre.ToString();
		}

		private void btDisableWnd_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			if(this.b_AskForConfirm && MessageBox.Show(
				"禁止窗口以后需要您自己恢复窗口的使能状态!确实要禁止该窗口吗？",
				MainFrm.MyAppName,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2)!=DialogResult.OK)
				return;

			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			bool pre=EnableWindow(info.hWnd,false);
			this.statusBar1.Text="此前使能="+pre.ToString();
		}


		private void btHighLight_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			//大约闪烁持续1秒钟
			this.m_TimerLife=6;
			this.timer1.Start();		
		}

		private void btMoveRight_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			RECT rc=new RECT(0,0,0,0);
			GetWindowRect(info.hWnd,ref rc);
			MoveWindow(info.hWnd,
				rc.Left+50,
				rc.Top,
				(rc.Right-rc.Left),
				(rc.Bottom-rc.Top),
				true);
			//更新显示
			this.UpdateWndNode(node);
		}

		private void btMoveLeft_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			RECT rc=new RECT(0,0,0,0);
			GetWindowRect(info.hWnd,ref rc);
			MoveWindow(info.hWnd,
				rc.Left-50,
				rc.Top,
				(rc.Right-rc.Left),
				(rc.Bottom-rc.Top),
				true);
			//更新显示
			this.UpdateWndNode(node);
		}

		private void btMoveDown_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			RECT rc=new RECT(0,0,0,0);
			GetWindowRect(info.hWnd,ref rc);
			MoveWindow(info.hWnd,
				rc.Left,
				rc.Top+50,
				rc.Right-rc.Left,
				rc.Bottom-rc.Top,
				true);
			//更新显示
			this.UpdateWndNode(node);
		}

		private void btMoveUp_Click(object sender, System.EventArgs e)
		{
			if(this.tvWnd.SelectedNode==null)
			{
				MessageBox.Show("请先选中一个节点！");
				return;
			}
			TreeNodeEx node=(TreeNodeEx)this.tvWnd.SelectedNode;
			if(node.NODETYPE!=NodeType.Window)
			{
				MessageBox.Show("选中的不是窗口节点！");
				return;
			}
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			RECT rc=new RECT(0,0,0,0);
			GetWindowRect(info.hWnd,ref rc);
			MoveWindow(info.hWnd,
				rc.Left,
				rc.Top-50,
				rc.Right-rc.Left,
				rc.Bottom-rc.Top,
				true);
			//更新显示
			this.UpdateWndNode(node);
		}
		#endregion
		//-----------------------------------------------------------------
		//           Context Menu Handler
		//-----------------------------------------------------------------
		#region 上下文菜单的处理
		//弹出窗口上下文菜单前，准备个别菜单的使能!
		private void cMenuWnd_Popup(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
            this.mnClickBtn.Enabled=(info.WndClassName=="Button");		
		}
		private void mnShowWnd_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			bool pre=ShowWindow(info.hWnd,SW_SHOWNOACTIVATE);
			//更新窗口节点得信息和显示
			this.UpdateWndNode(this.tvWnd.SelectedNode);
			this.statusBar1.Text="此前可见="+pre.ToString();
		}

		private void mnHideWnd_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			bool pre=ShowWindow(info.hWnd,SW_HIDE);
			//更新窗口节点得信息和显示
			this.UpdateWndNode(this.tvWnd.SelectedNode);
			this.statusBar1.Text="此前可见="+pre.ToString();
		}

		private void mnMinimizeWnd_Click(object sender, System.EventArgs e)
		{
			if(this.b_AskForConfirm && MessageBox.Show(
				"最小化窗口可能引发意外结果!确实要最小化该窗口吗？",
				MainFrm.MyAppName,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2)!=DialogResult.OK)
				return;
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			//sw_minimize不会激活窗口，sw_showminimized会激活窗口
			bool pre=ShowWindow(info.hWnd,SW_SHOWMINNOACTIVE);
			//更新窗口节点得信息和显示
			this.UpdateWndNode(this.tvWnd.SelectedNode);
			this.statusBar1.Text="此前可见="+pre.ToString();
		}
		private void mnMaximizeWnd_Click(object sender, System.EventArgs e)
		{
			if(this.b_AskForConfirm && MessageBox.Show(
				"最大化窗口可能引发意外结果!确实要最大化该窗口吗？",
				MainFrm.MyAppName,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2)!=DialogResult.OK)
				return;
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			//因为可能会激活其他窗口，所以需要重新设置焦点
			int currentFocusWnd=GetFocus();
			bool pre=ShowWindow(info.hWnd,SW_MAXIMIZE);
			this.statusBar1.Text="此前可见="+pre.ToString();
			//更新窗口节点得信息和显示
            this.UpdateWndNode(this.tvWnd.SelectedNode);
			this.BringToFront();
			SetFocus(currentFocusWnd);
		}

		private void mnRestoreWnd_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			bool pre=ShowWindow(info.hWnd,SW_RESTORE);
			//更新窗口节点得信息和显示
			this.UpdateWndNode(this.tvWnd.SelectedNode);
			this.statusBar1.Text="此前可见="+pre.ToString();
		}
		private void mnShowDefaultWnd_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			bool pre=ShowWindow(info.hWnd,SW_SHOWDEFAULT);
			//更新窗口节点得信息和显示
			this.UpdateWndNode(this.tvWnd.SelectedNode);
			this.statusBar1.Text="此前可见="+pre.ToString();
		}

		private void mnEnableWnd_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
            bool pre=EnableWindow(info.hWnd,true);
			this.statusBar1.Text="此前使能="+pre.ToString();
		}

		//禁止窗口
		private void mnDisableWnd_Click(object sender, System.EventArgs e)
		{
			if(this.b_AskForConfirm && MessageBox.Show(
				"禁止窗口以后需要您自己恢复窗口的使能状态!确实要禁止该窗口吗？",
				MainFrm.MyAppName,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2)!=DialogResult.OK)
				return;			
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			//返回值是此前的状态
			bool pre=EnableWindow(info.hWnd,false);
			this.statusBar1.Text="此前使能="+pre.ToString();
		}

		//关闭窗口
		private void mnlCloseWnd_Click(object sender, System.EventArgs e)
		{
			if(this.b_AskForConfirm && MessageBox.Show(
				"关闭窗口应用程序将无法提示你进行一些保存工作。关闭窗口后将删除这个窗口节点。是否确认关闭窗口？",
				MainFrm.MyAppName,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2)!=DialogResult.OK)
				return;	
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
            int result=SendMessage(info.hWnd,WM_CLOSE,0,0);
			//如果窗口处理了WM_CLOSE应返回0
			if(result!=0)
				return;
			//如果有父节点，那么检查父进程是否还存在
			if(this.tvWnd.SelectedNode.Parent!=null)
			{
				//获取爷爷节点，并判断爷爷是不是进程节点，如果是再判断是否进程已结束
				TreeNodeEx pronode=(TreeNodeEx)this.tvWnd.SelectedNode.Parent.Parent;
				if(pronode!=null && pronode.NODETYPE==NodeType.Process)
				{
					ProcessInfo proinfo=(ProcessInfo)pronode.Tag;
					//等待0.2秒
					Cursor.Current=Cursors.WaitCursor;
					Thread.Sleep(200);
					try
					{
						Process pro=Process.GetProcessById(proinfo.ID);
					}
					catch(Exception e2)
					{
						//父进程已经结束，说明关闭的是进程主窗口
						//则删除进程节点
						pronode.Remove();
					}
					Cursor.Current=Cursors.Default;
				}
			}
			//关闭窗口操作执行后，我将删除该节点
			this.tvWnd.SelectedNode.Remove();
			//清空显示信息
			this.tbDescription.Clear();
		}

		//重绘窗口
		private void mnRedrawWnd_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			if(IsWindowVisible(info.hWnd))
			{
				//wParam, lParam not used in WM_PAINT
				ShowWindow(info.hWnd,SW_HIDE);
				ShowWindow(info.hWnd,SW_SHOW);
			}
		}

		//突出显示
		private void mnHighLightWnd_Click(object sender, System.EventArgs e)
		{
			//大约闪烁持续1秒钟，一共画6次
			this.m_TimerLife=6;
			this.timer1.Start();
		}

		//点击（按钮）
		private void mnClickBtn_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			if(info.WndClassName!="Button")
				return;
			SendMessage(info.hWnd,BM_CLICK,0,0);            		
		}

		//截图
		private void mnSnapWnd_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			//获得窗口DC
			int hdcWnd=GetDC(info.hWnd);
			if(hdcWnd==0)
			{
				MessageBox.Show("获取窗口DC失败，可能窗口已经被关闭!");
				return;
			}
			//创建内存位图
			int hBitmap=CreateCompatibleBitmap(hdcWnd,info.Width,info.Height);
			//创建内存dc
			int hdcCompatible=CreateCompatibleDC(hdcWnd);
			//将位图选入内存dc
			SelectObject(hdcCompatible,hBitmap);

			//应用程序窗口隐藏（注意因为隐藏了自己，所以是不能截取应用程序自己的窗口的）
			ShowWindow((int)this.Handle,SW_HIDE);
//			//以非活动状态显示要截图的窗口！（不会夺走ActiveWindow）
//			ShowWindow(info.hWnd,SW_SHOWNOACTIVATE);
			//强制并显示该窗口（shownormal方式会恢复最大化或最小化的窗口为原大小）
			//ShowWindow(info.hWnd,SW_SHOW);
			SetForegroundWindow(info.hWnd);
			//休眠一会等待重绘完成
			Thread.Sleep(500);
			
			//传送位图
			BitBlt(hdcCompatible,
				0,0,info.Width,info.Height,
				hdcWnd,
				0,0,
				SRCCOPY);
			//重新显示应用程序窗口
			ShowWindow((int)this.Handle,SW_SHOW);
			Bitmap bm=Bitmap.FromHbitmap((IntPtr)hBitmap);
			//释放对象
			DeleteObject(hBitmap);
			ReleaseDC(info.hWnd,hdcWnd);
			
			//创建图片窗口
			if(this.m_BitmapBoxDlg==null)
			{
				this.m_BitmapBoxDlg=new BitmapBoxDlg();
			}
            //设置图片进去
			this.m_BitmapBoxDlg.BITMAP=bm;
			//设置标题
			this.m_BitmapBoxDlg.Text=string.Format("\"{0}\" [{1}*{2}] - 截图",
				info.Caption,bm.Width,bm.Height);
			//显示应用程序窗口
			this.Show();
			//显示图片窗口
			//设置最前端显示
			this.m_BitmapBoxDlg.TopMost=true;
			this.m_BitmapBoxDlg.Show();
		}

		//获取窗口的文本
		private void mnGetText_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			// wParam=buffer size, lParam=buffer pointer
            SendMessage(info.hWnd,WM_GETTEXT,256,this.m_StringBuilder);
            this.tbDescription.Text=string.Format("【窗口文本】:\r\n{1}",
				info.Caption,this.m_StringBuilder.ToString());
		}

		//发送字符A
		private void mnSendCharA_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			//发送字符A，wparam是unicode键码，lparam低16位是重复击键次数，高位信息基本无用
			//因为发往消息队列，所以不必使窗口获取键盘焦点也可以使窗口接收并处理该消息
			//'A'的ASCI码＝0x41，10进制为65，，最后一个参数表示击键一次
			SendMessage(info.hWnd,WM_CHAR,0x41,1);
		}

		//发送字符串
		private void mnSendString_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
			SendStringDlg dlg=new SendStringDlg();
			if(dlg.ShowDialog()==DialogResult.OK)
			{
				//获取字符串
				string text=dlg.UserInputString;
				//存放一个ANSI字符的字节数组，长度为1或者2
				byte[] bytes;
				int charcode;
				//模拟键盘输入尾巴内容 (WM_CHAR）
				for(int i=0;i<text.Length;i++)
				{
					bytes=Encoding.Default.GetBytes(text[i].ToString());
					if(bytes.Length>1)
						charcode=bytes[1]*256 + bytes[0];		//两个字节表示的字符
					else
						charcode=bytes[0];						//一个字节表示的字符
					SendMessage(info.hWnd,WM_CHAR,charcode,1);
				}
			}
		}

		/// <summary>
		/// 刷新窗口节点的信息
		/// </summary>
		private void mnUpdateInfo_Click(object sender, System.EventArgs e)
		{
			WndInfo info=(WndInfo)this.tvWnd.SelectedNode.Tag;
            this.tvWnd.SelectedNode.Tag=this.GetWndInfo(info.hWnd);
            this.ShowNodeInfo((TreeNodeEx)this.tvWnd.SelectedNode);		
		}

		//---------------------------------
		//	Process ContextMenu Handle
		//---------------------------------
		//查看进程详细信息
		private void mnViewProcessDetail_Click(object sender, System.EventArgs e)
		{
			ProcessInfo info=(ProcessInfo)this.tvWnd.SelectedNode.Tag;
			//创建进程详细信息对话框
			if(this.m_ProcessDetailDlg==null)
			{
				this.m_ProcessDetailDlg=new ProcessDetailDlg(info.ID);
			}
			else
				this.m_ProcessDetailDlg.PROCESSID=info.ID;

			//显示进程详细信息
			if(this.m_ProcessDetailDlg.PROCESSALIVE)
				this.m_ProcessDetailDlg.Show();
			else
			{
				MessageBox.Show("该进程可能已经退出！将马上删除该进程节点！",
					"窗口查看器",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);
				//删除该节点
				this.tvWnd.SelectedNode.Remove();
			}
		}

		//---------------------------------
		//	Thread ContextMenu Handle
		//---------------------------------
		//重新加载子节点
		private void mnReloadChild_Click(object sender, System.EventArgs e)
		{
            ThreadInfo info=(ThreadInfo)this.tvWnd.SelectedNode.Tag;
			//删除当前所有子节点
			this.tvWnd.SelectedNode.Nodes.Clear();
			EnumThreadWindows(info.ID,this.lpEnumThreadFunc,info.ID);
		}
		#endregion

		#region MainMenu Handler
		
		//IE管理器
		private void mnIEMan_Click(object sender, System.EventArgs e)
		{
            IEManDlg dlg=new IEManDlg();
			dlg.Show();
		}

		//扫雷
		private void mnWinMine_Click(object sender, System.EventArgs e)
		{
            WinMineDlg dlg=new WinMineDlg();
			dlg.Show();
		}

		//模拟QQ消息尾巴
		private void mnQQMsgtail_Click(object sender, System.EventArgs e)
		{
			QQMsgtailDlg dlg=new QQMsgtailDlg();
			//注意只是show，而不是showdialog
			dlg.Show();
		}

		//PE文件头查看器
		private void mnPEView_Click(object sender, System.EventArgs e)
		{
			PEViewDlg dlg=new PEViewDlg();
			dlg.Show();		
		}

		//关于...
		private void mnAbout_Click(object sender, System.EventArgs e)
		{
			AboutDlg dlg=new AboutDlg();
			dlg.ShowDialog();
		}
		#endregion	

		private void MainFrm_Load(object sender, System.EventArgs e)
		{
		
		}

	}
}
