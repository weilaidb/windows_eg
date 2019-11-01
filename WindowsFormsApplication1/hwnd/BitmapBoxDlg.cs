using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Imaging;	//save and open

namespace DesktopWndView
{
	/// <summary>
	/// BitmapBoxDlg ��ժҪ˵����
	/// </summary>
	public class BitmapBoxDlg : System.Windows.Forms.Form
	{
		#region �Զ������
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
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BitmapBoxDlg()
		{
			InitializeComponent();
			this.m_BitmapBox=new BitmapBox();
			this.m_BitmapBox.Dock=DockStyle.Fill;
			this.m_BitmapBox.Parent=this;
			//����Ϊ��ǰ����
//			this.TopMost=true;
			//�������¼�����
			this.MouseWheel+=new MouseEventHandler(BitmapBoxDlg_MouseWheel);
		}
		public BitmapBoxDlg(Bitmap bm)
		{
			InitializeComponent();
			this.m_BitmapBox=new BitmapBox(bm);
			this.m_BitmapBox.Dock=DockStyle.Fill;
			this.m_BitmapBox.Parent=this;
			//����Ϊ��ǰ����
//			this.TopMost=true;
			//�������¼�����
			this.MouseWheel+=new MouseEventHandler(BitmapBoxDlg_MouseWheel);
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
			this.menuItem1.Text = "�ļ�";
			// 
			// mnOpenImage
			// 
			this.mnOpenImage.Index = 0;
			this.mnOpenImage.Text = "��";
			this.mnOpenImage.Click += new System.EventHandler(this.mnOpenImage_Click);
			// 
			// mnSaveImage
			// 
			this.mnSaveImage.Index = 1;
			this.mnSaveImage.Text = "����";
			this.mnSaveImage.Click += new System.EventHandler(this.mnSaveImage_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem3});
			this.menuItem2.Text = "����";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnSpeedLow,
																					  this.mnSpeedNormal,
																					  this.mnSpeedFaster,
																					  this.mnSpeedFatest});
			this.menuItem3.Text = "�������ٶ�";
			// 
			// mnSpeedLow
			// 
			this.mnSpeedLow.Index = 0;
			this.mnSpeedLow.Text = "����";
			this.mnSpeedLow.Click += new System.EventHandler(this.mnSpeedLow_Click);
			// 
			// mnSpeedNormal
			// 
			this.mnSpeedNormal.Checked = true;
			this.mnSpeedNormal.Index = 1;
			this.mnSpeedNormal.Text = "��ͨ";
			this.mnSpeedNormal.Click += new System.EventHandler(this.mnSpeedNormal_Click);
			// 
			// mnSpeedFaster
			// 
			this.mnSpeedFaster.Index = 2;
			this.mnSpeedFaster.Text = "����";
			this.mnSpeedFaster.Click += new System.EventHandler(this.mnSpeedFaster_Click);
			// 
			// mnSpeedFatest
			// 
			this.mnSpeedFatest.Index = 3;
			this.mnSpeedFatest.Text = "�����";
			this.mnSpeedFatest.Click += new System.EventHandler(this.mnSpeedFatest_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnAbout});
			this.menuItem4.Text = "����";
			// 
			// mnAbout
			// 
			this.mnAbout.Index = 0;
			this.mnAbout.Text = "����...";
			this.mnAbout.Click += new System.EventHandler(this.mnAbout_Click);
			// 
			// BitmapBoxDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(464, 273);
			this.Menu = this.mainMenu1;
			this.Name = "BitmapBoxDlg";
			this.Text = "��ͼ";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.BitmapBoxDlg_Closing);

		}
		#endregion

		//--------------------
		// �ⲿ�ӿ�
		//--------------------
		public Image BITMAP
		{
			get
			{
				return this.m_BitmapBox.BITMAP;
			}
			set
			{
				//BitmapBox���Լ��ػ�
				this.m_BitmapBox.BITMAP=value;
			}
		}
		//���ⲿ��ʾ�رմ���ķ���
		public void ENDDIALOG()
		{
			this.Close();
		}

		/// <summary>
		/// ���عر���Ϣ��ֻ��������
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BitmapBoxDlg_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Hide();
			e.Cancel=true;
		}

		//����ͼƬ
		private void mnSaveImage_Click(object sender, System.EventArgs e)
		{
			if(this.m_BitmapBox.BITMAP==null)
			{
				MessageBox.Show("��ǰû��ͼƬ!");
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

		//��ͼƬ
		private void mnOpenImage_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg=new OpenFileDialog();
			dlg.Filter="ͼ���ļ�(*.bmp;*.jpg;*.jpeg;*.gif;*.ico;*.png;*.emf;*.wmf)|*.bmp;*.jpg;*.jpeg;*.gif;*.ico;*.png;*.emf;*.wmf";
			if(dlg.ShowDialog()==DialogResult.OK)
			{
				Image bm=Bitmap.FromFile(dlg.FileName);
				this.m_BitmapBox.BITMAP=bm;
				this.Text=string.Format("{0}-[{1}*{2}]",dlg.FileName,bm.Width,bm.Height);
			}
		}

		/// <summary>
		/// �ÿؼ����������Ϣ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BitmapBoxDlg_MouseWheel(object sender, MouseEventArgs e)
		{
			this.m_BitmapBox.ScrollByMouseWheel(e.Delta);
		}

		//----------------------------------------------
		//	�ڲ���������
		//----------------------------------------------
		//���ò˵���checked���ԣ��Լ��ؼ��Ĺ����ٶ�
		private void SetMenuAndBitmapBox(int pixels)
		{
			//�ж��Ƿ�ѡ�еľ��ǵ�ǰ��ѡ�˵�
			if(this.m_BitmapBox.PixelsPerWheel==pixels)
				return;
			//������е�checked���ԣ���Ϊ�ǻ����ϵ��
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
			
			//���������ø�BitmapBox�ؼ������ǿؼ��ṩ��һ���ⲿ�ӿ�
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
		//	�������ٶȲ˵��Ĵ���
		//----------------------------------------------
		private void mnSpeedLow_Click(object sender, System.EventArgs e)
		{
            this.SetMenuAndBitmapBox(10);	//����
		}

		private void mnSpeedNormal_Click(object sender, System.EventArgs e)
		{
             this.SetMenuAndBitmapBox(20);	//��ͨ�ٶ�
		}

		private void mnSpeedFaster_Click(object sender, System.EventArgs e)
		{
			 this.SetMenuAndBitmapBox(30);	//����
		}

		private void mnSpeedFatest_Click(object sender, System.EventArgs e)
		{
			 this.SetMenuAndBitmapBox(60);	//�����
		}

		//���ڶԻ���ע����ʾǰ����������topmost����Ϊfalse�������޷��㵽���ڶԻ�����޷�����
		private void mnAbout_Click(object sender, System.EventArgs e)
		{
			this.TopMost=false;
			AboutDlg dlg=new AboutDlg();
			dlg.ShowDialog();
		}
	}
}
