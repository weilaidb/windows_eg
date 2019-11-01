using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Imaging;	//save and open

namespace DesktopWndView
{
	/// <summary>
	/// BitmapBoxDlg 的摘要说明。
	/// </summary>
	public class BitmapBoxDlg : System.Windows.Forms.Form
	{
		#region 自定义变量
		private BitmapBox m_BitmapBox;
		#endregion
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnOpenImage;
		private System.Windows.Forms.MenuItem mnSaveImage;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem mnSpeedLow;
		private System.Windows.Forms.MenuItem mnSpeedNormal;
		private System.Windows.Forms.MenuItem mnSpeedFaster;
		private System.Windows.Forms.MenuItem mnSpeedFatest;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mnAbout;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BitmapBoxDlg()
		{
			InitializeComponent();
			this.m_BitmapBox=new BitmapBox();
			this.m_BitmapBox.Dock=DockStyle.Fill;
			this.m_BitmapBox.Parent=this;
			//设置为最前窗口
//			this.TopMost=true;
			//鼠标滚轮事件处理
			this.MouseWheel+=new MouseEventHandler(BitmapBoxDlg_MouseWheel);
		}
		public BitmapBoxDlg(Bitmap bm)
		{
			InitializeComponent();
			this.m_BitmapBox=new BitmapBox(bm);
			this.m_BitmapBox.Dock=DockStyle.Fill;
			this.m_BitmapBox.Parent=this;
			//设置为最前窗口
//			this.TopMost=true;
			//鼠标滚轮事件处理
			this.MouseWheel+=new MouseEventHandler(BitmapBoxDlg_MouseWheel);
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mnOpenImage = new System.Windows.Forms.MenuItem();
			this.mnSaveImage = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnSpeedLow = new System.Windows.Forms.MenuItem();
			this.mnSpeedNormal = new System.Windows.Forms.MenuItem();
			this.mnSpeedFaster = new System.Windows.Forms.MenuItem();
			this.mnSpeedFatest = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mnAbout = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem2,
																					  this.menuItem4});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnOpenImage,
																					  this.mnSaveImage});
			this.menuItem1.Text = "文件";
			// 
			// mnOpenImage
			// 
			this.mnOpenImage.Index = 0;
			this.mnOpenImage.Text = "打开";
			this.mnOpenImage.Click += new System.EventHandler(this.mnOpenImage_Click);
			// 
			// mnSaveImage
			// 
			this.mnSaveImage.Index = 1;
			this.mnSaveImage.Text = "保存";
			this.mnSaveImage.Click += new System.EventHandler(this.mnSaveImage_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem3});
			this.menuItem2.Text = "设置";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnSpeedLow,
																					  this.mnSpeedNormal,
																					  this.mnSpeedFaster,
																					  this.mnSpeedFatest});
			this.menuItem3.Text = "鼠标滚轮速度";
			// 
			// mnSpeedLow
			// 
			this.mnSpeedLow.Index = 0;
			this.mnSpeedLow.Text = "低速";
			this.mnSpeedLow.Click += new System.EventHandler(this.mnSpeedLow_Click);
			// 
			// mnSpeedNormal
			// 
			this.mnSpeedNormal.Checked = true;
			this.mnSpeedNormal.Index = 1;
			this.mnSpeedNormal.Text = "普通";
			this.mnSpeedNormal.Click += new System.EventHandler(this.mnSpeedNormal_Click);
			// 
			// mnSpeedFaster
			// 
			this.mnSpeedFaster.Index = 2;
			this.mnSpeedFaster.Text = "快速";
			this.mnSpeedFaster.Click += new System.EventHandler(this.mnSpeedFaster_Click);
			// 
			// mnSpeedFatest
			// 
			this.mnSpeedFatest.Index = 3;
			this.mnSpeedFatest.Text = "最快速";
			this.mnSpeedFatest.Click += new System.EventHandler(this.mnSpeedFatest_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnAbout});
			this.menuItem4.Text = "帮助";
			// 
			// mnAbout
			// 
			this.mnAbout.Index = 0;
			this.mnAbout.Text = "关于...";
			this.mnAbout.Click += new System.EventHandler(this.mnAbout_Click);
			// 
			// BitmapBoxDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(464, 273);
			this.Menu = this.mainMenu1;
			this.Name = "BitmapBoxDlg";
			this.Text = "截图";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.BitmapBoxDlg_Closing);

		}
		#endregion

		//--------------------
		// 外部接口
		//--------------------
		public Image BITMAP
		{
			get
			{
				return this.m_BitmapBox.BITMAP;
			}
			set
			{
				//BitmapBox会自己重绘
				this.m_BitmapBox.BITMAP=value;
			}
		}
		//从外部显示关闭窗体的方法
		public void ENDDIALOG()
		{
			this.Close();
		}

		/// <summary>
		/// 拦截关闭消息，只是隐藏它
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BitmapBoxDlg_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Hide();
			e.Cancel=true;
		}

		//保存图片
		private void mnSaveImage_Click(object sender, System.EventArgs e)
		{
			if(this.m_BitmapBox.BITMAP==null)
			{
				MessageBox.Show("当前没有图片!");
				return;
			}
			SaveFileDialog dlg=new SaveFileDialog();
			dlg.Filter="*.bmp|*.bmp|*.jpeg|*.jpg;*.jpeg|*.gif|*.gif";
			if(dlg.ShowDialog()==DialogResult.OK)
			{
				switch(dlg.FilterIndex)
				{
					case 1:
						this.m_BitmapBox.BITMAP.Save(dlg.FileName,ImageFormat.Bmp);
						break;
					case 2:
						this.m_BitmapBox.BITMAP.Save(dlg.FileName,ImageFormat.Jpeg);
						break;
					case 3:
						this.m_BitmapBox.BITMAP.Save(dlg.FileName,ImageFormat.Gif);
						break;
				}
			}
		}

		//打开图片
		private void mnOpenImage_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg=new OpenFileDialog();
			dlg.Filter="图像文件(*.bmp;*.jpg;*.jpeg;*.gif;*.ico;*.png;*.emf;*.wmf)|*.bmp;*.jpg;*.jpeg;*.gif;*.ico;*.png;*.emf;*.wmf";
			if(dlg.ShowDialog()==DialogResult.OK)
			{
				Image bm=Bitmap.FromFile(dlg.FileName);
				this.m_BitmapBox.BITMAP=bm;
				this.Text=string.Format("{0}-[{1}*{2}]",dlg.FileName,bm.Width,bm.Height);
			}
		}

		/// <summary>
		/// 让控件处理滚轮消息
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BitmapBoxDlg_MouseWheel(object sender, MouseEventArgs e)
		{
			this.m_BitmapBox.ScrollByMouseWheel(e.Delta);
		}

		//----------------------------------------------
		//	内部辅助函数
		//----------------------------------------------
		//设置菜单的checked属性，以及控件的滚动速度
		private void SetMenuAndBitmapBox(int pixels)
		{
			//判断是否选中的就是当前已选菜单
			if(this.m_BitmapBox.PixelsPerWheel==pixels)
				return;
			//清除已有的checked属性（因为是互斥关系）
			switch(this.m_BitmapBox.PixelsPerWheel)
			{
				case 10:
					this.mnSpeedLow.Checked=false;
					break;
				case 20:
					this.mnSpeedNormal.Checked=false;
					break;
				case 30:
					this.mnSpeedFaster.Checked=false;
					break;
				case 60:
					this.mnSpeedFatest.Checked=false;
					break;
				default:
					this.mnSpeedNormal.Checked=false;
					break;
			}
			
			//将属性设置给BitmapBox控件！它是控件提供的一个外部接口
			this.m_BitmapBox.PixelsPerWheel=pixels;
			switch(pixels)
			{
				case 10:
					this.mnSpeedLow.Checked=true;
					break;
				case 20:
					this.mnSpeedNormal.Checked=true;
					break;
				case 30:
					this.mnSpeedFaster.Checked=true;
					break;
				case 60:
					this.mnSpeedFatest.Checked=true;
					break;
				default:
					this.mnSpeedNormal.Checked=true;
					this.m_BitmapBox.PixelsPerWheel=20;
					return;
			}
		}
		//----------------------------------------------
		//	鼠标滚轮速度菜单的处理
		//----------------------------------------------
		private void mnSpeedLow_Click(object sender, System.EventArgs e)
		{
            this.SetMenuAndBitmapBox(10);	//低速
		}

		private void mnSpeedNormal_Click(object sender, System.EventArgs e)
		{
             this.SetMenuAndBitmapBox(20);	//普通速度
		}

		private void mnSpeedFaster_Click(object sender, System.EventArgs e)
		{
			 this.SetMenuAndBitmapBox(30);	//快速
		}

		private void mnSpeedFatest_Click(object sender, System.EventArgs e)
		{
			 this.SetMenuAndBitmapBox(60);	//最快速
		}

		//关于对话框，注意显示前，必须设置topmost属性为false，以免无法点到关于对话框而无法继续
		private void mnAbout_Click(object sender, System.EventArgs e)
		{
			this.TopMost=false;
			AboutDlg dlg=new AboutDlg();
			dlg.ShowDialog();
		}
	}
}
