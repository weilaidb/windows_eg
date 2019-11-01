using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Win32;		//RegistryKey
using System.IO;


namespace DesktopWndView
{
	/// <summary>
	/// IEManDlg 的摘要说明。
	/// </summary>
	public class IEManDlg : System.Windows.Forms.Form
	{
		#region 自定义变量
		//Key Path 注册表键路径
		public const string KP_TYPEDURLS=@"Software\Microsoft\Internet Explorer\TypedURLs";
		public const string KP_MENUEXT=@"Software\Microsoft\Internet Explorer\MenuExt";
		public const string KP_MAIN=@"Software\Microsoft\Internet Explorer\Main";
		//默认值，用于恢复 DF : Default 
		public const string DF_WINDOWTITLE="Microsoft Internet Explorer";
		public const string DF_BLANKPAGE="about:blank";
		public const string DF_LOCALPAGE="about:blank";
		public const string DF_DEFAULTPAGEURL=@"http://www.microsoft.com/windows/ie_intl/cn/start/";
		public const string DF_DEFAULTSEARCHPAGE=@"http://www.microsoft.com/isapi/redir.dll?prd=ie&ar=iesearch";
		//值名（Value Name）
		public const string VN_STARTPAGE="Start Page";
		public const string VN_SEARCHPAGE="Search Page";
		public const string VN_IEVERSION="Wizard_Version";
		public const string VN_DEFAULTPAGEURL="default_page_url";
		public const string VN_SAVEDIRECTORY="Save Directory";
		public const string VN_WINDOWTITLE="Window Title";
		public const string VN_LOCALPAGE="Local Page";
		public const string VN_DEFAULTSEARCHURL="Default_Search_URL";

		//文件后缀名
		public string[] LINKFILES_MEDIA=new string[]{".rm.lnk",".rmvb.lnk",".wmv.lnk",".avi.lnk",".asf.lnk",".mp3.lnk"};
		public string[] LINKFILES_OFFICE=new string[]{".doc.lnk",".xls.lnk",".mpp.lnk",".ppt.lnk",".mdb.lnk"};
		#endregion

		#region 窗体设计器变量

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage pageTypedURLs;
		private System.Windows.Forms.TabPage pageMenuExt;
		private System.Windows.Forms.TabPage pageMain;
		private System.Windows.Forms.ListView listTypedURLs;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button btClearUrls;
		private System.Windows.Forms.RadioButton rbSelectAllUrl;
		private System.Windows.Forms.RadioButton rbSelectNoneUrl;
		private System.Windows.Forms.ListView listMenuExt;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button btDelMenuExt;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbDefaultPage;
		private System.Windows.Forms.TextBox tbStartPage;
		private System.Windows.Forms.TextBox tbSearchPage;
		private System.Windows.Forms.TextBox tbIeVersion;
		private System.Windows.Forms.TextBox tbWindowTitle;
		private System.Windows.Forms.TextBox tbDefalutSearchPage;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button btBlankPage;
		private System.Windows.Forms.Button btFixDefaultSearchPage;
		private System.Windows.Forms.Button btFixWindowTitle;
		private System.Windows.Forms.Button btApplyMain;
		private System.Windows.Forms.Button btFixDefaultPage;
		private System.Windows.Forms.Button btFixSearchPage;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TabPage pageClearRecent;
		private System.Windows.Forms.Button btClearRecent;
		private System.Windows.Forms.RadioButton rbAllFiles;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rbMediaFiles;
		private System.Windows.Forms.RadioButton rbOfficeFiles;
		private System.ComponentModel.IContainer components;

		#endregion

