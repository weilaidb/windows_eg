using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Globalization;		//for NumberFormatInfo

namespace DesktopWndView
{
	#region ----STRUCTs （"winnt.h"中定义）----
	//dos exe header
	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_DOS_HEADER
	{
		public ushort e_magic;		//00 标志字
		public ushort e_cblp;		//02 Bytes on last page of file
		public ushort e_cp;			//04 Pages in file
		public ushort e_crlc;		//06 Relocations 重定向
		public ushort e_cparhdr;	//08 Size of header in paragraphs
		public ushort e_minalloc;	//0A Minimum extra paragraphs needed
		public ushort e_maxalloc;	//0C Maximum extra paragraphs needed
		public ushort e_ss;			//0E Initial (relative) SS value （ss初始偏移量）
		public ushort e_sp;			//10 Initial SP value
		public ushort e_csum;		//12 Checksum
		public ushort e_ip;			//14 Initial IP value
		public ushort e_cs;			//16 Initial (relative) CS value
		public ushort e_lfarlc;		//18 File address of relocation table
		public ushort e_ovno;		//1A Overlay number		
		[MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
		public ushort[] e_res;		//1C Reserved words （系统暂时保留）		
		public ushort e_oemid;		//24 OEM identifier (for e_oeminfo)
		public ushort e_oeminfo;	//26 OEM information; e_oemid specific
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=10)]
		public ushort[] e_res2;		//28 Reserved words （系统暂时保留）		
		public uint e_lfanew;		//3C File address of new exe header（指向PE Header）
	}


	//PE Header(IMAGE_NT_HEADERS) 的第二部分，（第一部分是 P E \0 \0）
	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_FILE_HEADER
	{
		public ushort Machine;				//所需CPU，对于Intel是14Ch
		public ushort NumberOfSections;		//PE文件的节数（非常重要）,数量较少，通常为3~5个左右
		public int TimeDateStamp;			//时间戳（从1969-10-31 4:00～创建文件时的秒数）
        public uint PointerToSymbolTable;	//用于调试
		public uint NumberOfSymbols;			//被obj文件使用
		public ushort SizeOfOptionalHeader;	//OptionalHeader的字节数
		public ushort Characteristics;		//文件信息，区分exe或dll
	}


	//Data Directory 数据字典
	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_DATA_DIRECTORY
	{
		public uint VirtualAddress;
		public uint Size;
	}

	//PE Header(IMAGE_NT_HEADERS) 的第三部分
	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_OPTIONAL_HEADER32
	{
		//--------------------------标准域--------------------------------
		public ushort Magic;				//标志，总为010Bh
		public byte MajorLinkerVersion;
		public byte MinorLinkerVersion;
		public uint SizeOfCode;
		public uint SizeOfInitializedData;
		public uint SizeOfUninitializedData;
		public uint AddressOfEntryPoint;		//重要，指令入口点的RVA（RAV：内存映射地址，偏移量）
		public uint BaseOfCode;				//code section的RVA
		public uint BaseOfData;				//data section的RAV

		//--------------------------NT附加域-------------------------------
		public uint ImageBase;
		public uint SectionAlignment;
		public uint FileAlignment;
		public ushort MajorOperatingSystemVersion;	//缺省为1.0
		public ushort MinorOperatingSystemVersion;
		public ushort MajorImageVersion;			//用户可定义的域，允许不同版本的exe/dll
		public ushort MinorImageVersion;
		public ushort MajorSubsystemVersion;		//最低操作系统要求，例如3.10
		public ushort MinorSubsystemVersion;
		public uint Win32VersionValue;
		public uint SizeOfImage;
		public uint SizeOfHeaders;
		public uint CheckSum;					//CRC校验，忽略，为0
		public ushort Subsystem;
		public ushort DllCharacteristics;
		public uint SizeOfStackReserve;			//保留的堆栈大小
		public uint SizeOfStackCommit;
		public uint SizeOfHeapReserve;
		public uint SizeOfHeapCommit;
		public uint LoaderFlags;				//调试有关
        public uint NumberOfRvaAndSizes;		//DataDirectory数组入口个数，总为16
		[MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
		public IMAGE_DATA_DIRECTORY[] DataDirectory;
	}

	//section table 节表的元素
	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_SECTION_HEADER
	{
		[MarshalAs(UnmanagedType.ByValArray,SizeConst=8)]
		public byte[] Name;			//8个字节组成的短名称
		//非托管定义中是union类型，(DWORD)PhysicalAddress (DWORD)VirtualSize，这里选取后者
		public uint VirtualSize;
		public uint VirtualAddress;
		public uint SizeOfRawData;
		public uint PointerToRawData;
		public uint PointerToRelocations;
		public uint PointerToLinenumbers;
		public ushort NumberOfRelocations;
		public ushort NumberOfLinenumbers;
		public uint Characteristics;
	}


	//NT Header
	[StructLayout(LayoutKind.Sequential)]
	public struct IMAGE_NT_HEADERS32
	{
		public uint Signature;							//第一部分，PE签名：PE00
		public IMAGE_FILE_HEADER FileHeader;			//PE文件头
		public IMAGE_OPTIONAL_HEADER32 OptionalHeader;	//可选头
	}
	#endregion

	/// <summary>
	/// 用于查看PE文件结构的一个视图窗口
	/// </summary>
	public class PEViewDlg : System.Windows.Forms.Form
	{

		#region "winnt.h"中定义的常量
		public const ushort IMAGE_DOS_SIGNATURE=0x5A4D;		//ME
		public const uint IMAGE_NT_SIGNATURE=0x00004550;	//PE00
		#endregion

		#region 自定义变量
		//ImageIndex const
		private const int IMAGE_INDEX_BYTES2=0;
		private const int IMAGE_INDEX_BYTES4=1;
		private const int IMAGE_INDEX_BYTES1=2;
		private const int IMAGE_INDEX_BYTES8=3;
		private const int IMAGE_INDEX_BYTES_UNKNOW=4;
		private const int IMAGE_INDEX_BOOK_CLOSE=5;
		private const int IMAGE_INDEX_BOOK_OPEN=6;

		private BinaryReader m_Reader;					//File Reader
		private IMAGE_DOS_HEADER m_DosHeader;			//DOS Header 64bytes
		private IMAGE_NT_HEADERS32 m_NtHeaders;			//NT Headers 248bytes
		private IMAGE_SECTION_HEADER m_SectionHeader;	//Section Header 40bytes
		//文件时间的秒数计时起始点（1969.10.31 4:00)
		private DateTime m_FileTimeBase=new DateTime(1969,10,31,4,0,0,0);
		//DataDirectory数组的说明(数据目录的说明, 内容依据winnt.h中的定义和注释)
		private string[] m_DataDirComments=new string[16]
			{
				"entry_export: Export Directory导出表",
				"entry_import: Import Directory导入表",
				"entry_resource: Resource Directory",
				"entry_exception: Exception Directory",
				"entry_security: Security Directory",
				"entry_basereloc: Base Relocation Table",
				"entry_debug: Debug Directory",
				"entry_architecture: Architecture Specific Data",
				"entry_globalptr: RVA of GP",
				"entry_tls: TLS Directory",
				"entry_load_config: Load Configuration Directory",
				"entry_bound_import: Bound Import Directory in headers",
				"entry_iat: Import Address Table",
				"delay_import: Delay Load Import Descriptors",
				"com_descriptor: COM Runtime descriptor",
				"MS对此无定义和说明"
			};
		#endregion

		#region 窗体设计器变量
		private System.Windows.Forms.TreeView tvPEView;
		private System.Windows.Forms.Button btOpenPE;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelInfo;

		#endregion
		private System.Windows.Forms.ImageList imagesOfNode;
		private System.ComponentModel.IContainer components;

		//构造函数
		public PEViewDlg()
		{
			//初始化UI界面
			InitializeComponent();

			//初始化所有的内部结构 (struct是值类型，不占用堆上的空间）
			this.m_DosHeader=new IMAGE_DOS_HEADER();
			this.m_DosHeader.e_res=new ushort[4];	//数组是引用类型，因此还需要为数组在堆上申请空间！
			this.m_DosHeader.e_res2=new ushort[10];

			this.m_NtHeaders=new IMAGE_NT_HEADERS32();
            this.m_NtHeaders.OptionalHeader.DataDirectory=new IMAGE_DATA_DIRECTORY[16];

			this.m_SectionHeader=new IMAGE_SECTION_HEADER();	//byte[]成员不必初始化，因为读取数组时会返回一个对象

			//设置labelInfo的内容
			this.labelInfo.Text=string.Format("{0}{1}{2}{3}{4}{5}\r\n{6}{7}{8}{9}{10}{11}{12}{13}{14}\r\n{15}",
				"◆IMAGE_OPTIONAL_HEADER.Subsystem: Runs in which subsystem\r\n",		//index=0
				"  1: NATIVE - Doesn't require a subsystem (device driver,eg.)\r\n",
				"  2: WINDOWS_GUI - Windows GUI subsystem\r\n",
				"  3: WINDOWS_CUI - Windows character subsystem (console app)\r\n",
				"  5: OS2_CUI - OS/2 character subsystem (OS/2 1.x only)\r\n",
				"  7: POSIX_CUI - Posix character subsystem\r\n",

				"◆IMAGE_SECTION_HEADER.Characteristics:\r\n",		//index=6
				"  0000 0020h: 代码段\r\n",
				"  0000 0040h: 已初始化数据段\r\n",
				"  0000 0080h: 未初始化数据段\r\n",
				"  0200 0000h: 可丢弃段\r\n",
				"  1000 0000h: 共享段\r\n",
				"  2000 0000h: 可执行段\r\n",
				"  4000 0000h: 可读段\r\n",
				"  8000 0000h: 可写段\r\n",

				"◆节点上所有的size均为RawDataSize(在文件中的物理大小)"		//index=15
				);
				
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PEViewDlg));
			this.tvPEView = new System.Windows.Forms.TreeView();
			this.imagesOfNode = new System.Windows.Forms.ImageList(this.components);
			this.btOpenPE = new System.Windows.Forms.Button();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelInfo = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvPEView
			// 
			this.tvPEView.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvPEView.ImageList = this.imagesOfNode;
			this.tvPEView.Location = new System.Drawing.Point(0, 0);
			this.tvPEView.Name = "tvPEView";
			this.tvPEView.Size = new System.Drawing.Size(384, 461);
			this.tvPEView.TabIndex = 0;
			// 
			// imagesOfNode
			// 
			this.imagesOfNode.ImageSize = new System.Drawing.Size(16, 16);
			this.imagesOfNode.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagesOfNode.ImageStream")));
			this.imagesOfNode.TransparentColor = System.Drawing.Color.Magenta;
			// 
			// btOpenPE
			// 
			this.btOpenPE.Location = new System.Drawing.Point(16, 16);
			this.btOpenPE.Name = "btOpenPE";
			this.btOpenPE.Size = new System.Drawing.Size(112, 23);
			this.btOpenPE.TabIndex = 1;
			this.btOpenPE.Text = "打开PE文件";
			this.btOpenPE.Click += new System.EventHandler(this.btOpenPE_Click);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(384, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 461);
			this.splitter1.TabIndex = 2;
			this.splitter1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.labelInfo);
			this.panel1.Controls.Add(this.btOpenPE);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(387, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(437, 461);
			this.panel1.TabIndex = 3;
			// 
			// labelInfo
			// 
			this.labelInfo.BackColor = System.Drawing.SystemColors.Info;
			this.labelInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelInfo.Location = new System.Drawing.Point(16, 56);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(400, 320);
			this.labelInfo.TabIndex = 2;
			// 
			// PEViewDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(824, 461);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.tvPEView);
			this.Name = "PEViewDlg";
			this.Text = "PE文件头查看器";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region 内部辅助方法
		//------------------------------
		//注意在调用这些方法前，m_Reader应该已经初始化过！！！读取结果填充到参数指定的结构

		/// <summary>
		/// 读取DOS Header
		/// </summary>
		private void ReadDosHeader()
		{
			//文件指针移动到文件起始处（00H）
			this.m_Reader.BaseStream.Seek(0,SeekOrigin.Begin);
			//00h～3Fh，共64个字节,开始填充成员
			this.m_DosHeader.e_magic=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_cblp=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_cp=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_crlc=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_cparhdr=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_minalloc=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_maxalloc=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_ss=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_sp=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_csum=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_ip=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_cs=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_lfarlc=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_ovno=this.m_Reader.ReadUInt16();
			for(int i=0;i<4;i++)
				this.m_DosHeader.e_res[i]=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_oemid=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_oeminfo=this.m_Reader.ReadUInt16();
			for(int i=0;i<10;i++)
				this.m_DosHeader.e_res2[i]=this.m_Reader.ReadUInt16();
			this.m_DosHeader.e_lfanew=this.m_Reader.ReadUInt32();
		}

		/// <summary>
		/// 读取所有的NT头（共三部分）
		/// </summary>
		/// <param name="NtHeaderAddress"></param>
		private void ReadNtHeaders(uint NtHeaderAddress)
		{
			//文件指针移动到Nt Header的起始处
			this.m_Reader.BaseStream.Seek(NtHeaderAddress,SeekOrigin.Begin);
			//读取PE文件签名（PE00）
			this.m_NtHeaders.Signature=this.m_Reader.ReadUInt32();

			//开始读取IMAGE_FILE_HEADER
			this.m_NtHeaders.FileHeader.Machine=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.FileHeader.NumberOfSections=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.FileHeader.TimeDateStamp=this.m_Reader.ReadInt32();
			this.m_NtHeaders.FileHeader.PointerToSymbolTable=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.FileHeader.NumberOfSymbols=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.FileHeader.SizeOfOptionalHeader=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.FileHeader.Characteristics=this.m_Reader.ReadUInt16();

			//开始读取IMAGE_OPTIONAL_HEADER
			this.m_NtHeaders.OptionalHeader.Magic=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.MajorLinkerVersion=this.m_Reader.ReadByte();
			this.m_NtHeaders.OptionalHeader.MinorLinkerVersion=this.m_Reader.ReadByte();
			this.m_NtHeaders.OptionalHeader.SizeOfCode=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.SizeOfInitializedData=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.SizeOfUninitializedData=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.AddressOfEntryPoint=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.BaseOfCode=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.BaseOfData=this.m_Reader.ReadUInt32();

			this.m_NtHeaders.OptionalHeader.ImageBase=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.SectionAlignment=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.FileAlignment=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.MajorOperatingSystemVersion=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.MinorOperatingSystemVersion=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.MajorImageVersion=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.MinorImageVersion=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.MajorSubsystemVersion=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.MinorSubsystemVersion=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.Win32VersionValue=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.SizeOfImage=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.SizeOfHeaders=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.CheckSum=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.Subsystem=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.DllCharacteristics=this.m_Reader.ReadUInt16();
			this.m_NtHeaders.OptionalHeader.SizeOfStackReserve=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.SizeOfStackCommit=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.SizeOfHeapReserve=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.SizeOfHeapCommit=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.LoaderFlags=this.m_Reader.ReadUInt32();
			this.m_NtHeaders.OptionalHeader.NumberOfRvaAndSizes=this.m_Reader.ReadUInt32();
			for(int i=0;i<this.m_NtHeaders.OptionalHeader.NumberOfRvaAndSizes;i++)
			{
				//读取DataDirectory数组（16个）
				this.m_NtHeaders.OptionalHeader.DataDirectory[i].VirtualAddress=this.m_Reader.ReadUInt32();
				this.m_NtHeaders.OptionalHeader.DataDirectory[i].Size=this.m_Reader.ReadUInt32();
			}
		}

		/// <summary>
		/// 读取SectionHeader (40 bytes)并填充到内部结构，注意函数中不会调整文件指针位置，
		/// 因此Reader的文件指针必须处于正确位置
		/// </summary>
		private void ReadSectionHeader()
		{
			//读取8个字符，是section name
            this.m_SectionHeader.Name=this.m_Reader.ReadBytes(8);
			this.m_SectionHeader.VirtualSize=this.m_Reader.ReadUInt32();
			this.m_SectionHeader.VirtualAddress=this.m_Reader.ReadUInt32();
			this.m_SectionHeader.SizeOfRawData=this.m_Reader.ReadUInt32();
			this.m_SectionHeader.PointerToRawData=this.m_Reader.ReadUInt32();
			this.m_SectionHeader.PointerToRelocations=this.m_Reader.ReadUInt32();
			this.m_SectionHeader.PointerToLinenumbers=this.m_Reader.ReadUInt32();
			this.m_SectionHeader.NumberOfRelocations=this.m_Reader.ReadUInt16();
			this.m_SectionHeader.NumberOfLinenumbers=this.m_Reader.ReadUInt16();
			this.m_SectionHeader.Characteristics=this.m_Reader.ReadUInt32();
		}

		/// <summary>
		/// 读取PE文件并填充内部结构，然后把更新的结构数据装配到TreeView控件
		/// </summary>
		/// <param name="IsPE">是否是PE文件</param>
		private bool FillTreeView(out string errorMsg)
		{
			//读取Dos Header
			this.ReadDosHeader();
			if(this.m_DosHeader.e_magic!=PEViewDlg.IMAGE_DOS_SIGNATURE)
			{
				errorMsg="不是有效的可执行文件！Dos Header不符合MZ签名!";
				return false;
			}
			//读取Nt Header
			this.ReadNtHeaders(this.m_DosHeader.e_lfanew);

			//清空节点
			this.tvPEView.Nodes.Clear();

			//声明临时变量
			TreeNode node,node_2,new_node;	//node为被添加子的节点，node_2为辅助节点
			string sectionName;		//节（段）名称
			int indexOfNull;		//段名中的\0所在索引

			//装配Dos Header节点-----------------------------------------------------------------
			this.tvPEView.Nodes.Add("◆IMAGE_DOS_HEADER (size=40h,64bytes)");
			node=this.tvPEView.Nodes[0];
            node.Nodes.Add(string.Format("e_magic:\t0x{0}\t\"Magic Number (MZ)\"",this.m_DosHeader.e_magic.ToString("X").PadLeft(4,'0')));	//index=0
			node.Nodes.Add(string.Format("e_cblp:\t0x{0}\t\"bytes on last page\"",this.m_DosHeader.e_cblp.ToString("X").PadLeft(4,'0')));		
			node.Nodes.Add(string.Format("e_cp:\t0x{0}\t\"pages in file\"",this.m_DosHeader.e_cp.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_crlc:\t0x{0}\t\"Relocations\"",this.m_DosHeader.e_crlc.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_cparhdr:\t0x{0}\t\"size of header in paragraph\"",this.m_DosHeader.e_cparhdr.ToString("X").PadLeft(4,'0')));
            node.Nodes.Add(string.Format("e_minalloc:\t0x{0}\t\"Minimum extra paragraphs needed\"",this.m_DosHeader.e_minalloc.ToString("X").PadLeft(4,'0')));	//5
			node.Nodes.Add(string.Format("e_maxalloc:\t0x{0}\t\"Maximum extra paragraphs needed\"",this.m_DosHeader.e_maxalloc.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_ss:\t0x{0}\t\"Initial (relative) SS value\"",this.m_DosHeader.e_ss.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_sp:\t0x{0}\t\"Initial SP value\"",this.m_DosHeader.e_sp.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_csum:\t0x{0}\t\"checksum\"",this.m_DosHeader.e_csum.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_ip:\t0x{0}\t\"Initial IP value\"",this.m_DosHeader.e_ip.ToString("X").PadLeft(4,'0')));		//10
			node.Nodes.Add(string.Format("e_cs:\t0x{0}\t\"Initial (relative) CS value\"",this.m_DosHeader.e_cs.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_lfarlc:\t0x{0}\t\"File address of relocation table\"",this.m_DosHeader.e_lfarlc.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_ovno:\t0x{0}\t\"Overlay number\"",this.m_DosHeader.e_ovno.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add("e_res[4]:\t\"Reserved words\"");		//index=14
            node.Nodes.Add(string.Format("e_oemid:\t0x{0}\t\"OEM identifier (for e_oeminfo)\"",this.m_DosHeader.e_oemid.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("e_oeminfo:\t0x{0}\t\"OEM information; e_oemid specific\"",this.m_DosHeader.e_oeminfo.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add("e_res2[10]:\t\"Reserved words\"");	//index=17
            node.Nodes.Add(string.Format("e_lfanew:\t0x{0}\t\"File address of Nt_Headers\"",this.m_DosHeader.e_lfanew.ToString("X").PadLeft(8,'0')));
			node.Nodes[18].ImageIndex=node.Nodes[18].SelectedImageIndex=IMAGE_INDEX_BYTES4;
			//添加保留数组1
			node=this.tvPEView.Nodes[0].Nodes[14];
			for(int i=0;i<4;i++)
				node.Nodes.Add(string.Format("e_res[{0}]: 0x{1}",i,Convert.ToString(this.m_DosHeader.e_res[i],16).PadLeft(4,'0')));
			//添加保留数组2
			node=this.tvPEView.Nodes[0].Nodes[17];
			for(int i=0;i<10;i++)
				node.Nodes.Add(string.Format("e_res2[{0}]: 0x{1}",i,Convert.ToString(this.m_DosHeader.e_res2[i],16).PadLeft(4,'0')));

			//添加DOS Stub（Dos执行体）index=1----------------------------------------
			this.tvPEView.Nodes.Add("◆DOS STUB");
            

			//装配Nt Header节点, index=2----------------------------------------------
			if(this.m_NtHeaders.Signature!=PEViewDlg.IMAGE_NT_SIGNATURE)
			{
				errorMsg="该文件不是PE格式！Dos Header符合MZ签名，但Nt Header不符合PE00签名！";
				return false;
			}
			this.tvPEView.Nodes.Add("◆IMAGE_NT_HEADERS (size=F8h,248bytes)");
			node=this.tvPEView.Nodes[2];
			node.Nodes.Add(string.Format("●Signature:\t0x{0}\t\"PE00\" (size=04h,4bytes)",this.m_NtHeaders.Signature.ToString("X").PadLeft(8,'0')));
			node.Nodes[0].ImageIndex=node.Nodes[0].SelectedImageIndex=IMAGE_INDEX_BYTES4;
			node.Nodes.Add("●IMAGE_FILE_HEADER (size=14h,20bytes)");			//index=1
			node.Nodes.Add("●IMAGE_OPTIONAL_HEADER32 (size=E0h,224bytes)");	//index=2

			//添加IMAGE_FILE_HEADER （第二部分）index=1--------------------------------
			node=this.tvPEView.Nodes[2].Nodes[1];
			node.Nodes.Add(string.Format("Machine:\t0x{0}\t\"CPU Needed (=14Ch for Intel 386)\"",
				this.m_NtHeaders.FileHeader.Machine.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("NumberOfSections:\t0x{0}",
				this.m_NtHeaders.FileHeader.NumberOfSections.ToString("X").PadLeft(4,'0')));

			node.Nodes.Add(string.Format("TimeDateStamp:\t0x{0}\t\"{1} (seconds from 1969.10.31 4:00)\"",
				this.m_NtHeaders.FileHeader.TimeDateStamp.ToString("X").PadLeft(8,'0'),
				this.GetFileTime(this.m_NtHeaders.FileHeader.TimeDateStamp)));
			node.Nodes[2].ImageIndex=node.Nodes[2].SelectedImageIndex=IMAGE_INDEX_BYTES4;		//设置图标

			node.Nodes.Add(string.Format("PointerToSymbolTable:\t0x{0}\t\"debug used\"",
				this.m_NtHeaders.FileHeader.PointerToSymbolTable.ToString("X").PadLeft(8,'0')));
			node.Nodes[3].ImageIndex=node.Nodes[3].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("NumberOfSymbols:\t0x{0}\t\"OBJ File used\"",
				this.m_NtHeaders.FileHeader.NumberOfSymbols.ToString("X").PadLeft(8,'0')));
			node.Nodes[4].ImageIndex=node.Nodes[4].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SizeOfOptionalHeader:\t0x{0}\t\"size of IMAGE_OPTIONAL_HEADER in bytes\"",
				this.m_NtHeaders.FileHeader.SizeOfOptionalHeader.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("Characteristics:\t0x{0}\t\"is exe or dll\"",
				this.m_NtHeaders.FileHeader.Characteristics.ToString("X").PadLeft(4,'0')));

			//添加IMAGE_OPTIONAL_HEADER (第三部分) index=2-----------------------------------
			node=this.tvPEView.Nodes[2].Nodes[2];
			node.Nodes.Add(string.Format("Magic:\t0x{0}\t\"Signature,is 010Bh\"",
				this.m_NtHeaders.OptionalHeader.Magic.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("MajorLinkerVersion:\t0x{0}",
				this.m_NtHeaders.OptionalHeader.MajorLinkerVersion.ToString("X").PadLeft(2,'0')));
			node.Nodes[1].ImageIndex=node.Nodes[1].SelectedImageIndex=IMAGE_INDEX_BYTES1;

			node.Nodes.Add(string.Format("MinorLinkerVersion:\t0x{0}",
				this.m_NtHeaders.OptionalHeader.MinorLinkerVersion.ToString("X").PadLeft(2,'0')));
			node.Nodes[2].ImageIndex=node.Nodes[2].SelectedImageIndex=IMAGE_INDEX_BYTES1;

			node.Nodes.Add(string.Format("SizeOfCode:\t0x{0}",
				this.m_NtHeaders.OptionalHeader.SizeOfCode.ToString("X").PadLeft(8,'0')));
			node.Nodes[3].ImageIndex=node.Nodes[3].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SizeOfInitializedData:\t0x{0}",
				this.m_NtHeaders.OptionalHeader.SizeOfInitializedData.ToString("X").PadLeft(8,'0')));
			node.Nodes[4].ImageIndex=node.Nodes[4].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SizeOfUninitializedData:\t0x{0}",
				this.m_NtHeaders.OptionalHeader.SizeOfUninitializedData.ToString("X").PadLeft(8,'0')));
			node.Nodes[5].ImageIndex=node.Nodes[5].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("AddressOfEntryPoint:\t0x{0}\t\"程序入口RAV地址，和ImageBase有关\"",
				this.m_NtHeaders.OptionalHeader.AddressOfEntryPoint.ToString("X").PadLeft(8,'0')));
			node.Nodes[6].ImageIndex=node.Nodes[6].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("BaseOfCode:\t0x{0}\t\"代码段起始RAV\"",
				this.m_NtHeaders.OptionalHeader.BaseOfCode.ToString("X").PadLeft(8,'0')));
			node.Nodes[7].ImageIndex=node.Nodes[7].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("BaseOfData:\t0x{0}\t\"数据段起始RAV\"",
				this.m_NtHeaders.OptionalHeader.BaseOfData.ToString("X").PadLeft(8,'0')));		//index=8
			node.Nodes[8].ImageIndex=node.Nodes[8].SelectedImageIndex=IMAGE_INDEX_BYTES4;
            
			node.Nodes.Add(string.Format("ImageBase:\t0x{0}\"基址\"",
				this.m_NtHeaders.OptionalHeader.ImageBase.ToString("X").PadLeft(8,'0')));
			node.Nodes[9].ImageIndex=node.Nodes[9].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SectionAlignment:\t0x{0}\t\"节对齐，缺省值=0x1000,每节基址是其整数倍\"",
				this.m_NtHeaders.OptionalHeader.SectionAlignment.ToString("X").PadLeft(8,'0')));
			node.Nodes[10].ImageIndex=node.Nodes[10].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("FileAlignment:\t0x{0}\t\"文件中节对齐，缺省值=0x200\"",
				this.m_NtHeaders.OptionalHeader.SectionAlignment.ToString("X").PadLeft(8,'0')));
			node.Nodes[11].ImageIndex=node.Nodes[11].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("MajorOperatingSystemVersion:\t0x{0}\t\"操作系统版本，意义不大，缺省1.0\"",
				this.m_NtHeaders.OptionalHeader.MajorOperatingSystemVersion.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("MinorOperatingSystemVersion:\t0x{0}",
				this.m_NtHeaders.OptionalHeader.MinorOperatingSystemVersion.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("MajorImageVersion:\t0x{0}\t\"用户程序版本，用户可定义\"",
				this.m_NtHeaders.OptionalHeader.MajorImageVersion.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("MinorImageVersion:\t0x{0}",
				this.m_NtHeaders.OptionalHeader.MinorImageVersion.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("MajorSubsystemVersion:\t0x{0}\t\"子系统版本\"",
				this.m_NtHeaders.OptionalHeader.MajorSubsystemVersion.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("MinorSubsystemVersion:\t0x{0}",
				this.m_NtHeaders.OptionalHeader.MinorSubsystemVersion.ToString("X").PadLeft(4,'0')));

			node.Nodes.Add(string.Format("Win32VersionValue:\t0x{0}\t\"保留\"",
				this.m_NtHeaders.OptionalHeader.Win32VersionValue.ToString("X").PadLeft(8,'0')));
			node.Nodes[18].ImageIndex=node.Nodes[18].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SizeOfImage:\t0x{0}\t\"映象大小\"",
				this.m_NtHeaders.OptionalHeader.SizeOfImage.ToString("X").PadLeft(8,'0')));
			node.Nodes[19].ImageIndex=node.Nodes[19].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SizeOfHeaders:\t0x{0}\t\"块前头部大小\"",
				this.m_NtHeaders.OptionalHeader.SizeOfHeaders.ToString("X").PadLeft(8,'0')));
			node.Nodes[20].ImageIndex=node.Nodes[20].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("CheckSum:\t0x{0}\t\"忽略，=0\"",
				this.m_NtHeaders.OptionalHeader.CheckSum.ToString("X").PadLeft(8,'0')));
			node.Nodes[21].ImageIndex=node.Nodes[21].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("Subsystem:\t0x{0}\t\"用于生成用户界面的子系统\"",
				this.m_NtHeaders.OptionalHeader.Subsystem.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("DllCharacteristics:\t0x{0}\t\"文件为dll时使用,何种情况下调用DllMain，但系统未用（总是为0）\"",
				this.m_NtHeaders.OptionalHeader.DllCharacteristics.ToString("X").PadLeft(4,'0')));
			node.Nodes.Add(string.Format("SizeOfStackReserve:\t0x{0}\t\"保留栈大小,缺省值=0x100000(1MB)\"",
				this.m_NtHeaders.OptionalHeader.SizeOfStackReserve.ToString("X").PadLeft(8,'0')));
			node.Nodes[24].ImageIndex=node.Nodes[24].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SizeOfStackCommit:\t0x{0}\t\"使用栈大小,初始值=0x1000(1页)\"",
				this.m_NtHeaders.OptionalHeader.SizeOfStackCommit.ToString("X").PadLeft(8,'0')));
			node.Nodes[25].ImageIndex=node.Nodes[25].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SizeOfHeapReserve:\t0x{0}\t\"保留堆大小\"",
				this.m_NtHeaders.OptionalHeader.SizeOfHeapReserve.ToString("X").PadLeft(8,'0')));
			node.Nodes[26].ImageIndex=node.Nodes[26].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("SizeOfHeapCommit:\t0x{0}\t\"使用堆大小,缺省值=1页。\"",
				this.m_NtHeaders.OptionalHeader.SizeOfHeapCommit.ToString("X").PadLeft(8,'0')));
			node.Nodes[27].ImageIndex=node.Nodes[27].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("LoaderFlags:\t0x{0}\t\"设置自动调用断点或调试器\"",
				this.m_NtHeaders.OptionalHeader.LoaderFlags.ToString("X").PadLeft(8,'0')));
			node.Nodes[28].ImageIndex=node.Nodes[28].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("NumberOfRvaAndSizes:\t0x{0}\t\"DataDirctory数组长度，总=16\"",
				this.m_NtHeaders.OptionalHeader.NumberOfRvaAndSizes.ToString("X").PadLeft(8,'0')));
			node.Nodes[29].ImageIndex=node.Nodes[29].SelectedImageIndex=IMAGE_INDEX_BYTES4;

			node.Nodes.Add(string.Format("IMAGE_DATA_DIRECTORY[{0}] (size=8bytes*{0})",
				this.m_NtHeaders.OptionalHeader.NumberOfRvaAndSizes));		//index=30
			//添加DataDirectroy数组元素
			node=this.tvPEView.Nodes[2].Nodes[2].Nodes[30];
			for(int i=0;i<this.m_NtHeaders.OptionalHeader.NumberOfRvaAndSizes;i++)
			{
				node_2=new TreeNode(string.Format("DataDirectory[{0}]\t\"{1}\"",i,this.m_DataDirComments[i]),3,3);
				node_2.Nodes.Add(string.Format("VirutualAddress:\t0x{0}\"RAV值\"",
					this.m_NtHeaders.OptionalHeader.DataDirectory[i].VirtualAddress.ToString("X").PadLeft(8,'0')
					));
				node_2.Nodes[0].ImageIndex=node_2.Nodes[0].SelectedImageIndex=IMAGE_INDEX_BYTES4;

				node_2.Nodes.Add(string.Format("Size:\t0x{0}",
					this.m_NtHeaders.OptionalHeader.DataDirectory[i].Size.ToString("X").PadLeft(8,'0')
					));
				node_2.Nodes[1].ImageIndex=node_2.Nodes[1].SelectedImageIndex=IMAGE_INDEX_BYTES4;

				node.Nodes.Add(node_2);
			}

			//装配Section Table, index=3-------------------------------
			this.tvPEView.Nodes.Add("◆SECTION TABLE");
			//装配Sections,index=4-------------------------------------
			this.tvPEView.Nodes.Add("◆SECTIONS");


			//添加所有节的节头和节到相应节点
			node=this.tvPEView.Nodes[3];
			for(int i=0;i<this.m_NtHeaders.FileHeader.NumberOfSections;i++)
			{
				//读出一个节头
				this.ReadSectionHeader();
				sectionName=Encoding.Default.GetString(this.m_SectionHeader.Name);
				//去掉结尾的所有\0，即把\0以及以后的字符去掉
				if((indexOfNull=sectionName.IndexOf('\0'))>=0)
					sectionName=sectionName.Substring(0,indexOfNull);

				node_2=new TreeNode(string.Format("IMAGE_SECTION_HEADER [{0}]: \"{1}\"",i,sectionName));
				node_2.Nodes.Add(string.Format("Name: \"{0}\"",sectionName));
				node_2.Nodes[0].ImageIndex=node_2.Nodes[0].SelectedImageIndex=IMAGE_INDEX_BYTES8;
				for(int j=0;j<8;j++)
				{
					node_2.Nodes[0].Nodes.Add(string.Format("byte[{0}]: 0x{1}",j,this.m_SectionHeader.Name[j].ToString("X").PadLeft(2,'0')));
					node_2.Nodes[0].Nodes[j].ImageIndex=node_2.Nodes[0].Nodes[j].SelectedImageIndex=IMAGE_INDEX_BYTES1;
				}

				node_2.Nodes.Add(string.Format("VirtualSize:\t0x{0}\t\"节在虚拟内存中的大小（对齐后的大小）\"",
					this.m_SectionHeader.VirtualSize.ToString("X").PadLeft(8,'0')));
				node_2.Nodes[1].ImageIndex=node_2.Nodes[1].SelectedImageIndex=IMAGE_INDEX_BYTES4;

				node_2.Nodes.Add(string.Format("VirtualAddress:\t0x{0}\t\"节在虚拟内存中的RAV，第一个节缺省=0x1000，(在Obj文件中无意义，=0）\"",
					this.m_SectionHeader.VirtualAddress.ToString("X").PadLeft(8,'0')));
				node_2.Nodes[2].ImageIndex=node_2.Nodes[2].SelectedImageIndex=IMAGE_INDEX_BYTES4;

				node_2.Nodes.Add(string.Format("SizeOfRawData:\t0x{0}\t\"节在文件中的物理大小\"",
					this.m_SectionHeader.SizeOfRawData.ToString("X").PadLeft(8,'0')));
				node_2.Nodes[3].ImageIndex=node_2.Nodes[3].SelectedImageIndex=IMAGE_INDEX_BYTES4;

				node_2.Nodes.Add(string.Format("PointerToRawData:\t0x{0}\t\"节在文件中的地址\"",
					this.m_SectionHeader.PointerToRawData.ToString("X").PadLeft(8,'0')));
				node_2.Nodes[4].ImageIndex=node_2.Nodes[4].SelectedImageIndex=IMAGE_INDEX_BYTES4;

				node_2.Nodes.Add(string.Format("PointerToRelocations:\t0x{0}\t\"Obj文件使用，节基于文件重定位，（在EXE文件中无意义，=0）\"",
					this.m_SectionHeader.PointerToRelocations.ToString("X").PadLeft(8,'0')));
				node_2.Nodes[5].ImageIndex=node_2.Nodes[5].SelectedImageIndex=IMAGE_INDEX_BYTES4;

				node_2.Nodes.Add(string.Format("PointerToLinenumbers:\t0x{0}\"Obj文件使用，文件行号表偏移地址\"",
					this.m_SectionHeader.PointerToLinenumbers.ToString("X").PadLeft(8,'0')));
				node_2.Nodes[6].ImageIndex=node_2.Nodes[6].SelectedImageIndex=IMAGE_INDEX_BYTES4;

				node_2.Nodes.Add(string.Format("NumberOfRelocations:\t0x{0}\"Obj文件使用\"",
					this.m_SectionHeader.NumberOfRelocations.ToString("X").PadLeft(4,'0')));

				node_2.Nodes.Add(string.Format("NumberOfLinenumbers:\t0x{0}\"Obj文件使用\"",
					this.m_SectionHeader.NumberOfLinenumbers.ToString("X").PadLeft(4,'0')));

				node_2.Nodes.Add(string.Format("Characteristics:\t0x{0}\t\"节（段）属性\"",
					this.m_SectionHeader.Characteristics.ToString("X").PadLeft(8,'0')));
				node_2.Nodes[9].ImageIndex=node_2.Nodes[9].SelectedImageIndex=IMAGE_INDEX_BYTES4;
				//在section table节点下添加这个节头
				node.Nodes.Add(node_2);

				//在section集合节点下添加这个节,注意所有的size都是指RawDataSize!
				this.tvPEView.Nodes[4].Nodes.Add(string.Format("section:\"{0}\" (size={1:X}h,{1}bytes)",sectionName,this.m_SectionHeader.SizeOfRawData));
			}

			//装配Others,index=5-------------------------------------
			this.tvPEView.Nodes.Add("◆OTHERS");
			//添加others子节点
			node=this.tvPEView.Nodes[5];
			node.Nodes.Add("COFF Line Numbers");
			node.Nodes.Add("COFF Symbols");
			node.Nodes.Add("CodeView Debug Informations");
			//----------------- FILL END-----------------------------
			errorMsg="";
			return true;
		}

		/// <summary>
		/// 根据时间戳，返回时间戳表示的时间的字符串形式
		/// </summary>
		/// <param name="timeDateStamp"></param>
		/// <returns></returns>
		private string GetFileTime(int timeDateStamp)
		{
			DateTime filetime=this.m_FileTimeBase.AddSeconds(timeDateStamp);
			return filetime.ToString("yyyy.MM.dd HH:mm:ss");
		}

		#endregion

		//“打开”按钮的处理
		private void btOpenPE_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg=new OpenFileDialog();
			dlg.Title="请选择一个PE文件";
			dlg.Filter="可执行文件(*.exe)|*.exe|动态连接(*.Dll)|*.Dll|控件(*.ocx)|*.ocx|All Files(*.*)|*.*";
			string pefile;
			if(dlg.ShowDialog()==DialogResult.OK)
			{
				pefile=dlg.FileName;
			}
			else
				return;

			//更新标题
			this.Text="PEView - "+pefile;
			//只读打开
			Stream input=new FileStream(pefile,FileMode.Open,FileAccess.Read);
            this.m_Reader=new BinaryReader(input,Encoding.Default);

			string errorMsg;
			//重新装配treeview
			if(!this.FillTreeView(out errorMsg))
				MessageBox.Show(errorMsg);

			//关闭文件
			this.m_Reader.Close();
		}
	}
}
