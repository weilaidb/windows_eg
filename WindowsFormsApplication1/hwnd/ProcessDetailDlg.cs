using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace DesktopWndView
{
	/// <summary>
	/// ProcessDetailDlg 的摘要说明。
	/// </summary>
	public class ProcessDetailDlg : System.Windows.Forms.Form
	{
		#region 自定义变量
		private Process m_Process;
		private bool b_CanEnumModules=true;
		//内部包含的进程是否依然在运行
		private bool b_ProcessALive=false;
		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ListView LvModules;
		private System.Windows.Forms.ListView lvProcessInfo;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.ListView lvModuleInfo;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// 构造函数
		/// </summary>
		public ProcessDetailDlg(int PID)
		{
			InitializeComponent();
			this.b_CanEnumModules=(PID>8);
			try
			{
				this.m_Process=Process.GetProcessById(PID);
				this.b_ProcessALive=true;
				//填充Listview
				this.FillProcessInfo();
				this.FillModules();
			}
			catch(Exception e)
			{
				//设置当前进程不在运行的标志
				this.b_ProcessALive=false;
				this.lvProcessInfo.Items.Clear();
				this.LvModules.Items.Clear();
				TraceIgnoredError(e);
			}
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "基址",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "入口地址",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "说明",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "版本",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "公司",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "语言",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "虚拟内存",
																													 ""}, -1);
			this.LvModules = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lvProcessInfo = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.lvModuleInfo = new System.Windows.Forms.ListView();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// LvModules
			// 
			this.LvModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader3,
																						this.columnHeader4});
			this.LvModules.Dock = System.Windows.Forms.DockStyle.Left;
			this.LvModules.FullRowSelect = true;
			this.LvModules.GridLines = true;
			this.LvModules.HideSelection = false;
			this.LvModules.Location = new System.Drawing.Point(3, 17);
			this.LvModules.MultiSelect = false;
			this.LvModules.Name = "LvModules";
			this.LvModules.Size = new System.Drawing.Size(501, 249);
			this.LvModules.TabIndex = 0;
			this.LvModules.View = System.Windows.Forms.View.Details;
			this.LvModules.SelectedIndexChanged += new System.EventHandler(this.LvModules_SelectedIndexChanged);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "模块名称";
			this.columnHeader3.Width = 94;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lvModuleInfo);
			this.groupBox1.Controls.Add(this.splitter2);
			this.groupBox1.Controls.Add(this.LvModules);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(688, 269);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "加载模块";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.lvProcessInfo);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(688, 144);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "进程的详细信息";
			// 
			// lvProcessInfo
			// 
			this.lvProcessInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.columnHeader1,
																							this.columnHeader2});
			this.lvProcessInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvProcessInfo.FullRowSelect = true;
			this.lvProcessInfo.GridLines = true;
			this.lvProcessInfo.Location = new System.Drawing.Point(3, 17);
			this.lvProcessInfo.MultiSelect = false;
			this.lvProcessInfo.Name = "lvProcessInfo";
			this.lvProcessInfo.Size = new System.Drawing.Size(682, 124);
			this.lvProcessInfo.TabIndex = 0;
			this.lvProcessInfo.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "字段名";
			this.columnHeader1.Width = 145;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "值";
			this.columnHeader2.Width = 430;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 144);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(688, 4);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// splitter2
			// 
			this.splitter2.Location = new System.Drawing.Point(504, 17);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(3, 249);
			this.splitter2.TabIndex = 1;
			this.splitter2.TabStop = false;
			// 
			// lvModuleInfo
			// 
			this.lvModuleInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.columnHeader5,
																						   this.columnHeader6});
			this.lvModuleInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvModuleInfo.GridLines = true;
			this.lvModuleInfo.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
																						 listViewItem1,
																						 listViewItem2,
																						 listViewItem3,
																						 listViewItem4,
																						 listViewItem5,
																						 listViewItem6,
																						 listViewItem7});
			this.lvModuleInfo.Location = new System.Drawing.Point(507, 17);
			this.lvModuleInfo.MultiSelect = false;
			this.lvModuleInfo.Name = "lvModuleInfo";
			this.lvModuleInfo.Size = new System.Drawing.Size(178, 249);
			this.lvModuleInfo.TabIndex = 2;
			this.lvModuleInfo.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "模块字段";
			this.columnHeader5.Width = 74;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "值";
			this.columnHeader6.Width = 96;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "路径";
			this.columnHeader4.Width = 395;
			// 
			// ProcessDetailDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(688, 413);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.Name = "ProcessDetailDlg";
			this.ShowInTaskbar = false;
			this.Text = "进程信息";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ProcessDetailDlg_Closing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		//----------------------------------
		//		内部辅助函数
		//----------------------------------
		/// <summary>
		/// 填充进程信息列表
		/// </summary>
		private void FillProcessInfo()
		{
			//注意无论如何要向将该值置为true因为以后其他进程还要填充信息
			this.b_CanEnumModules=true;
			this.lvProcessInfo.Items.Clear();
			int i=0;
			this.lvProcessInfo.Items.Add("进程ID");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.Id.ToString());

			this.lvProcessInfo.Items.Add("进程名称");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.ProcessName);

			this.lvProcessInfo.Items.Add("启动时间");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
			
			this.lvProcessInfo.Items.Add("线程数");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.Threads.Count.ToString());
			
			try
			{
				if(this.m_Process.ProcessName.ToLower()!="idle")
				{
					this.lvProcessInfo.Items.Add("进程句柄");
					this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.Handle.ToString());
				}
				else
				{
					this.lvProcessInfo.Items.Add("进程句柄");
					this.lvProcessInfo.Items[i++].SubItems.Add("无法获取");
					this.b_CanEnumModules=false;
				}
			}
			catch(Exception e1)
			{
				this.lvProcessInfo.Items.Add("进程句柄");
				this.lvProcessInfo.Items[i++].SubItems.Add("无法获取");
				this.b_CanEnumModules=false;
				TraceIgnoredError(e1);
			}
			
			this.lvProcessInfo.Items.Add("句柄数");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.HandleCount.ToString());
			
			this.lvProcessInfo.Items.Add("计算机");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.MachineName);
			
			this.lvProcessInfo.Items.Add("主窗口句柄");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.MainWindowHandle.ToString());
			
			this.lvProcessInfo.Items.Add("主窗口标题");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.MainWindowTitle);
			
			try
			{
				if(this.b_CanEnumModules)
				{
					this.lvProcessInfo.Items.Add("加载模块数");
					this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.Modules.Count.ToString());
				}
				else
				{
					this.lvProcessInfo.Items.Add("加载模块数");
					this.lvProcessInfo.Items[i++].SubItems.Add("无法获取");
				}
			}
			catch(Exception e2)
			{
				
				this.b_CanEnumModules=false;
				this.lvProcessInfo.Items.Add("加载模块数");
				this.lvProcessInfo.Items[i++].SubItems.Add("无法获取");
				TraceIgnoredError(e2);
			}
			
			this.lvProcessInfo.Items.Add("未分页系统内存");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.NonpagedSystemMemorySize.ToString());
			
			this.lvProcessInfo.Items.Add("分页内存");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.PagedMemorySize.ToString());

			this.lvProcessInfo.Items.Add("分页内存峰值");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.PeakPagedMemorySize.ToString());
			
			this.lvProcessInfo.Items.Add("分页系统内存");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.PagedSystemMemorySize.ToString());
			
			this.lvProcessInfo.Items.Add("虚拟内存");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.VirtualMemorySize.ToString());
			
			this.lvProcessInfo.Items.Add("虚拟内存峰值");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.PeakVirtualMemorySize.ToString());
			
			this.lvProcessInfo.Items.Add("用户处理器时间");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.UserProcessorTime.TotalSeconds.ToString()+"秒");
			
			this.lvProcessInfo.Items.Add("总处理器时间");
			this.lvProcessInfo.Items[i++].SubItems.Add(this.m_Process.TotalProcessorTime.TotalSeconds.ToString()+"秒");
		}

		/// <summary>
		/// 填充加载模块列表
		/// </summary>
		private void FillModules()
		{
			this.LvModules.Items.Clear();
			if(!this.b_CanEnumModules)
				return;
			ProcessModule pm;
			int imax=this.m_Process.Modules.Count;;
			for(int i=0;i<imax;i++)
			{
				pm=this.m_Process.Modules[i];
				//模块名称
				this.LvModules.Items.Add(pm.ModuleName);
				this.LvModules.Items[i].SubItems.Add(pm.FileName);
			}
		}

		//模块选择改变时，更新显示当前模块的详细信息
		private void LvModules_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int i=-1;
			if(this.LvModules.SelectedItems.Count==0)
			{
				for(i=0;i<this.lvModuleInfo.Items.Count;i++)
					this.lvModuleInfo.Items[i].SubItems[1].Text=string.Empty;
			}
			else
			{
				ProcessModule pm=this.m_Process.Modules[this.LvModules.SelectedItems[0].Index];
				this.lvModuleInfo.Items[++i].SubItems[1].Text="0x"+Convert.ToString((int)pm.BaseAddress,16);
				this.lvModuleInfo.Items[++i].SubItems[1].Text="0x"+Convert.ToString((int)pm.EntryPointAddress,16);
				this.lvModuleInfo.Items[++i].SubItems[1].Text=pm.FileVersionInfo.Comments;
				this.lvModuleInfo.Items[++i].SubItems[1].Text=pm.FileVersionInfo.FileVersion;
				this.lvModuleInfo.Items[++i].SubItems[1].Text=pm.FileVersionInfo.CompanyName;
				this.lvModuleInfo.Items[++i].SubItems[1].Text=pm.FileVersionInfo.Language;
				this.lvModuleInfo.Items[++i].SubItems[1].Text=pm.ModuleMemorySize.ToString();
			}
		}


		/// <summary>
		/// Writes information about any ignored exception to the trace.
		/// 忽略异常！
		/// </summary>
		/// <param name="e">The exception which is being ignored</param>
		private void TraceIgnoredError(Exception e)
		{
			//It's ok if there is any error
			System.Diagnostics.Trace.WriteLine(e.Message);
		}

		/// <summary>
		/// 拦截关闭窗口的消息，只是隐藏！
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProcessDetailDlg_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Hide();
			e.Cancel=true;
		}

		//----------------------------------
		//		外部接口
		//----------------------------------
		public int PROCESSID
		{
			get
			{
				return this.m_Process.Id;
			}
			set
			{
				if(this.m_Process.Id!=value)
				{
					try
					{
						this.m_Process=Process.GetProcessById(value);
						this.b_ProcessALive=true;
						this.FillProcessInfo();
						this.FillModules();
					}
					catch(Exception e)
					{
						this.b_ProcessALive=false;
						this.lvProcessInfo.Items.Clear();
						this.LvModules.Items.Clear();
						TraceIgnoredError(e);
					}
				}
			}
		}

		//从外部获取和设置进程
		public Process PROCESS
		{
			get
			{
				return this.m_Process;
			}
			set
			{
				this.m_Process=value;
				this.FillProcessInfo();
				this.FillModules();
			}
		}


		//获取进程是否在运行
		public bool PROCESSALIVE
		{
			get{return this.b_ProcessALive;}
		}

		//从外部显示关闭窗体的方法
		public void ENDDIALOG()
		{
			this.Close();
		}

	}
}
