using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace DesktopWndView
{
	/// <summary>
	/// ProcessSelDlg ��ժҪ˵����
	/// </summary>
	public class ProcessSelDlg : System.Windows.Forms.Form
	{
		#region �Զ������
		private Process[] m_Processes;
		#endregion
		private System.Windows.Forms.Button btOk;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.ListView listViewProcesses;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProcessSelDlg()
		{
			InitializeComponent();
			this.m_Processes=Process.GetProcesses();
			this.FillListView(this.m_Processes);
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
			this.btOk = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.listViewProcesses = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btOk
			// 
			this.btOk.Location = new System.Drawing.Point(280, 8);
			this.btOk.Name = "btOk";
			this.btOk.TabIndex = 1;
			this.btOk.Text = "ȷ��";
			this.btOk.Click += new System.EventHandler(this.btOk_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(280, 40);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 2;
			this.btCancel.Text = "ȡ��";
			// 
			// listViewProcesses
			// 
			this.listViewProcesses.AutoArrange = false;
			this.listViewProcesses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								this.columnHeader1,
																								this.columnHeader2});
			this.listViewProcesses.FullRowSelect = true;
			this.listViewProcesses.HideSelection = false;
			this.listViewProcesses.Location = new System.Drawing.Point(8, 8);
			this.listViewProcesses.MultiSelect = false;
			this.listViewProcesses.Name = "listViewProcesses";
			this.listViewProcesses.Size = new System.Drawing.Size(256, 296);
			this.listViewProcesses.TabIndex = 3;
			this.listViewProcesses.View = System.Windows.Forms.View.Details;
			this.listViewProcesses.DoubleClick += new System.EventHandler(this.listViewProcesses_DoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "��������";
			this.columnHeader1.Width = 120;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "PID";
			this.columnHeader2.Width = 77;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(280, 88);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 208);
			this.label1.TabIndex = 4;
			this.label1.Text = "����˫��ĳһ�����ѡ��";
			// 
			// ProcessSelDlg
			// 
			this.AcceptButton = this.btOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.btCancel;
			this.ClientSize = new System.Drawing.Size(362, 314);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listViewProcesses);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProcessSelDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ѡ��һ������";
			this.ResumeLayout(false);

		}
		#endregion
		//--------------------------------------------
		//	�ڲ���������
		//--------------------------------------------
		private void FillListView(Process[] pro)
		{
			for(int i=0;i<pro.Length;i++)
			{
				this.listViewProcesses.Items.Add(pro[i].ProcessName);
				this.listViewProcesses.Items[i].SubItems.Add(pro[i].Id.ToString());
			}
		}

		private void btOk_Click(object sender, System.EventArgs e)
		{
			if(this.listViewProcesses.SelectedItems.Count==0)
			{
				MessageBox.Show("����ѡ��һ�����̣�",
					"ѡ�����",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
				return;
			}
			else
				this.DialogResult=DialogResult.OK;
		}

		/// <summary>
		/// ˫��ĳ��ѡ��ʱ������ok��ť
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listViewProcesses_DoubleClick(object sender, System.EventArgs e)
		{
			if(this.listViewProcesses.SelectedItems.Count==1)
				this.btOk.PerformClick();
		}
		
		//---------------------------------------------
		//	�ⲿ�ӿ�
		//---------------------------------------------
		public Process SelectedProcess
		{
			get
			{
				if(this.listViewProcesses.SelectedItems.Count==0)
					return null;
				else
					return this.m_Processes[this.listViewProcesses.SelectedItems[0].Index];
			}
		}

		public string SelectedProcessName
		{
			get
			{
				if(this.listViewProcesses.SelectedItems.Count==0)
					return string.Empty;
				else
					return this.listViewProcesses.SelectedItems[0].Text;
			}
		}

		public int SelectedProcessId
		{
			get
			{
				if(this.listViewProcesses.SelectedItems.Count==0)
					return 0;
				else
					return this.m_Processes[this.listViewProcesses.SelectedItems[0].Index].Id;
			}
		}

	}
}
