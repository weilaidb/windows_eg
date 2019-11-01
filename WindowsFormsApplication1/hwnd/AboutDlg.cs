using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DesktopWndView
{
	/// <summary>
	/// AboutDlg 的摘要说明。
	/// </summary>
	public class AboutDlg : System.Windows.Forms.Form
	{
		//版本和功能说明
		private string[] DescriptionInTextBox=
		{
			"1) 窗口截图窗口增加了滚动条和鼠标滚轮的处理;",
			"2) 添加线程时增加了对线程是否有子节点的判断;",
			"3) 修正了查看进程详细信息中的Bug;",
			"4) 放宽查找窗口时的标题匹配条件;",
			"5) 改进QQ消息尾巴模拟的自动停止功能;",
			"6) 增加IE管理器，可以修正部分IE信息，如地址栏等;",
			"7) 增加扫雷作弊器，建立在老外的研究基础上-《Minesweeper, Behind the scenes》。"
		};
		private System.Windows.Forms.Button btOk;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbDescription;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutDlg()
		{
			InitializeComponent();
			this.LoadDescription();
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
			this.btOk = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btOk
			// 
			this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btOk.Location = new System.Drawing.Point(320, 264);
			this.btOk.Name = "btOk";
			this.btOk.Size = new System.Drawing.Size(80, 23);
			this.btOk.TabIndex = 0;
			this.btOk.Text = "确定";
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Font = new System.Drawing.Font("幼圆", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(408, 40);
			this.label1.TabIndex = 1;
			this.label1.Text = "桌面窗口查看器 1.01";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(400, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "AUTHOR: JINFD   QQ: 17777538";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tbDescription
			// 
			this.tbDescription.BackColor = System.Drawing.SystemColors.InactiveBorder;
			this.tbDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbDescription.Location = new System.Drawing.Point(8, 96);
			this.tbDescription.Multiline = true;
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.ReadOnly = true;
			this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbDescription.Size = new System.Drawing.Size(392, 160);
			this.tbDescription.TabIndex = 3;
			this.tbDescription.Text = "";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(134)));
			this.label3.Location = new System.Drawing.Point(8, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(400, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "功能与版本说明：";
			// 
			// AboutDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(410, 295);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbDescription);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "关于桌面窗口查看器";
			this.ResumeLayout(false);

		}
		#endregion
		#region 内部辅助函数
		private void LoadDescription()
		{
			this.tbDescription.Lines=this.DescriptionInTextBox;
		}
		#endregion
	}
}
