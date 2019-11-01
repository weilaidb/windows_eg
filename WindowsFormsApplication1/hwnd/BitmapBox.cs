using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime .InteropServices;	// for p\invoke

namespace DesktopWndView
{
	//滚动信息
	public struct SCROLLINFO
	{
		public uint cbSize;
		public uint fMask;
		public int nMin;	//最小位置
		public int nMax;	//最大位置
		public uint nPage;	//page size
		public int nPos;	//当前位置（拖动滑块时这个值不会被更新
		public int nTrackPos;	//拖动中的滑块位置（程序无法设置它）
	}

	//鼠标滚轮焦点 ( MouseWheel's Focus )
	public enum MWF : int
	{
		MWF_VSCROLL=0,
		MWF_HSCROLL=1
	}
	/// <summary>
	/// BitmapBox 的摘要说明:
	/// 从Panel继承，用来装载一个bitmap
	/// </summary>
	public class BitmapBox : System.Windows.Forms.Panel
	{
		#region 自定义变量
		//为了出现滚动条的一个静态文本控件，通过它来改变bounds属性来更新滚动条
		private Label m_label=new Label();
		//核心数据，控件持有的位图
		private Bitmap m_Bitmap;
		//用于获取scroll信息的结构
		private SCROLLINFO m_ScrollInfo;
		//水平滚动条信息
		private int m_HMin;
		private int m_HMax;
		private int m_HPos;

		//垂直滚动条信息
		private int m_VMin;
		private int m_VMax;
		private int m_VPos;
		
		//设置一个标志，指示当前滚轮焦点是垂直滚动条还是水平滚动条：
		private MWF m_WheelFocus=MWF.MWF_VSCROLL;
		//每滚动一次鼠标滚轮，代表的象素距离！
		private int m_PixelsPerWheel=20;
		#endregion

