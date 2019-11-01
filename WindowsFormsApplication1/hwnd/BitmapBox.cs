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
	//������Ϣ
	public struct SCROLLINFO
	{
		public uint cbSize;
		public uint fMask;
		public int nMin;	//��Сλ��
		public int nMax;	//���λ��
		public uint nPage;	//page size
		public int nPos;	//��ǰλ�ã��϶�����ʱ���ֵ���ᱻ����
		public int nTrackPos;	//�϶��еĻ���λ�ã������޷���������
	}

	//�����ֽ��� ( MouseWheel's Focus )
	public enum MWF : int
	{
		MWF_VSCROLL=0,
		MWF_HSCROLL=1
	}
	/// <summary>
	/// BitmapBox ��ժҪ˵��:
	/// ��Panel�̳У�����װ��һ��bitmap
	/// </summary>
	public class BitmapBox : System.Windows.Forms.Panel
	{
		#region �Զ������
		//Ϊ�˳��ֹ�������һ����̬�ı��ؼ���ͨ�������ı�bounds���������¹�����
		private Label m_label=new Label();
		//�������ݣ��ؼ����е�λͼ
		private Bitmap m_Bitmap;
		//���ڻ�ȡscroll��Ϣ�Ľṹ
		private SCROLLINFO m_ScrollInfo;
		//ˮƽ��������Ϣ
		private int m_HMin;
		private int m_HMax;
		private int m_HPos;

		//��ֱ��������Ϣ
		private int m_VMin;
		private int m_VMax;
		private int m_VPos;
		
		//����һ����־��ָʾ��ǰ���ֽ����Ǵ�ֱ����������ˮƽ��������
		private MWF m_WheelFocus=MWF.MWF_VSCROLL;
		//ÿ����һ�������֣���������ؾ��룡
		private int m_PixelsPerWheel=20;
		#endregion

		#region WINAPI const defines
		//�͹������йص���Ϣ (BitmapBox�ؼ��õ�)
		public const int WM_HSCROLL  =0x0114;
		public const int WM_VSCROLL  =0x0115;
		/*
		 * Scroll Bar Commands (16λ��short����Ϊ��wParam�Ľذ�)
		 */
		public const short SB_LINEUP       =  0;
		public const short SB_LINELEFT     =  0;
		public const short SB_LINEDOWN     =  1;
		public const short SB_LINERIGHT    =  1;
		public const short SB_PAGEUP       =  2;	//���Ϸ�ҳ
		public const short SB_PAGELEFT     =  2;	//����ҳ
		public const short SB_PAGEDOWN     =  3;
		public const short SB_PAGERIGHT    =  3;
		public const short SB_THUMBPOSITION=  4;	//�û�����ͷ��˻���
		public const short SB_THUMBTRACK   =  5;	//�û������϶�����
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
		 * fMask��ȡֵ����ȡ��������Щ��Ϣ�����룩
		 */
		public const uint SIF_RANGE           =0x0001;
		public const uint SIF_PAGE            =0x0002;
		public const uint SIF_POS             =0x0004;
		public const uint SIF_DISABLENOSCROLL =0x0008;
		public const uint SIF_TRACKPOS        =0x0010;
		public const uint SIF_ALL             =(SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

//		/*
//		 * �����ϢID
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
//		public const int WM_MOUSEWHEEL      = 0x020A;	//������Ϣ
//		public const int WM_XBUTTONDOWN     = 0x020B;
//		public const int WM_XBUTTONUP       = 0x020C;
//		public const int WM_XBUTTONDBLCLK   = 0x020D;
		#endregion

		#region P/Invokd USER32
		//��ȡ������Ϣ
		[DllImport("user32")]
		public static extern bool GetScrollInfo(
			IntPtr hwnd,			//scroll ctl���������ӵ�й������Ĵ��ھ��
			int fnBar,				//Scroll���ͣ��ؼ���ˮƽ���Ǵ�ֱ��
			ref SCROLLINFO lpsi
			);
		[DllImport("user32")]
		public static extern bool SetScrollInfo(
			IntPtr hwnd,			//scroll ctl���������ӵ�й������Ĵ��ھ��
			int fnBar,				//Scroll���ͣ��ؼ���ˮƽ���Ǵ�ֱ��
			ref SCROLLINFO lpsi,
			bool fRedraw			//�Ƿ��ػ��Է�ӳ�仯
			);

		//��ȡ����λ��
		[DllImport("user32")]
		public static extern int GetScrollPos(
			IntPtr hwnd,
			int nBar
			);

		//����λ�ã�����ǰһ��λ��
		[DllImport("user32")]
		public static extern int SetScrollPos(
			IntPtr hwnd,
			int nBar,
			int nPos,
			bool bRedraw	//�Ƿ��ػ�scrollbar�Է�ӳ�仯
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
			bool bRedraw	//�Ƿ��ػ�scrollbar�Է�ӳ�仯
			);
		#endregion
		/// <summary> 
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// �޲ι��캯��
		/// </summary>
		public BitmapBox()
		{
			InitializeComponent();
			//���ڲ��ؼ�λ�ó�����ʾ����ʱ�Զ����ֹ�����
			this.AutoScroll=true;
			//3D��Ե
			this.BorderStyle=BorderStyle.Fixed3D;
			//���label
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
			//���ڲ��ؼ�λ�ó�����ʾ����ʱ�Զ����ֹ�����
			this.AutoScroll=true;
			//3D��Ե
			this.BorderStyle=BorderStyle.Fixed3D;
			//���label
			this.m_label.Bounds=new Rectangle(0,0,1,1);
			this.m_label.TextAlign=ContentAlignment.TopLeft;
			this.m_label.Visible=false;
			this.m_label.Parent=this;
			//����bitmap
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

		#region �����������ɵĴ���
		/// <summary> 
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭�� 
		/// �޸Ĵ˷��������ݡ�
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
					//���Դ�ֱ����������Ϣ
				case WM_VSCROLL:
					//���¹��ֽ���
					this.m_WheelFocus=MWF.MWF_VSCROLL;
					//���´�ֱλ��
					GetScrollInfo(this.Handle,SB_VERT,ref this.m_ScrollInfo);
					this.m_VPos=this.m_ScrollInfo.nPos;
					switch(this.GetLowWord((int)m.WParam))
					{
							//�����϶����飨��ֱ��������
						case SB_THUMBTRACK:
							this.AutoScrollPosition=new Point(
								this.m_HPos,
								this.m_ScrollInfo.nTrackPos);
							break;
					}
					break;

					//����ˮƽ����������Ϣ
				case WM_HSCROLL:
					//���¹��ֽ���
					this.m_WheelFocus=MWF.MWF_HSCROLL;
					//����ˮƽλ��
					GetScrollInfo(this.Handle,SB_HORZ,ref this.m_ScrollInfo);
					this.m_HPos=this.m_ScrollInfo.nPos;
					switch(this.GetLowWord((int)m.WParam))
					{
							//�����϶����飬�ڴ˸��û�ʵʱ����
						case SB_THUMBTRACK:
							this.AutoScrollPosition=new Point(this.m_ScrollInfo.nTrackPos,this.m_VPos);
							break;
					}
					break;

//					//���Ĺ�����Ϣ,wParam�ĸ�λ����������120����-120����λ�������״̬
//				case WM_MOUSEWHEEL:
//					//�жϹ��ֵĹ�������
//					if(this.GetHighWord((int)m.WParam)>0)
//					{
//						//��ǰ/Զ���û�
//						if(this.m_WheelFocus==MWF.MWF_VSCROLL)
//							MainFrm.SendMessage((int)this.Handle,WM_VSCROLL,(int)SB_LINEUP,0);
//						else
//							MainFrm.SendMessage((int)this.Handle,WM_HSCROLL,(int)SB_LEFT,0);
//					}
//					else
//					{
//						//���/�����û�
//						if(this.m_WheelFocus==MWF.MWF_VSCROLL)
//							MainFrm.SendMessage((int)this.Handle,WM_VSCROLL,(int)SB_LINEDOWN,0);
//						else
//							MainFrm.SendMessage((int)this.Handle,WM_HSCROLL,(int)SB_RIGHT,0);
//					}
//					break;
				default:
					break;
			}
			//����Ϣ���ݸ�ϵͳĬ�ϴ���
			base.WndProc(ref m);
		}

		//------------------
		//�ڲ���������
		//-----------------
		//ȡ��16λ��
		private short GetHighWord(int wParam)
		{
			return (short)((wParam & 0xFFFF0000)>>8);
		}

		//ȡ��16λ��
		private short GetLowWord(int wParam)
		{
			return (short)(wParam & 0x0000FFFF);
		}

		private void UpdateScrollInfo()
		{
			//��ȡȫ����Ϣ
            this.m_ScrollInfo.fMask=SIF_ALL;
			GetScrollInfo(this.Handle,SB_HORZ,ref this.m_ScrollInfo);
			this.m_HMin=this.m_ScrollInfo.nMin;
			this.m_HMax=this.m_ScrollInfo.nMax;
			this.m_HPos=this.m_ScrollInfo.nPos;

			GetScrollInfo(this.Handle,SB_VERT,ref this.m_ScrollInfo);
            this.m_VMin=this.m_ScrollInfo.nMin;
			this.m_VMax=this.m_ScrollInfo.nMax;
			this.m_VPos=this.m_ScrollInfo.nPos;
			//��ȡ����λ�ú͹�����λ��
			this.m_ScrollInfo.fMask=(SIF_POS | SIF_TRACKPOS);
		}

		private string GetStatusText()
		{
			return string.Format("H[{0}:{1}:{2}] V[{3}:{4}:{5}]",
				this.m_HMin,this.m_HPos,this.m_HMax,
				this.m_VMin,this.m_VPos,this.m_VMax);
		}

		//------------------
		//�ⲿ�ӿ�
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
				//��λ����λ��
				this.AutoScrollPosition=new Point(0,0);
				//�ػ�֮
				this.Invalidate();
				//����scroll����Ϣ
				this.UpdateScrollInfo();
			}
		}

		//��ȡ��������Ľ���
		public MWF MouseWheel_Focus
		{
			get	{return this.m_WheelFocus;}
		}

		//ÿ����������һ�Σ��ؼ��Ϲ��������ؾ���,10-���٣�20����ͨ��30�����٣�40�����
		public int PixelsPerWheel
		{
			get{return this.m_PixelsPerWheel;}
			set{this.m_PixelsPerWheel=value;}
		}

		/// <summary>
		/// �������Թ��ֵ���Ϣ��ʹ�������
		/// </summary>
		/// <param name="delta">120��������������Ϊ������������MouseEventArgs�еĸò���</param>
		public void ScrollByMouseWheel(int delta)
		{
			if(this.m_WheelFocus==MWF.MWF_VSCROLL)
			{
				//ÿ�ι���20����
				this.m_VPos-=this.m_PixelsPerWheel*delta/120;
				if(this.m_VPos<0)
					this.m_VPos=0;
				else if(this.m_VPos>this.m_VMax)
					this.m_VPos=this.m_VMax;
			}
			else
			{
				//ÿ�ι���20����
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
