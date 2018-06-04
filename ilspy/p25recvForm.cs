using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace p25recv_tuner
{
	public class p25recvForm : Form
	{
		private const byte CMD_NOP = 255;

		private const byte CMD_GET_INFO = 0;

		private const byte CMD_GET_STATE = 1;

		private const byte CMD_MEM_READ = 2;

		private const byte CMD_MEM_WRITE = 3;

		private const byte CMD_PARAMS_READ = 4;

		private const byte CMD_PARAMS_WRITE = 5;

		private const byte CMD_MEM_SELECT = 6;

		private const byte CMD_VOICE_FRAME = 7;

		private const byte CMD_MEM_SEND = 8;

		private const byte CMD_SCAN = 9;

		private const byte CMD_SET_FREQ = 10;

		private const byte CMD_ = 11;

		private const byte CMD_AUTO_SCAN = 12;

		private const byte CMD_TSBK = 13;

		private const byte CMD_PDU = 14;

		private const byte CMD_TT_SET = 15;

		private const byte CMD_TT_EVT = 16;

		private const byte CMD_TT_HR = 17;

		private const byte CMD_TT_FILT_RD = 18;

		private const byte CMD_TT_FILT_WR = 19;

		private const byte CMD_KEYS_WR = 20;

		private const byte CMD_UPG_INIT = 50;

		private const byte CMD_UPG = 51;

		private const byte CMD_RST = 52;

		private const byte CMD_RST_UPG = 53;

		private const byte CMD_HW_RST = 54;

		public byte curMem;

		private byte curStep;

		private byte curVol;

		public Color mainColor;

		public byte[] dev;

		private byte tx_op;

		public byte ai;

		public byte mem2read;

		public int dbm;

		public int state;

		private bool nac_dec;

		private bool ids_dec;

		private bool rx_mode;

		public uint curFreq;

		public uint minFreq;

		public uint maxFreq;

		public byte[] upg_buff;

		public int upg_size;

		public int upg_ptr;

		public OperatingSystem osInfo;

		public Version ver;

		private DoubleBufferPanel dbPanel;

		private VUmeter vuPanel;

		public smeter smForm;

		public SetFreqForm sfForm;

		public RecFreqForm rfForm;

		public ScanForm scanForm;

		public MemScanForm memScanForm;

		public GroupForm groupForm;

		public FormAutoScan autoScanForm;

		public FormTrunk trunkForm;

		public Decrypt decryptForm;

		private ComPort usp;

		private System.Timers.Timer uspTimer;

		private System.Timers.Timer updTimer;

		private System.Timers.Timer lnkTimer;

		private System.Timers.Timer clrTimer;

		private bool log_strobe;

		private bool logging;

		private StreamWriter log_file;

		private IContainer components;

		private Panel panelTL;

		private Panel panelInd;

		private Panel panelCtrl;

		private Panel panelFreq;

		private TextBox textBoxFreq;

		private TrackBar trackBar1;

		private CheckBox checkBoxStepUp;

		private Panel panelComments;

		private Panel panelEq;

		private TrackBar trackBar8;

		private TrackBar trackBar7;

		private TrackBar trackBar6;

		private TrackBar trackBar5;

		private TrackBar trackBar4;

		private TrackBar trackBar3;

		private TrackBar trackBar2;

		private Panel panelStatus;

		private Label label8v;

		private Label label7v;

		private Label label6v;

		private Label label5v;

		private Label label4v;

		private Label label3v;

		private Label label2v;

		private Label label17;

		private Label label16;

		private Label label15;

		private Label label14;

		private Label label13;

		private Label label12;

		private Label label11;

		private Label label10;

		private Label label1v;

		private Label labelLink;

		private CheckBox checkBoxExit;

		private CheckBox checkBoxAbout;

		private CheckBox checkBoxUpgrade;

		private Panel panelAi;

		private Panel panelSrcId;

		private Label labelSrcId;

		private Panel panelNac;

		private Label label21;

		private Label labelSrc;

		private Label labelAccCode;

		private Label labelNac;

		private Label labelFT;

		private Panel panelET;

		private Label labelEncType;

		private Label labelCT;

		private Label labelManuf;

		private Label labelPrivacy;

		private Label labelTgt;

		private Label labelMem;

		private Panel panelStep;

		private Label labelStep;

		private Label labelStp;

		private CheckBox checkBoxStepDown;

		private CheckBox checkBoxSave;

		private CheckBox checkBoxSmeter;

		private CheckBox checkBoxLoad;

		private ToolTip toolTipMainForm;

		private Panel panelMem;

		private TextBox textBoxMemory;

		private Label labelMHz;

		private Label labelComment;

		private Panel panelCT;

		private Label labelCallType;

		private Panel panelManuf;

		private Label labelMfid;

		private Panel panelTgtId;

		private Label labelTgtId;

		private Label labelKey;

		private Panel panelEC;

		private Label labelEncKey;

		private Panel panelFT;

		private Label labelFrmType;

		private Panel panelProto;

		private Label labelAi;

		private CheckBox checkBoxStepLock;

		private CheckBox checkBoxMemLock;

		private CheckBox checkBoxMemDown;

		private CheckBox checkBoxMemUp;

		private CheckBox checkBoxAiDown;

		private CheckBox checkBoxAiUp;

		private OpenFileDialog openFileDialogMem;

		private OpenFileDialog openFileDialogUpgrade;

		private SaveFileDialog saveFileDialogMem;

		private Panel panelVol;

		private CheckBox checkBoxVolDown;

		private CheckBox checkBoxVolUp;

		private Label labelVol;

		private CheckBox checkBoxMemRecall;

		private CheckBox checkBoxMemStore;

		private Panel panelConnStatus;

		private TextBox textBoxVol;

		private CheckBox checkBoxHex;

		private CheckBox checkBoxDec;

		private CheckBox checkBoxSearch;

		private CheckBox checkBoxMemScan;

		private CheckBox checkBoxTrunk;

		private CheckBox checkBoxGroupIDs;

		private CheckBox checkBoxAutoScan;

		private CheckBox checkBoxLog;

		private SaveFileDialog saveFileDialogLog;

		private Panel panelGen;

		private Panel panelVU;

		private CheckBox checkBoxDecrypt;

		public p25recvForm()
		{
			this.InitializeComponent();
			int num = 0;
			object obj = this.regGetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "step");
			if (obj == null)
			{
				this.regSetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "step", num);
			}
			else
			{
				num = (int)obj;
			}
			this.curStep = (byte)num;
			this.stepPrint();
			this.nac_dec = false;
			obj = this.regGetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "nac");
			if (obj == null)
			{
				this.regSetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "nac", this.nac_dec);
			}
			else
			{
				this.nac_dec = ((string)obj == "True");
			}
			this.checkBoxHex.Checked = this.nac_dec;
			this.ids_dec = false;
			obj = this.regGetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "ids");
			if (obj == null)
			{
				this.regSetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "ids", this.ids_dec);
			}
			else
			{
				this.ids_dec = ((string)obj == "True");
			}
			this.checkBoxDec.Checked = this.ids_dec;
			base.MouseWheel += new MouseEventHandler(this.p25recvForm_MouseWheel);
			this.textBoxFreq.MouseWheel += new MouseEventHandler(this.p25recvForm_MouseWheel);
			this.textBoxMemory.MouseWheel += new MouseEventHandler(this.Memory_MouseWheel);
			this.textBoxVol.MouseWheel += new MouseEventHandler(this.Volume_MouseWheel);
			this.mainColor = default(Color);
			this.mainColor = Color.FromArgb(70, 70, 70);
			Rectangle clientRectangle = base.ClientRectangle;
			int num2 = base.Width - clientRectangle.Width;
			int num3 = base.Height - clientRectangle.Height;
			this.panelStatus.Top = clientRectangle.Height - this.panelStatus.Height - 3;
			this.panelStatus.Width = clientRectangle.Width - 6;
			this.panelStatus.Left = 3;
			this.curFreq = 430000000u;
			this.minFreq = 410000000u;
			this.maxFreq = 480000000u;
			this.dbm = -135;
			this.rx_mode = false;
			this.stepPrint();
			this.memPrint();
			this.textBoxFreq.Select(0, 0);
			this.dev = new byte[6172];
			this.memset(this.dev, 0, 0, 6172);
			this.putUInt32(this.dev, 12, this.curFreq);
			this.putUInt32(this.dev, 16, this.curFreq);
			for (int i = 0; i < 150; i++)
			{
				this.putUInt32(this.dev, 172 + i * 40 + 20, this.curFreq);
			}
			this.osInfo = Environment.OSVersion;
			this.ver = this.osInfo.Version;
			if (this.ver.Major != 5 && this.ver.Major == 6)
			{
				int arg_304_0 = this.ver.Minor;
			}
			this.smForm = new smeter(this, this.mainColor);
			this.sfForm = new SetFreqForm(this, this.mainColor);
			this.rfForm = new RecFreqForm(this, this.mainColor);
			this.scanForm = new ScanForm(this, this.mainColor);
			this.memScanForm = new MemScanForm(this, this.mainColor);
			this.groupForm = new GroupForm(this, this.mainColor);
			this.autoScanForm = new FormAutoScan(this, this.mainColor);
			this.trunkForm = new FormTrunk(this, this.mainColor);
			this.decryptForm = new Decrypt(this, this.mainColor);
			this.rfForm.setItems();
			this.usp = new ComPort(this);
			this.panelInd.Top = (this.panelInd.Left = (this.panelTL.Top = 3));
			this.panelAi.Left = (this.panelEq.Left = (this.panelStatus.Left = (this.panelGen.Left = 3)));
			this.panelTL.Left = (this.panelVU.Left = this.panelInd.Left + this.panelInd.Width);
			this.panelCtrl.Left = this.panelAi.Left + this.panelAi.Width;
			this.panelGen.Top = (this.panelVU.Top = this.panelInd.Top + this.panelTL.Height);
			this.panelAi.Top = (this.panelCtrl.Top = this.panelInd.Top + this.panelTL.Height + this.panelGen.Height);
			this.panelEq.Top = this.panelInd.Height + this.panelAi.Height + this.panelGen.Height + 3;
			this.panelEq.Width = (this.panelStatus.Width = this.panelInd.Width + this.panelTL.Width);
			this.panelStatus.Top = this.panelInd.Height + this.panelAi.Height + this.panelEq.Height + this.panelGen.Height + 3;
			base.Width = this.panelInd.Width + this.panelTL.Width + 6 + num2;
			base.Height = this.panelStatus.Top + this.panelStatus.Height + 3 + num3;
			this.checkBoxExit.Left = this.panelStatus.Width - this.checkBoxExit.Width - 3;
			this.checkBoxAbout.Left = this.checkBoxExit.Left - this.checkBoxAbout.Width;
			this.labelMHz.Top = 2;
			this.labelMHz.Left = this.panelFreq.Width - this.labelMHz.Width - 2;
			this.labelMHz.Height = this.panelFreq.Height - 4;
			this.compAlign(this.panelFreq, this.textBoxFreq, 2, this.labelMHz.Width);
			this.compAlign(this.panelComments, this.labelComment, 2, 0);
			this.compAlign(this.panelProto, this.labelAi, 2, this.checkBoxAiUp.Width);
			this.compAlign(this.panelNac, this.labelNac, 2, 0);
			this.compAlign(this.panelCT, this.labelCallType, 2, 0);
			this.compAlign(this.panelSrcId, this.labelSrcId, 2, 0);
			this.compAlign(this.panelTgtId, this.labelTgtId, 2, 0);
			this.compAlign(this.panelET, this.labelEncType, 2, 0);
			this.compAlign(this.panelEC, this.labelEncKey, 2, 0);
			this.compAlign(this.panelFT, this.labelFrmType, 2, 0);
			this.compAlign(this.panelManuf, this.labelMfid, 2, 0);
			this.compAlign(this.panelStep, this.labelStep, 2, this.checkBoxStepUp.Width);
			this.compAlign(this.panelMem, this.textBoxMemory, 2, this.checkBoxMemUp.Width);
			this.compAlign(this.panelVol, this.textBoxVol, 2, this.checkBoxVolUp.Width);
			this.compAlign(this.panelConnStatus, this.labelLink, 2, 0);
			this.dbPanel = new DoubleBufferPanel(this);
			this.dbPanel.Width = this.panelStep.Left + this.panelStep.Width - this.labelStp.Left - 1;
			this.dbPanel.Height = this.panelComments.Top + this.panelComments.Height - this.panelFreq.Top - 1;
			this.dbPanel.Top = this.panelFreq.Top + 3;
			this.dbPanel.Left = this.panelTL.Left + this.checkBoxStepLock.Left + 1;
			this.dbPanel.setDbm(this.dbm);
			base.Controls.Add(this.dbPanel);
			this.dbPanel.BringToFront();
			this.vuPanel = new VUmeter(this);
			this.vuPanel.Width = this.dbPanel.Width;
			this.vuPanel.Height = this.panelVol.Top + this.panelVol.Height - this.panelStep.Top + 1;
			this.vuPanel.Top = this.panelVU.Top + this.panelStep.Top - 1;
			this.vuPanel.Left = this.dbPanel.Left;
			base.Controls.Add(this.vuPanel);
			this.vuPanel.BringToFront();
			this.uspTimer = new System.Timers.Timer();
			this.uspTimer.Elapsed += new ElapsedEventHandler(this.uspTimer_Elapsed);
			this.uspTimer.SynchronizingObject = this;
			this.updTimer = new System.Timers.Timer();
			this.updTimer.Elapsed += new ElapsedEventHandler(this.updTimer_Elapsed);
			this.updTimer.SynchronizingObject = this;
			this.lnkTimer = new System.Timers.Timer();
			this.lnkTimer.Elapsed += new ElapsedEventHandler(this.lnkTimer_Elapsed);
			this.lnkTimer.SynchronizingObject = this;
			this.clrTimer = new System.Timers.Timer();
			this.clrTimer.Elapsed += new ElapsedEventHandler(this.clrTimer_Elapsed);
			this.clrTimer.SynchronizingObject = this;
			this.clrTimer.Interval = 2000.0;
		}

		public uint crc32(byte[] ptr, int idx, int len)
		{
			uint num = 0u;
			for (int i = 0; i < len; i++)
			{
				for (int j = 7; j >= 0; j--)
				{
					if ((((ulong)(num >> 31) ^ (ulong)((long)(ptr[i] >> j))) & 1uL) == 1uL)
					{
						num = (num << 1 ^ 79764919u);
					}
					else
					{
						num <<= 1;
					}
				}
			}
			return num ^ 4294967295u;
		}

		public uint crc32func(uint crc, byte v)
		{
			uint num = (uint)((uint)v << 24);
			for (int i = 0; i < 8; i++)
			{
				if (((crc ^ num) & 2147483648u) == 2147483648u)
				{
					crc = (crc << 1 ^ 79764919u);
				}
				else
				{
					crc <<= 1;
				}
				num <<= 1;
			}
			return crc;
		}

		public uint crc32file(uint crc, byte[] src, int idx, int size)
		{
			for (int i = 0; i < size; i++)
			{
				crc = this.crc32func(crc, src[idx + i]);
			}
			crc ^= 4294967295u;
			return crc;
		}

		public int strLen(byte[] str, int idx, int maxlen)
		{
			int i;
			for (i = 0; i < maxlen; i++)
			{
				if (str[idx + i] == 0)
				{
					return i;
				}
			}
			return i;
		}

		public void putStr(byte[] data, int idx, int size, string str)
		{
			byte[] bytes = Encoding.Default.GetBytes(str);
			this.memset(data, idx, 0, size);
			if (str.Length < size)
			{
				size = str.Length;
			}
			this.memcpy(data, idx, bytes, 0, size);
		}

		public string getStr(byte[] data, int idx, int size)
		{
			int num = 0;
			while (num < size && data[idx + num] != 0)
			{
				num++;
			}
			if (num > 0)
			{
				return Encoding.Default.GetString(data, idx, num);
			}
			return "";
		}

		private void statusMsg(string txt, Color c)
		{
			this.labelLink.ForeColor = c;
			this.labelLink.Text = txt;
		}

		public bool memcmp(byte[] a, byte[] b, int size)
		{
			for (int i = 0; i < size; i++)
			{
				if (a[i] != b[i])
				{
					return true;
				}
			}
			return false;
		}

		public void memset(byte[] adr, int idx, byte f, int size)
		{
			for (int i = 0; i < size; i++)
			{
				adr[idx + i] = f;
			}
		}

		public void memcpy(byte[] dst, int didx, byte[] src, int sidx, int size)
		{
			for (int i = 0; i < size; i++)
			{
				dst[didx + i] = src[sidx + i];
			}
		}

		public void putUInt16(byte[] buf, int idx, ushort v)
		{
			buf[idx] = (byte)v;
			buf[idx + 1] = (byte)(v >> 8);
		}

		public ushort getUInt16(byte[] buf, int idx)
		{
			return (ushort)((int)buf[idx] | (int)buf[idx + 1] << 8);
		}

		public void putUInt16r(byte[] buf, int idx, ushort v)
		{
			buf[idx] = (byte)(v >> 8);
			buf[idx + 1] = (byte)v;
		}

		public ushort getUInt16r(byte[] buf, int idx)
		{
			return (ushort)((int)buf[idx + 1] | (int)buf[idx] << 8);
		}

		public void putUInt32(byte[] buf, int idx, uint v)
		{
			buf[idx] = (byte)v;
			buf[idx + 1] = (byte)(v >> 8);
			buf[idx + 2] = (byte)(v >> 16);
			buf[idx + 3] = (byte)(v >> 24);
		}

		public uint getUInt32(byte[] buf, int idx)
		{
			return (uint)((int)buf[idx] | (int)buf[idx + 1] << 8 | (int)buf[idx + 2] << 16 | (int)buf[idx + 3] << 24);
		}

		public void putUInt32r(byte[] buf, int idx, uint v)
		{
			buf[idx] = (byte)(v >> 24);
			buf[idx + 1] = (byte)(v >> 16);
			buf[idx + 2] = (byte)(v >> 8);
			buf[idx + 3] = (byte)v;
		}

		public uint getUInt32r(byte[] buf, int idx)
		{
			return (uint)((int)buf[idx + 3] | (int)buf[idx + 2] << 8 | (int)buf[idx + 1] << 16 | (int)buf[idx] << 24);
		}

		public void putUInt24r(byte[] buf, int idx, uint v)
		{
			buf[idx] = (byte)(v >> 16);
			buf[idx + 1] = (byte)(v >> 8);
			buf[idx + 2] = (byte)v;
		}

		public uint getUInt24r(byte[] buf, int idx)
		{
			return (uint)((int)buf[idx + 2] | (int)buf[idx + 1] << 8 | (int)buf[idx] << 16);
		}

		public decimal str2freq(string str)
		{
			decimal result;
			if (!decimal.TryParse(str, out result))
			{
				if (str.IndexOf(',') > -1)
				{
					str = str.Replace(",", ".");
				}
				else if (str.IndexOf('.') > -1)
				{
					str = str.Replace(".", ",");
				}
				if (!decimal.TryParse(str, out result))
				{
					return this.minFreq / 1000000u;
				}
			}
			return result;
		}

		private void timerStart(int interv)
		{
			this.timerStop();
			this.uspTimer.Interval = (double)interv;
			this.uspTimer.Enabled = true;
		}

		private void timerStop()
		{
			this.uspTimer.Enabled = false;
		}

		private void timerLinkStart(int interv)
		{
			this.timerLinkStop();
			this.lnkTimer.Interval = (double)interv;
			this.lnkTimer.Enabled = true;
		}

		private void timerLinkStop()
		{
			this.lnkTimer.Enabled = false;
		}

		private void timerUpdateStart()
		{
			this.timerUpdateStop();
			this.updTimer.Interval = 300.0;
			this.updTimer.Enabled = true;
		}

		private void timerUpdateStop()
		{
			this.updTimer.Enabled = false;
		}

		private void printFreq()
		{
			this.textBoxFreq.Text = (this.curFreq / 1000000m).ToString("N6");
			this.timerUpdateStart();
		}

		private void printFreqNoUpdt()
		{
			this.textBoxFreq.Text = (this.curFreq / 1000000m).ToString("N6");
		}

		private int tbSetValue(byte v)
		{
			if (v > 8)
			{
				v = 0;
			}
			if (v == 0)
			{
				return 4;
			}
			if (v < 5)
			{
				return (int)(9 - v);
			}
			return (int)(3 - (v - 5));
		}

		private byte tbGetValue(int v)
		{
			if (v == 4)
			{
				return 0;
			}
			if (v > 4)
			{
				return (byte)(9 - v);
			}
			return (byte)(8 - v);
		}

		private void putParams(byte[] data, int idx)
		{
			this.ai = this.dev[2];
			this.switchAi(false);
			this.curVol = data[idx];
			this.textBoxVol.Text = this.curVol.ToString();
			this.trackBar1.Value = this.tbSetValue(data[idx + 4]);
			this.trackBar2.Value = this.tbSetValue(data[idx + 5]);
			this.trackBar3.Value = this.tbSetValue(data[idx + 6]);
			this.trackBar4.Value = this.tbSetValue(data[idx + 7]);
			this.trackBar5.Value = this.tbSetValue(data[idx + 8]);
			this.trackBar6.Value = this.tbSetValue(data[idx + 9]);
			this.trackBar7.Value = this.tbSetValue(data[idx + 10]);
			this.trackBar8.Value = this.tbSetValue(data[idx + 11]);
			this.curFreq = this.getUInt32(data, idx + 12);
		}

		private void getParams()
		{
			this.dev[0] = this.curVol;
			this.dev[2] = this.ai;
			this.dev[4] = this.tbGetValue(this.trackBar1.Value);
			this.dev[5] = this.tbGetValue(this.trackBar2.Value);
			this.dev[6] = this.tbGetValue(this.trackBar3.Value);
			this.dev[7] = this.tbGetValue(this.trackBar4.Value);
			this.dev[8] = this.tbGetValue(this.trackBar5.Value);
			this.dev[9] = this.tbGetValue(this.trackBar6.Value);
			this.dev[10] = this.tbGetValue(this.trackBar7.Value);
			this.dev[11] = this.tbGetValue(this.trackBar8.Value);
			this.putUInt32(this.dev, 12, this.curFreq);
			this.putUInt32(this.dev, 16, this.curFreq);
		}

		public byte[] getMemory(byte num)
		{
			byte[] array = new byte[40];
			this.memcpy(array, 0, this.dev, (int)(172 + num * 40), 40);
			return array;
		}

		private void putMemory(byte[] data, int idx, int chan)
		{
			this.textBoxVol.Text = data[0].ToString();
			this.curFreq = this.getUInt32(data, 12);
			this.printFreq();
		}

		private uint getStep()
		{
			uint result;
			if (this.curStep == 0)
			{
				result = 100u;
			}
			else if (this.curStep == 1)
			{
				result = 250u;
			}
			else if (this.curStep == 2)
			{
				result = 500u;
			}
			else if (this.curStep == 3)
			{
				result = 1000u;
			}
			else if (this.curStep == 4)
			{
				result = 2500u;
			}
			else if (this.curStep == 5)
			{
				result = 3125u;
			}
			else if (this.curStep == 6)
			{
				result = 5000u;
			}
			else if (this.curStep == 7)
			{
				result = 6250u;
			}
			else if (this.curStep == 8)
			{
				result = 8333u;
			}
			else if (this.curStep == 9)
			{
				result = 9000u;
			}
			else if (this.curStep == 10)
			{
				result = 10000u;
			}
			else if (this.curStep == 11)
			{
				result = 12500u;
			}
			else if (this.curStep == 12)
			{
				result = 20000u;
			}
			else if (this.curStep == 13)
			{
				result = 25000u;
			}
			else if (this.curStep == 14)
			{
				result = 30000u;
			}
			else if (this.curStep == 15)
			{
				result = 50000u;
			}
			else
			{
				result = 100000u;
			}
			return result;
		}

		private uint freqAlign(uint f, uint s)
		{
			uint num = f / s * s;
			uint num2 = f - num;
			if (num2 < s / 2u)
			{
				s = num;
			}
			else
			{
				s = num + s;
			}
			return s;
		}

		private void lightCtl(bool bright)
		{
			Color foreColor;
			if (bright)
			{
				foreColor = Color.Lime;
			}
			else
			{
				foreColor = Color.ForestGreen;
			}
			this.labelNac.ForeColor = foreColor;
			this.labelCallType.ForeColor = foreColor;
			this.labelSrcId.ForeColor = foreColor;
			this.labelTgtId.ForeColor = foreColor;
			this.labelEncType.ForeColor = foreColor;
			this.labelEncKey.ForeColor = foreColor;
			this.labelFrmType.ForeColor = foreColor;
			this.labelMfid.ForeColor = foreColor;
		}

		protected RegistryKey openSubKey(string s)
		{
			RegistryKey result;
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(s, true);
				if (registryKey == null)
				{
					registryKey = Registry.LocalMachine.CreateSubKey(s);
				}
				result = registryKey;
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		public object regGetValue(string subkey, string key)
		{
			RegistryKey registryKey = this.openSubKey(subkey);
			if (registryKey == null)
			{
				return null;
			}
			object result;
			try
			{
				object value = registryKey.GetValue(key);
				registryKey.Close();
				result = value;
			}
			catch (Exception)
			{
				registryKey.Close();
				result = null;
			}
			return result;
		}

		public bool regSetValue(string subkey, string key, object val)
		{
			RegistryKey registryKey = this.openSubKey(subkey);
			if (registryKey == null)
			{
				return false;
			}
			bool result;
			try
			{
				registryKey.SetValue(key, val);
				registryKey.Close();
				result = true;
			}
			catch (Exception)
			{
				registryKey.Close();
				result = false;
			}
			return result;
		}

		public void compAlign(Control o, Control i, int border, int corr)
		{
			i.Top = border;
			i.Left = border;
			i.Height = o.Height - 2 * border;
			i.Width = o.Width - 2 * border - corr;
		}

		private void aiUp()
		{
			this.ai += 1;
			if (this.ai > 6)
			{
				this.ai = 0;
			}
			this.switchAi(true);
		}

		private void aiDown()
		{
			if (this.ai == 0)
			{
				this.ai = 6;
			}
			else
			{
				this.ai -= 1;
			}
			this.switchAi(true);
		}

		public void switchAi(bool upd)
		{
			this.dev[2] = this.ai;
			if (this.ai == 0)
			{
				this.labelAi.Text = "P25";
				this.labelAccCode.Text = "NAC";
				this.labelSrc.Text = "SOURCE ID";
				this.labelTgt.Text = "TARGET ID";
				this.labelPrivacy.Text = "ENCRYPTION TYPE";
				this.labelKey.Text = "ENCRYPTION KEY";
				this.labelFT.Text = "FRAME TYPE";
				this.labelManuf.Text = "MANUFACTURER";
				this.labelManuf.Visible = (this.panelManuf.Visible = true);
				this.checkBoxTrunk.Visible = true;
			}
			else if (this.ai < 4)
			{
				if (this.ai == 1)
				{
					this.labelAi.Text = "DMR";
				}
				else if (this.ai == 2)
				{
					this.labelAi.Text = "DMR TS1";
				}
				else if (this.ai == 3)
				{
					this.labelAi.Text = "DMR TS2";
				}
				this.labelSrc.Text = "SOURCE ID";
				this.labelTgt.Text = "TARGET ID";
				this.labelAccCode.Text = "COLOR CODE";
				this.labelPrivacy.Text = "PRIVACY INDICATOR";
				this.labelKey.Text = "ENCRYPTION KEY";
				this.labelFT.Text = "TDMA TIME SLOT";
				this.labelManuf.Text = "ACCESS TYPE";
				this.labelManuf.Visible = (this.panelManuf.Visible = true);
				this.checkBoxTrunk.Visible = false;
			}
			else if (this.ai == 4)
			{
				this.labelAi.Text = "S.FUSION";
				this.labelAccCode.Text = "SQUELCH CODE";
				this.labelPrivacy.Text = "SQUELCH CODE ENABLED";
				this.labelSrc.Text = "SOURCE CALLSIGN";
				this.labelTgt.Text = "TARGET CALLSIGN";
				this.labelKey.Text = "DATA TYPE";
				this.labelFT.Text = "VOIP PATH";
				this.labelManuf.Visible = (this.panelManuf.Visible = false);
				this.checkBoxTrunk.Visible = false;
			}
			else if (this.ai == 5 || this.ai == 6)
			{
				if (this.ai == 5)
				{
					this.labelAi.Text = "NXDN 4800";
				}
				else
				{
					this.labelAi.Text = "NXDN 9600";
				}
				this.labelAccCode.Text = "RAN";
				this.labelPrivacy.Text = "CIPHER TYPE";
				this.labelSrc.Text = "SOURCE ID";
				this.labelTgt.Text = "TARGET ID";
				this.labelKey.Text = "KEY ID";
				this.labelFT.Text = "EMERGENCY";
				this.labelManuf.Text = "VOCODER RATE";
				this.labelManuf.Visible = (this.panelManuf.Visible = true);
				this.checkBoxTrunk.Visible = false;
			}
			if (upd)
			{
				this.timerUpdateStart();
			}
		}

		public bool comGetInfo()
		{
			bool flag = this.usp.uspSend(null, 0, 0);
			if (flag)
			{
				this.tx_op = 0;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comMemRead(byte num)
		{
			byte[] data = new byte[]
			{
				num,
				0
			};
			bool flag = this.usp.uspSend(data, 2, 2);
			if (flag)
			{
				this.tx_op = 2;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comMemWrite(byte num)
		{
			byte[] array = new byte[42];
			this.memcpy(array, 2, this.dev, (int)(172 + num * 40), 40);
			array[0] = num;
			array[1] = 0;
			bool flag = this.usp.uspSend(array, (ushort)array.Length, 3);
			if (flag)
			{
				this.tx_op = 3;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comMemSend(byte num)
		{
			byte[] array = new byte[42];
			this.memcpy(array, 2, this.dev, (int)(172 + num * 40), 40);
			array[0] = num;
			array[1] = 0;
			bool flag = this.usp.uspSend(array, (ushort)array.Length, 8);
			if (flag)
			{
				this.tx_op = 8;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comParamsRead()
		{
			byte[] data = new byte[]
			{
				0,
				0
			};
			bool flag = this.usp.uspSend(data, 2, 4);
			if (flag)
			{
				this.tx_op = 4;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comParamsWrite()
		{
			bool flag = this.usp.uspSend(this.dev, 172, 5);
			if (flag)
			{
				this.tx_op = 5;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comMemSelect(byte num, byte flush)
		{
			byte[] data = new byte[]
			{
				num,
				flush
			};
			bool flag = this.usp.uspSend(data, 2, 6);
			if (flag)
			{
				this.tx_op = 6;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comTTHR(ushort grp)
		{
			byte[] data = new byte[]
			{
				(byte)(grp & 255),
				(byte)(grp >> 8 & 255)
			};
			return this.usp.uspSend(data, 2, 17);
		}

		public bool comTTSet(bool set)
		{
			if (this.tx_op != 255)
			{
				return false;
			}
			byte[] array = new byte[2];
			array[0] = (array[1] = 0);
			if (set)
			{
				array[0] = 1;
			}
			bool flag = this.usp.uspSend(array, 2, 15);
			if (flag)
			{
				this.tx_op = 15;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comTTFiltRd()
		{
			if (this.tx_op != 255)
			{
				return false;
			}
			byte[] array = new byte[2];
			array[0] = (array[1] = 0);
			bool flag = this.usp.uspSend(array, 2, 18);
			if (flag)
			{
				this.tx_op = 18;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comTTFiltWr()
		{
			if (this.tx_op != 255)
			{
				return false;
			}
			byte[] array = this.trunkForm.filtGet();
			bool flag = this.usp.uspSend(array, (ushort)array.Length, 19);
			if (flag)
			{
				this.tx_op = 19;
				this.timerStart(1000);
			}
			return flag;
		}

		public bool comTune(uint freq, byte rxmode)
		{
			byte[] array = new byte[4];
			this.putUInt32(array, 0, freq);
			return this.usp.uspSend(array, 4, 10);
		}

		public bool comSetFreq(uint freq, byte ai)
		{
			byte[] array = new byte[6];
			this.putUInt32(array, 0, freq);
			array[4] = ai;
			return this.usp.uspSend(array, 6, 10);
		}

		public bool comScan(uint freq, ushort wt, byte ai)
		{
			byte[] array = new byte[8];
			this.putUInt32(array, 0, freq);
			this.putUInt16(array, 4, wt);
			array[6] = ai;
			array[7] = 0;
			return this.usp.uspSend(array, 8, 9);
		}

		public bool comHwReset()
		{
			byte[] data = new byte[]
			{
				170,
				85
			};
			return this.usp.uspSend(data, 2, 54);
		}

		public bool comReset()
		{
			return this.usp.uspSend(null, 0, 52);
		}

		public bool comResetUpg()
		{
			return this.usp.uspSend(null, 0, 53);
		}

		public bool comKeysWrite(byte[] buff)
		{
			return this.usp.uspSend(buff, 138, 20);
		}

		public void showDevInfo(byte[] data, int idx)
		{
			string portName = this.usp.portName;
			string text = BitConverter.ToString(data, idx, 8);
			text = text.Replace("-", "");
			text = string.Concat(new string[]
			{
				"LINK: ",
				portName.ToString(),
				",  S/N: ",
				text,
				" (v",
				data[idx + 8].ToString(),
				".",
				data[idx + 9].ToString(),
				")"
			});
			this.labelLink.ForeColor = Color.Lime;
			this.labelLink.Text = text;
		}

		public void hideDevInfo()
		{
			this.labelLink.ForeColor = Color.Red;
			this.labelLink.Text = "LINK";
		}

		public void nxdnSetStats(byte[] data, int idx, int size)
		{
			if (size > 20 && !this.updTimer.Enabled && !this.uspTimer.Enabled)
			{
				uint uInt = this.getUInt32(data, idx + 20);
				if (uInt != this.curFreq)
				{
					this.curFreq = uInt;
					this.printFreqNoUpdt();
				}
			}
			if (data[idx] == 2)
			{
				if (!this.rx_mode)
				{
					this.rx_mode = true;
					this.lightCtl(true);
					this.memScanForm.enterRxMode();
					this.clrTimer.Enabled = false;
					this.log_strobe = true;
				}
				if (!this.nac_dec)
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString("X2");
				}
				else
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString();
				}
				if (!this.ids_dec)
				{
					this.labelSrcId.Text = this.getUInt32(data, idx + 12).ToString("X4");
					this.labelTgtId.Text = this.getUInt32(data, idx + 16).ToString("X4");
				}
				else
				{
					this.labelSrcId.Text = this.getUInt32(data, idx + 12).ToString();
					this.labelTgtId.Text = this.getUInt32(data, idx + 16).ToString();
				}
				byte b = (byte)(data[idx + 6] >> 5);
				if (b == 0)
				{
					this.labelCallType.Text = "BROADCAST";
				}
				else if (b == 1)
				{
					this.labelCallType.Text = "CONFERENCE";
				}
				else if (b == 2)
				{
					this.labelCallType.Text = "UNSPEC.";
				}
				else if (b == 3 || b == 5)
				{
					this.labelCallType.Text = "RESERVED";
				}
				else if (b == 4)
				{
					this.labelCallType.Text = "INDIVIDUAL";
				}
				else if (b == 6)
				{
					this.labelCallType.Text = "INTERCONN.";
				}
				else if (b == 7)
				{
					this.labelCallType.Text = "SPEED DIAL";
				}
				else
				{
					this.labelCallType.Text = "UNKNOWN";
				}
				if (data[idx + 1] == 0)
				{
					this.labelEncType.Text = "PUBLIC";
				}
				else if (data[idx + 1] == 1)
				{
					this.labelEncType.Text = "SCRAMBLE";
				}
				else if (data[idx + 1] == 2)
				{
					this.labelEncType.Text = "DES";
				}
				else if (data[idx + 1] == 3)
				{
					this.labelEncType.Text = "AES";
				}
				else
				{
					this.labelEncType.Text = "PRIVATE";
				}
				this.labelEncKey.Text = this.getUInt16(data, 10).ToString();
				if ((data[idx + 2] & 128) == 128)
				{
					this.labelFrmType.Text = "EMERGENCY";
				}
				else
				{
					this.labelFrmType.Text = "NORMAL";
				}
				byte b2 = data[idx + 6] & 7;
				if (b2 == 3)
				{
					this.labelMfid.Text = "FULL RATE";
				}
				else
				{
					this.labelMfid.Text = "HALF RATE";
				}
				if (this.logging && this.log_strobe)
				{
					this.log_strobe = false;
					string text = "\"" + DateTime.Now.ToString() + "\";";
					text = text + "\"" + this.textBoxFreq.Text + "\";";
					text = text + "\"" + this.labelAi.Text + "\";";
					text = text + "\"" + this.labelNac.Text + "\";";
					text = text + "\"" + this.labelCallType.Text + "\";";
					text = text + "\"" + this.labelSrcId.Text + "\";";
					text = text + "\"" + this.labelTgtId.Text + "\";";
					text = text + "\"" + this.labelEncType.Text + "\";";
					text = text + "\"" + this.labelEncKey.Text + "\";";
					text += "\"\";";
					text += "\"\";";
					text += "\"\";";
					this.log2file(text);
				}
			}
			else if (this.rx_mode)
			{
				this.rx_mode = false;
				this.lightCtl(false);
				this.memScanForm.exitRxMode();
				this.clrTimer.Enabled = true;
				this.log_strobe = false;
			}
			sbyte b3 = (sbyte)data[idx + 3];
			this.dbm = (int)b3;
			this.dbPanel.setDbm(this.dbm);
			this.dbPanel.Refresh();
		}

		public void sfSetStats(byte[] data, int idx, int size)
		{
			if (size > 20 && !this.updTimer.Enabled && !this.uspTimer.Enabled)
			{
				uint uInt = this.getUInt32(data, idx + 20);
				if (uInt != this.curFreq)
				{
					this.curFreq = uInt;
					this.printFreqNoUpdt();
				}
			}
			if (data[idx] == 2)
			{
				if (!this.rx_mode)
				{
					this.rx_mode = true;
					this.lightCtl(true);
					this.memScanForm.enterRxMode();
					this.clrTimer.Enabled = false;
					this.log_strobe = true;
				}
				if (!this.nac_dec)
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString("X2");
				}
				else
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString();
				}
				this.labelTgtId.Text = this.getStr(data, idx + 24, 10);
				this.labelSrcId.Text = this.getStr(data, idx + 34, 10);
				if ((data[idx + 2] & 16) == 16)
				{
					this.labelCallType.Text = "INDIVIDUAL";
				}
				else
				{
					this.labelCallType.Text = "GROUP";
				}
				if ((data[idx + 2] & 8) == 8)
				{
					this.labelEncType.Text = "YES";
				}
				else
				{
					this.labelEncType.Text = "NO";
				}
				if ((data[idx + 2] & 3) == 0)
				{
					this.labelEncKey.Text = "V/D MODE 1";
				}
				else if ((data[idx + 2] & 3) == 2)
				{
					this.labelEncKey.Text = "V/D MODE 2";
				}
				else
				{
					this.labelEncKey.Text = "VOICE FR";
				}
				if ((data[idx + 2] & 4) == 4)
				{
					this.labelFrmType.Text = "REPEATED";
				}
				else
				{
					this.labelFrmType.Text = "LOCAL";
				}
				if (this.logging && this.log_strobe)
				{
					this.log_strobe = false;
					string text = "\"" + DateTime.Now.ToString() + "\";";
					text = text + "\"" + this.textBoxFreq.Text + "\";";
					text = text + "\"" + this.labelAi.Text + "\";";
					text = text + "\"" + this.labelNac.Text + "\";";
					text = text + "\"" + this.labelCallType.Text + "\";";
					text = text + "\"" + this.labelSrcId.Text + "\";";
					text = text + "\"" + this.labelTgtId.Text + "\";";
					text = text + "\"" + this.labelEncType.Text + "\";";
					text += "\"0\";";
					text += "\"\";";
					text = text + "\"" + this.labelEncKey.Text + "\";";
					text = text + "\"" + this.labelFrmType.Text + "\"";
					this.log2file(text);
				}
			}
			else if (this.rx_mode)
			{
				this.rx_mode = false;
				this.lightCtl(false);
				this.memScanForm.exitRxMode();
				this.clrTimer.Enabled = true;
				this.log_strobe = false;
			}
			sbyte b = (sbyte)data[idx + 3];
			this.dbm = (int)b;
			this.dbPanel.setDbm(this.dbm);
			this.dbPanel.Refresh();
		}

		public void dmrSetStats(byte[] data, int idx, int size)
		{
			if (size > 20 && !this.updTimer.Enabled && !this.uspTimer.Enabled)
			{
				uint uInt = this.getUInt32(data, idx + 20);
				if (uInt != this.curFreq)
				{
					this.curFreq = uInt;
					this.printFreqNoUpdt();
				}
			}
			if (data[idx] == 2)
			{
				if (!this.rx_mode)
				{
					this.rx_mode = true;
					this.lightCtl(true);
					this.memScanForm.enterRxMode();
					this.clrTimer.Enabled = false;
					this.log_strobe = true;
				}
				if (!this.nac_dec)
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString("X2");
				}
				else
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString();
				}
				if (!this.ids_dec)
				{
					this.labelSrcId.Text = this.getUInt32(data, idx + 12).ToString("X6");
					this.labelTgtId.Text = this.getUInt32(data, idx + 16).ToString("X6");
				}
				else
				{
					this.labelSrcId.Text = this.getUInt32(data, idx + 12).ToString();
					this.labelTgtId.Text = this.getUInt32(data, idx + 16).ToString();
				}
				if ((data[idx + 2] & 16) == 16)
				{
					this.labelCallType.Text = "INDIVIDUAL";
				}
				else
				{
					this.labelCallType.Text = "GROUP";
				}
				if ((data[idx + 2] & 8) == 8)
				{
					this.labelEncType.Text = "PRIVATE";
				}
				else
				{
					this.labelEncType.Text = "PUBLIC";
				}
				this.labelEncKey.Text = this.getUInt16(data, idx + 10).ToString();
				if ((data[idx + 2] & 1) == 1)
				{
					if ((data[idx + 2] & 2) == 2)
					{
						this.labelFrmType.Text = "2";
					}
					else
					{
						this.labelFrmType.Text = "1";
					}
					if ((data[idx + 2] & 4) == 4)
					{
						this.labelMfid.Text = "BUSY";
					}
					else
					{
						this.labelMfid.Text = "IDLE";
					}
				}
				else
				{
					this.labelFrmType.Text = "";
					this.labelMfid.Text = "";
				}
				if (this.logging && this.log_strobe)
				{
					this.log_strobe = false;
					string text = "\"" + DateTime.Now.ToString() + "\";";
					text = text + "\"" + this.textBoxFreq.Text + "\";";
					text = text + "\"" + this.labelAi.Text + "\";";
					text = text + "\"" + this.labelNac.Text + "\";";
					text = text + "\"" + this.labelCallType.Text + "\";";
					text = text + "\"" + this.labelSrcId.Text + "\";";
					text = text + "\"" + this.labelTgtId.Text + "\";";
					text = text + "\"" + this.labelEncType.Text + "\";";
					text = text + "\"" + this.labelEncKey.Text + "\";";
					text += "\"\";";
					text = text + "\"" + this.labelEncKey.Text + "\";";
					text = text + "\"" + this.labelFrmType.Text + "\"";
					this.log2file(text);
				}
			}
			else if (this.rx_mode)
			{
				this.rx_mode = false;
				this.lightCtl(false);
				this.memScanForm.exitRxMode();
				this.clrTimer.Enabled = true;
				this.log_strobe = false;
			}
			sbyte b = (sbyte)data[idx + 3];
			this.dbm = (int)b;
			this.dbPanel.setDbm(this.dbm);
			this.dbPanel.Refresh();
		}

		public void p25SetStats(byte[] data, int idx, int size)
		{
			if (size > 20 && !this.updTimer.Enabled && !this.uspTimer.Enabled)
			{
				uint uInt = this.getUInt32(data, idx + 20);
				if (uInt != this.curFreq)
				{
					this.curFreq = uInt;
					this.printFreqNoUpdt();
				}
			}
			if (data[idx] == 2)
			{
				if (!this.rx_mode)
				{
					this.rx_mode = true;
					this.lightCtl(true);
					this.memScanForm.enterRxMode();
					this.clrTimer.Enabled = false;
					this.log_strobe = true;
				}
				if (data[idx + 1] == 128)
				{
					this.labelEncType.Text = "PUBLIC";
				}
				else if (data[idx + 1] == 129)
				{
					this.labelEncType.Text = "DES";
				}
				else if (data[idx + 1] == 131)
				{
					this.labelEncType.Text = "TRIPLE DES";
				}
				else if (data[idx + 1] == 132)
				{
					this.labelEncType.Text = "AES - 256";
				}
				else if (data[idx + 1] == 133)
				{
					this.labelEncType.Text = "AES - 128";
				}
				else if (data[idx + 1] == 160)
				{
					this.labelEncType.Text = "DVI - XL";
				}
				else if (data[idx + 1] == 161)
				{
					this.labelEncType.Text = "DVP - XL";
				}
				else if (data[idx + 1] == 170)
				{
					this.labelEncType.Text = "ADP";
				}
				else if (data[idx + 1] == 0)
				{
					this.labelEncType.Text = "ACCORDION";
				}
				else if (data[idx + 1] == 1)
				{
					this.labelEncType.Text = "BATON EVEN";
				}
				else if (data[idx + 1] == 2)
				{
					this.labelEncType.Text = "FIREFLY";
				}
				else if (data[idx + 1] == 3)
				{
					this.labelEncType.Text = "MAYFLY";
				}
				else if (data[idx + 1] == 4)
				{
					this.labelEncType.Text = "SAVILLE";
				}
				else if (data[idx + 1] == 65)
				{
					this.labelEncType.Text = "BATON ODD";
				}
				else
				{
					this.labelEncType.Text = "= " + data[idx + 1].ToString();
				}
				if (data[idx + 2] == 0)
				{
					this.labelFrmType.Text = "HEADER";
				}
				else if (data[idx + 2] == 1)
				{
					this.labelFrmType.Text = "LDU1";
				}
				else if (data[idx + 2] == 2)
				{
					this.labelFrmType.Text = "LDU2";
				}
				else if (data[idx + 2] == 3)
				{
					this.labelFrmType.Text = "TERM";
				}
				else if (data[idx + 2] == 4)
				{
					this.labelFrmType.Text = "TSBK";
				}
				else if (data[idx + 2] == 5)
				{
					this.labelFrmType.Text = "PDU";
				}
				else
				{
					this.labelFrmType.Text = "= " + data[idx + 2].ToString();
				}
				sbyte b = (sbyte)data[idx + 3];
				this.dbm = (int)b;
				this.dbPanel.setDbm(this.dbm);
				this.dbPanel.Refresh();
				if (data[idx + 5] == 0)
				{
					this.labelMfid.Text = "STANDARD";
				}
				else if (data[idx + 5] == 16)
				{
					this.labelMfid.Text = "BK RADIO";
				}
				else if (data[idx + 5] == 32)
				{
					this.labelMfid.Text = "CYCOMM";
				}
				else if (data[idx + 5] == 40)
				{
					this.labelMfid.Text = "EFRATOM";
				}
				else if (data[idx + 5] == 48)
				{
					this.labelMfid.Text = "ERICSSON";
				}
				else if (data[idx + 5] == 64)
				{
					this.labelMfid.Text = "EFJOHNSON";
				}
				else if (data[idx + 5] == 72)
				{
					this.labelMfid.Text = "GARMIN";
				}
				else if (data[idx + 5] == 80)
				{
					this.labelMfid.Text = "GTE";
				}
				else if (data[idx + 5] == 96)
				{
					this.labelMfid.Text = "MARCONI";
				}
				else if (data[idx + 5] == 112)
				{
					this.labelMfid.Text = "GLENAYRE";
				}
				else if (data[idx + 5] == 116)
				{
					this.labelMfid.Text = "J. R. CO";
				}
				else if (data[idx + 5] == 120)
				{
					this.labelMfid.Text = "KOKUSAI";
				}
				else if (data[idx + 5] == 124)
				{
					this.labelMfid.Text = "MAXON";
				}
				else if (data[idx + 5] == 128)
				{
					this.labelMfid.Text = "MIDLAND";
				}
				else if (data[idx + 5] == 144)
				{
					this.labelMfid.Text = "MOTOROLA";
				}
				else if (data[idx + 5] == 160)
				{
					this.labelMfid.Text = "RACAL";
				}
				else if (data[idx + 5] == 176)
				{
					this.labelMfid.Text = "RAYTHEON";
				}
				else if (data[idx + 5] == 192)
				{
					this.labelMfid.Text = "SEA";
				}
				else if (data[idx + 5] == 200)
				{
					this.labelMfid.Text = "SECURICOR";
				}
				else if (data[idx + 5] == 208)
				{
					this.labelMfid.Text = "STANILITE";
				}
				else if (data[idx + 5] == 224)
				{
					this.labelMfid.Text = "TELETEC";
				}
				else if (data[idx + 5] == 240)
				{
					this.labelMfid.Text = "TRANSCRYPT";
				}
				else
				{
					this.labelMfid.Text = "= " + data[idx + 5].ToString();
				}
				if (data[idx + 6] == 1)
				{
					this.labelCallType.Text = "INDIVIDUAL";
				}
				else
				{
					this.labelCallType.Text = "GROUP";
				}
				this.labelEncKey.Text = this.getUInt16(data, idx + 10).ToString();
				if (!this.nac_dec)
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString("X3");
				}
				else
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString();
				}
				if (!this.ids_dec)
				{
					this.labelSrcId.Text = this.getUInt32(data, idx + 12).ToString("X6");
					if (data[idx + 6] == 1)
					{
						this.labelTgtId.Text = this.getUInt32(data, idx + 16).ToString("X6");
					}
					else
					{
						this.labelTgtId.Text = this.getUInt32(data, idx + 16).ToString("X4");
					}
				}
				else
				{
					this.labelSrcId.Text = this.getUInt32(data, idx + 12).ToString();
					this.labelTgtId.Text = this.getUInt32(data, idx + 16).ToString();
				}
				if (this.logging && this.log_strobe)
				{
					this.log_strobe = false;
					string text = "\"" + DateTime.Now.ToString() + "\";";
					text = text + "\"" + this.textBoxFreq.Text + "\";";
					text = text + "\"" + this.labelAi.Text + "\";";
					text = text + "\"" + this.labelNac.Text + "\";";
					text = text + "\"" + this.labelCallType.Text + "\";";
					text = text + "\"" + this.labelSrcId.Text + "\";";
					text = text + "\"" + this.labelTgtId.Text + "\";";
					text = text + "\"" + this.labelEncType.Text + "\";";
					text = text + "\"" + this.labelEncKey.Text + "\";";
					text = text + "\"" + this.labelMfid.Text + "\";";
					text += "\"\";\"\"";
					this.log2file(text);
				}
			}
			else if (data[idx] == 4 || data[idx] == 5)
			{
				if (!this.rx_mode)
				{
					this.rx_mode = true;
					this.lightCtl(true);
					this.memScanForm.enterRxMode();
					this.clrTimer.Enabled = false;
				}
				this.trunkForm.tt_nac(this.getUInt16(data, idx + 8));
				if (!this.nac_dec)
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString("X3");
				}
				else
				{
					this.labelNac.Text = this.getUInt16(data, idx + 8).ToString();
				}
				if (data[idx] == 4)
				{
					this.labelFrmType.Text = "TSBK";
				}
				else
				{
					this.labelFrmType.Text = "PDU";
				}
				sbyte b2 = (sbyte)data[idx + 3];
				this.dbm = (int)b2;
				this.dbPanel.setDbm(this.dbm);
				this.dbPanel.Refresh();
				this.labelCallType.Text = (this.labelMfid.Text = "");
				this.labelSrcId.Text = (this.labelTgtId.Text = "");
				this.labelEncType.Text = (this.labelEncKey.Text = "");
			}
			else
			{
				if (this.rx_mode)
				{
					this.rx_mode = false;
					this.lightCtl(false);
					this.memScanForm.exitRxMode();
					this.clrTimer.Enabled = true;
					this.log_strobe = false;
				}
				sbyte b3 = (sbyte)data[idx + 3];
				this.dbm = (int)b3;
				this.dbPanel.setDbm(this.dbm);
				this.dbPanel.Refresh();
			}
			this.trunkForm.curRssi(this.dbm);
		}

		public void comDataRdy(byte code, byte[] data, int size)
		{
			if (this.lnkTimer.Enabled)
			{
				this.timerLinkStart(3000);
			}
			if (code == 0)
			{
				if (!this.lnkTimer.Enabled)
				{
					ushort uInt = this.getUInt16(data, 10);
					if (uInt != 1 && uInt != 4 && uInt != 5)
					{
						return;
					}
					if (uInt == 1)
					{
						this.minFreq = 410000000u;
						this.maxFreq = 480000000u;
					}
					else if (uInt == 5)
					{
						this.minFreq = 820000000u;
						this.maxFreq = 960000000u;
					}
					else
					{
						this.minFreq = 139000000u;
						this.maxFreq = 192000000u;
					}
					this.showDevInfo(data, 0);
					this.scanForm.deviceConnEvt();
					if (data[8] >= 4)
					{
						this.comParamsRead();
					}
					this.timerLinkStart(3000);
					return;
				}
			}
			else if (code == 1)
			{
				if (this.lnkTimer.Enabled)
				{
					ushort uInt2 = this.getUInt16(this.dev, 24);
					if (data[7] == 255)
					{
						data[7] = 1;
					}
					if (this.ai != data[7] && (this.memScanForm.isScanning() || (uInt2 & 512) == 512))
					{
						this.ai = data[7];
						this.switchAi(false);
					}
					if (size >= 44)
					{
						uint uInt3 = this.getUInt32(data, 44);
						ushort uInt4 = this.getUInt16(data, 48);
						double num = (double)uInt4 / uInt3 * 100.0;
						num = Math.Round(num / 5.0);
						if (num > 19.0)
						{
							num = 19.0;
						}
						this.vuPanel.setPL((int)num);
					}
					if (size >= 50)
					{
						double num2 = (double)this.getUInt16(data, 50);
						num2 = 20.0 * Math.Log10(num2 / 65536.0);
						num2 = Math.Round((num2 + 100.0) / 5.0);
						if (num2 > 19.0)
						{
							num2 = 19.0;
						}
						this.vuPanel.setVU((int)num2);
					}
					if (this.ai == 0)
					{
						this.p25SetStats(data, 0, size);
						return;
					}
					if (this.ai < 4)
					{
						this.dmrSetStats(data, 0, size);
						return;
					}
					if (this.ai == 4)
					{
						this.sfSetStats(data, 0, size);
						return;
					}
					if (this.ai == 5 || this.ai == 6)
					{
						this.nxdnSetStats(data, 0, size);
						return;
					}
				}
			}
			else if (code == 2)
			{
				this.tx_op = 255;
				this.timerStop();
				this.memcpy(this.dev, (int)(172 + this.mem2read * 40), data, 0, size);
				this.sfForm.chanReadEvt(this.mem2read);
				this.mem2read += 1;
				if (this.mem2read < 150)
				{
					this.comMemRead(this.mem2read);
					return;
				}
			}
			else
			{
				if (code == 3)
				{
					this.tx_op = 255;
					this.timerStop();
					this.sfForm.resStore(true);
					this.scanForm.devStore();
					return;
				}
				if (code == 8)
				{
					this.tx_op = 255;
					this.timerStop();
					this.mem2read += 1;
					if (this.mem2read < 149)
					{
						this.comMemSend(this.mem2read);
						return;
					}
					if (this.mem2read < 150)
					{
						this.comMemWrite(this.mem2read);
						return;
					}
				}
				else
				{
					if (code == 4)
					{
						this.tx_op = 255;
						this.timerStop();
						this.memcpy(this.dev, 0, data, 0, size);
						this.groupForm.setData(this.dev);
						this.autoScanForm.setData(this.dev);
						this.trunkForm.putData(this.dev);
						this.smForm.setData(this.dev);
						this.trunkForm.putData(this.dev);
						this.putParams(this.dev, 0);
						this.printFreqNoUpdt();
						this.timerUpdateStop();
						this.mem2read = 0;
						this.comMemRead(this.mem2read);
						return;
					}
					if (code == 5)
					{
						this.tx_op = 255;
						this.timerStop();
						this.groupForm.resStore(true);
						this.smForm.resStore(true);
						this.autoScanForm.resStore(true);
						this.trunkForm.resStore(true);
						return;
					}
					if (code == 6)
					{
						this.tx_op = 255;
						this.timerStop();
						return;
					}
					if (code == 12)
					{
						this.tx_op = 255;
						this.timerStop();
						return;
					}
					if (code == 7)
					{
						return;
					}
					if (code == 9)
					{
						this.scanForm.devData(data);
						return;
					}
					if (code == 13)
					{
						this.trunkForm.putTSBK(data);
						return;
					}
					if (code == 14)
					{
						this.trunkForm.putPDU(data, size);
						return;
					}
					if (code == 15)
					{
						this.tx_op = 255;
						this.timerStop();
						return;
					}
					if (code == 16)
					{
						this.trunkForm.ttEvt(data);
						return;
					}
					if (code == 17)
					{
						this.tx_op = 255;
						this.timerStop();
						return;
					}
					if (code == 18)
					{
						this.tx_op = 255;
						this.timerStop();
						this.trunkForm.filtSet(data);
						return;
					}
					if (code == 19)
					{
						this.tx_op = 255;
						this.timerStop();
						return;
					}
					if (code == 20)
					{
						this.tx_op = 255;
						this.timerStop();
						this.decryptForm.resStore(true);
						return;
					}
					if (code == 50 || code == 51)
					{
						this.timerStop();
						this.tx_op = 255;
						if (data[0] != 0)
						{
							this.statusMsg("Upgrade error", Color.Red);
							return;
						}
						if (this.upg_ptr >= this.upg_size)
						{
							this.statusMsg("Upgrade in progress... Please wait", Color.Lime);
							this.comResetUpg();
							return;
						}
						int num3 = 256;
						byte[] array = new byte[256];
						string txt = "Upgrade: " + (this.upg_ptr * 100 / this.upg_size).ToString() + "%";
						this.statusMsg(txt, Color.Lime);
						int num4 = this.upg_size - this.upg_ptr;
						if (num3 > num4)
						{
							num3 = num4;
						}
						this.memcpy(array, 0, this.upg_buff, this.upg_ptr, num3);
						bool flag = this.usp.uspSend(array, (ushort)num3, 51);
						if (flag)
						{
							this.tx_op = 51;
							this.upg_ptr += num3;
							this.timerStart(3000);
						}
					}
				}
			}
		}

		public void comDisc()
		{
		}

		public void comConn()
		{
		}

		private void stepPrint()
		{
			if (this.curStep == 0)
			{
				this.labelStep.Text = "100 Hz";
				return;
			}
			if (this.curStep == 1)
			{
				this.labelStep.Text = "250 Hz";
				return;
			}
			if (this.curStep == 2)
			{
				this.labelStep.Text = "500 Hz";
				return;
			}
			if (this.curStep == 3)
			{
				this.labelStep.Text = "1000 Hz";
				return;
			}
			if (this.curStep == 4)
			{
				this.labelStep.Text = "2,5 kHz";
				return;
			}
			if (this.curStep == 5)
			{
				this.labelStep.Text = "3,125 kHz";
				return;
			}
			if (this.curStep == 6)
			{
				this.labelStep.Text = "5 kHz";
				return;
			}
			if (this.curStep == 7)
			{
				this.labelStep.Text = "6,25 kHz";
				return;
			}
			if (this.curStep == 8)
			{
				this.labelStep.Text = "8,333 kHz";
				return;
			}
			if (this.curStep == 9)
			{
				this.labelStep.Text = "9 kHz";
				return;
			}
			if (this.curStep == 10)
			{
				this.labelStep.Text = "10 kHz";
				return;
			}
			if (this.curStep == 11)
			{
				this.labelStep.Text = "12,5 kHz";
				return;
			}
			if (this.curStep == 12)
			{
				this.labelStep.Text = "20 kHz";
				return;
			}
			if (this.curStep == 13)
			{
				this.labelStep.Text = "25 kHz";
				return;
			}
			if (this.curStep == 14)
			{
				this.labelStep.Text = "30 kHz";
				return;
			}
			if (this.curStep == 15)
			{
				this.labelStep.Text = "50 kHz";
				return;
			}
			this.labelStep.Text = "100 kHz";
		}

		private void stepUp()
		{
			if (this.curStep < 16)
			{
				this.curStep += 1;
				this.stepPrint();
				int num = (int)this.curStep;
				this.regSetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "step", num);
			}
		}

		private void stepDown()
		{
			if (this.curStep > 0)
			{
				this.curStep -= 1;
				this.stepPrint();
				int num = (int)this.curStep;
				this.regSetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "step", num);
			}
		}

		public string dbm2smetr(int val)
		{
			string result;
			if (val < -121)
			{
				result = "S0";
			}
			else if (val < -115)
			{
				result = "S1";
			}
			else if (val < -109)
			{
				result = "S2";
			}
			else if (val < -103)
			{
				result = "S3";
			}
			else if (val < -97)
			{
				result = "S4";
			}
			else if (val < -91)
			{
				result = "S5";
			}
			else if (val < -85)
			{
				result = "S6";
			}
			else if (val < -79)
			{
				result = "S7";
			}
			else if (val < -73)
			{
				result = "S8";
			}
			else if (val < -63)
			{
				result = "S9";
			}
			else if (val < -53)
			{
				result = "S9+10";
			}
			else if (val < -43)
			{
				result = "S9+20";
			}
			else if (val < -33)
			{
				result = "S9+30";
			}
			else if (val < -33)
			{
				result = "S9+40";
			}
			else
			{
				result = "S9+50";
			}
			return result;
		}

		public string att2smetr()
		{
			string text = "RF: ";
			if (this.dev[3] == 0)
			{
				text += "0 dB";
			}
			else if (this.dev[3] == 1)
			{
				text += "-6 dB";
			}
			else if (this.dev[3] == 2)
			{
				text += "-12 dB";
			}
			else if (this.dev[3] == 3)
			{
				text += "-18 dB";
			}
			else if (this.dev[3] == 4)
			{
				text += "-24 dB";
			}
			else if (this.dev[3] == 5)
			{
				text += "-30 dB";
			}
			else
			{
				text += "-39 dB";
			}
			return text;
		}

		public Point dbm_calc(int val, int w, int h)
		{
			Point result = default(Point);
			Point point = new Point((int)Math.Round(w / 2m * 0.60m), (int)Math.Round(h * 0.30m));
			Point point2 = new Point((int)Math.Round(w / 2m * 0.35m), (int)Math.Round(h * 0.50m));
			Point point3 = new Point(0, (int)Math.Round(h * 0.56m));
			Point point4 = new Point(-(int)Math.Round(w / 2m * 0.35m), (int)Math.Round(h * 0.50m));
			Point point5 = new Point(-(int)Math.Round(w / 2m * 0.60m), (int)Math.Round(h * 0.30m));
			if (val < -110)
			{
				double num = Math.Abs(-140.0 - (double)val) / 30.0;
				result.X = w / 2 - (int)Math.Round((double)point2.X * num + (double)point.X * (1.0 - num));
				result.Y = h - (int)Math.Round((double)point2.Y * num + (double)point.Y * (1.0 - num));
			}
			else if (val < -80)
			{
				double num = Math.Abs(-110.0 - (double)val) / 30.0;
				result.X = w / 2 - (int)Math.Round((double)point3.X * num + (double)point2.X * (1.0 - num));
				result.Y = h - (int)Math.Round((double)point3.Y * num + (double)point2.Y * (1.0 - num));
			}
			else if (val < -50)
			{
				double num = Math.Abs(-80.0 - (double)val) / 30.0;
				result.X = w / 2 - (int)Math.Round((double)point4.X * num + (double)point3.X * (1.0 - num));
				result.Y = h - (int)Math.Round((double)point4.Y * num + (double)point3.Y * (1.0 - num));
			}
			else
			{
				double num = Math.Abs(-50.0 - (double)val) / 30.0;
				result.X = w / 2 - (int)Math.Round((double)point5.X * num + (double)point4.X * (1.0 - num));
				result.Y = h - (int)Math.Round((double)point5.Y * num + (double)point4.Y * (1.0 - num));
			}
			return result;
		}

		private void panel_onpaint(object sender, PaintEventArgs e)
		{
			Panel panel = (Panel)sender;
			Graphics graphics = e.Graphics;
			Brush brush = new SolidBrush(this.mainColor);
			graphics.FillRectangle(brush, panel.ClientRectangle);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 4), (int)(this.mainColor.G * 3 / 4), (int)(this.mainColor.B * 3 / 4)), 1f), 0, 0, 0, panel.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 7), (int)(this.mainColor.G * 3 / 7), (int)(this.mainColor.B * 3 / 7)), 1f), 1, 1, 1, panel.Height - 2);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 4), (int)(this.mainColor.G * 3 / 4), (int)(this.mainColor.B * 3 / 4)), 1f), 0, 0, panel.Width - 1, 0);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 7), (int)(this.mainColor.G * 3 / 7), (int)(this.mainColor.B * 3 / 7)), 1f), 1, 1, panel.Width - 2, 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), panel.Width - 1, 0, panel.Width - 1, panel.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), panel.Width - 2, 1, panel.Width - 2, panel.Height - 2);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 0, panel.Height - 1, panel.Width - 1, panel.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, panel.Height - 2, panel.Width - 2, panel.Height - 2);
		}

		private void smeter_paint(object sender, PaintEventArgs e)
		{
		}

		private void arrowup_paint(object sender, PaintEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			Graphics graphics = e.Graphics;
			Point[] array = new Point[3];
			SolidBrush brush = new SolidBrush(Color.Lime);
			Brush brush2 = new SolidBrush(this.mainColor);
			graphics.FillRectangle(brush2, checkBox.ClientRectangle);
			if (checkBox.Checked)
			{
				array[0].X = checkBox.Width / 4 + 1;
				array[0].Y = checkBox.Height / 4 * 3 + 1;
				array[1].X = checkBox.Width / 4 * 3 + 1;
				array[1].Y = checkBox.Height / 4 * 3 + 1;
				array[2].X = checkBox.Width / 2 + 1;
				array[2].Y = checkBox.Height / 4 + 1;
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), 2, 2, 2, checkBox.Height - 2);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, checkBox.Width - 1, 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), 2, 2, checkBox.Width - 2, 2);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), checkBox.Width - 1, 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), checkBox.Width - 2, 2, checkBox.Width - 2, checkBox.Height - 2);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 1, checkBox.Height - 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 2, checkBox.Height - 2, checkBox.Width - 2, checkBox.Height - 2);
			}
			else
			{
				array[0].X = checkBox.Width / 4;
				array[0].Y = checkBox.Height / 4 * 3;
				array[1].X = checkBox.Width / 4 * 3;
				array[1].Y = checkBox.Height / 4 * 3;
				array[2].X = checkBox.Width / 2;
				array[2].Y = checkBox.Height / 4;
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 2, 2, 2, checkBox.Height - 2);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, checkBox.Width - 1, 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 2, 2, checkBox.Width - 2, 2);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), checkBox.Width - 1, 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), checkBox.Width - 2, 2, checkBox.Width - 2, checkBox.Height - 2);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), 1, checkBox.Height - 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 2, checkBox.Height - 2, checkBox.Width - 2, checkBox.Height - 2);
			}
			graphics.FillPolygon(brush, array);
		}

		private void checkBox_MouseDown(object sender, MouseEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			checkBox.Checked = true;
			if (checkBox == this.checkBoxStepUp)
			{
				this.stepUp();
				return;
			}
			if (checkBox == this.checkBoxStepDown)
			{
				this.stepDown();
				return;
			}
			if (checkBox == this.checkBoxMemUp)
			{
				this.memUp();
				return;
			}
			if (checkBox == this.checkBoxMemDown)
			{
				this.memDown();
				return;
			}
			if (checkBox == this.checkBoxAiUp)
			{
				this.aiUp();
				return;
			}
			if (checkBox == this.checkBoxAiDown)
			{
				this.aiDown();
				return;
			}
			if (checkBox == this.checkBoxVolUp)
			{
				this.volUp();
				return;
			}
			if (checkBox == this.checkBoxVolDown)
			{
				this.volDown();
				return;
			}
			if (checkBox == this.checkBoxGroupIDs)
			{
				checkBox.Checked = false;
				this.groupForm.ShowDialog();
				return;
			}
			if (checkBox == this.checkBoxExit)
			{
				checkBox.Checked = false;
				DialogResult dialogResult = MessageBox.Show("Are you sure ?", "Hardware Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (dialogResult == DialogResult.Yes)
				{
					this.comHwReset();
					return;
				}
			}
			else
			{
				if (checkBox == this.checkBoxAbout)
				{
					checkBox.Checked = false;
					MessageBox.Show("ADCR25 Tuner (v5.10)", "Ham Radio Works", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				if (checkBox == this.checkBoxSave)
				{
					checkBox.Checked = false;
					this.FileWrite();
					return;
				}
				if (checkBox == this.checkBoxLoad)
				{
					checkBox.Checked = false;
					bool flag = this.FileRead();
					if (flag)
					{
						this.mem2read = 0;
						this.comMemSend(this.mem2read);
						return;
					}
				}
				else
				{
					if (checkBox == this.checkBoxSmeter)
					{
						checkBox.Checked = false;
						this.smForm.ShowDialog();
						return;
					}
					if (checkBox == this.checkBoxTrunk)
					{
						checkBox.Checked = false;
						this.trunkForm.ShowDialog();
						return;
					}
					if (checkBox == this.checkBoxUpgrade)
					{
						checkBox.Checked = false;
						this.openFileDialogUpgrade.Filter = "ldr files (*.ldr)|*.ldr";
						this.openFileDialogUpgrade.FileName = "";
						this.upg_buff = this.FileReadUpgrade();
						if (this.upg_buff == null)
						{
							return;
						}
						this.upg_size = this.upg_buff.Length;
						byte[] array = new byte[256];
						this.memcpy(array, 0, this.upg_buff, 0, 256);
						bool flag2 = this.usp.uspSend(array, 256, 50);
						if (flag2)
						{
							this.tx_op = 50;
							this.upg_ptr = 256;
							this.statusMsg("Upgrade: 0%", Color.Lime);
							this.timerStart(10000);
							return;
						}
					}
					else
					{
						if (checkBox == this.checkBoxMemStore)
						{
							checkBox.Checked = false;
							this.sfForm.setData((int)this.curMem);
							this.sfForm.ShowDialog();
							return;
						}
						if (checkBox == this.checkBoxMemRecall)
						{
							checkBox.Checked = false;
							this.rfForm.setItems();
							this.rfForm.ShowDialog();
							return;
						}
						if (checkBox == this.checkBoxSearch)
						{
							checkBox.Checked = false;
							this.scanForm.ShowDialog();
							return;
						}
						if (checkBox == this.checkBoxMemScan)
						{
							checkBox.Checked = false;
							this.memScanForm.ShowDialog();
							return;
						}
						if (checkBox == this.checkBoxAutoScan)
						{
							checkBox.Checked = false;
							this.autoScanForm.ShowDialog();
							return;
						}
						if (checkBox == this.checkBoxDecrypt)
						{
							checkBox.Checked = false;
							this.decryptForm.ShowDialog();
							return;
						}
						if (checkBox == this.checkBoxLog)
						{
							checkBox.Checked = false;
							if (this.logging)
							{
								string s = "Log stop: " + DateTime.Now.ToString();
								this.checkBoxLog.Text = "LOG START";
								this.log2file(s);
								this.log_file.Close();
								this.logging = false;
								return;
							}
							if (this.saveFileDialogLog.ShowDialog() == DialogResult.OK)
							{
								try
								{
									this.log_file = new StreamWriter(this.saveFileDialogLog.FileName);
									string s2 = "Log start: " + DateTime.Now.ToString();
									this.checkBoxLog.Text = "LOG STOP";
									this.logging = true;
									this.log2file(s2);
									this.log2file("Stamp;Frequency (MHz);Air interface;NAC/CC;\"Call Type\";Source;Target;\"Encryption type\";\"Encryption key\";Manufacturer;\"Time slot\";\"CC state\"");
								}
								catch (Exception)
								{
								}
							}
						}
					}
				}
			}
		}

		private void checkBox_MouseUp(object sender, MouseEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			checkBox.Checked = false;
			if (checkBox == this.checkBoxSave)
			{
				this.FileWrite();
				return;
			}
			if (checkBox == this.checkBoxLoad)
			{
				this.FileRead();
			}
		}

		public void evtClosing()
		{
			if (this.logging)
			{
				string s = "Log stop: " + DateTime.Now.ToString();
				this.checkBoxLog.Text = "LOG START";
				this.log2file(s);
				this.log_file.Close();
				this.logging = false;
			}
		}

		private void log2file(string s)
		{
			if (this.logging)
			{
				try
				{
					this.log_file.WriteLine(s);
					this.log_file.Flush();
				}
				catch (Exception)
				{
				}
			}
		}

		private void p25recvForm_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			Form form = (Form)sender;
			Rectangle clientRectangle = form.ClientRectangle;
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R + 50), (int)(this.mainColor.G + 50), (int)(this.mainColor.B + 50)), 1f), 0, 0, 0, clientRectangle.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R + 50), (int)(this.mainColor.G + 50), (int)(this.mainColor.B + 50)), 1f), 1, 1, 1, clientRectangle.Height - 2);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R + 50), (int)(this.mainColor.G + 50), (int)(this.mainColor.B + 50)), 1f), 2, 2, 2, clientRectangle.Height - 3);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R + 50), (int)(this.mainColor.G + 50), (int)(this.mainColor.B + 50)), 1f), 0, 0, clientRectangle.Width - 1, 0);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R + 50), (int)(this.mainColor.G + 50), (int)(this.mainColor.B + 50)), 1f), 1, 1, clientRectangle.Width - 2, 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R + 50), (int)(this.mainColor.G + 50), (int)(this.mainColor.B + 50)), 1f), 2, 2, clientRectangle.Width - 3, 2);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R - 40), (int)(this.mainColor.G - 40), (int)(this.mainColor.B - 40)), 1f), clientRectangle.Width - 1, 0, clientRectangle.Width - 1, clientRectangle.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R - 40), (int)(this.mainColor.G - 40), (int)(this.mainColor.B - 40)), 1f), clientRectangle.Width - 2, 1, clientRectangle.Width - 2, clientRectangle.Height - 2);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R - 40), (int)(this.mainColor.G - 40), (int)(this.mainColor.B - 40)), 1f), clientRectangle.Width - 3, 2, clientRectangle.Width - 3, clientRectangle.Height - 3);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R - 40), (int)(this.mainColor.G - 40), (int)(this.mainColor.B - 40)), 1f), 0, clientRectangle.Height - 1, clientRectangle.Width - 1, clientRectangle.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R - 40), (int)(this.mainColor.G - 40), (int)(this.mainColor.B - 40)), 1f), 1, clientRectangle.Height - 2, clientRectangle.Width - 2, clientRectangle.Height - 2);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R - 40), (int)(this.mainColor.G - 40), (int)(this.mainColor.B - 40)), 1f), 2, clientRectangle.Height - 3, clientRectangle.Width - 3, clientRectangle.Height - 3);
		}

		private void textbutton_paint(object sender, PaintEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			Graphics graphics = e.Graphics;
			SolidBrush brush = new SolidBrush(Color.Lime);
			Brush brush2 = new SolidBrush(this.mainColor);
			Font font = new Font("Arial", 8f, FontStyle.Bold);
			SizeF sizeF = graphics.MeasureString(checkBox.Text, font);
			Rectangle arg_4D_0 = checkBox.ClientRectangle;
			graphics.FillRectangle(brush2, checkBox.ClientRectangle);
			if (checkBox.Checked)
			{
				graphics.DrawString(checkBox.Text, font, brush, ((float)checkBox.Width - sizeF.Width) / 2f + 2f, ((float)checkBox.Height - sizeF.Height) / 2f + 1f);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, checkBox.Width - 1, 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), checkBox.Width - 1, 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 1, checkBox.Height - 1, checkBox.Width - 1, checkBox.Height - 1);
				return;
			}
			graphics.DrawString(checkBox.Text, font, brush, ((float)checkBox.Width - sizeF.Width) / 2f + 1f, ((float)checkBox.Height - sizeF.Height) / 2f + 0f);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, 1, checkBox.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, checkBox.Width - 1, 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), checkBox.Width - 1, 1, checkBox.Width - 1, checkBox.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), 1, checkBox.Height - 1, checkBox.Width - 1, checkBox.Height - 1);
		}

		private void arrowupthin_paint(object sender, PaintEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			Graphics graphics = e.Graphics;
			Point[] array = new Point[3];
			SolidBrush brush = new SolidBrush(Color.Lime);
			Brush brush2 = new SolidBrush(this.mainColor);
			graphics.FillRectangle(brush2, checkBox.ClientRectangle);
			if (checkBox.Checked)
			{
				array[0].X = checkBox.Width / 2 - 4 + 1;
				array[0].Y = checkBox.Height - 1 + 1;
				array[1].X = checkBox.Width / 2 + 4 + 1;
				array[1].Y = checkBox.Height - 1 + 1;
				array[2].X = checkBox.Width / 2 + 1;
				array[2].Y = 2;
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, checkBox.Width - 1, 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), checkBox.Width - 1, 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 1, checkBox.Height - 1, checkBox.Width - 1, checkBox.Height - 1);
			}
			else
			{
				array[0].X = checkBox.Width / 2 - 4;
				array[0].Y = checkBox.Height - 1;
				array[1].X = checkBox.Width / 2 + 4;
				array[1].Y = checkBox.Height - 1;
				array[2].X = checkBox.Width / 2;
				array[2].Y = 1;
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, checkBox.Width - 1, 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), checkBox.Width - 1, 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), 1, checkBox.Height - 1, checkBox.Width - 1, checkBox.Height - 1);
			}
			graphics.FillPolygon(brush, array);
		}

		private void arrowdownthin_paint(object sender, PaintEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			Graphics graphics = e.Graphics;
			Point[] array = new Point[3];
			SolidBrush brush = new SolidBrush(Color.Lime);
			Brush brush2 = new SolidBrush(this.mainColor);
			graphics.FillRectangle(brush2, checkBox.ClientRectangle);
			if (checkBox.Checked)
			{
				array[0].X = checkBox.Width / 2 - 3 + 1;
				array[0].Y = 3;
				array[1].X = checkBox.Width / 2 + 4 + 1;
				array[1].Y = 3;
				array[2].X = checkBox.Width / 2 + 1;
				array[2].Y = checkBox.Height - 1 + 1;
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, checkBox.Width - 1, 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), checkBox.Width - 1, 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 1, checkBox.Height - 1, checkBox.Width - 1, checkBox.Height - 1);
			}
			else
			{
				array[0].X = checkBox.Width / 2 - 3;
				array[0].Y = 2;
				array[1].X = checkBox.Width / 2 + 4;
				array[1].Y = 2;
				array[2].X = checkBox.Width / 2;
				array[2].Y = checkBox.Height - 1;
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, checkBox.Width - 1, 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), checkBox.Width - 1, 1, checkBox.Width - 1, checkBox.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), 1, checkBox.Height - 1, checkBox.Width - 1, checkBox.Height - 1);
			}
			graphics.FillPolygon(brush, array);
		}

		private void lnkTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.timerStop();
			this.tx_op = 255;
			this.timerLinkStop();
			this.hideDevInfo();
			this.labelCallType.Text = (this.labelNac.Text = "");
			this.labelSrcId.Text = (this.labelTgtId.Text = "");
			this.labelEncType.Text = (this.labelEncKey.Text = "");
			this.labelFrmType.Text = (this.labelMfid.Text = "");
			this.dbm = -135;
			this.dbPanel.setDbm(this.dbm);
			this.dbPanel.Refresh();
			this.clrTimer.Enabled = false;
			this.scanForm.devDisc();
			this.smForm.discEvt();
			this.sfForm.discEvt();
			this.groupForm.discEvt();
			this.trunkForm.discEvt();
			this.autoScanForm.discEvt();
		}

		private void updTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.timerUpdateStop();
			this.getParams();
			this.comParamsWrite();
		}

		private void clrTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.clrTimer.Enabled = false;
			this.labelCallType.Text = (this.labelNac.Text = "");
			this.labelSrcId.Text = (this.labelTgtId.Text = "");
			this.labelEncType.Text = (this.labelEncKey.Text = "");
			this.labelFrmType.Text = (this.labelMfid.Text = "");
			this.dbPanel.Refresh();
		}

		private void uspTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.timerStop();
			if (this.tx_op == 5)
			{
				this.autoScanForm.resStore(false);
				this.groupForm.resStore(false);
				this.smForm.resStore(false);
				this.trunkForm.resStore(false);
			}
			else if (this.tx_op == 3)
			{
				this.tx_op = 255;
				this.timerStop();
				this.sfForm.resStore(false);
			}
			else if (this.tx_op != 12)
			{
				if (this.tx_op == 20)
				{
					this.decryptForm.resStore(false);
				}
				else
				{
					byte arg_94_0 = this.tx_op;
				}
			}
			this.tx_op = 255;
		}

		private void trackBar1_ValueChanged(object sender, EventArgs e)
		{
			TrackBar trackBar = (TrackBar)sender;
			Label label = this.label1v;
			if (trackBar == this.trackBar1)
			{
				label = this.label1v;
			}
			if (trackBar == this.trackBar2)
			{
				label = this.label2v;
			}
			if (trackBar == this.trackBar3)
			{
				label = this.label3v;
			}
			if (trackBar == this.trackBar4)
			{
				label = this.label4v;
			}
			if (trackBar == this.trackBar5)
			{
				label = this.label5v;
			}
			if (trackBar == this.trackBar6)
			{
				label = this.label6v;
			}
			if (trackBar == this.trackBar7)
			{
				label = this.label7v;
			}
			if (trackBar == this.trackBar8)
			{
				label = this.label8v;
			}
			switch (trackBar.Value)
			{
			case 0:
				label.Text = "-12 dB";
				break;
			case 1:
				label.Text = "-9 dB";
				break;
			case 2:
				label.Text = "-6 dB";
				break;
			case 3:
				label.Text = "-3 dB";
				break;
			case 4:
				label.Text = "0 dB";
				break;
			case 5:
				label.Text = "+3 dB";
				break;
			case 6:
				label.Text = "+6 dB";
				break;
			case 7:
				label.Text = "+9 dB";
				break;
			case 8:
				label.Text = "+12 dB";
				break;
			}
			this.timerUpdateStart();
		}

		private void validVol(TextBox tb)
		{
			if (tb.Text == "")
			{
				tb.Text = this.curVol.ToString();
				return;
			}
			int num;
			int.TryParse(tb.Text, out num);
			if (num > 15)
			{
				num = 15;
			}
			if (num != (int)this.curVol)
			{
				this.curVol = (byte)num;
				this.volPrint();
				this.volUpdate();
			}
		}

		private void textBoxVol_KeyPress(object sender, KeyPressEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
			{
				e.Handled = true;
				return;
			}
			if (e.KeyChar == '\r')
			{
				e.Handled = true;
				this.validVol(tb);
			}
		}

		private void textBoxVol_Leave(object sender, EventArgs e)
		{
			TextBox tb = (TextBox)sender;
			this.validVol(tb);
		}

		private void textBoxVol_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Up)
			{
				this.volUp();
				return;
			}
			if (e.KeyCode == Keys.Down)
			{
				this.volDown();
			}
		}

		private void Volume_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				this.volUp();
				return;
			}
			this.volDown();
		}

		private void volUpdate()
		{
			this.dev[0] = this.curVol;
			this.timerUpdateStart();
		}

		private void volPrint()
		{
			this.textBoxVol.Text = this.curVol.ToString();
		}

		private void volUp()
		{
			if (this.curVol < 15)
			{
				this.curVol += 1;
				this.volPrint();
				this.volUpdate();
			}
		}

		private void volDown()
		{
			if (this.curVol > 0)
			{
				this.curVol -= 1;
				this.volPrint();
				this.volUpdate();
			}
		}

		private void validMem(TextBox tb)
		{
			if (tb.Text == "")
			{
				this.memPrint();
				return;
			}
			int num;
			int.TryParse(tb.Text, out num);
			if (num < 1)
			{
				num = 1;
			}
			if (num > 150)
			{
				num = 150;
			}
			num--;
			if (num != (int)this.curMem)
			{
				this.curMem = (byte)num;
				this.memPrint();
				this.memUpdate();
			}
		}

		private void textBoxMemory_KeyPress(object sender, KeyPressEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
			{
				e.Handled = true;
				return;
			}
			if (e.KeyChar == '\r')
			{
				e.Handled = true;
				this.validMem(tb);
			}
		}

		private void textBoxMemory_Leave(object sender, EventArgs e)
		{
			TextBox tb = (TextBox)sender;
			this.validMem(tb);
		}

		private void textBoxMemory_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Up)
			{
				this.memUp();
				return;
			}
			if (e.KeyCode == Keys.Down)
			{
				this.memDown();
			}
		}

		private void Memory_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				this.memUp();
				return;
			}
			this.memDown();
		}

		private void memUpdate()
		{
			int num = (int)(172 + 40 * this.curMem);
			byte b = this.dev[num + 19];
			ushort num2 = this.getUInt16(this.dev, 24);
			this.memcpy(this.dev, 28, this.dev, num + 24, 16);
			if ((b & 1) == 1)
			{
				num2 |= 128;
			}
			else
			{
				num2 &= 65407;
			}
			if ((b & 2) == 2)
			{
				num2 |= 256;
			}
			else
			{
				num2 &= 65279;
			}
			this.putUInt16(this.dev, 24, num2);
			this.groupForm.setNacs(this.dev);
			this.labelComment.Text = this.getStr(this.dev, num, 16);
			this.curFreq = this.getUInt32(this.dev, num + 20);
			this.putUInt32(this.dev, 12, this.curFreq);
			this.printFreqNoUpdt();
			this.ai = this.dev[num + 16];
			this.dev[2] = this.ai;
			this.switchAi(false);
			this.timerUpdateStart();
		}

		private void memPrint()
		{
			this.textBoxMemory.Text = ((int)(this.curMem + 1)).ToString();
		}

		private void memUp()
		{
			if (this.curMem < 149)
			{
				this.curMem += 1;
				this.memPrint();
				this.memUpdate();
			}
		}

		private void memDown()
		{
			if (this.curMem > 0)
			{
				this.curMem -= 1;
				this.memPrint();
				this.memUpdate();
			}
		}

		private void validFreq(TextBox tb)
		{
			decimal num = this.minFreq / 1000000u;
			decimal num2 = this.maxFreq / 1000000u;
			decimal d = this.str2freq(tb.Text);
			if (d == 0m)
			{
				d = num;
			}
			while (d < num)
			{
				d *= 10m;
				if (d > num2)
				{
					d = num;
				}
			}
			while (d > num2)
			{
				d /= 10m;
				if (d < num)
				{
					d = num2;
				}
			}
			tb.Text = d.ToString("N6");
			uint num3 = (uint)(d * 1000000m);
			if (num3 != this.curFreq)
			{
				this.curFreq = num3;
				this.labelComment.Text = "";
				this.timerUpdateStart();
			}
		}

		private void textBoxFreq_KeyPress(object sender, KeyPressEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
			{
				e.Handled = true;
				return;
			}
			if (e.KeyChar == ',' && (textBox.Text == "" || textBox.Text.IndexOf(',') > -1 || textBox.Text.IndexOf('.') > -1))
			{
				e.Handled = true;
				return;
			}
			if (e.KeyChar == '.' && (textBox.Text == "" || textBox.Text.IndexOf('.') > -1 || textBox.Text.IndexOf(',') > -1))
			{
				e.Handled = true;
				return;
			}
			if (textBox.Text.Length == 10 && !char.IsControl(e.KeyChar) && textBox.SelectionLength == 0 && textBox.SelectionStart < 10)
			{
				string text = textBox.Text;
				int selectionStart = textBox.SelectionStart;
				textBox.Text = text.Substring(0, selectionStart) + e.KeyChar.ToString() + text.Substring(selectionStart + 1, text.Length - selectionStart - 1);
				textBox.SelectionStart = selectionStart + 1;
				return;
			}
			if (e.KeyChar == '\r')
			{
				this.validFreq(textBox);
				e.Handled = true;
			}
		}

		private void textBoxFreq_Leave(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			textBox.Text = (this.curFreq / 1000000m).ToString("N6");
		}

		private void p25recvForm_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				if (this.checkBoxStepLock.Checked)
				{
					uint step = this.getStep();
					uint num = this.curFreq + step;
					if (num > this.maxFreq)
					{
						this.curFreq = this.maxFreq;
					}
					else
					{
						this.curFreq = this.freqAlign(num, step);
					}
					this.labelComment.Text = "";
					this.printFreq();
					return;
				}
				this.memUp();
				return;
			}
			else
			{
				if (this.checkBoxStepLock.Checked)
				{
					uint step2 = this.getStep();
					uint num2 = this.curFreq - step2;
					if (num2 < this.minFreq)
					{
						this.curFreq = this.minFreq;
					}
					else
					{
						this.curFreq = this.freqAlign(num2, step2);
					}
					this.labelComment.Text = "";
					this.printFreq();
					return;
				}
				this.memDown();
				return;
			}
		}

		private void stepmem_MouseUp(object sender, MouseEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			if (checkBox == this.checkBoxStepLock)
			{
				if (!checkBox.Checked && !this.checkBoxMemLock.Checked)
				{
					checkBox.Checked = true;
				}
				if (checkBox.Checked && this.checkBoxMemLock.Checked)
				{
					this.checkBoxMemLock.Checked = false;
					return;
				}
			}
			else if (checkBox == this.checkBoxMemLock)
			{
				if (!checkBox.Checked && !this.checkBoxStepLock.Checked)
				{
					checkBox.Checked = true;
				}
				if (checkBox.Checked && this.checkBoxStepLock.Checked)
				{
					this.checkBoxStepLock.Checked = false;
				}
				if (this.checkBoxMemLock.Checked)
				{
					this.memUpdate();
				}
			}
		}

		private bool FileRead()
		{
			if (this.openFileDialogMem.ShowDialog() == DialogResult.OK)
			{
				Stream stream = this.openFileDialogMem.OpenFile();
				if (stream != null)
				{
					if (stream.Length != 6004L)
					{
						return false;
					}
					byte[] array = new byte[stream.Length];
					try
					{
						stream.Read(array, 0, array.Length);
						uint num = this.crc32(array, 0, array.Length - 4);
						if (num != this.getUInt32(array, array.Length - 4))
						{
							bool result = false;
							return result;
						}
						this.memcpy(this.dev, 172, array, 0, 6000);
						this.mem2read = 0;
						this.comMemSend(0);
					}
					catch (Exception)
					{
						bool result = false;
						return result;
					}
					stream.Close();
					return true;
				}
			}
			return false;
		}

		private bool FileWrite()
		{
			if (this.saveFileDialogMem.ShowDialog() == DialogResult.OK)
			{
				byte[] array = new byte[6004];
				this.memcpy(array, 0, this.dev, 172, 6000);
				this.putUInt32(array, 6000, this.crc32(array, 0, 6000));
				Stream stream = this.saveFileDialogMem.OpenFile();
				if (stream != null)
				{
					try
					{
						stream.Write(array, 0, array.Length);
					}
					catch (Exception)
					{
						return false;
					}
					stream.Close();
					return true;
				}
			}
			return false;
		}

		private byte[] FileReadUpgrade()
		{
			byte[] array = null;
			if (this.openFileDialogUpgrade.ShowDialog() == DialogResult.OK)
			{
				Stream stream = this.openFileDialogUpgrade.OpenFile();
				if (stream == null)
				{
					return null;
				}
				if (stream.Length > 1048576L || stream.Length < 256L)
				{
					stream.Close();
					return null;
				}
				array = new byte[stream.Length];
				try
				{
					stream.Read(array, 0, array.Length);
				}
				catch (Exception)
				{
					array = null;
				}
				stream.Close();
			}
			return array;
		}

		private void p25recvForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Up)
			{
				if (this.checkBoxStepLock.Checked)
				{
					uint step = this.getStep();
					uint num = this.curFreq + step;
					if (num > this.maxFreq)
					{
						this.curFreq = this.maxFreq;
					}
					else
					{
						this.curFreq = this.freqAlign(num, step);
					}
					this.labelComment.Text = "";
					this.printFreq();
					return;
				}
				this.memUp();
				return;
			}
			else
			{
				if (e.KeyCode != Keys.Down)
				{
					Keys arg_DD_0 = e.KeyCode;
					return;
				}
				if (this.checkBoxStepLock.Checked)
				{
					uint step2 = this.getStep();
					uint num2 = this.curFreq - step2;
					if (num2 < this.minFreq)
					{
						this.curFreq = this.minFreq;
					}
					else
					{
						this.curFreq = this.freqAlign(num2, step2);
					}
					this.labelComment.Text = "";
					this.printFreq();
					return;
				}
				this.memDown();
				return;
			}
		}

		private void hexdec_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void checkBoxHex_MouseDown(object sender, MouseEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			this.nac_dec = !checkBox.Checked;
			this.regSetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "nac", this.nac_dec);
		}

		private void checkBoxDec_MouseDown(object sender, MouseEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			this.ids_dec = !checkBox.Checked;
			this.regSetValue("SOFTWARE\\Ham Radio Works\\ADCR25", "ids", this.ids_dec);
		}

		private void checkBoxMemUp_KeyDown(object sender, KeyEventArgs e)
		{
			this.memUp();
		}

		private void checkBoxMemDown_KeyDown(object sender, KeyEventArgs e)
		{
			this.memDown();
		}

		private void checkBoxVolUp_KeyDown(object sender, KeyEventArgs e)
		{
			this.volUp();
		}

		private void checkBoxVolDown_KeyDown(object sender, KeyEventArgs e)
		{
			this.volDown();
		}

		private void checkBoxAiUp_KeyDown(object sender, KeyEventArgs e)
		{
			this.aiUp();
		}

		private void checkBoxAiDown_KeyDown(object sender, KeyEventArgs e)
		{
			this.aiDown();
		}

		private void p25recvForm_Shown(object sender, EventArgs e)
		{
			this.checkBoxAbout.Focus();
		}

		private void p25recvForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.usp.stopPortMonitor();
			this.scanForm.evtClosing();
			this.trunkForm.evtClosing();
			this.evtClosing();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(p25recvForm));
			this.panelTL = new Panel();
			this.panelInd = new Panel();
			this.panelComments = new Panel();
			this.labelComment = new Label();
			this.panelFreq = new Panel();
			this.labelMHz = new Label();
			this.textBoxFreq = new TextBox();
			this.panelCtrl = new Panel();
			this.checkBoxSmeter = new CheckBox();
			this.checkBoxDecrypt = new CheckBox();
			this.checkBoxUpgrade = new CheckBox();
			this.checkBoxHex = new CheckBox();
			this.checkBoxStepLock = new CheckBox();
			this.checkBoxMemScan = new CheckBox();
			this.checkBoxTrunk = new CheckBox();
			this.checkBoxSearch = new CheckBox();
			this.checkBoxMemStore = new CheckBox();
			this.checkBoxMemRecall = new CheckBox();
			this.checkBoxDec = new CheckBox();
			this.checkBoxMemLock = new CheckBox();
			this.checkBoxSave = new CheckBox();
			this.checkBoxLoad = new CheckBox();
			this.panelVol = new Panel();
			this.textBoxVol = new TextBox();
			this.checkBoxVolDown = new CheckBox();
			this.checkBoxVolUp = new CheckBox();
			this.panelMem = new Panel();
			this.textBoxMemory = new TextBox();
			this.checkBoxMemDown = new CheckBox();
			this.checkBoxMemUp = new CheckBox();
			this.labelVol = new Label();
			this.labelMem = new Label();
			this.panelStep = new Panel();
			this.labelStep = new Label();
			this.checkBoxStepDown = new CheckBox();
			this.checkBoxStepUp = new CheckBox();
			this.labelStp = new Label();
			this.trackBar1 = new TrackBar();
			this.panelEq = new Panel();
			this.label8v = new Label();
			this.label7v = new Label();
			this.label6v = new Label();
			this.label5v = new Label();
			this.label4v = new Label();
			this.label3v = new Label();
			this.label2v = new Label();
			this.label17 = new Label();
			this.label16 = new Label();
			this.label15 = new Label();
			this.label14 = new Label();
			this.label13 = new Label();
			this.label12 = new Label();
			this.label11 = new Label();
			this.label10 = new Label();
			this.label1v = new Label();
			this.trackBar8 = new TrackBar();
			this.trackBar7 = new TrackBar();
			this.trackBar6 = new TrackBar();
			this.trackBar5 = new TrackBar();
			this.trackBar4 = new TrackBar();
			this.trackBar3 = new TrackBar();
			this.trackBar2 = new TrackBar();
			this.panelStatus = new Panel();
			this.panelConnStatus = new Panel();
			this.labelLink = new Label();
			this.checkBoxExit = new CheckBox();
			this.checkBoxLog = new CheckBox();
			this.checkBoxAutoScan = new CheckBox();
			this.checkBoxGroupIDs = new CheckBox();
			this.checkBoxAbout = new CheckBox();
			this.panelAi = new Panel();
			this.panelManuf = new Panel();
			this.labelMfid = new Label();
			this.panelFT = new Panel();
			this.labelFrmType = new Label();
			this.panelEC = new Panel();
			this.labelEncKey = new Label();
			this.panelET = new Panel();
			this.labelEncType = new Label();
			this.panelTgtId = new Panel();
			this.labelTgtId = new Label();
			this.panelSrcId = new Panel();
			this.labelSrcId = new Label();
			this.panelCT = new Panel();
			this.labelCallType = new Label();
			this.panelProto = new Panel();
			this.labelAi = new Label();
			this.checkBoxAiDown = new CheckBox();
			this.checkBoxAiUp = new CheckBox();
			this.panelNac = new Panel();
			this.labelNac = new Label();
			this.label21 = new Label();
			this.labelFT = new Label();
			this.labelCT = new Label();
			this.labelManuf = new Label();
			this.labelKey = new Label();
			this.labelPrivacy = new Label();
			this.labelTgt = new Label();
			this.labelSrc = new Label();
			this.labelAccCode = new Label();
			this.toolTipMainForm = new ToolTip(this.components);
			this.openFileDialogMem = new OpenFileDialog();
			this.openFileDialogUpgrade = new OpenFileDialog();
			this.saveFileDialogMem = new SaveFileDialog();
			this.saveFileDialogLog = new SaveFileDialog();
			this.panelGen = new Panel();
			this.panelVU = new Panel();
			this.panelInd.SuspendLayout();
			this.panelComments.SuspendLayout();
			this.panelFreq.SuspendLayout();
			this.panelCtrl.SuspendLayout();
			this.panelVol.SuspendLayout();
			this.panelMem.SuspendLayout();
			this.panelStep.SuspendLayout();
			((ISupportInitialize)this.trackBar1).BeginInit();
			this.panelEq.SuspendLayout();
			((ISupportInitialize)this.trackBar8).BeginInit();
			((ISupportInitialize)this.trackBar7).BeginInit();
			((ISupportInitialize)this.trackBar6).BeginInit();
			((ISupportInitialize)this.trackBar5).BeginInit();
			((ISupportInitialize)this.trackBar4).BeginInit();
			((ISupportInitialize)this.trackBar3).BeginInit();
			((ISupportInitialize)this.trackBar2).BeginInit();
			this.panelStatus.SuspendLayout();
			this.panelConnStatus.SuspendLayout();
			this.panelAi.SuspendLayout();
			this.panelManuf.SuspendLayout();
			this.panelFT.SuspendLayout();
			this.panelEC.SuspendLayout();
			this.panelET.SuspendLayout();
			this.panelTgtId.SuspendLayout();
			this.panelSrcId.SuspendLayout();
			this.panelCT.SuspendLayout();
			this.panelProto.SuspendLayout();
			this.panelNac.SuspendLayout();
			this.panelGen.SuspendLayout();
			base.SuspendLayout();
			this.panelTL.Location = new Point(279, 3);
			this.panelTL.Name = "panelTL";
			this.panelTL.Size = new Size(276, 100);
			this.panelTL.TabIndex = 1;
			this.panelTL.Paint += new PaintEventHandler(this.panel_onpaint);
			this.panelInd.Controls.Add(this.panelComments);
			this.panelInd.Controls.Add(this.panelFreq);
			this.panelInd.Location = new Point(3, 3);
			this.panelInd.Name = "panelInd";
			this.panelInd.Size = new Size(276, 100);
			this.panelInd.TabIndex = 3;
			this.panelInd.Paint += new PaintEventHandler(this.panel_onpaint);
			this.panelComments.Controls.Add(this.labelComment);
			this.panelComments.Location = new Point(10, 69);
			this.panelComments.Name = "panelComments";
			this.panelComments.Size = new Size(257, 21);
			this.panelComments.TabIndex = 1;
			this.panelComments.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelComment.BackColor = Color.Black;
			this.labelComment.ForeColor = Color.Lime;
			this.labelComment.Location = new Point(2, 2);
			this.labelComment.Name = "labelComment";
			this.labelComment.Size = new Size(253, 17);
			this.labelComment.TabIndex = 0;
			this.labelComment.TextAlign = ContentAlignment.MiddleCenter;
			this.panelFreq.Controls.Add(this.labelMHz);
			this.panelFreq.Controls.Add(this.textBoxFreq);
			this.panelFreq.Location = new Point(10, 10);
			this.panelFreq.Name = "panelFreq";
			this.panelFreq.Size = new Size(257, 45);
			this.panelFreq.TabIndex = 0;
			this.panelFreq.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelMHz.BackColor = Color.Black;
			this.labelMHz.Font = new Font("Arial", 17.25f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelMHz.ForeColor = Color.Lime;
			this.labelMHz.Location = new Point(195, 2);
			this.labelMHz.Name = "labelMHz";
			this.labelMHz.Size = new Size(60, 41);
			this.labelMHz.TabIndex = 2;
			this.labelMHz.Text = "MHz";
			this.labelMHz.TextAlign = ContentAlignment.MiddleCenter;
			this.textBoxFreq.BackColor = Color.Black;
			this.textBoxFreq.BorderStyle = BorderStyle.None;
			this.textBoxFreq.Font = new Font("Arial", 25f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxFreq.ForeColor = Color.Lime;
			this.textBoxFreq.Location = new Point(2, 2);
			this.textBoxFreq.MaxLength = 10;
			this.textBoxFreq.Multiline = true;
			this.textBoxFreq.Name = "textBoxFreq";
			this.textBoxFreq.Size = new Size(200, 41);
			this.textBoxFreq.TabIndex = 0;
			this.textBoxFreq.Text = "430,000000";
			this.textBoxFreq.WordWrap = false;
			this.textBoxFreq.KeyDown += new KeyEventHandler(this.p25recvForm_KeyDown);
			this.textBoxFreq.Leave += new EventHandler(this.textBoxFreq_Leave);
			this.textBoxFreq.KeyPress += new KeyPressEventHandler(this.textBoxFreq_KeyPress);
			this.panelCtrl.Controls.Add(this.checkBoxSmeter);
			this.panelCtrl.Controls.Add(this.checkBoxDecrypt);
			this.panelCtrl.Controls.Add(this.checkBoxUpgrade);
			this.panelCtrl.Controls.Add(this.checkBoxHex);
			this.panelCtrl.Controls.Add(this.checkBoxStepLock);
			this.panelCtrl.Controls.Add(this.checkBoxMemScan);
			this.panelCtrl.Controls.Add(this.checkBoxTrunk);
			this.panelCtrl.Controls.Add(this.checkBoxSearch);
			this.panelCtrl.Controls.Add(this.checkBoxMemStore);
			this.panelCtrl.Controls.Add(this.checkBoxMemRecall);
			this.panelCtrl.Controls.Add(this.checkBoxDec);
			this.panelCtrl.Controls.Add(this.checkBoxMemLock);
			this.panelCtrl.Controls.Add(this.checkBoxSave);
			this.panelCtrl.Controls.Add(this.checkBoxLoad);
			this.panelCtrl.Location = new Point(279, 192);
			this.panelCtrl.Name = "panelCtrl";
			this.panelCtrl.Size = new Size(276, 188);
			this.panelCtrl.TabIndex = 5;
			this.panelCtrl.Paint += new PaintEventHandler(this.panel_onpaint);
			this.checkBoxSmeter.Appearance = Appearance.Button;
			this.checkBoxSmeter.FlatStyle = FlatStyle.Flat;
			this.checkBoxSmeter.Location = new Point(90, 93);
			this.checkBoxSmeter.Name = "checkBoxSmeter";
			this.checkBoxSmeter.Size = new Size(74, 20);
			this.checkBoxSmeter.TabIndex = 13;
			this.checkBoxSmeter.Text = "RF SET.";
			this.checkBoxSmeter.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxSmeter, "RF settings");
			this.checkBoxSmeter.UseVisualStyleBackColor = true;
			this.checkBoxSmeter.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxSmeter.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxSmeter.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxDecrypt.Appearance = Appearance.Button;
			this.checkBoxDecrypt.FlatStyle = FlatStyle.Flat;
			this.checkBoxDecrypt.Location = new Point(9, 121);
			this.checkBoxDecrypt.Name = "checkBoxDecrypt";
			this.checkBoxDecrypt.Size = new Size(74, 20);
			this.checkBoxDecrypt.TabIndex = 13;
			this.checkBoxDecrypt.Text = "DECRYPT";
			this.checkBoxDecrypt.TextAlign = ContentAlignment.MiddleCenter;
			this.checkBoxDecrypt.UseVisualStyleBackColor = true;
			this.checkBoxDecrypt.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxDecrypt.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxDecrypt.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxUpgrade.Appearance = Appearance.Button;
			this.checkBoxUpgrade.FlatStyle = FlatStyle.Flat;
			this.checkBoxUpgrade.Location = new Point(9, 93);
			this.checkBoxUpgrade.Name = "checkBoxUpgrade";
			this.checkBoxUpgrade.Size = new Size(74, 20);
			this.checkBoxUpgrade.TabIndex = 13;
			this.checkBoxUpgrade.Text = "UPGRADE";
			this.checkBoxUpgrade.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxUpgrade, "Firmware upgrade");
			this.checkBoxUpgrade.UseVisualStyleBackColor = true;
			this.checkBoxUpgrade.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxUpgrade.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxUpgrade.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxHex.Appearance = Appearance.Button;
			this.checkBoxHex.FlatStyle = FlatStyle.Flat;
			this.checkBoxHex.Location = new Point(9, 37);
			this.checkBoxHex.Name = "checkBoxHex";
			this.checkBoxHex.Size = new Size(74, 20);
			this.checkBoxHex.TabIndex = 13;
			this.checkBoxHex.Text = "NAC DEC";
			this.checkBoxHex.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxHex, "NAC decimal display");
			this.checkBoxHex.UseVisualStyleBackColor = true;
			this.checkBoxHex.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxHex.MouseDown += new MouseEventHandler(this.checkBoxHex_MouseDown);
			this.checkBoxHex.MouseUp += new MouseEventHandler(this.hexdec_MouseUp);
			this.checkBoxStepLock.Appearance = Appearance.Button;
			this.checkBoxStepLock.Checked = true;
			this.checkBoxStepLock.CheckState = CheckState.Checked;
			this.checkBoxStepLock.FlatStyle = FlatStyle.Flat;
			this.checkBoxStepLock.Location = new Point(9, 9);
			this.checkBoxStepLock.Name = "checkBoxStepLock";
			this.checkBoxStepLock.Size = new Size(74, 20);
			this.checkBoxStepLock.TabIndex = 13;
			this.checkBoxStepLock.Text = "VFO";
			this.checkBoxStepLock.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxStepLock, "VFO mouse scroll");
			this.checkBoxStepLock.UseVisualStyleBackColor = true;
			this.checkBoxStepLock.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxStepLock.MouseUp += new MouseEventHandler(this.stepmem_MouseUp);
			this.checkBoxMemScan.Appearance = Appearance.Button;
			this.checkBoxMemScan.FlatStyle = FlatStyle.Flat;
			this.checkBoxMemScan.Location = new Point(169, 37);
			this.checkBoxMemScan.Name = "checkBoxMemScan";
			this.checkBoxMemScan.Size = new Size(95, 20);
			this.checkBoxMemScan.TabIndex = 13;
			this.checkBoxMemScan.Text = "MEM. SCAN";
			this.checkBoxMemScan.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxMemScan, "Memory scanning");
			this.checkBoxMemScan.UseVisualStyleBackColor = true;
			this.checkBoxMemScan.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxMemScan.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxMemScan.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxTrunk.Appearance = Appearance.Button;
			this.checkBoxTrunk.FlatStyle = FlatStyle.Flat;
			this.checkBoxTrunk.Location = new Point(169, 93);
			this.checkBoxTrunk.Name = "checkBoxTrunk";
			this.checkBoxTrunk.Size = new Size(95, 20);
			this.checkBoxTrunk.TabIndex = 13;
			this.checkBoxTrunk.Text = "P25 TRUNKING";
			this.checkBoxTrunk.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxTrunk, "P25 trunking");
			this.checkBoxTrunk.UseVisualStyleBackColor = true;
			this.checkBoxTrunk.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxTrunk.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxTrunk.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxSearch.Appearance = Appearance.Button;
			this.checkBoxSearch.FlatStyle = FlatStyle.Flat;
			this.checkBoxSearch.Location = new Point(169, 65);
			this.checkBoxSearch.Name = "checkBoxSearch";
			this.checkBoxSearch.Size = new Size(95, 20);
			this.checkBoxSearch.TabIndex = 13;
			this.checkBoxSearch.Text = "SEARCH";
			this.checkBoxSearch.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxSearch, "Searching");
			this.checkBoxSearch.UseVisualStyleBackColor = true;
			this.checkBoxSearch.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxSearch.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxSearch.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxMemStore.Appearance = Appearance.Button;
			this.checkBoxMemStore.FlatStyle = FlatStyle.Flat;
			this.checkBoxMemStore.Location = new Point(90, 65);
			this.checkBoxMemStore.Name = "checkBoxMemStore";
			this.checkBoxMemStore.Size = new Size(74, 20);
			this.checkBoxMemStore.TabIndex = 13;
			this.checkBoxMemStore.Text = "STORE";
			this.checkBoxMemStore.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxMemStore, "Store frequency");
			this.checkBoxMemStore.UseVisualStyleBackColor = true;
			this.checkBoxMemStore.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxMemStore.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxMemStore.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxMemRecall.Appearance = Appearance.Button;
			this.checkBoxMemRecall.FlatStyle = FlatStyle.Flat;
			this.checkBoxMemRecall.Location = new Point(9, 65);
			this.checkBoxMemRecall.Name = "checkBoxMemRecall";
			this.checkBoxMemRecall.Size = new Size(74, 20);
			this.checkBoxMemRecall.TabIndex = 13;
			this.checkBoxMemRecall.Text = "RECALL";
			this.checkBoxMemRecall.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxMemRecall, "Recall frequency");
			this.checkBoxMemRecall.UseVisualStyleBackColor = true;
			this.checkBoxMemRecall.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxMemRecall.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxMemRecall.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxDec.Appearance = Appearance.Button;
			this.checkBoxDec.FlatStyle = FlatStyle.Flat;
			this.checkBoxDec.Location = new Point(90, 37);
			this.checkBoxDec.Name = "checkBoxDec";
			this.checkBoxDec.Size = new Size(74, 20);
			this.checkBoxDec.TabIndex = 13;
			this.checkBoxDec.Text = "IDs DEC";
			this.checkBoxDec.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxDec, "IDs decimal display");
			this.checkBoxDec.UseVisualStyleBackColor = true;
			this.checkBoxDec.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxDec.MouseDown += new MouseEventHandler(this.checkBoxDec_MouseDown);
			this.checkBoxDec.MouseUp += new MouseEventHandler(this.hexdec_MouseUp);
			this.checkBoxMemLock.Appearance = Appearance.Button;
			this.checkBoxMemLock.FlatStyle = FlatStyle.Flat;
			this.checkBoxMemLock.Location = new Point(90, 9);
			this.checkBoxMemLock.Name = "checkBoxMemLock";
			this.checkBoxMemLock.Size = new Size(74, 20);
			this.checkBoxMemLock.TabIndex = 13;
			this.checkBoxMemLock.Text = "MEMORY";
			this.checkBoxMemLock.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxMemLock, "Memory mouse scroll");
			this.checkBoxMemLock.UseVisualStyleBackColor = true;
			this.checkBoxMemLock.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxMemLock.MouseUp += new MouseEventHandler(this.stepmem_MouseUp);
			this.checkBoxSave.Appearance = Appearance.Button;
			this.checkBoxSave.FlatStyle = FlatStyle.Flat;
			this.checkBoxSave.Location = new Point(219, 9);
			this.checkBoxSave.Name = "checkBoxSave";
			this.checkBoxSave.Size = new Size(45, 20);
			this.checkBoxSave.TabIndex = 13;
			this.checkBoxSave.Text = "SAVE";
			this.checkBoxSave.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxSave, "Save memory to file");
			this.checkBoxSave.UseVisualStyleBackColor = true;
			this.checkBoxSave.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxSave.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxSave.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxLoad.Appearance = Appearance.Button;
			this.checkBoxLoad.FlatStyle = FlatStyle.Flat;
			this.checkBoxLoad.Location = new Point(169, 9);
			this.checkBoxLoad.Name = "checkBoxLoad";
			this.checkBoxLoad.Size = new Size(45, 20);
			this.checkBoxLoad.TabIndex = 13;
			this.checkBoxLoad.Text = "LOAD";
			this.checkBoxLoad.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxLoad, "Load memory from file");
			this.checkBoxLoad.UseVisualStyleBackColor = true;
			this.checkBoxLoad.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxLoad.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxLoad.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.panelVol.Controls.Add(this.textBoxVol);
			this.panelVol.Controls.Add(this.checkBoxVolDown);
			this.panelVol.Controls.Add(this.checkBoxVolUp);
			this.panelVol.Location = new Point(172, 51);
			this.panelVol.Name = "panelVol";
			this.panelVol.Size = new Size(95, 19);
			this.panelVol.TabIndex = 14;
			this.panelVol.Paint += new PaintEventHandler(this.panel_onpaint);
			this.textBoxVol.BackColor = Color.Black;
			this.textBoxVol.BorderStyle = BorderStyle.None;
			this.textBoxVol.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxVol.ForeColor = Color.Lime;
			this.textBoxVol.Location = new Point(2, 2);
			this.textBoxVol.MaxLength = 2;
			this.textBoxVol.Multiline = true;
			this.textBoxVol.Name = "textBoxVol";
			this.textBoxVol.Size = new Size(76, 15);
			this.textBoxVol.TabIndex = 0;
			this.textBoxVol.Text = "0";
			this.textBoxVol.TextAlign = HorizontalAlignment.Center;
			this.textBoxVol.KeyDown += new KeyEventHandler(this.textBoxVol_KeyDown);
			this.textBoxVol.Leave += new EventHandler(this.textBoxVol_Leave);
			this.textBoxVol.KeyPress += new KeyPressEventHandler(this.textBoxVol_KeyPress);
			this.checkBoxVolDown.Appearance = Appearance.Button;
			this.checkBoxVolDown.FlatStyle = FlatStyle.Flat;
			this.checkBoxVolDown.Location = new Point(77, 9);
			this.checkBoxVolDown.Name = "checkBoxVolDown";
			this.checkBoxVolDown.Size = new Size(16, 7);
			this.checkBoxVolDown.TabIndex = 13;
			this.checkBoxVolDown.UseVisualStyleBackColor = true;
			this.checkBoxVolDown.Paint += new PaintEventHandler(this.arrowdownthin_paint);
			this.checkBoxVolDown.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxVolDown.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxVolDown.KeyDown += new KeyEventHandler(this.checkBoxVolDown_KeyDown);
			this.checkBoxVolUp.Appearance = Appearance.Button;
			this.checkBoxVolUp.FlatStyle = FlatStyle.Flat;
			this.checkBoxVolUp.Location = new Point(77, 2);
			this.checkBoxVolUp.Name = "checkBoxVolUp";
			this.checkBoxVolUp.Size = new Size(16, 7);
			this.checkBoxVolUp.TabIndex = 10;
			this.checkBoxVolUp.UseVisualStyleBackColor = true;
			this.checkBoxVolUp.Paint += new PaintEventHandler(this.arrowupthin_paint);
			this.checkBoxVolUp.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxVolUp.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxVolUp.KeyDown += new KeyEventHandler(this.checkBoxVolUp_KeyDown);
			this.panelMem.Controls.Add(this.textBoxMemory);
			this.panelMem.Controls.Add(this.checkBoxMemDown);
			this.panelMem.Controls.Add(this.checkBoxMemUp);
			this.panelMem.Location = new Point(172, 31);
			this.panelMem.Name = "panelMem";
			this.panelMem.Size = new Size(95, 19);
			this.panelMem.TabIndex = 14;
			this.panelMem.Paint += new PaintEventHandler(this.panel_onpaint);
			this.textBoxMemory.BackColor = Color.Black;
			this.textBoxMemory.BorderStyle = BorderStyle.None;
			this.textBoxMemory.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxMemory.ForeColor = Color.Lime;
			this.textBoxMemory.Location = new Point(2, 2);
			this.textBoxMemory.MaxLength = 2;
			this.textBoxMemory.Multiline = true;
			this.textBoxMemory.Name = "textBoxMemory";
			this.textBoxMemory.Size = new Size(76, 15);
			this.textBoxMemory.TabIndex = 0;
			this.textBoxMemory.Text = "1";
			this.textBoxMemory.TextAlign = HorizontalAlignment.Center;
			this.textBoxMemory.KeyDown += new KeyEventHandler(this.textBoxMemory_KeyDown);
			this.textBoxMemory.Leave += new EventHandler(this.textBoxMemory_Leave);
			this.textBoxMemory.KeyPress += new KeyPressEventHandler(this.textBoxMemory_KeyPress);
			this.checkBoxMemDown.Appearance = Appearance.Button;
			this.checkBoxMemDown.FlatStyle = FlatStyle.Flat;
			this.checkBoxMemDown.Location = new Point(77, 9);
			this.checkBoxMemDown.Name = "checkBoxMemDown";
			this.checkBoxMemDown.Size = new Size(16, 7);
			this.checkBoxMemDown.TabIndex = 13;
			this.checkBoxMemDown.UseVisualStyleBackColor = true;
			this.checkBoxMemDown.Paint += new PaintEventHandler(this.arrowdownthin_paint);
			this.checkBoxMemDown.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxMemDown.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxMemDown.KeyDown += new KeyEventHandler(this.checkBoxMemDown_KeyDown);
			this.checkBoxMemUp.Appearance = Appearance.Button;
			this.checkBoxMemUp.FlatStyle = FlatStyle.Flat;
			this.checkBoxMemUp.Location = new Point(77, 2);
			this.checkBoxMemUp.Name = "checkBoxMemUp";
			this.checkBoxMemUp.Size = new Size(16, 7);
			this.checkBoxMemUp.TabIndex = 10;
			this.checkBoxMemUp.UseVisualStyleBackColor = true;
			this.checkBoxMemUp.Paint += new PaintEventHandler(this.arrowupthin_paint);
			this.checkBoxMemUp.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxMemUp.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxMemUp.KeyDown += new KeyEventHandler(this.checkBoxMemUp_KeyDown);
			this.labelVol.BackColor = Color.DimGray;
			this.labelVol.ForeColor = Color.Gold;
			this.labelVol.Location = new Point(10, 54);
			this.labelVol.Name = "labelVol";
			this.labelVol.Size = new Size(156, 13);
			this.labelVol.TabIndex = 1;
			this.labelVol.Text = "VOLUME";
			this.labelMem.BackColor = Color.DimGray;
			this.labelMem.ForeColor = Color.Gold;
			this.labelMem.Location = new Point(10, 34);
			this.labelMem.Name = "labelMem";
			this.labelMem.Size = new Size(156, 13);
			this.labelMem.TabIndex = 1;
			this.labelMem.Text = "MEMORY";
			this.panelStep.Controls.Add(this.labelStep);
			this.panelStep.Controls.Add(this.checkBoxStepDown);
			this.panelStep.Controls.Add(this.checkBoxStepUp);
			this.panelStep.Location = new Point(172, 11);
			this.panelStep.Name = "panelStep";
			this.panelStep.Size = new Size(95, 19);
			this.panelStep.TabIndex = 2;
			this.panelStep.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelStep.BackColor = Color.Black;
			this.labelStep.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelStep.ForeColor = Color.Lime;
			this.labelStep.Location = new Point(2, 2);
			this.labelStep.Name = "labelStep";
			this.labelStep.Size = new Size(76, 15);
			this.labelStep.TabIndex = 0;
			this.labelStep.Text = "8,333 kHz";
			this.labelStep.TextAlign = ContentAlignment.MiddleCenter;
			this.checkBoxStepDown.Appearance = Appearance.Button;
			this.checkBoxStepDown.FlatStyle = FlatStyle.Flat;
			this.checkBoxStepDown.Location = new Point(77, 9);
			this.checkBoxStepDown.Name = "checkBoxStepDown";
			this.checkBoxStepDown.Size = new Size(16, 7);
			this.checkBoxStepDown.TabIndex = 13;
			this.checkBoxStepDown.UseVisualStyleBackColor = true;
			this.checkBoxStepDown.Paint += new PaintEventHandler(this.arrowdownthin_paint);
			this.checkBoxStepDown.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxStepDown.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxStepUp.Appearance = Appearance.Button;
			this.checkBoxStepUp.FlatStyle = FlatStyle.Flat;
			this.checkBoxStepUp.Location = new Point(77, 2);
			this.checkBoxStepUp.Name = "checkBoxStepUp";
			this.checkBoxStepUp.Size = new Size(16, 7);
			this.checkBoxStepUp.TabIndex = 10;
			this.checkBoxStepUp.UseVisualStyleBackColor = true;
			this.checkBoxStepUp.Paint += new PaintEventHandler(this.arrowupthin_paint);
			this.checkBoxStepUp.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxStepUp.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.labelStp.BackColor = Color.DimGray;
			this.labelStp.ForeColor = Color.Gold;
			this.labelStp.Location = new Point(10, 14);
			this.labelStp.Name = "labelStp";
			this.labelStp.Size = new Size(156, 13);
			this.labelStp.TabIndex = 1;
			this.labelStp.Text = "STEP";
			this.trackBar1.AutoSize = false;
			this.trackBar1.BackColor = Color.DimGray;
			this.trackBar1.Location = new Point(10, 26);
			this.trackBar1.Maximum = 8;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Orientation = Orientation.Vertical;
			this.trackBar1.Size = new Size(60, 60);
			this.trackBar1.TabIndex = 10;
			this.trackBar1.Value = 4;
			this.trackBar1.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
			this.panelEq.Controls.Add(this.label8v);
			this.panelEq.Controls.Add(this.label7v);
			this.panelEq.Controls.Add(this.label6v);
			this.panelEq.Controls.Add(this.label5v);
			this.panelEq.Controls.Add(this.label4v);
			this.panelEq.Controls.Add(this.label3v);
			this.panelEq.Controls.Add(this.label2v);
			this.panelEq.Controls.Add(this.label17);
			this.panelEq.Controls.Add(this.label16);
			this.panelEq.Controls.Add(this.label15);
			this.panelEq.Controls.Add(this.label14);
			this.panelEq.Controls.Add(this.label13);
			this.panelEq.Controls.Add(this.label12);
			this.panelEq.Controls.Add(this.label11);
			this.panelEq.Controls.Add(this.label10);
			this.panelEq.Controls.Add(this.label1v);
			this.panelEq.Controls.Add(this.trackBar8);
			this.panelEq.Controls.Add(this.trackBar7);
			this.panelEq.Controls.Add(this.trackBar6);
			this.panelEq.Controls.Add(this.trackBar5);
			this.panelEq.Controls.Add(this.trackBar4);
			this.panelEq.Controls.Add(this.trackBar3);
			this.panelEq.Controls.Add(this.trackBar2);
			this.panelEq.Controls.Add(this.trackBar1);
			this.panelEq.Location = new Point(3, 380);
			this.panelEq.Name = "panelEq";
			this.panelEq.Size = new Size(552, 110);
			this.panelEq.TabIndex = 11;
			this.panelEq.Paint += new PaintEventHandler(this.panel_onpaint);
			this.label8v.BackColor = Color.DimGray;
			this.label8v.ForeColor = Color.Gold;
			this.label8v.Location = new Point(479, 10);
			this.label8v.Name = "label8v";
			this.label8v.Size = new Size(60, 13);
			this.label8v.TabIndex = 1;
			this.label8v.Text = "0 dB";
			this.label8v.TextAlign = ContentAlignment.MiddleCenter;
			this.label7v.BackColor = Color.DimGray;
			this.label7v.ForeColor = Color.Gold;
			this.label7v.Location = new Point(412, 10);
			this.label7v.Name = "label7v";
			this.label7v.Size = new Size(60, 13);
			this.label7v.TabIndex = 1;
			this.label7v.Text = "0 dB";
			this.label7v.TextAlign = ContentAlignment.MiddleCenter;
			this.label6v.BackColor = Color.DimGray;
			this.label6v.ForeColor = Color.Gold;
			this.label6v.Location = new Point(346, 10);
			this.label6v.Name = "label6v";
			this.label6v.Size = new Size(60, 13);
			this.label6v.TabIndex = 1;
			this.label6v.Text = "0 dB";
			this.label6v.TextAlign = ContentAlignment.MiddleCenter;
			this.label5v.BackColor = Color.DimGray;
			this.label5v.ForeColor = Color.Gold;
			this.label5v.Location = new Point(279, 10);
			this.label5v.Name = "label5v";
			this.label5v.Size = new Size(60, 13);
			this.label5v.TabIndex = 1;
			this.label5v.Text = "0 dB";
			this.label5v.TextAlign = ContentAlignment.MiddleCenter;
			this.label4v.BackColor = Color.DimGray;
			this.label4v.ForeColor = Color.Gold;
			this.label4v.Location = new Point(213, 10);
			this.label4v.Name = "label4v";
			this.label4v.Size = new Size(60, 13);
			this.label4v.TabIndex = 1;
			this.label4v.Text = "0 dB";
			this.label4v.TextAlign = ContentAlignment.MiddleCenter;
			this.label3v.BackColor = Color.DimGray;
			this.label3v.ForeColor = Color.Gold;
			this.label3v.Location = new Point(146, 10);
			this.label3v.Name = "label3v";
			this.label3v.Size = new Size(60, 13);
			this.label3v.TabIndex = 1;
			this.label3v.Text = "0 dB";
			this.label3v.TextAlign = ContentAlignment.MiddleCenter;
			this.label2v.BackColor = Color.DimGray;
			this.label2v.ForeColor = Color.Gold;
			this.label2v.Location = new Point(78, 10);
			this.label2v.Name = "label2v";
			this.label2v.Size = new Size(60, 13);
			this.label2v.TabIndex = 1;
			this.label2v.Text = "0 dB";
			this.label2v.TextAlign = ContentAlignment.MiddleCenter;
			this.label17.BackColor = Color.DimGray;
			this.label17.ForeColor = Color.Gold;
			this.label17.Location = new Point(479, 89);
			this.label17.Name = "label17";
			this.label17.Size = new Size(60, 13);
			this.label17.TabIndex = 1;
			this.label17.Text = "3200 Hz";
			this.label17.TextAlign = ContentAlignment.MiddleCenter;
			this.label16.BackColor = Color.DimGray;
			this.label16.ForeColor = Color.Gold;
			this.label16.Location = new Point(412, 89);
			this.label16.Name = "label16";
			this.label16.Size = new Size(60, 13);
			this.label16.TabIndex = 1;
			this.label16.Text = "2400 Hz";
			this.label16.TextAlign = ContentAlignment.MiddleCenter;
			this.label15.BackColor = Color.DimGray;
			this.label15.ForeColor = Color.Gold;
			this.label15.Location = new Point(346, 89);
			this.label15.Name = "label15";
			this.label15.Size = new Size(60, 13);
			this.label15.TabIndex = 1;
			this.label15.Text = "1600 Hz";
			this.label15.TextAlign = ContentAlignment.MiddleCenter;
			this.label14.BackColor = Color.DimGray;
			this.label14.ForeColor = Color.Gold;
			this.label14.Location = new Point(279, 89);
			this.label14.Name = "label14";
			this.label14.Size = new Size(60, 13);
			this.label14.TabIndex = 1;
			this.label14.Text = "800 Hz";
			this.label14.TextAlign = ContentAlignment.MiddleCenter;
			this.label13.BackColor = Color.DimGray;
			this.label13.ForeColor = Color.Gold;
			this.label13.Location = new Point(213, 89);
			this.label13.Name = "label13";
			this.label13.Size = new Size(60, 13);
			this.label13.TabIndex = 1;
			this.label13.Text = "400 Hz";
			this.label13.TextAlign = ContentAlignment.MiddleCenter;
			this.label12.BackColor = Color.DimGray;
			this.label12.ForeColor = Color.Gold;
			this.label12.Location = new Point(146, 89);
			this.label12.Name = "label12";
			this.label12.Size = new Size(60, 13);
			this.label12.TabIndex = 1;
			this.label12.Text = "200 Hz";
			this.label12.TextAlign = ContentAlignment.MiddleCenter;
			this.label11.BackColor = Color.DimGray;
			this.label11.ForeColor = Color.Gold;
			this.label11.Location = new Point(78, 89);
			this.label11.Name = "label11";
			this.label11.Size = new Size(60, 13);
			this.label11.TabIndex = 1;
			this.label11.Text = "100 Hz";
			this.label11.TextAlign = ContentAlignment.MiddleCenter;
			this.label10.BackColor = Color.DimGray;
			this.label10.ForeColor = Color.Gold;
			this.label10.Location = new Point(10, 89);
			this.label10.Name = "label10";
			this.label10.Size = new Size(60, 13);
			this.label10.TabIndex = 1;
			this.label10.Text = "50 Hz";
			this.label10.TextAlign = ContentAlignment.MiddleCenter;
			this.label1v.BackColor = Color.DimGray;
			this.label1v.ForeColor = Color.Gold;
			this.label1v.Location = new Point(10, 10);
			this.label1v.Name = "label1v";
			this.label1v.Size = new Size(60, 13);
			this.label1v.TabIndex = 1;
			this.label1v.Text = "0 dB";
			this.label1v.TextAlign = ContentAlignment.MiddleCenter;
			this.trackBar8.AutoSize = false;
			this.trackBar8.BackColor = Color.DimGray;
			this.trackBar8.Location = new Point(479, 26);
			this.trackBar8.Maximum = 8;
			this.trackBar8.Name = "trackBar8";
			this.trackBar8.Orientation = Orientation.Vertical;
			this.trackBar8.Size = new Size(60, 60);
			this.trackBar8.TabIndex = 10;
			this.trackBar8.Value = 4;
			this.trackBar8.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
			this.trackBar7.AutoSize = false;
			this.trackBar7.BackColor = Color.DimGray;
			this.trackBar7.Location = new Point(412, 26);
			this.trackBar7.Maximum = 8;
			this.trackBar7.Name = "trackBar7";
			this.trackBar7.Orientation = Orientation.Vertical;
			this.trackBar7.Size = new Size(60, 60);
			this.trackBar7.TabIndex = 10;
			this.trackBar7.Value = 4;
			this.trackBar7.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
			this.trackBar6.AutoSize = false;
			this.trackBar6.BackColor = Color.DimGray;
			this.trackBar6.Location = new Point(346, 26);
			this.trackBar6.Maximum = 8;
			this.trackBar6.Name = "trackBar6";
			this.trackBar6.Orientation = Orientation.Vertical;
			this.trackBar6.Size = new Size(60, 60);
			this.trackBar6.TabIndex = 10;
			this.trackBar6.Value = 4;
			this.trackBar6.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
			this.trackBar5.AutoSize = false;
			this.trackBar5.BackColor = Color.DimGray;
			this.trackBar5.Location = new Point(279, 26);
			this.trackBar5.Maximum = 8;
			this.trackBar5.Name = "trackBar5";
			this.trackBar5.Orientation = Orientation.Vertical;
			this.trackBar5.Size = new Size(60, 60);
			this.trackBar5.TabIndex = 10;
			this.trackBar5.Value = 4;
			this.trackBar5.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
			this.trackBar4.AutoSize = false;
			this.trackBar4.BackColor = Color.DimGray;
			this.trackBar4.Location = new Point(213, 26);
			this.trackBar4.Maximum = 8;
			this.trackBar4.Name = "trackBar4";
			this.trackBar4.Orientation = Orientation.Vertical;
			this.trackBar4.Size = new Size(60, 60);
			this.trackBar4.TabIndex = 10;
			this.trackBar4.Value = 4;
			this.trackBar4.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
			this.trackBar3.AutoSize = false;
			this.trackBar3.BackColor = Color.DimGray;
			this.trackBar3.Location = new Point(146, 26);
			this.trackBar3.Maximum = 8;
			this.trackBar3.Name = "trackBar3";
			this.trackBar3.Orientation = Orientation.Vertical;
			this.trackBar3.Size = new Size(60, 60);
			this.trackBar3.TabIndex = 10;
			this.trackBar3.Value = 4;
			this.trackBar3.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
			this.trackBar2.AutoSize = false;
			this.trackBar2.BackColor = Color.DimGray;
			this.trackBar2.Location = new Point(78, 26);
			this.trackBar2.Maximum = 8;
			this.trackBar2.Name = "trackBar2";
			this.trackBar2.Orientation = Orientation.Vertical;
			this.trackBar2.Size = new Size(60, 60);
			this.trackBar2.TabIndex = 10;
			this.trackBar2.Value = 4;
			this.trackBar2.ValueChanged += new EventHandler(this.trackBar1_ValueChanged);
			this.panelStatus.Controls.Add(this.panelConnStatus);
			this.panelStatus.Controls.Add(this.checkBoxExit);
			this.panelStatus.Controls.Add(this.checkBoxLog);
			this.panelStatus.Controls.Add(this.checkBoxAutoScan);
			this.panelStatus.Controls.Add(this.checkBoxGroupIDs);
			this.panelStatus.Controls.Add(this.checkBoxAbout);
			this.panelStatus.Location = new Point(3, 490);
			this.panelStatus.Name = "panelStatus";
			this.panelStatus.Size = new Size(552, 24);
			this.panelStatus.TabIndex = 12;
			this.panelStatus.Paint += new PaintEventHandler(this.panel_onpaint);
			this.panelConnStatus.Controls.Add(this.labelLink);
			this.panelConnStatus.Location = new Point(3, 3);
			this.panelConnStatus.Name = "panelConnStatus";
			this.panelConnStatus.Size = new Size(273, 18);
			this.panelConnStatus.TabIndex = 14;
			this.panelConnStatus.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelLink.BackColor = Color.Black;
			this.labelLink.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Regular, GraphicsUnit.Point, 204);
			this.labelLink.ForeColor = Color.Red;
			this.labelLink.Location = new Point(2, 2);
			this.labelLink.Name = "labelLink";
			this.labelLink.Size = new Size(269, 14);
			this.labelLink.TabIndex = 1;
			this.labelLink.Text = "LINK";
			this.labelLink.TextAlign = ContentAlignment.MiddleLeft;
			this.checkBoxExit.Appearance = Appearance.Button;
			this.checkBoxExit.FlatStyle = FlatStyle.Flat;
			this.checkBoxExit.Location = new Point(504, 2);
			this.checkBoxExit.Name = "checkBoxExit";
			this.checkBoxExit.Size = new Size(45, 18);
			this.checkBoxExit.TabIndex = 13;
			this.checkBoxExit.Text = "RESET";
			this.checkBoxExit.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxExit, "Hardware reset");
			this.checkBoxExit.UseVisualStyleBackColor = true;
			this.checkBoxExit.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxExit.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxExit.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxLog.Appearance = Appearance.Button;
			this.checkBoxLog.FlatStyle = FlatStyle.Flat;
			this.checkBoxLog.Location = new Point(406, 2);
			this.checkBoxLog.Name = "checkBoxLog";
			this.checkBoxLog.Size = new Size(72, 18);
			this.checkBoxLog.TabIndex = 13;
			this.checkBoxLog.Text = "LOG START";
			this.checkBoxLog.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxLog, "Start/Stop call log");
			this.checkBoxLog.UseVisualStyleBackColor = true;
			this.checkBoxLog.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxLog.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxLog.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxAutoScan.Appearance = Appearance.Button;
			this.checkBoxAutoScan.FlatStyle = FlatStyle.Flat;
			this.checkBoxAutoScan.Location = new Point(336, 2);
			this.checkBoxAutoScan.Name = "checkBoxAutoScan";
			this.checkBoxAutoScan.Size = new Size(68, 18);
			this.checkBoxAutoScan.TabIndex = 13;
			this.checkBoxAutoScan.Text = "AUTOSCAN";
			this.checkBoxAutoScan.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxAutoScan, "Memory autoscan mode settings");
			this.checkBoxAutoScan.UseVisualStyleBackColor = true;
			this.checkBoxAutoScan.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxAutoScan.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxAutoScan.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxGroupIDs.Appearance = Appearance.Button;
			this.checkBoxGroupIDs.FlatStyle = FlatStyle.Flat;
			this.checkBoxGroupIDs.Location = new Point(279, 2);
			this.checkBoxGroupIDs.Name = "checkBoxGroupIDs";
			this.checkBoxGroupIDs.Size = new Size(55, 18);
			this.checkBoxGroupIDs.TabIndex = 13;
			this.checkBoxGroupIDs.Text = "FILTERS";
			this.checkBoxGroupIDs.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxGroupIDs, "Filter settings");
			this.checkBoxGroupIDs.UseVisualStyleBackColor = true;
			this.checkBoxGroupIDs.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxGroupIDs.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxGroupIDs.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxAbout.Appearance = Appearance.Button;
			this.checkBoxAbout.FlatStyle = FlatStyle.Flat;
			this.checkBoxAbout.Location = new Point(478, 2);
			this.checkBoxAbout.Name = "checkBoxAbout";
			this.checkBoxAbout.Size = new Size(25, 18);
			this.checkBoxAbout.TabIndex = 13;
			this.checkBoxAbout.Text = "?";
			this.checkBoxAbout.TextAlign = ContentAlignment.MiddleCenter;
			this.toolTipMainForm.SetToolTip(this.checkBoxAbout, "About me");
			this.checkBoxAbout.UseVisualStyleBackColor = true;
			this.checkBoxAbout.Paint += new PaintEventHandler(this.textbutton_paint);
			this.checkBoxAbout.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxAbout.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.panelAi.Controls.Add(this.panelManuf);
			this.panelAi.Controls.Add(this.panelFT);
			this.panelAi.Controls.Add(this.panelEC);
			this.panelAi.Controls.Add(this.panelET);
			this.panelAi.Controls.Add(this.panelTgtId);
			this.panelAi.Controls.Add(this.panelSrcId);
			this.panelAi.Controls.Add(this.panelCT);
			this.panelAi.Controls.Add(this.panelProto);
			this.panelAi.Controls.Add(this.panelNac);
			this.panelAi.Controls.Add(this.label21);
			this.panelAi.Controls.Add(this.labelFT);
			this.panelAi.Controls.Add(this.labelCT);
			this.panelAi.Controls.Add(this.labelManuf);
			this.panelAi.Controls.Add(this.labelKey);
			this.panelAi.Controls.Add(this.labelPrivacy);
			this.panelAi.Controls.Add(this.labelTgt);
			this.panelAi.Controls.Add(this.labelSrc);
			this.panelAi.Controls.Add(this.labelAccCode);
			this.panelAi.Location = new Point(3, 192);
			this.panelAi.Name = "panelAi";
			this.panelAi.Size = new Size(276, 188);
			this.panelAi.TabIndex = 4;
			this.panelAi.Paint += new PaintEventHandler(this.panel_onpaint);
			this.panelManuf.Controls.Add(this.labelMfid);
			this.panelManuf.Location = new Point(172, 161);
			this.panelManuf.Name = "panelManuf";
			this.panelManuf.Size = new Size(95, 18);
			this.panelManuf.TabIndex = 3;
			this.panelManuf.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelMfid.BackColor = Color.Black;
			this.labelMfid.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelMfid.ForeColor = Color.Lime;
			this.labelMfid.Location = new Point(2, 2);
			this.labelMfid.Name = "labelMfid";
			this.labelMfid.Size = new Size(91, 14);
			this.labelMfid.TabIndex = 0;
			this.panelFT.Controls.Add(this.labelFrmType);
			this.panelFT.Location = new Point(172, 142);
			this.panelFT.Name = "panelFT";
			this.panelFT.Size = new Size(95, 18);
			this.panelFT.TabIndex = 3;
			this.panelFT.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelFrmType.BackColor = Color.Black;
			this.labelFrmType.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelFrmType.ForeColor = Color.Lime;
			this.labelFrmType.Location = new Point(2, 2);
			this.labelFrmType.Name = "labelFrmType";
			this.labelFrmType.Size = new Size(91, 14);
			this.labelFrmType.TabIndex = 0;
			this.panelEC.Controls.Add(this.labelEncKey);
			this.panelEC.Location = new Point(172, 123);
			this.panelEC.Name = "panelEC";
			this.panelEC.Size = new Size(95, 18);
			this.panelEC.TabIndex = 3;
			this.panelEC.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelEncKey.BackColor = Color.Black;
			this.labelEncKey.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelEncKey.ForeColor = Color.Lime;
			this.labelEncKey.Location = new Point(2, 2);
			this.labelEncKey.Name = "labelEncKey";
			this.labelEncKey.Size = new Size(91, 14);
			this.labelEncKey.TabIndex = 0;
			this.panelET.Controls.Add(this.labelEncType);
			this.panelET.Location = new Point(172, 104);
			this.panelET.Name = "panelET";
			this.panelET.Size = new Size(95, 18);
			this.panelET.TabIndex = 3;
			this.panelET.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelEncType.BackColor = Color.Black;
			this.labelEncType.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelEncType.ForeColor = Color.Lime;
			this.labelEncType.Location = new Point(2, 2);
			this.labelEncType.Name = "labelEncType";
			this.labelEncType.Size = new Size(91, 14);
			this.labelEncType.TabIndex = 0;
			this.panelTgtId.Controls.Add(this.labelTgtId);
			this.panelTgtId.Location = new Point(172, 85);
			this.panelTgtId.Name = "panelTgtId";
			this.panelTgtId.Size = new Size(95, 18);
			this.panelTgtId.TabIndex = 2;
			this.panelTgtId.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelTgtId.BackColor = Color.Black;
			this.labelTgtId.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelTgtId.ForeColor = Color.Lime;
			this.labelTgtId.Location = new Point(2, 2);
			this.labelTgtId.Name = "labelTgtId";
			this.labelTgtId.Size = new Size(91, 14);
			this.labelTgtId.TabIndex = 0;
			this.panelSrcId.Controls.Add(this.labelSrcId);
			this.panelSrcId.Location = new Point(172, 66);
			this.panelSrcId.Name = "panelSrcId";
			this.panelSrcId.Size = new Size(95, 18);
			this.panelSrcId.TabIndex = 2;
			this.panelSrcId.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelSrcId.BackColor = Color.Black;
			this.labelSrcId.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelSrcId.ForeColor = Color.Lime;
			this.labelSrcId.Location = new Point(2, 2);
			this.labelSrcId.Name = "labelSrcId";
			this.labelSrcId.Size = new Size(91, 14);
			this.labelSrcId.TabIndex = 0;
			this.panelCT.Controls.Add(this.labelCallType);
			this.panelCT.Location = new Point(172, 47);
			this.panelCT.Name = "panelCT";
			this.panelCT.Size = new Size(95, 18);
			this.panelCT.TabIndex = 0;
			this.panelCT.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelCallType.BackColor = Color.Black;
			this.labelCallType.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelCallType.ForeColor = Color.Lime;
			this.labelCallType.Location = new Point(2, 2);
			this.labelCallType.Name = "labelCallType";
			this.labelCallType.Size = new Size(91, 14);
			this.labelCallType.TabIndex = 0;
			this.panelProto.Controls.Add(this.labelAi);
			this.panelProto.Controls.Add(this.checkBoxAiDown);
			this.panelProto.Controls.Add(this.checkBoxAiUp);
			this.panelProto.Location = new Point(172, 9);
			this.panelProto.Name = "panelProto";
			this.panelProto.Size = new Size(95, 18);
			this.panelProto.TabIndex = 0;
			this.panelProto.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelAi.BackColor = Color.Black;
			this.labelAi.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelAi.ForeColor = Color.Lime;
			this.labelAi.Location = new Point(2, 2);
			this.labelAi.Name = "labelAi";
			this.labelAi.Size = new Size(76, 14);
			this.labelAi.TabIndex = 0;
			this.labelAi.Text = "P25";
			this.checkBoxAiDown.Appearance = Appearance.Button;
			this.checkBoxAiDown.FlatStyle = FlatStyle.Flat;
			this.checkBoxAiDown.Location = new Point(77, 9);
			this.checkBoxAiDown.Name = "checkBoxAiDown";
			this.checkBoxAiDown.Size = new Size(16, 7);
			this.checkBoxAiDown.TabIndex = 13;
			this.checkBoxAiDown.UseVisualStyleBackColor = true;
			this.checkBoxAiDown.Paint += new PaintEventHandler(this.arrowdownthin_paint);
			this.checkBoxAiDown.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxAiDown.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxAiDown.KeyDown += new KeyEventHandler(this.checkBoxAiDown_KeyDown);
			this.checkBoxAiUp.Appearance = Appearance.Button;
			this.checkBoxAiUp.FlatStyle = FlatStyle.Flat;
			this.checkBoxAiUp.Location = new Point(77, 2);
			this.checkBoxAiUp.Name = "checkBoxAiUp";
			this.checkBoxAiUp.Size = new Size(16, 7);
			this.checkBoxAiUp.TabIndex = 10;
			this.checkBoxAiUp.UseVisualStyleBackColor = true;
			this.checkBoxAiUp.Paint += new PaintEventHandler(this.arrowupthin_paint);
			this.checkBoxAiUp.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxAiUp.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxAiUp.KeyDown += new KeyEventHandler(this.checkBoxAiUp_KeyDown);
			this.panelNac.Controls.Add(this.labelNac);
			this.panelNac.Location = new Point(172, 28);
			this.panelNac.Name = "panelNac";
			this.panelNac.Size = new Size(95, 18);
			this.panelNac.TabIndex = 0;
			this.panelNac.Paint += new PaintEventHandler(this.panel_onpaint);
			this.labelNac.BackColor = Color.Black;
			this.labelNac.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.labelNac.ForeColor = Color.Lime;
			this.labelNac.Location = new Point(2, 2);
			this.labelNac.Name = "labelNac";
			this.labelNac.Size = new Size(91, 14);
			this.labelNac.TabIndex = 0;
			this.label21.BackColor = Color.DimGray;
			this.label21.ForeColor = Color.Gold;
			this.label21.Location = new Point(10, 11);
			this.label21.Name = "label21";
			this.label21.Size = new Size(156, 13);
			this.label21.TabIndex = 1;
			this.label21.Text = "AIR INTERFACE";
			this.labelFT.BackColor = Color.DimGray;
			this.labelFT.ForeColor = Color.Gold;
			this.labelFT.Location = new Point(10, 144);
			this.labelFT.Name = "labelFT";
			this.labelFT.Size = new Size(157, 13);
			this.labelFT.TabIndex = 1;
			this.labelFT.Text = "FRAME TYPE";
			this.labelCT.BackColor = Color.DimGray;
			this.labelCT.ForeColor = Color.Gold;
			this.labelCT.Location = new Point(10, 49);
			this.labelCT.Name = "labelCT";
			this.labelCT.Size = new Size(156, 13);
			this.labelCT.TabIndex = 1;
			this.labelCT.Text = "CALL TYPE";
			this.labelManuf.BackColor = Color.DimGray;
			this.labelManuf.ForeColor = Color.Gold;
			this.labelManuf.Location = new Point(10, 163);
			this.labelManuf.Name = "labelManuf";
			this.labelManuf.Size = new Size(157, 13);
			this.labelManuf.TabIndex = 1;
			this.labelManuf.Text = "MANUFACTURER";
			this.labelKey.BackColor = Color.DimGray;
			this.labelKey.ForeColor = Color.Gold;
			this.labelKey.Location = new Point(10, 125);
			this.labelKey.Name = "labelKey";
			this.labelKey.Size = new Size(157, 13);
			this.labelKey.TabIndex = 1;
			this.labelKey.Text = "ENCRYPTION KEY";
			this.labelPrivacy.BackColor = Color.DimGray;
			this.labelPrivacy.ForeColor = Color.Gold;
			this.labelPrivacy.Location = new Point(10, 106);
			this.labelPrivacy.Name = "labelPrivacy";
			this.labelPrivacy.Size = new Size(157, 13);
			this.labelPrivacy.TabIndex = 1;
			this.labelPrivacy.Text = "ENCRYPTION TYPE";
			this.labelTgt.BackColor = Color.DimGray;
			this.labelTgt.ForeColor = Color.Gold;
			this.labelTgt.Location = new Point(10, 87);
			this.labelTgt.Name = "labelTgt";
			this.labelTgt.Size = new Size(157, 13);
			this.labelTgt.TabIndex = 1;
			this.labelTgt.Text = "TARGET ID";
			this.labelSrc.BackColor = Color.DimGray;
			this.labelSrc.ForeColor = Color.Gold;
			this.labelSrc.Location = new Point(10, 68);
			this.labelSrc.Name = "labelSrc";
			this.labelSrc.Size = new Size(156, 13);
			this.labelSrc.TabIndex = 1;
			this.labelSrc.Text = "SOURCE ID";
			this.labelAccCode.BackColor = Color.DimGray;
			this.labelAccCode.ForeColor = Color.Gold;
			this.labelAccCode.Location = new Point(10, 30);
			this.labelAccCode.Name = "labelAccCode";
			this.labelAccCode.Size = new Size(156, 13);
			this.labelAccCode.TabIndex = 1;
			this.labelAccCode.Text = "NAC";
			this.openFileDialogMem.Filter = "mem files (*.mem)|*.mem|All files (*.*)|*.*";
			this.saveFileDialogMem.Filter = "mem files (*.mem)|*.mem|All files (*.*)|*.*";
			this.saveFileDialogMem.RestoreDirectory = true;
			this.saveFileDialogLog.Filter = "log files (*.csv)|*.csv|All files (*.*)|*.*";
			this.panelGen.Controls.Add(this.panelStep);
			this.panelGen.Controls.Add(this.labelStp);
			this.panelGen.Controls.Add(this.labelMem);
			this.panelGen.Controls.Add(this.labelVol);
			this.panelGen.Controls.Add(this.panelMem);
			this.panelGen.Controls.Add(this.panelVol);
			this.panelGen.Location = new Point(3, 106);
			this.panelGen.Name = "panelGen";
			this.panelGen.Size = new Size(276, 80);
			this.panelGen.TabIndex = 1;
			this.panelGen.Paint += new PaintEventHandler(this.panel_onpaint);
			this.panelVU.Location = new Point(279, 106);
			this.panelVU.Name = "panelVU";
			this.panelVU.Size = new Size(276, 80);
			this.panelVU.TabIndex = 1;
			this.panelVU.Paint += new PaintEventHandler(this.panel_onpaint);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(557, 517);
			base.Controls.Add(this.panelCtrl);
			base.Controls.Add(this.panelStatus);
			base.Controls.Add(this.panelEq);
			base.Controls.Add(this.panelAi);
			base.Controls.Add(this.panelInd);
			base.Controls.Add(this.panelVU);
			base.Controls.Add(this.panelGen);
			base.Controls.Add(this.panelTL);
			this.DoubleBuffered = true;
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.Name = "p25recvForm";
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "ADCR25 Tuner (v5.10)";
			base.Paint += new PaintEventHandler(this.p25recvForm_Paint);
			base.Shown += new EventHandler(this.p25recvForm_Shown);
			base.FormClosing += new FormClosingEventHandler(this.p25recvForm_FormClosing);
			base.KeyDown += new KeyEventHandler(this.p25recvForm_KeyDown);
			this.panelInd.ResumeLayout(false);
			this.panelComments.ResumeLayout(false);
			this.panelFreq.ResumeLayout(false);
			this.panelFreq.PerformLayout();
			this.panelCtrl.ResumeLayout(false);
			this.panelVol.ResumeLayout(false);
			this.panelVol.PerformLayout();
			this.panelMem.ResumeLayout(false);
			this.panelMem.PerformLayout();
			this.panelStep.ResumeLayout(false);
			((ISupportInitialize)this.trackBar1).EndInit();
			this.panelEq.ResumeLayout(false);
			((ISupportInitialize)this.trackBar8).EndInit();
			((ISupportInitialize)this.trackBar7).EndInit();
			((ISupportInitialize)this.trackBar6).EndInit();
			((ISupportInitialize)this.trackBar5).EndInit();
			((ISupportInitialize)this.trackBar4).EndInit();
			((ISupportInitialize)this.trackBar3).EndInit();
			((ISupportInitialize)this.trackBar2).EndInit();
			this.panelStatus.ResumeLayout(false);
			this.panelConnStatus.ResumeLayout(false);
			this.panelAi.ResumeLayout(false);
			this.panelManuf.ResumeLayout(false);
			this.panelFT.ResumeLayout(false);
			this.panelEC.ResumeLayout(false);
			this.panelET.ResumeLayout(false);
			this.panelTgtId.ResumeLayout(false);
			this.panelSrcId.ResumeLayout(false);
			this.panelCT.ResumeLayout(false);
			this.panelProto.ResumeLayout(false);
			this.panelNac.ResumeLayout(false);
			this.panelGen.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