		#region WINAPI const defines
		//和滚动条有关的消息 (BitmapBox控件用到)
		public const int WM_HSCROLL  =0x0114;
		public const int WM_VSCROLL  =0x0115;
		/*
		 * Scroll Bar Commands (16位的short，因为是wParam的截半)
		 */
		public const short SB_LINEUP       =  0;
		public const short SB_LINELEFT     =  0;
		public const short SB_LINEDOWN     =  1;
		public const short SB_LINERIGHT    =  1;
		public const short SB_PAGEUP       =  2;	//向上翻页
		public const short SB_PAGELEFT     =  2;	//向左翻页
		public const short SB_PAGEDOWN     =  3;
		public const short SB_PAGERIGHT    =  3;
		public const short SB_THUMBPOSITION=  4;	//用户鼠标释放了滑块
		public const short SB_THUMBTRACK   =  5;	//用户正在拖动滑块
		public const short SB_TOP          =  6;
		public const short SB_LEFT         =  6;
		public const short SB_BOTTOM       =  7;
		public const short SB_RIGHT        =  7;
		public const short SB_ENDSCROLL    =  8;
		/*
		 * Scroll Bar Constants
		*/
		public const int SB_HORZ =           0;
		public const int SB_VERT =           1;
		public const int SB_CTL  =           2;
		public const int SB_BOTH =           3;
		/*
		 * fMask的取值（获取或设置哪些信息的掩码）
		 */
		public const uint SIF_RANGE           =0x0001;
		public const uint SIF_PAGE            =0x0002;
		public const uint SIF_POS             =0x0004;
		public const uint SIF_DISABLENOSCROLL =0x0008;
		public const uint SIF_TRACKPOS        =0x0010;
		public const uint SIF_ALL             =(SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

//		/*
//		 * 鼠标消息ID
//		 */
//		public const int WM_MOUSEFIRST      = 0x0200;
//		public const int WM_MOUSEMOVE       = 0x0200;
//		public const int WM_LBUTTONDOWN     = 0x0201;
//		public const int WM_LBUTTONUP       = 0x0202;
//		public const int WM_LBUTTONDBLCLK   = 0x0203;
//		public const int WM_RBUTTONDOWN     = 0x0204;
//		public const int WM_RBUTTONUP       = 0x0205;
//		public const int WM_RBUTTONDBLCLK   = 0x0206;
//		public const int WM_MBUTTONDOWN     = 0x0207;
//		public const int WM_MBUTTONUP       = 0x0208;
//		public const int WM_MBUTTONDBLCLK   = 0x0209;
//		public const int WM_MOUSEWHEEL      = 0x020A;	//滚轮消息
//		public const int WM_XBUTTONDOWN     = 0x020B;
//		public const int WM_XBUTTONUP       = 0x020C;
//		public const int WM_XBUTTONDBLCLK   = 0x020D;
		#endregion

		#region P/Invokd USER32
		//获取滚动信息
		[DllImport("user32")]
		public static extern bool GetScrollInfo(
			IntPtr hwnd,			//scroll ctl句柄，或者拥有滚动条的窗口句柄
			int fnBar,				//Scroll类型（控件，水平还是垂直）
			ref SCROLLINFO lpsi
			);
		[DllImport("user32")]
		public static extern bool SetScrollInfo(
			IntPtr hwnd,			//scroll ctl句柄，或者拥有滚动条的窗口句柄
			int fnBar,				//Scroll类型（控件，水平还是垂直）
			ref SCROLLINFO lpsi,
			bool fRedraw			//是否重绘以反映变化
			);

		//获取滚动位置
		[DllImport("user32")]
		public static extern int GetScrollPos(
			IntPtr hwnd,
			int nBar
			);

		//设置位置，返回前一个位置
		[DllImport("user32")]
		public static extern int SetScrollPos(
			IntPtr hwnd,
			int nBar,
			int nPos,
			bool bRedraw	//是否重绘scrollbar以反映变化
			);

		[DllImport("user32")]
		public static extern bool GetScrollRange(
			IntPtr hwnd,
			int nBar,
			ref int lpMinPos,
			ref int lpMaxPos
			);

		[DllImport("user32")]
		public static extern int SetScrollRange(
			IntPtr hwnd,
			int nBar,
			int nMinPos,
			int nMaxPos,
			bool bRedraw	//是否重绘scrollbar以反映变化
			);
		#endregion
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// 无参构造函数
		/// </summary>
		public BitmapBox()
		{
			InitializeComponent();
			//当内部控件位置超出显示区域时自动出现滚动条
			this.AutoScroll=true;
			//3D边缘
			this.BorderStyle=BorderStyle.Fixed3D;
			//添加label
			this.m_label.Bounds=new Rectangle(0,0,1,1);
			this.m_label.Text="Bitmap Box v1.01";
			this.m_label.TextAlign=ContentAlignment.TopLeft;
			this.Controls.Add(this.m_label);
			//scroll info
			this.m_ScrollInfo=new SCROLLINFO();
			this.m_ScrollInfo.cbSize=(uint)Marshal.SizeOf(this.m_ScrollInfo);
			//this.MouseWheel+=new MouseEventHandler(BitmapBox_MouseWheel);
			this.SetStyle(ControlStyles.DoubleBuffer
				|ControlStyles.UserPaint
				|ControlStyles.AllPaintingInWmPaint,
				true);
			this.UpdateStyles();
		}

		public BitmapBox(Bitmap bm)
		{
			InitializeComponent();
			//当内部控件位置超出显示区域时自动出现滚动条
			this.AutoScroll=true;
			//3D边缘
			this.BorderStyle=BorderStyle.Fixed3D;
			//添加label
			this.m_label.Bounds=new Rectangle(0,0,1,1);
			this.m_label.TextAlign=ContentAlignment.TopLeft;
			this.m_label.Visible=false;
			this.m_label.Parent=this;
			//设置bitmap
			this.m_Bitmap=bm;
			this.m_ScrollInfo=new SCROLLINFO();
			this.m_ScrollInfo.cbSize=(uint)Marshal.SizeOf(this.m_ScrollInfo);
			//this.MouseWheel+=new MouseEventHandler(BitmapBox_MouseWheel);

			this.SetStyle(ControlStyles.DoubleBuffer
				|ControlStyles.UserPaint
				|ControlStyles.AllPaintingInWmPaint,
				true);
			this.UpdateStyles();
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

		#region 组件设计器生成的代码
		/// <summary> 
		/// 设计器支持所需的方法 - 不要使用代码编辑器 
		/// 修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		//override onpaint
		protected override void OnPaint(PaintEventArgs e)
		{
			//erase background
			e.Graphics.FillRectangle(Brushes.DimGray,
				0,0,this.Width,this.Height);

			e.Graphics.DrawImage(this.m_Bitmap,
				this.AutoScrollPosition.X,
				this.AutoScrollPosition.Y);
			//base.OnPaint (e);
		}

		//
		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
					//来自垂直滚动条的消息
				case WM_VSCROLL:
					//更新滚轮焦点
					this.m_WheelFocus=MWF.MWF_VSCROLL;
					//更新垂直位置
					GetScrollInfo(this.Handle,SB_VERT,ref this.m_ScrollInfo);
					this.m_VPos=this.m_ScrollInfo.nPos;
					switch(this.GetLowWord((int)m.WParam))
					{
							//正在拖动滑块（垂直滚动条）
						case SB_THUMBTRACK:
							this.AutoScrollPosition=new Point(
								this.m_HPos,
								this.m_ScrollInfo.nTrackPos);
							break;
					}
					break;

					//来自水平滚动条的消息
				case WM_HSCROLL:
					//更新滚轮焦点
					this.m_WheelFocus=MWF.MWF_HSCROLL;
					//更新水平位置
					GetScrollInfo(this.Handle,SB_HORZ,ref this.m_ScrollInfo);
					this.m_HPos=this.m_ScrollInfo.nPos;
					switch(this.GetLowWord((int)m.WParam))
					{
							//正在拖动滑块，在此给用户实时反馈
						case SB_THUMBTRACK:
							this.AutoScrollPosition=new Point(this.m_ScrollInfo.nTrackPos,this.m_VPos);
							break;
					}
					break;

//					//鼠标的滚轮消息,wParam的高位－滚动方向，120或者-120，低位－虚拟键状态
//				case WM_MOUSEWHEEL:
//					//判断滚轮的滚动方向：
//					if(this.GetHighWord((int)m.WParam)>0)
//					{
//						//向前/远离用户
//						if(this.m_WheelFocus==MWF.MWF_VSCROLL)
//							MainFrm.SendMessage((int)this.Handle,WM_VSCROLL,(int)SB_LINEUP,0);
//						else
//							MainFrm.SendMessage((int)this.Handle,WM_HSCROLL,(int)SB_LEFT,0);
//					}
//					else
//					{
//						//向后/朝向用户
//						if(this.m_WheelFocus==MWF.MWF_VSCROLL)
//							MainFrm.SendMessage((int)this.Handle,WM_VSCROLL,(int)SB_LINEDOWN,0);
//						else
//							MainFrm.SendMessage((int)this.Handle,WM_HSCROLL,(int)SB_RIGHT,0);
//					}
//					break;
				default:
					break;
			}
			//把消息传递给系统默认处理
			base.WndProc(ref m);
		}