		//构造函数
		public IEManDlg()
		{
			InitializeComponent();
			this.LoadMain();
			this.LoadTypedURLs();
			this.LoadMenuExt();
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.pageMain = new System.Windows.Forms.TabPage();
			this.btFixDefaultPage = new System.Windows.Forms.Button();
			this.btApplyMain = new System.Windows.Forms.Button();
			this.btFixWindowTitle = new System.Windows.Forms.Button();
			this.btFixDefaultSearchPage = new System.Windows.Forms.Button();
			this.btFixSearchPage = new System.Windows.Forms.Button();
			this.btBlankPage = new System.Windows.Forms.Button();
			this.tbDefalutSearchPage = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tbWindowTitle = new System.Windows.Forms.TextBox();
			this.tbIeVersion = new System.Windows.Forms.TextBox();
			this.tbSearchPage = new System.Windows.Forms.TextBox();
			this.tbStartPage = new System.Windows.Forms.TextBox();
			this.tbDefaultPage = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pageTypedURLs = new System.Windows.Forms.TabPage();
			this.rbSelectNoneUrl = new System.Windows.Forms.RadioButton();
			this.rbSelectAllUrl = new System.Windows.Forms.RadioButton();
			this.btClearUrls = new System.Windows.Forms.Button();
			this.listTypedURLs = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.pageMenuExt = new System.Windows.Forms.TabPage();
			this.btDelMenuExt = new System.Windows.Forms.Button();
			this.listMenuExt = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.pageClearRecent = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbOfficeFiles = new System.Windows.Forms.RadioButton();
			this.rbMediaFiles = new System.Windows.Forms.RadioButton();
			this.rbAllFiles = new System.Windows.Forms.RadioButton();
			this.btClearRecent = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tabControl1.SuspendLayout();
			this.pageMain.SuspendLayout();
			this.pageTypedURLs.SuspendLayout();
			this.pageMenuExt.SuspendLayout();
			this.pageClearRecent.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.pageMain);
			this.tabControl1.Controls.Add(this.pageTypedURLs);
			this.tabControl1.Controls.Add(this.pageMenuExt);
			this.tabControl1.Controls.Add(this.pageClearRecent);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(528, 328);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// pageMain
			// 
			this.pageMain.Controls.Add(this.btFixDefaultPage);
			this.pageMain.Controls.Add(this.btApplyMain);
			this.pageMain.Controls.Add(this.btFixWindowTitle);
			this.pageMain.Controls.Add(this.btFixDefaultSearchPage);
			this.pageMain.Controls.Add(this.btFixSearchPage);
			this.pageMain.Controls.Add(this.btBlankPage);
			this.pageMain.Controls.Add(this.tbDefalutSearchPage);
			this.pageMain.Controls.Add(this.label6);
			this.pageMain.Controls.Add(this.tbWindowTitle);
			this.pageMain.Controls.Add(this.tbIeVersion);
			this.pageMain.Controls.Add(this.tbSearchPage);
			this.pageMain.Controls.Add(this.tbStartPage);
			this.pageMain.Controls.Add(this.tbDefaultPage);
			this.pageMain.Controls.Add(this.label5);
			this.pageMain.Controls.Add(this.label4);
			this.pageMain.Controls.Add(this.label3);
			this.pageMain.Controls.Add(this.label2);
			this.pageMain.Controls.Add(this.label1);
			this.pageMain.Location = new System.Drawing.Point(4, 24);
			this.pageMain.Name = "pageMain";
			this.pageMain.Size = new System.Drawing.Size(520, 300);
			this.pageMain.TabIndex = 2;
			this.pageMain.Text = "主选项";
			// 
			// btFixDefaultPage
			// 
			this.btFixDefaultPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btFixDefaultPage.Location = new System.Drawing.Point(448, 24);
			this.btFixDefaultPage.Name = "btFixDefaultPage";
			this.btFixDefaultPage.Size = new System.Drawing.Size(64, 23);
			this.btFixDefaultPage.TabIndex = 1;
			this.btFixDefaultPage.Text = "恢复";
			this.btFixDefaultPage.Click += new System.EventHandler(this.btFixDefaultPage_Click);
			// 
			// btApplyMain
			// 
			this.btApplyMain.Location = new System.Drawing.Point(448, 264);
			this.btApplyMain.Name = "btApplyMain";
			this.btApplyMain.Size = new System.Drawing.Size(72, 23);
			this.btApplyMain.TabIndex = 11;
			this.btApplyMain.Text = "应用";
			this.btApplyMain.Click += new System.EventHandler(this.btApplyMain_Click);
			// 
			// btFixWindowTitle
			// 
			this.btFixWindowTitle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btFixWindowTitle.Location = new System.Drawing.Point(448, 144);
			this.btFixWindowTitle.Name = "btFixWindowTitle";
			this.btFixWindowTitle.Size = new System.Drawing.Size(64, 23);
			this.btFixWindowTitle.TabIndex = 10;
			this.btFixWindowTitle.Text = "恢复";
			this.btFixWindowTitle.Click += new System.EventHandler(this.btFixWindowTitle_Click);
			// 
			// btFixDefaultSearchPage
			// 
			this.btFixDefaultSearchPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btFixDefaultSearchPage.Location = new System.Drawing.Point(448, 96);
			this.btFixDefaultSearchPage.Name = "btFixDefaultSearchPage";
			this.btFixDefaultSearchPage.Size = new System.Drawing.Size(64, 23);
			this.btFixDefaultSearchPage.TabIndex = 7;
			this.btFixDefaultSearchPage.Text = "恢复";
			this.btFixDefaultSearchPage.Click += new System.EventHandler(this.btFixDefaultSearchPage_Click);
			// 
			// btFixSearchPage
			// 
			this.btFixSearchPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btFixSearchPage.Location = new System.Drawing.Point(448, 72);
			this.btFixSearchPage.Name = "btFixSearchPage";
			this.btFixSearchPage.Size = new System.Drawing.Size(64, 23);
			this.btFixSearchPage.TabIndex = 5;
			this.btFixSearchPage.Text = "默认";
			this.btFixSearchPage.Click += new System.EventHandler(this.btFixSearchPage_Click);
			// 
			// btBlankPage
			// 
			this.btBlankPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btBlankPage.Location = new System.Drawing.Point(448, 48);
			this.btBlankPage.Name = "btBlankPage";
			this.btBlankPage.Size = new System.Drawing.Size(64, 23);
			this.btBlankPage.TabIndex = 3;
			this.btBlankPage.Text = "空白页";
			this.btBlankPage.Click += new System.EventHandler(this.btBlankPage_Click);
			// 
			// tbDefalutSearchPage
			// 
			this.tbDefalutSearchPage.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.tbDefalutSearchPage.Location = new System.Drawing.Point(88, 96);
			this.tbDefalutSearchPage.Name = "tbDefalutSearchPage";
			this.tbDefalutSearchPage.Size = new System.Drawing.Size(352, 21);
			this.tbDefalutSearchPage.TabIndex = 6;
			this.tbDefalutSearchPage.Text = "";
			this.tbDefalutSearchPage.MouseEnter += new System.EventHandler(this.TextBoxMouseEnterHandler);
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label6.Location = new System.Drawing.Point(16, 96);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 23);
			this.label6.TabIndex = 10;
			this.label6.Text = "默认搜索页";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbWindowTitle
			// 
			this.tbWindowTitle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.tbWindowTitle.Location = new System.Drawing.Point(88, 144);
			this.tbWindowTitle.Name = "tbWindowTitle";
			this.tbWindowTitle.Size = new System.Drawing.Size(352, 21);
			this.tbWindowTitle.TabIndex = 9;
			this.tbWindowTitle.Text = "";
			this.tbWindowTitle.MouseEnter += new System.EventHandler(this.TextBoxMouseEnterHandler);
			// 
			// tbIeVersion
			// 
			this.tbIeVersion.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.tbIeVersion.Location = new System.Drawing.Point(88, 120);
			this.tbIeVersion.Name = "tbIeVersion";
			this.tbIeVersion.ReadOnly = true;
			this.tbIeVersion.Size = new System.Drawing.Size(352, 21);
			this.tbIeVersion.TabIndex = 8;
			this.tbIeVersion.Text = "";
			this.tbIeVersion.MouseEnter += new System.EventHandler(this.TextBoxMouseEnterHandler);
			// 
			// tbSearchPage
			// 
			this.tbSearchPage.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.tbSearchPage.Location = new System.Drawing.Point(88, 72);
			this.tbSearchPage.Name = "tbSearchPage";
			this.tbSearchPage.Size = new System.Drawing.Size(352, 21);
			this.tbSearchPage.TabIndex = 4;
			this.tbSearchPage.Text = "";
			this.tbSearchPage.MouseEnter += new System.EventHandler(this.TextBoxMouseEnterHandler);
			// 
			// tbStartPage
			// 
			this.tbStartPage.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.tbStartPage.Location = new System.Drawing.Point(88, 48);
			this.tbStartPage.Name = "tbStartPage";
			this.tbStartPage.Size = new System.Drawing.Size(352, 21);
			this.tbStartPage.TabIndex = 2;
			this.tbStartPage.Text = "";
			this.tbStartPage.MouseEnter += new System.EventHandler(this.TextBoxMouseEnterHandler);
			// 
			// tbDefaultPage
			// 
			this.tbDefaultPage.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.tbDefaultPage.Location = new System.Drawing.Point(88, 24);
			this.tbDefaultPage.Name = "tbDefaultPage";
			this.tbDefaultPage.Size = new System.Drawing.Size(352, 21);
			this.tbDefaultPage.TabIndex = 0;
			this.tbDefaultPage.Text = "";
			this.tbDefaultPage.MouseEnter += new System.EventHandler(this.TextBoxMouseEnterHandler);
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label5.Location = new System.Drawing.Point(16, 144);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 23);
			this.label5.TabIndex = 4;
			this.label5.Text = "IE标题";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label4.Location = new System.Drawing.Point(16, 120);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 23);
			this.label4.TabIndex = 3;
			this.label4.Text = "IE版本";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label3.Location = new System.Drawing.Point(16, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 23);
			this.label3.TabIndex = 2;
			this.label3.Text = "搜索页";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label2.Location = new System.Drawing.Point(16, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "首页";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "默认页";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pageTypedURLs
			// 
			this.pageTypedURLs.Controls.Add(this.rbSelectNoneUrl);
			this.pageTypedURLs.Controls.Add(this.rbSelectAllUrl);
			this.pageTypedURLs.Controls.Add(this.btClearUrls);
			this.pageTypedURLs.Controls.Add(this.listTypedURLs);
			this.pageTypedURLs.Location = new System.Drawing.Point(4, 24);
			this.pageTypedURLs.Name = "pageTypedURLs";
			this.pageTypedURLs.Size = new System.Drawing.Size(520, 300);
			this.pageTypedURLs.TabIndex = 0;
			this.pageTypedURLs.Text = "IE地址栏";
			// 
			// rbSelectNoneUrl
			// 
			this.rbSelectNoneUrl.Checked = true;
			this.rbSelectNoneUrl.Location = new System.Drawing.Point(368, 272);
			this.rbSelectNoneUrl.Name = "rbSelectNoneUrl";
			this.rbSelectNoneUrl.Size = new System.Drawing.Size(64, 24);
			this.rbSelectNoneUrl.TabIndex = 4;
			this.rbSelectNoneUrl.TabStop = true;
			this.rbSelectNoneUrl.Text = "全不选";
			this.rbSelectNoneUrl.CheckedChanged += new System.EventHandler(this.rbSelectNoneUrl_CheckedChanged);
			// 
			// rbSelectAllUrl
			// 
			this.rbSelectAllUrl.Location = new System.Drawing.Point(304, 272);
			this.rbSelectAllUrl.Name = "rbSelectAllUrl";
			this.rbSelectAllUrl.Size = new System.Drawing.Size(48, 24);
			this.rbSelectAllUrl.TabIndex = 3;
			this.rbSelectAllUrl.Text = "全选";
			this.rbSelectAllUrl.CheckedChanged += new System.EventHandler(this.rbSelectAllUrl_CheckedChanged);
			// 
			// btClearUrls
			// 
			this.btClearUrls.Location = new System.Drawing.Point(440, 272);
			this.btClearUrls.Name = "btClearUrls";
			this.btClearUrls.TabIndex = 2;
			this.btClearUrls.Text = "清理";
			this.btClearUrls.Click += new System.EventHandler(this.btClearUrls_Click);
			// 
			// listTypedURLs
			// 
			this.listTypedURLs.CheckBoxes = true;
			this.listTypedURLs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.columnHeader1,
																							this.columnHeader2});
			this.listTypedURLs.FullRowSelect = true;
			this.listTypedURLs.GridLines = true;
			this.listTypedURLs.Location = new System.Drawing.Point(0, 0);
			this.listTypedURLs.MultiSelect = false;
			this.listTypedURLs.Name = "listTypedURLs";
			this.listTypedURLs.Size = new System.Drawing.Size(520, 264);
			this.listTypedURLs.TabIndex = 0;
			this.listTypedURLs.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "名称";
			this.columnHeader1.Width = 67;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "数据";
			this.columnHeader2.Width = 437;
			// 
			// pageMenuExt
			// 
			this.pageMenuExt.Controls.Add(this.btDelMenuExt);
			this.pageMenuExt.Controls.Add(this.listMenuExt);
			this.pageMenuExt.Location = new System.Drawing.Point(4, 24);
			this.pageMenuExt.Name = "pageMenuExt";
			this.pageMenuExt.Size = new System.Drawing.Size(520, 300);
			this.pageMenuExt.TabIndex = 1;
			this.pageMenuExt.Text = "右键菜单";
			// 
			// btDelMenuExt
			// 
			this.btDelMenuExt.Location = new System.Drawing.Point(440, 272);
			this.btDelMenuExt.Name = "btDelMenuExt";
			this.btDelMenuExt.TabIndex = 5;
			this.btDelMenuExt.Text = "删除";
			this.btDelMenuExt.Click += new System.EventHandler(this.btDelMenuExt_Click);
			// 
			// listMenuExt
			// 
			this.listMenuExt.CheckBoxes = true;
			this.listMenuExt.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.columnHeader3,
																						  this.columnHeader4});
			this.listMenuExt.FullRowSelect = true;
			this.listMenuExt.GridLines = true;
			this.listMenuExt.Location = new System.Drawing.Point(0, 0);
			this.listMenuExt.MultiSelect = false;
			this.listMenuExt.Name = "listMenuExt";
			this.listMenuExt.Size = new System.Drawing.Size(520, 264);
			this.listMenuExt.TabIndex = 1;
			this.listMenuExt.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "名称";
			this.columnHeader3.Width = 184;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "数据";
			this.columnHeader4.Width = 329;
			// 
			// pageClearRecent
			// 
			this.pageClearRecent.Controls.Add(this.groupBox1);
			this.pageClearRecent.Location = new System.Drawing.Point(4, 24);
			this.pageClearRecent.Name = "pageClearRecent";
			this.pageClearRecent.Size = new System.Drawing.Size(520, 300);
			this.pageClearRecent.TabIndex = 3;
			this.pageClearRecent.Text = "清理";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbOfficeFiles);
			this.groupBox1.Controls.Add(this.rbMediaFiles);
			this.groupBox1.Controls.Add(this.rbAllFiles);
			this.groupBox1.Controls.Add(this.btClearRecent);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(248, 112);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "清理最近文档";
			// 
			// rbOfficeFiles
			// 
			this.rbOfficeFiles.Location = new System.Drawing.Point(168, 32);
			this.rbOfficeFiles.Name = "rbOfficeFiles";
			this.rbOfficeFiles.Size = new System.Drawing.Size(64, 24);
			this.rbOfficeFiles.TabIndex = 3;
			this.rbOfficeFiles.Text = "Office";
			// 
			// rbMediaFiles
			// 
			this.rbMediaFiles.Location = new System.Drawing.Point(88, 32);
			this.rbMediaFiles.Name = "rbMediaFiles";
			this.rbMediaFiles.Size = new System.Drawing.Size(72, 24);
			this.rbMediaFiles.TabIndex = 2;
			this.rbMediaFiles.Text = "多媒体";
			// 
			// rbAllFiles
			// 
			this.rbAllFiles.Checked = true;
			this.rbAllFiles.Location = new System.Drawing.Point(24, 32);
			this.rbAllFiles.Name = "rbAllFiles";
			this.rbAllFiles.Size = new System.Drawing.Size(48, 24);
			this.rbAllFiles.TabIndex = 1;
			this.rbAllFiles.TabStop = true;
			this.rbAllFiles.Text = "全部";
			// 
			// btClearRecent
			// 
			this.btClearRecent.Location = new System.Drawing.Point(152, 72);
			this.btClearRecent.Name = "btClearRecent";
			this.btClearRecent.Size = new System.Drawing.Size(80, 24);
			this.btClearRecent.TabIndex = 0;
			this.btClearRecent.Text = "清理";
			this.btClearRecent.Click += new System.EventHandler(this.btClearRecent_Click);
			// 
			// IEManDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(536, 333);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "IEManDlg";
			this.Text = "IE管理器";
			this.tabControl1.ResumeLayout(false);
			this.pageMain.ResumeLayout(false);
			this.pageTypedURLs.ResumeLayout(false);
			this.pageMenuExt.ResumeLayout(false);
			this.pageClearRecent.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region 内部辅助方法

		//加载Main选项
		private void LoadMain()
		{
			RegistryKey keyCU=Registry.CurrentUser.OpenSubKey(KP_MAIN);
			if(keyCU!=null)
			{
				this.tbDefaultPage.Text=(string)keyCU.GetValue(VN_DEFAULTPAGEURL,"");
				this.tbStartPage.Text=(string)keyCU.GetValue(VN_STARTPAGE,"");
				this.tbSearchPage.Text=(string)keyCU.GetValue(VN_SEARCHPAGE,"");
				this.tbWindowTitle.Text=(string)keyCU.GetValue(VN_WINDOWTITLE,"");
				keyCU.Close();
			}
			
			RegistryKey keyLM=Registry.LocalMachine.OpenSubKey(KP_MAIN);
			if(keyLM!=null)
			{
				this.tbIeVersion.Text=(string)keyLM.GetValue(VN_IEVERSION,"");
				this.tbDefalutSearchPage.Text=(string)keyLM.GetValue(VN_DEFAULTSEARCHURL,"");
				keyLM.Close();
			}
		}

		//加载IE地址栏的TypedURLs
		private void LoadTypedURLs()
		{
			//清空原有的items
			this.listTypedURLs.Items.Clear();
			RegistryKey key=Registry.CurrentUser.OpenSubKey(KP_TYPEDURLS);
			if(key!=null)
			{
				int i=1;
				string url;
				while((url=(string)key.GetValue("url"+i,""))!="")
				{
					this.listTypedURLs.Items.Add("url"+i);
					this.listTypedURLs.Items[i-1].SubItems.Add(url);
					i++;
				}
				key.Close();
			}
		}

		//加载右键上下文菜单
		private void LoadMenuExt()
		{
			//清空原有的items
			this.listMenuExt.Items.Clear();
			RegistryKey key=Registry.CurrentUser.OpenSubKey(KP_MENUEXT);
			if(key!=null)
			{
				RegistryKey subkey;
				string text;
				string[] subkeys=key.GetSubKeyNames();
				for(int i=0;i<subkeys.Length;i++)
				{
					this.listMenuExt.Items.Add(subkeys[i]);
					subkey=key.OpenSubKey(subkeys[i]);
					//string[] values=subkey.GetValueNames();
					//（默认）的ValueName就是空字符串
					text=(string)subkey.GetValue(string.Empty,string.Empty);
					this.listMenuExt.Items[i].SubItems.Add(text);
				}
				key.Close();
			}
		}

		/// <summary>
		/// 鼠标进入文本框的处理，如果超出宽度，则设置tip
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxMouseEnterHandler(object sender, System.EventArgs e)
		{
			TextBox tb=(TextBox)sender;
			Graphics g=tb.CreateGraphics();
			//测量字符串的单行尺寸
			SizeF size=g.MeasureString(tb.Text,tb.Font);
			if(tb.Width<size.Width)
				this.toolTip1.SetToolTip(tb,tb.Text);
			else
				this.toolTip1.SetToolTip(tb,string.Empty);
			g.Dispose();
		}

		/// <summary>
		/// 根据指定的后缀名删除快捷方式文件
		/// </summary>
		/// <param name="files">快捷方式的文件名数组</param>
		/// <param name="filters">后缀名数组（全小写形式），形式为 ".后缀.lnk" </param>
		/// <returns>返回删除的文件数量</returns>
		private int DeleteLinkFiles(string[] files,string[] filters)
		{
			int n=0;	//返回值为删除文件数量
			for(int i=0;i<files.Length;i++)
			{
				for(int j=0;j<filters.Length;j++)
				{
					if(files[i].ToLower().EndsWith(filters[j]))
					{
						File.Delete(files[i]);
						n++;
						break;
					}
				}
			}
			return n;
		}

		#endregion

		//全选按钮的处理
		private void rbSelectAllUrl_CheckedChanged(object sender, System.EventArgs e)
		{
            foreach(ListViewItem item in this.listTypedURLs.Items)
				item.Checked=true;
		}

		//全不选
		private void rbSelectNoneUrl_CheckedChanged(object sender, System.EventArgs e)
		{
			foreach(ListViewItem item in this.listTypedURLs.Items)
				item.Checked=false;		
		}

		//清理按钮
		private void btClearUrls_Click(object sender, System.EventArgs e)
		{
			//以可写方式打开
            RegistryKey key=Registry.CurrentUser.OpenSubKey(KP_TYPEDURLS,true);
			if(key!=null)
			{
				foreach(ListViewItem item in this.listTypedURLs.Items)
				{
					if(item.Checked)
					{
						key.DeleteValue(item.Text,false);	//不抛出空值异常
						item.Remove();
					}
				}
				key.Close();
				MessageBox.Show("清理成功!","IE管理器");
			}
			else
				MessageBox.Show("未能检索到相应的注册表键",
                    "清理地址栏",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
		}

		//删除右键菜单
		private void btDelMenuExt_Click(object sender, System.EventArgs e)
		{
			//询问用户，避免用户误操作删除
			if(MessageBox.Show(
				"确实要删除选中的菜单项吗?",
				"IE管理器",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2)!=DialogResult.OK)
				return;

			RegistryKey key=Registry.CurrentUser.OpenSubKey(KP_MENUEXT,true);
			if(key!=null)
			{
				foreach(ListViewItem item in this.listMenuExt.Items)
				{
					if(item.Checked)
					{
						key.DeleteSubKeyTree(item.Text);
						item.Remove();
					}
				}
				key.Close();
			}
			else
				MessageBox.Show("未能检索到相应的注册表键",
					"清理地址栏",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
		}

		private void btFixDefaultPage_Click(object sender, System.EventArgs e)
		{
			this.tbDefaultPage.Text=DF_DEFAULTPAGEURL;
		}

		private void btBlankPage_Click(object sender, System.EventArgs e)
		{
			this.tbStartPage.Text=DF_BLANKPAGE;
		}

		private void btFixSearchPage_Click(object sender, System.EventArgs e)
		{
			this.tbSearchPage.Text=DF_DEFAULTSEARCHPAGE;
		}

		private void btFixDefaultSearchPage_Click(object sender, System.EventArgs e)
		{
			this.tbDefalutSearchPage.Text=DF_DEFAULTSEARCHPAGE;
		}

		private void btFixWindowTitle_Click(object sender, System.EventArgs e)
		{
			this.tbWindowTitle.Text=DF_WINDOWTITLE;
		}

		/// <summary>
		/// 在第一个页面上的应用按钮的处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btApplyMain_Click(object sender, System.EventArgs e)
		{
			//应用按钮
			RegistryKey keyCU=Registry.CurrentUser.OpenSubKey(KP_MAIN,true);
			if(keyCU!=null)
			{
				keyCU.SetValue(VN_DEFAULTPAGEURL,this.tbDefaultPage.Text);
				keyCU.SetValue(VN_STARTPAGE,this.tbStartPage.Text);
				keyCU.SetValue(VN_SEARCHPAGE,this.tbSearchPage.Text);
				keyCU.SetValue(VN_WINDOWTITLE,this.tbWindowTitle.Text);
				keyCU.Flush();
				keyCU.Close();
			}
			RegistryKey keyLM=Registry.LocalMachine.OpenSubKey(KP_MAIN,true);
			if(keyLM!=null)
			{
				//keyLM.GetValue(VN_IEVERSION,this.tbIeVersion.Text);
				keyLM.SetValue(VN_DEFAULTSEARCHURL,this.tbDefalutSearchPage.Text);
				keyLM.Flush();
				keyLM.Close();
			}
            MessageBox.Show("应用成功!","IE管理器");
		}

		//如果选择的是Main页，则重新加载
		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(this.tabControl1.SelectedIndex==0)
				this.LoadMain();
		}

		/// <summary>
		/// 清空最近的文档
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btClearRecent_Click(object sender, System.EventArgs e)
		{
			string recent=System.Environment.GetFolderPath(Environment.SpecialFolder.Recent);
			string[] files=Directory.GetFiles(recent);
			int count=0;
			if(this.rbAllFiles.Checked)
			{
				for(int i=0;i<files.Length;i++)
					File.Delete(files[i]);
				count=files.Length;
			}
			else if(this.rbMediaFiles.Checked)
				count=this.DeleteLinkFiles(files,this.LINKFILES_MEDIA);
			else if(this.rbOfficeFiles.Checked)
				count=this.DeleteLinkFiles(files,this.LINKFILES_OFFICE);
			
			MessageBox.Show("清除完毕！删除快捷方式数量="+count,"IE管理器",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}
		//////////////////////////////////////////////////////////
	}
}