		//------------------
		//内部辅助函数
		//-----------------
		//取高16位字
		private short GetHighWord(int wParam)
		{
			return (short)((wParam & 0xFFFF0000)>>8);
		}

		//取低16位字
		private short GetLowWord(int wParam)
		{
			return (short)(wParam & 0x0000FFFF);
		}

		private void UpdateScrollInfo()
		{
			//获取全部信息
            this.m_ScrollInfo.fMask=SIF_ALL;
			GetScrollInfo(this.Handle,SB_HORZ,ref this.m_ScrollInfo);
			this.m_HMin=this.m_ScrollInfo.nMin;
			this.m_HMax=this.m_ScrollInfo.nMax;
			this.m_HPos=this.m_ScrollInfo.nPos;

			GetScrollInfo(this.Handle,SB_VERT,ref this.m_ScrollInfo);
            this.m_VMin=this.m_ScrollInfo.nMin;
			this.m_VMax=this.m_ScrollInfo.nMax;
			this.m_VPos=this.m_ScrollInfo.nPos;
			//获取滑块位置和滚动条位置
			this.m_ScrollInfo.fMask=(SIF_POS | SIF_TRACKPOS);
		}

		private string GetStatusText()
		{
			return string.Format("H[{0}:{1}:{2}] V[{3}:{4}:{5}]",
				this.m_HMin,this.m_HPos,this.m_HMax,
				this.m_VMin,this.m_VPos,this.m_VMax);
		}

		//------------------
		//外部接口
		//-----------------
		public Image BITMAP
		{
			get
			{
				return this.m_Bitmap;
			}
			set
			{
				this.m_Bitmap=(Bitmap)value;
				this.m_label.Location=new Point(value.Width,value.Height);
				//复位滚动位置
				this.AutoScrollPosition=new Point(0,0);
				//重绘之
				this.Invalidate();
				//更新scroll的信息
				this.UpdateScrollInfo();
			}
		}

		//获取滚轮输入的焦点
		public MWF MouseWheel_Focus
		{
			get	{return this.m_WheelFocus;}
		}

		//每滚动鼠标滚轮一次，控件上滚动的象素距离,10-慢速，20－普通，30－快速，40－最快
		public int PixelsPerWheel
		{
			get{return this.m_PixelsPerWheel;}
			set{this.m_PixelsPerWheel=value;}
		}

		/// <summary>
		/// 处理来自滚轮的消息，使自身滚动
		/// </summary>
		/// <param name="delta">120的整数倍，符号为滚动方向，请用MouseEventArgs中的该参数</param>
		public void ScrollByMouseWheel(int delta)
		{
			if(this.m_WheelFocus==MWF.MWF_VSCROLL)
			{
				//每次滚动20象素
				this.m_VPos-=this.m_PixelsPerWheel*delta/120;
				if(this.m_VPos<0)
					this.m_VPos=0;
				else if(this.m_VPos>this.m_VMax)
					this.m_VPos=this.m_VMax;
			}
			else
			{
				//每次滚动20象素
				this.m_HPos-=this.m_PixelsPerWheel*delta/120;
				if(this.m_HPos<0)
					this.m_HPos=0;
				else if(this.m_HPos>this.m_HMax)
					this.m_HPos=this.m_HMax;
			}
			this.AutoScrollPosition=new Point(this.m_HPos,this.m_VPos);
		}
	}
}
