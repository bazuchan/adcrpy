using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace p25recv_tuner
{
	public class FormTrunk : Form
	{
		private p25recvForm parent;

		private Color mainColor;

		private uint[] tt_ids_bf = new uint[16];

		private uint[] tt_ids_cs = new uint[16];

		private int[] tt_ids_txo = new int[16];

		private byte[] tt_ids_bw = new byte[16];

		private ushort[] tt_chan_ids = new ushort[100];

		private int[] tt_chan_cnts = new int[100];

		private uint wacn_id;

		private ushort system_id;

		private ushort nac_id;

		private int num_chans;

		private int load_pct;

		private int load_cnt;

		private bool par_lock;

		private bool logging;

		private StreamWriter log_file;

		private System.Timers.Timer chanTimer;

		private System.Timers.Timer clrTimer;

		private System.Timers.Timer loadTimer;

		private System.Timers.Timer updTimer;

		private int MAX_CHANS = 100;

		private int MAX_HOLD_TIME = 4;

		private IContainer components;

		private Panel panelMain;

		private CheckBox checkBoxTTEna;

		private CheckBox checkBoxClose;

		private Panel panelCCFreq;

		private TextBox textBoxCCFreq;

		private ToolTip toolTip;

		private Panel panelTrunkInfo;

		private Label labelInfoSystem;

		private Panel panelInfoSystem;

		private TextBox textBoxInfoSystem;

		private Label labelInfoNAC;

		private Panel panelInfoNAC;

		private TextBox textBoxInfoNAC;

		private Label labelInfoSite;

		private Panel panelInfoSite;

		private TextBox textBoxInfoSite;

		private Panel panelTracker;

		private Label labelInfoChans;

		private Panel panelInfoChans;

		private TextBox textBoxInfoChans;

		private Label labelInfoControl;

		private Panel panelInfoControl;

		private TextBox textBoxInfoControl;

		private RadioButton radioButtonInfo;

		private RadioButton radioButtonChans;

		private RadioButton radioButtonCallHist;

		private RadioButton radioButtonPeers;

		private RadioButton radioButtonBandPlan;

		private RadioButton radioButtonTracker;

		private Panel panelPeers;

		private ListView listViewPeers;

		private Panel panelBandPlan;

		private ListView listViewBandPlan;

		private ColumnHeader id;

		private ColumnHeader bf;

		private ColumnHeader lo;

		private ColumnHeader hi;

		private ColumnHeader cs;

		private ColumnHeader bw;

		private ColumnHeader txo;

		private Label labelBandPlanID;

		private Label labelPeersSystem;

		private ColumnHeader system;

		private ColumnHeader site;

		private ColumnHeader control;

		private ColumnHeader frequency;

		private ColumnHeader last;

		private Panel panelChans;

		private Label labelChansLCN;

		private ListView listViewChans;

		private ColumnHeader chan_lcn;

		private ColumnHeader chan_freq;

		private ColumnHeader chan_label;

		private ColumnHeader chan_hits;

		private ColumnHeader chan_tgt;

		private ColumnHeader chan_t;

		private ColumnHeader chan_src;

		private ColumnHeader chan_last;

		private Panel panelCallHist;

		private Label labelCallHistStamp;

		private ListView listViewCallHist;

		private ColumnHeader ch_stamp;

		private ColumnHeader ch_srcid;

		private ColumnHeader ch_action;

		private ColumnHeader ch_res;

		private ColumnHeader ch_tgtid;

		private ColumnHeader ch_svc;

		private Panel panelCHCtrl;

		private CheckBox checkBoxCHClear;

		private CheckBox checkBoxCHScroll;

		private CheckBox checkBoxCHData;

		private CheckBox checkBoxCHVoice;

		private CheckBox checkBoxCHCntl;

		private Panel panelTraffic;

		private CheckBox checkBoxHold;

		private Label labelCC;

		private Label labelChanType;

		private Panel panelChanType;

		private TextBox textBoxChanType;

		private Label labelSrcID;

		private Label labelChanRSSI;

		private Panel panelSrcID;

		private TextBox textBoxSrcID;

		private Panel panelChanRSSI;

		private TextBox textBoxChanRSSI;

		private Label labelTgtID;

		private Panel panelTgtID;

		private TextBox textBoxTgtID;

		private Label labelVCFreq;

		private Panel panelVCFreq;

		private TextBox textBoxVCFreq;

		private Label labelPeersLast;

		private Label labelPeersFreq;

		private Label labelPeersControl;

		private Label labelPeersSite;

		private Label labelBandPlanBase;

		private Label labelBandPlanLo;

		private Label labelBandPlanHi;

		private Label labelBandPlanSpacing;

		private Label labelBandPlanBW;

		private Label labelBandPlanTXO;

		private Label labelCallHistSrcID;

		private Label labelCallHistAction;

		private Label labelCallHistResult;

		private Label labelCallHistTgtID;

		private Label labelCallHistService;

		private Label labelChansFreq;

		private Label labelChansLabel;

		private Label labelChansHits;

		private Label labelChansTarget;

		private Label labelChansT;

		private Label labelChansSource;

		private Label labelChansLast;

		private SaveFileDialog saveFileDialogLog;

		private CheckBox checkBoxLog;

		private Panel panelFilt;

		private ListView listViewFilt1;

		private ColumnHeader columnHeaderIDs;

		private CheckBox checkBoxRem;

		private CheckBox checkBoxAdd;

		private ListView listViewFilt2;

		private ColumnHeader columnHeader1;

		private CheckBox checkBoxTTFExclude;

		private CheckBox checkBoxTTFEna;

		private Label labelTTFExclude;

		private Label labelTTFEna;

		private ListView listViewFilt4;

		private ColumnHeader columnHeader3;

		private ListView listViewFilt3;

		private ColumnHeader columnHeader2;

		private CheckBox checkBoxCCSEna;

		private Label labelCCSEna;

		public FormTrunk()
		{
			this.InitializeComponent();
		}

		public FormTrunk(p25recvForm p, Color col)
		{
			this.InitializeComponent();
			this.mainColor = col;
			this.parent = p;
			this.panelCHCtrl.BackColor = this.mainColor;
			this.panelTrunkInfo.BackColor = this.mainColor;
			this.panelTracker.BackColor = this.mainColor;
			this.panelPeers.BackColor = this.mainColor;
			this.panelBandPlan.BackColor = this.mainColor;
			this.panelChans.BackColor = this.mainColor;
			this.panelCallHist.BackColor = this.mainColor;
			this.panelTrunkInfo.Top = 50;
			this.panelTrunkInfo.Left = 14;
			this.panelTracker.Top = 50;
			this.panelTracker.Left = 14;
			this.panelPeers.Top = 50;
			this.panelPeers.Left = 14;
			this.panelBandPlan.Top = 50;
			this.panelBandPlan.Left = 14;
			this.panelChans.Top = 50;
			this.panelChans.Left = 14;
			this.panelCallHist.Top = 50;
			this.panelCallHist.Left = 14;
			this.labelChansLCN.Top = (this.labelChansLCN.Left = 2);
			this.listViewChans.Left = 2;
			this.listViewChans.Top = this.labelChansLCN.Height + 2;
			this.listViewChans.Items.Add("P25");
			int height = this.listViewChans.GetItemRect(0).Height;
			this.listViewChans.Items.Clear();
			this.listViewChans.Height = height * 16;
			this.panelChans.Height = this.labelChansLCN.Height + this.listViewChans.Height + 4;
			this.labelChansLCN.Width = this.listViewChans.Columns[0].Width + 4;
			this.labelChansFreq.Width = this.listViewChans.Columns[1].Width;
			this.labelChansLabel.Width = this.listViewChans.Columns[2].Width;
			this.labelChansHits.Width = this.listViewChans.Columns[3].Width;
			this.labelChansTarget.Width = this.listViewChans.Columns[4].Width;
			this.labelChansT.Width = this.listViewChans.Columns[5].Width;
			this.labelChansSource.Width = this.listViewChans.Columns[6].Width;
			this.labelChansFreq.Left = this.labelChansLCN.Width;
			this.labelChansLabel.Left = this.labelChansLCN.Width + this.labelChansFreq.Width;
			this.labelChansHits.Left = this.labelChansLCN.Width + this.labelChansFreq.Width + this.labelChansLabel.Width;
			this.labelChansTarget.Left = this.labelChansLCN.Width + this.labelChansFreq.Width + this.labelChansLabel.Width + this.labelChansHits.Width;
			this.labelChansT.Left = this.labelChansLCN.Width + this.labelChansFreq.Width + this.labelChansLabel.Width + this.labelChansHits.Width + this.labelChansTarget.Width;
			this.labelChansSource.Left = this.labelChansLCN.Width + this.labelChansFreq.Width + this.labelChansLabel.Width + this.labelChansHits.Width + this.labelChansTarget.Width + this.labelChansT.Width;
			this.labelChansLast.Left = this.labelChansLCN.Width + this.labelChansFreq.Width + this.labelChansLabel.Width + this.labelChansHits.Width + this.labelChansTarget.Width + this.labelChansT.Width + this.labelChansSource.Width;
			this.listViewChans.Width = this.panelChans.Width - 4;
			this.labelChansLast.Width = this.listViewChans.Width - this.labelChansLast.Left + 2;
			this.labelBandPlanID.Top = (this.labelBandPlanID.Left = 2);
			this.listViewBandPlan.Left = 2;
			this.listViewBandPlan.Top = this.labelBandPlanID.Height + 2;
			this.listViewBandPlan.Items.Add("P25");
			height = this.listViewBandPlan.GetItemRect(0).Height;
			this.listViewBandPlan.Items.Clear();
			this.listViewBandPlan.Height = height * 16;
			this.panelBandPlan.Height = this.labelBandPlanID.Height + this.listViewBandPlan.Height + 4;
			this.labelBandPlanID.Width = this.listViewBandPlan.Columns[0].Width + 4;
			this.labelBandPlanBase.Width = this.listViewBandPlan.Columns[1].Width;
			this.labelBandPlanLo.Width = this.listViewBandPlan.Columns[2].Width;
			this.labelBandPlanHi.Width = this.listViewBandPlan.Columns[3].Width;
			this.labelBandPlanSpacing.Width = this.listViewBandPlan.Columns[4].Width;
			this.labelBandPlanBW.Width = this.listViewBandPlan.Columns[5].Width;
			this.labelBandPlanTXO.Width = this.listViewBandPlan.Columns[6].Width;
			this.labelBandPlanBase.Left = this.labelBandPlanID.Width;
			this.labelBandPlanLo.Left = this.labelBandPlanID.Width + this.labelBandPlanBase.Width;
			this.labelBandPlanHi.Left = this.labelBandPlanID.Width + this.labelBandPlanBase.Width + this.labelBandPlanLo.Width;
			this.labelBandPlanSpacing.Left = this.labelBandPlanID.Width + this.labelBandPlanBase.Width + this.labelBandPlanLo.Width + this.labelBandPlanHi.Width;
			this.labelBandPlanBW.Left = this.labelBandPlanID.Width + this.labelBandPlanBase.Width + this.labelBandPlanLo.Width + this.labelBandPlanHi.Width + this.labelBandPlanSpacing.Width;
			this.labelBandPlanTXO.Left = this.labelBandPlanID.Width + this.labelBandPlanBase.Width + this.labelBandPlanLo.Width + this.labelBandPlanHi.Width + this.labelBandPlanSpacing.Width + this.labelBandPlanBW.Width;
			this.panelBandPlan.Width = this.panelChans.Width;
			this.listViewBandPlan.Width = this.listViewChans.Width;
			this.labelBandPlanTXO.Width = this.listViewBandPlan.Width - this.labelBandPlanTXO.Left + 2;
			this.labelCallHistStamp.Top = (this.labelCallHistStamp.Left = 2);
			this.listViewCallHist.Left = 2;
			this.listViewCallHist.Top = this.labelCallHistStamp.Height + 2;
			this.listViewCallHist.Items.Add("P25");
			height = this.listViewCallHist.GetItemRect(0).Height;
			this.listViewCallHist.Items.Clear();
			this.listViewCallHist.Height = height * 16;
			this.panelCallHist.Height = this.labelCallHistStamp.Height + this.listViewCallHist.Height + 4;
			this.panelCallHist.Width = this.labelCallHistStamp.Width + 4;
			this.panelCHCtrl.Width = this.panelCallHist.Width;
			this.panelCHCtrl.Left = this.panelCallHist.Left - 3;
			this.panelCHCtrl.Top = this.panelCallHist.Top + this.panelCallHist.Height + 9;
			this.labelCallHistStamp.Width = this.listViewCallHist.Columns[0].Width + 4;
			this.labelCallHistSrcID.Width = this.listViewCallHist.Columns[1].Width;
			this.labelCallHistAction.Width = this.listViewCallHist.Columns[2].Width;
			this.labelCallHistResult.Width = this.listViewCallHist.Columns[3].Width;
			this.labelCallHistTgtID.Width = this.listViewCallHist.Columns[4].Width;
			this.labelCallHistService.Width = this.listViewCallHist.Columns[5].Width;
			this.labelCallHistSrcID.Left = this.labelCallHistStamp.Width;
			this.labelCallHistAction.Left = this.labelCallHistStamp.Width + this.labelCallHistSrcID.Width;
			this.labelCallHistResult.Left = this.labelCallHistStamp.Width + this.labelCallHistSrcID.Width + this.labelCallHistAction.Width;
			this.labelCallHistTgtID.Left = this.labelCallHistStamp.Width + this.labelCallHistSrcID.Width + this.labelCallHistAction.Width + this.labelCallHistResult.Width;
			this.labelCallHistService.Left = this.labelCallHistStamp.Width + this.labelCallHistSrcID.Width + this.labelCallHistAction.Width + this.labelCallHistResult.Width + this.labelCallHistTgtID.Width;
			this.panelCallHist.Width = this.panelChans.Width;
			this.listViewCallHist.Width = this.listViewChans.Width;
			this.labelCallHistService.Width = this.listViewCallHist.Width - this.labelCallHistService.Left + 2;
			this.panelCHCtrl.Width = this.panelCallHist.Width;
			this.labelPeersSystem.Top = (this.labelPeersSystem.Left = 2);
			this.listViewPeers.Left = 2;
			this.listViewPeers.Top = this.labelPeersSystem.Height + 2;
			this.listViewPeers.Items.Add("P25");
			height = this.listViewPeers.GetItemRect(0).Height;
			this.listViewPeers.Items.Clear();
			this.listViewPeers.Height = height * 16;
			this.panelPeers.Height = this.labelPeersSystem.Height + this.listViewPeers.Height + 4;
			this.labelPeersSystem.Width = this.listViewPeers.Columns[0].Width + 4;
			this.labelPeersSite.Width = this.listViewPeers.Columns[1].Width;
			this.labelPeersControl.Width = this.listViewPeers.Columns[2].Width;
			this.labelPeersFreq.Width = this.listViewPeers.Columns[3].Width;
			this.labelPeersSite.Left = this.labelPeersSystem.Width;
			this.labelPeersControl.Left = this.labelPeersSystem.Width + this.labelPeersSite.Width;
			this.labelPeersFreq.Left = this.labelPeersSystem.Width + this.labelPeersSite.Width + this.labelPeersControl.Width;
			this.labelPeersLast.Left = this.labelPeersSystem.Width + this.labelPeersSite.Width + this.labelPeersControl.Width + this.labelPeersFreq.Width;
			this.labelPeersLast.Width = this.listViewPeers.Width - this.labelPeersLast.Left;
			this.panelPeers.Width = this.panelChans.Width;
			this.listViewPeers.Width = this.listViewChans.Width;
			this.labelPeersLast.Width = this.listViewPeers.Width - this.labelPeersLast.Left + 2;
			this.panelTrunkInfo.Height = (this.panelTracker.Height = this.panelChans.Height);
			this.panelTrunkInfo.Width = (this.panelTracker.Width = this.panelChans.Width);
			this.panelMain.Top = (this.panelMain.Left = 3);
			this.parent.compAlign(this.panelVCFreq, this.textBoxVCFreq, 2, 0);
			this.parent.compAlign(this.panelCCFreq, this.textBoxCCFreq, 2, 0);
			this.parent.compAlign(this.panelChanType, this.textBoxChanType, 2, 0);
			this.parent.compAlign(this.panelChanRSSI, this.textBoxChanRSSI, 2, 0);
			this.parent.compAlign(this.panelSrcID, this.textBoxSrcID, 2, 0);
			this.parent.compAlign(this.panelTgtID, this.textBoxTgtID, 2, 0);
			this.parent.compAlign(this.panelInfoSystem, this.textBoxInfoSystem, 2, 0);
			this.parent.compAlign(this.panelInfoSite, this.textBoxInfoSite, 2, 0);
			this.parent.compAlign(this.panelInfoNAC, this.textBoxInfoNAC, 2, 0);
			this.parent.compAlign(this.panelInfoChans, this.textBoxInfoChans, 2, 0);
			this.parent.compAlign(this.panelInfoControl, this.textBoxInfoControl, 2, 0);
			base.Width = 34 + this.panelChans.Width;
			base.Height = base.Height - base.ClientRectangle.Height + 28 + this.panelTrunkInfo.Top + this.checkBoxCHClear.Height + this.panelTrunkInfo.Height;
			this.panelMain.Width = base.ClientRectangle.Width - 6;
			this.panelMain.Height = base.ClientRectangle.Height - 6;
			this.panelTraffic.Width = this.panelBandPlan.Left + this.panelBandPlan.Width - this.panelTraffic.Left - 3;
			this.panelInfoSite.Top = this.panelInfoSystem.Top + this.panelInfoSystem.Height + 1;
			this.panelInfoControl.Top = this.panelInfoSite.Top + this.panelInfoSite.Height + 1;
			this.panelInfoNAC.Top = this.panelInfoControl.Top + this.panelInfoControl.Height + 1;
			this.panelInfoChans.Top = this.panelInfoNAC.Top + this.panelInfoNAC.Height + 1;
			this.panelVCFreq.Top = this.panelCCFreq.Top + this.panelCCFreq.Height + 1;
			this.panelChanType.Top = this.panelVCFreq.Top + this.panelVCFreq.Height + 1;
			this.panelChanRSSI.Top = this.panelChanType.Top + this.panelChanType.Height + 1;
			this.panelSrcID.Top = this.panelChanRSSI.Top + this.panelChanRSSI.Height + 1;
			this.panelTgtID.Top = this.panelSrcID.Top + this.panelSrcID.Height + 1;
			this.labelCC.Top = this.panelCCFreq.Top + 3;
			this.labelVCFreq.Top = this.panelVCFreq.Top + 3;
			this.labelChanType.Top = this.panelChanType.Top + 3;
			this.labelChanRSSI.Top = this.panelChanRSSI.Top + 3;
			this.labelSrcID.Top = this.panelSrcID.Top + 3;
			this.labelTgtID.Top = this.panelTgtID.Top + 3;
			this.labelInfoSystem.Top = this.panelInfoSystem.Top + 3;
			this.labelInfoSite.Top = this.panelInfoSite.Top + 3;
			this.labelInfoControl.Top = this.panelInfoControl.Top + 3;
			this.labelInfoNAC.Top = this.panelInfoNAC.Top + 3;
			this.labelInfoChans.Top = this.panelInfoChans.Top + 3;
			this.checkBoxTTEna.Top = (this.checkBoxHold.Top = this.panelTgtID.Top + this.panelTgtID.Height + 5);
			this.updTimer = new System.Timers.Timer();
			this.updTimer.Elapsed += new ElapsedEventHandler(this.updTimer_Elapsed);
			this.updTimer.SynchronizingObject = this;
			this.chanTimer = new System.Timers.Timer();
			this.chanTimer.Elapsed += new ElapsedEventHandler(this.chanTimer_Elapsed);
			this.chanTimer.SynchronizingObject = this.listViewChans;
			this.chanTimer.Interval = 500.0;
			this.loadTimer = new System.Timers.Timer();
			this.loadTimer.Elapsed += new ElapsedEventHandler(this.loadTimer_Elapsed);
			this.loadTimer.SynchronizingObject = this;
			this.loadTimer.Interval = 2500.0;
			this.clrTimer = new System.Timers.Timer();
			this.clrTimer.Elapsed += new ElapsedEventHandler(this.clrTimer_Elapsed);
			this.clrTimer.SynchronizingObject = this;
			this.clrTimer.Interval = 2000.0;
			this.chanTimer.Enabled = true;
			this.loadTimer.Enabled = true;
		}

		private void FormTrunk_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			Form form = (Form)sender;
			Rectangle clientRectangle = form.ClientRectangle;
			graphics.FillRectangle(new SolidBrush(this.mainColor), form.ClientRectangle);
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

		private void panelMain_Paint(object sender, PaintEventArgs e)
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

		private void panelTraffic_Paint(object sender, PaintEventArgs e)
		{
			Panel panel = (Panel)sender;
			Graphics graphics = e.Graphics;
			SolidBrush brush = new SolidBrush(Color.Black);
			Brush brush2 = new SolidBrush(this.mainColor);
			Font font = new Font("Arial", 8f, FontStyle.Bold);
			SizeF sizeF = graphics.MeasureString("HEALTH", font);
			graphics.FillRectangle(brush2, panel.ClientRectangle);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 4), (int)(this.mainColor.G * 3 / 4), (int)(this.mainColor.B * 3 / 4)), 1f), 0, 0, 0, panel.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 7), (int)(this.mainColor.G * 3 / 7), (int)(this.mainColor.B * 3 / 7)), 1f), 1, 1, 1, panel.Height - 2);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 4), (int)(this.mainColor.G * 3 / 4), (int)(this.mainColor.B * 3 / 4)), 1f), 0, 0, panel.Width - 1, 0);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 7), (int)(this.mainColor.G * 3 / 7), (int)(this.mainColor.B * 3 / 7)), 1f), 1, 1, panel.Width - 2, 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), panel.Width - 1, 0, panel.Width - 1, panel.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), panel.Width - 2, 1, panel.Width - 2, panel.Height - 2);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 0, panel.Height - 1, panel.Width - 1, panel.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, panel.Height - 2, panel.Width - 2, panel.Height - 2);
			int value = panel.ClientRectangle.Width - 4;
			Rectangle rect = new Rectangle(2, 2, panel.ClientRectangle.Width - 4, panel.ClientRectangle.Height - 4);
			rect.Width = (int)decimal.Round(this.load_pct / 100m * value);
			Brush brush3;
			if (this.load_pct < 33)
			{
				brush3 = new SolidBrush(Color.Red);
			}
			else if (this.load_pct < 66)
			{
				brush3 = new SolidBrush(Color.Yellow);
			}
			else
			{
				brush3 = new SolidBrush(Color.Lime);
			}
			graphics.FillRectangle(brush3, rect);
			graphics.DrawString("HEALTH", font, brush, ((float)panel.Width - sizeF.Width) / 2f + 2f, ((float)panel.Height - sizeF.Height) / 2f);
		}

		private void checkBox_Paint(object sender, PaintEventArgs e)
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

		private void radioButton_Paint(object sender, PaintEventArgs e)
		{
			RadioButton radioButton = (RadioButton)sender;
			Graphics graphics = e.Graphics;
			SolidBrush brush = new SolidBrush(Color.Lime);
			Brush brush2 = new SolidBrush(this.mainColor);
			Font font = new Font("Arial", 8f, FontStyle.Bold);
			SizeF sizeF = graphics.MeasureString(radioButton.Text, font);
			Rectangle arg_4D_0 = radioButton.ClientRectangle;
			graphics.FillRectangle(brush2, radioButton.ClientRectangle);
			if (radioButton.Checked)
			{
				graphics.DrawString(radioButton.Text, font, brush, ((float)radioButton.Width - sizeF.Width) / 2f + 2f, ((float)radioButton.Height - sizeF.Height) / 2f + 1f);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, 1, radioButton.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 2), (int)(this.mainColor.G / 2), (int)(this.mainColor.B / 2)), 1f), 1, 1, radioButton.Width - 1, 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), radioButton.Width - 1, 1, radioButton.Width - 1, radioButton.Height - 1);
				graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 3 / 2), (int)(this.mainColor.G * 3 / 2), (int)(this.mainColor.B * 3 / 2)), 1f), 1, radioButton.Height - 1, radioButton.Width - 1, radioButton.Height - 1);
				return;
			}
			graphics.DrawString(radioButton.Text, font, brush, ((float)radioButton.Width - sizeF.Width) / 2f + 1f, ((float)radioButton.Height - sizeF.Height) / 2f + 0f);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, 1, radioButton.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R * 2), (int)(this.mainColor.G * 2), (int)(this.mainColor.B * 2)), 1f), 1, 1, radioButton.Width - 1, 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), radioButton.Width - 1, 1, radioButton.Width - 1, radioButton.Height - 1);
			graphics.DrawLine(new Pen(Color.FromArgb((int)(this.mainColor.R / 4), (int)(this.mainColor.G / 4), (int)(this.mainColor.B / 4)), 1f), 1, radioButton.Height - 1, radioButton.Width - 1, radioButton.Height - 1);
		}

		public void resStore(bool res)
		{
			if (res)
			{
				this.Text = "P25 TRUNKING (SUCCESS)";
			}
			else
			{
				this.Text = "P25 TRUNKING (ERROR)";
			}
			this.clrTimer.Enabled = true;
			this.checkBoxTTEna.Enabled = true;
		}

		public void discEvt()
		{
			this.Text = "P25 TRUNKING";
			this.clrTimer.Enabled = false;
			this.checkBoxTTEna.Enabled = true;
			this.checkBoxHold.Checked = false;
			this.checkBoxHold.Text = "HOLD";
		}

		public void ttEvt(byte[] data)
		{
			uint uInt = this.parent.getUInt32(data, 0);
			sbyte b = (sbyte)data[17];
			this.textBoxCCFreq.Text = this.freq2str(uInt);
			if (data[16] == 0)
			{
				this.textBoxChanType.Text = "CONTROL";
			}
			else
			{
				this.textBoxChanType.Text = "VOICE";
			}
			this.textBoxChanRSSI.Text = b.ToString() + " dBm";
			if (data[16] == 0)
			{
				this.textBoxSrcID.Text = "";
				this.textBoxTgtID.Text = "";
				this.textBoxVCFreq.Text = "";
				return;
			}
			uInt = this.parent.getUInt32(data, 8);
			if (uInt == 0u)
			{
				this.textBoxSrcID.Text = "LATE ENTRY";
			}
			else
			{
				this.textBoxSrcID.Text = uInt.ToString();
			}
			uInt = this.parent.getUInt32(data, 12);
			this.textBoxTgtID.Text = uInt.ToString();
			uInt = this.parent.getUInt32(data, 4);
			this.textBoxVCFreq.Text = this.freq2str(uInt);
		}

		private string freq2str(uint freq)
		{
			string text;
			if (freq == 0u)
			{
				text = "0,00000";
			}
			else
			{
				text = freq.ToString();
				text = text.Substring(0, 3) + "," + text.Substring(3, 5);
			}
			return text;
		}

		private uint str2freq(string freq)
		{
			decimal d;
			decimal.TryParse(freq, out d);
			return (uint)(d * 1000000m);
		}

		public void curRssi(int rssi)
		{
			if (this.textBoxChanType.Text != "")
			{
				this.textBoxChanRSSI.Text = rssi.ToString() + " dBm";
			}
		}

		private void updTimerStop()
		{
			this.updTimer.Enabled = false;
		}

		private void updTimerStart()
		{
			this.updTimerStop();
			this.updTimer.Interval = 2000.0;
			this.updTimer.Enabled = true;
		}

		private void tt_clear()
		{
			this.textBoxInfoSystem.Text = (this.textBoxInfoSite.Text = (this.textBoxInfoControl.Text = ""));
			this.textBoxInfoNAC.Text = (this.textBoxInfoChans.Text = "");
			for (int i = 0; i < 16; i++)
			{
				this.tt_ids_bf[i] = 0u;
			}
			for (int j = 0; j < this.MAX_CHANS; j++)
			{
				this.tt_chan_ids[j] = 0;
			}
			this.listViewCallHist.Items.Clear();
			this.listViewChans.Items.Clear();
			this.listViewBandPlan.Items.Clear();
			this.listViewPeers.Items.Clear();
			this.num_chans = (int)(this.system_id = 0);
			this.wacn_id = 0u;
		}

		public void tt_nac(ushort nac)
		{
			if (this.nac_id != nac)
			{
				this.tt_clear();
			}
			this.nac_id = nac;
		}

		private bool tt_add_chan_id(ushort chan)
		{
			for (int i = 0; i < this.MAX_CHANS; i++)
			{
				if (this.tt_chan_ids[i] == 0)
				{
					this.tt_chan_ids[i] = chan;
					this.tt_chan_cnts[i] = 0;
					return true;
				}
			}
			return false;
		}

		private void tt_add_cc(ushort chanT, bool prim)
		{
			int num = chanT >> 12 & 15;
			int num2 = (int)(chanT & 4095);
			if (this.tt_ids_bf[num] == 0u)
			{
				return;
			}
			string text;
			for (int i = 0; i < this.listViewChans.Items.Count; i++)
			{
				text = num.ToString("D2") + "-" + num2.ToString("D4");
				if (text == this.listViewChans.Items[i].SubItems[0].Text)
				{
					return;
				}
			}
			this.num_chans++;
			this.tt_add_chan_id(chanT);
			this.textBoxInfoChans.Text = this.num_chans.ToString();
			text = num.ToString("D2") + "-" + num2.ToString("D4");
			ListViewItem listViewItem;
			if (this.listViewChans.Items.Count == 0)
			{
				listViewItem = this.listViewChans.Items.Add(text);
			}
			else if (this.listViewChans.Items.Count == 1)
			{
				if (prim)
				{
					listViewItem = this.listViewChans.Items.Insert(0, text);
				}
				else
				{
					listViewItem = this.listViewChans.Items.Add(text);
				}
			}
			else if (prim)
			{
				listViewItem = this.listViewChans.Items.Insert(0, text);
			}
			else
			{
				listViewItem = this.listViewChans.Items.Insert(1, text);
			}
			text = ((uint)((ulong)this.tt_ids_bf[num] + (ulong)((long)num2 * (long)((ulong)this.tt_ids_cs[num])))).ToString();
			text = text.Substring(0, 3) + "," + text.Substring(3, 5);
			listViewItem.SubItems.Add(text);
			if (prim)
			{
				listViewItem.SubItems.Add("Primary CC");
			}
			else
			{
				listViewItem.SubItems.Add("Secondary CC");
			}
			listViewItem.SubItems.Add("0");
			listViewItem.SubItems.Add("");
			listViewItem.SubItems.Add("");
			listViewItem.SubItems.Add("");
			text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			listViewItem.SubItems.Add(text);
		}

		private void tt_reg_rsp(byte res, ushort system, uint src_id, uint src_adr)
		{
			if (!this.checkBoxCHCntl.Checked)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.ToLocalTime();
			for (int i = this.listViewCallHist.Items.Count - 1; i >= 0; i--)
			{
				DateTime dateTime2;
				DateTime.TryParse(this.listViewCallHist.Items[i].SubItems[0].Text, out dateTime2);
				if (dateTime.TimeOfDay.Seconds - dateTime2.TimeOfDay.Seconds > 3)
				{
					break;
				}
				if (this.listViewCallHist.Items[i].SubItems[2].Text == "Login" && this.listViewCallHist.Items[i].SubItems[1].Text == src_id.ToString())
				{
					return;
				}
			}
			string text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			ListViewItem listViewItem = this.listViewCallHist.Items.Add(text);
			listViewItem.SubItems.Add(src_id.ToString());
			listViewItem.SubItems.Add("Login");
			if (res == 0)
			{
				text = "Accept";
			}
			else if (res == 1)
			{
				text = "Fail";
			}
			else if (res == 2)
			{
				text = "Deny";
			}
			else
			{
				text = "Refused";
			}
			listViewItem.SubItems.Add(text);
			listViewItem.SubItems.Add(src_adr.ToString());
			listViewItem.SubItems.Add("-");
			if (this.checkBoxCHScroll.Checked)
			{
				this.listViewCallHist.EnsureVisible(this.listViewCallHist.Items.Count - 1);
			}
			this.log2file(listViewItem);
		}

		private void tt_dereg_rsp(uint wacn, ushort system, uint src_id)
		{
			if (!this.checkBoxCHCntl.Checked)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.ToLocalTime();
			for (int i = this.listViewCallHist.Items.Count - 1; i >= 0; i--)
			{
				DateTime dateTime2;
				DateTime.TryParse(this.listViewCallHist.Items[i].SubItems[0].Text, out dateTime2);
				if (dateTime.TimeOfDay.Seconds - dateTime2.TimeOfDay.Seconds > 3)
				{
					break;
				}
				if (this.listViewCallHist.Items[i].SubItems[2].Text == "Logout" && this.listViewCallHist.Items[i].SubItems[1].Text == src_id.ToString())
				{
					return;
				}
			}
			string text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			ListViewItem listViewItem = this.listViewCallHist.Items.Add(text);
			listViewItem.SubItems.Add(src_id.ToString());
			listViewItem.SubItems.Add("Logout");
			listViewItem.SubItems.Add("Accept");
			listViewItem.SubItems.Add(src_id.ToString());
			listViewItem.SubItems.Add("-");
			if (this.checkBoxCHScroll.Checked)
			{
				this.listViewCallHist.EnsureVisible(this.listViewCallHist.Items.Count - 1);
			}
			this.log2file(listViewItem);
		}

		private void tt_loc_reg_rsp(byte res, ushort grp_adr, byte rfss, byte site, uint tgt_adr)
		{
			if (!this.checkBoxCHCntl.Checked)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.ToLocalTime();
			for (int i = this.listViewCallHist.Items.Count - 1; i >= 0; i--)
			{
				DateTime dateTime2;
				DateTime.TryParse(this.listViewCallHist.Items[i].SubItems[0].Text, out dateTime2);
				if (dateTime.TimeOfDay.Seconds - dateTime2.TimeOfDay.Seconds > 3)
				{
					break;
				}
				if (this.listViewCallHist.Items[i].SubItems[2].Text == "Joins" && this.listViewCallHist.Items[i].SubItems[1].Text == tgt_adr.ToString())
				{
					return;
				}
			}
			string text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			ListViewItem listViewItem = this.listViewCallHist.Items.Add(text);
			listViewItem.SubItems.Add(tgt_adr.ToString());
			listViewItem.SubItems.Add("Joins");
			if (res == 0)
			{
				text = "Accept";
			}
			else if (res == 1)
			{
				text = "Fail";
			}
			else if (res == 2)
			{
				text = "Deny";
			}
			else
			{
				text = "Refused";
			}
			listViewItem.SubItems.Add(text);
			listViewItem.SubItems.Add(grp_adr.ToString());
			listViewItem.SubItems.Add("-");
			if (this.checkBoxCHScroll.Checked)
			{
				this.listViewCallHist.EnsureVisible(this.listViewCallHist.Items.Count - 1);
			}
			this.log2file(listViewItem);
		}

		private void tt_grp_aff(byte lg, byte gav, ushort ann_grp_adr, ushort grp_adr, uint tgt_adr)
		{
			if (!this.checkBoxCHCntl.Checked)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.ToLocalTime();
			for (int i = this.listViewCallHist.Items.Count - 1; i >= 0; i--)
			{
				DateTime dateTime2;
				DateTime.TryParse(this.listViewCallHist.Items[i].SubItems[0].Text, out dateTime2);
				if (dateTime.TimeOfDay.Seconds - dateTime2.TimeOfDay.Seconds > 3)
				{
					break;
				}
				if (this.listViewCallHist.Items[i].SubItems[2].Text == "Joins" && this.listViewCallHist.Items[i].SubItems[1].Text == tgt_adr.ToString())
				{
					return;
				}
			}
			string text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			ListViewItem listViewItem = this.listViewCallHist.Items.Add(text);
			listViewItem.SubItems.Add(tgt_adr.ToString());
			listViewItem.SubItems.Add("Joins");
			if (gav == 0)
			{
				text = "Accept";
			}
			else if (gav == 1)
			{
				text = "Fail";
			}
			else if (gav == 2)
			{
				text = "Deny";
			}
			else
			{
				text = "Refused";
			}
			listViewItem.SubItems.Add(text);
			listViewItem.SubItems.Add(grp_adr.ToString());
			listViewItem.SubItems.Add("-");
			if (this.checkBoxCHScroll.Checked)
			{
				this.listViewCallHist.EnsureVisible(this.listViewCallHist.Items.Count - 1);
			}
			this.log2file(listViewItem);
		}

		private void tt_grp_vc_evt(byte srv, ushort grp, uint src)
		{
			if (!this.checkBoxCHVoice.Checked)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.ToLocalTime();
			for (int i = this.listViewCallHist.Items.Count - 1; i >= 0; i--)
			{
				DateTime dateTime2;
				DateTime.TryParse(this.listViewCallHist.Items[i].SubItems[0].Text, out dateTime2);
				if (dateTime.TimeOfDay.Seconds - dateTime2.TimeOfDay.Seconds > 3)
				{
					break;
				}
				if (this.listViewCallHist.Items[i].SubItems[2].Text == "Voice" && this.listViewCallHist.Items[i].SubItems[1].Text == src.ToString())
				{
					return;
				}
			}
			string text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			ListViewItem listViewItem = this.listViewCallHist.Items.Add(text);
			listViewItem.SubItems.Add(src.ToString());
			listViewItem.SubItems.Add("Voice");
			listViewItem.SubItems.Add("Grant");
			listViewItem.SubItems.Add(grp.ToString());
			listViewItem.SubItems.Add(srv.ToString());
			if (this.checkBoxCHScroll.Checked)
			{
				this.listViewCallHist.EnsureVisible(this.listViewCallHist.Items.Count - 1);
			}
			this.log2file(listViewItem);
		}

		private void tt_uu_vc_evt(uint tgt_adr, uint src_adr)
		{
			if (!this.checkBoxCHVoice.Checked)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.ToLocalTime();
			for (int i = this.listViewCallHist.Items.Count - 1; i >= 0; i--)
			{
				DateTime dateTime2;
				DateTime.TryParse(this.listViewCallHist.Items[i].SubItems[0].Text, out dateTime2);
				if (dateTime.TimeOfDay.Seconds - dateTime2.TimeOfDay.Seconds > 3)
				{
					break;
				}
				if (this.listViewCallHist.Items[i].SubItems[2].Text == "Voice" && this.listViewCallHist.Items[i].SubItems[1].Text == src_adr.ToString())
				{
					return;
				}
			}
			string text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			ListViewItem listViewItem = this.listViewCallHist.Items.Add(text);
			listViewItem.SubItems.Add(src_adr.ToString());
			listViewItem.SubItems.Add("Voice");
			listViewItem.SubItems.Add("Grant");
			listViewItem.SubItems.Add(tgt_adr.ToString());
			listViewItem.SubItems.Add("-");
			if (this.checkBoxCHScroll.Checked)
			{
				this.listViewCallHist.EnsureVisible(this.listViewCallHist.Items.Count - 1);
			}
			this.log2file(listViewItem);
		}

		private void tt_grp_dc_grant(byte svc, ushort chanT, ushort chanR, uint tgt_adr)
		{
			if (!this.checkBoxCHData.Checked)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.ToLocalTime();
			for (int i = this.listViewCallHist.Items.Count - 1; i >= 0; i--)
			{
				DateTime dateTime2;
				DateTime.TryParse(this.listViewCallHist.Items[i].SubItems[0].Text, out dateTime2);
				if (dateTime.TimeOfDay.Seconds - dateTime2.TimeOfDay.Seconds > 3)
				{
					break;
				}
				if (this.listViewCallHist.Items[i].SubItems[2].Text == "Data" && this.listViewCallHist.Items[i].SubItems[1].Text == tgt_adr.ToString())
				{
					return;
				}
			}
			string text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			ListViewItem listViewItem = this.listViewCallHist.Items.Add(text);
			listViewItem.SubItems.Add(tgt_adr.ToString());
			listViewItem.SubItems.Add("Data");
			listViewItem.SubItems.Add("Grant");
			listViewItem.SubItems.Add(tgt_adr.ToString());
			listViewItem.SubItems.Add(svc.ToString());
			if (this.checkBoxCHScroll.Checked)
			{
				this.listViewCallHist.EnsureVisible(this.listViewCallHist.Items.Count - 1);
			}
			this.log2file(listViewItem);
		}

		private void tt_page_req(byte svc, ushort dataAccCtrl, uint tgt_adr)
		{
			if (!this.checkBoxCHData.Checked)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.ToLocalTime();
			for (int i = this.listViewCallHist.Items.Count - 1; i >= 0; i--)
			{
				DateTime dateTime2;
				DateTime.TryParse(this.listViewCallHist.Items[i].SubItems[0].Text, out dateTime2);
				if (dateTime.TimeOfDay.Seconds - dateTime2.TimeOfDay.Seconds > 3)
				{
					break;
				}
				if (this.listViewCallHist.Items[i].SubItems[2].Text == "Page" && this.listViewCallHist.Items[i].SubItems[1].Text == tgt_adr.ToString())
				{
					return;
				}
			}
			string text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			ListViewItem listViewItem = this.listViewCallHist.Items.Add(text);
			listViewItem.SubItems.Add(tgt_adr.ToString());
			listViewItem.SubItems.Add("Page");
			listViewItem.SubItems.Add("Grant");
			listViewItem.SubItems.Add(tgt_adr.ToString());
			listViewItem.SubItems.Add(svc.ToString());
			if (this.checkBoxCHScroll.Checked)
			{
				this.listViewCallHist.EnsureVisible(this.listViewCallHist.Items.Count - 1);
			}
			this.log2file(listViewItem);
		}

		private void tt_sccb(byte rfss, byte site, ushort chan1, byte ss_class1, ushort chan2, byte ss_class2)
		{
			this.tt_add_cc(chan1, false);
			this.tt_add_cc(chan2, false);
		}

		private void tt_sccb_exp(byte rfss, byte site, ushort chanT, ushort chanR, byte ss_class)
		{
			this.tt_add_cc(chanT, false);
		}

		private void tt_iden_up(byte id, byte bw, int txo, uint cs, uint bf)
		{
			string text;
			for (int i = 0; i < this.listViewBandPlan.Items.Count; i++)
			{
				text = this.listViewBandPlan.Items[i].SubItems[0].Text;
				if (text == id.ToString("D2"))
				{
					return;
				}
			}
			this.tt_ids_bf[(int)id] = bf;
			this.tt_ids_cs[(int)id] = cs;
			this.tt_ids_txo[(int)id] = txo;
			this.tt_ids_bw[(int)id] = bw;
			ListViewItem listViewItem = this.listViewBandPlan.Items.Add(id.ToString("D2"));
			text = bf.ToString();
			text = text.Substring(0, 3) + "," + text.Substring(3, 5);
			listViewItem.SubItems.Add(text);
			listViewItem.SubItems.Add(id.ToString("D2") + "-0000");
			listViewItem.SubItems.Add(id.ToString("D2") + "-4095");
			text = (cs / 1000m).ToString("N2");
			listViewItem.SubItems.Add(text);
			if (bw == 5)
			{
				text = "12,5";
			}
			else if (bw == 4)
			{
				text = "6,25";
			}
			else
			{
				text = (bw / 1000m).ToString("N3");
			}
			listViewItem.SubItems.Add(text);
			text = (txo / 1000000m).ToString("N3");
			listViewItem.SubItems.Add(text);
		}

		private void tt_rfss_sts(byte lra, byte flags, ushort system, byte rfss, byte site, ushort chanT, ushort chanR, byte ss_class)
		{
			int num = chanT >> 12 & 15;
			int num2 = (int)(chanT & 4095);
			if (this.tt_ids_bf[num] == 0u)
			{
				return;
			}
			string text = ((uint)((ulong)this.tt_ids_bf[num] + (ulong)((long)num2 * (long)((ulong)this.tt_ids_cs[num])))).ToString();
			this.textBoxInfoSite.Text = rfss.ToString("D3") + "-" + site.ToString("D3");
			string text2 = num.ToString("D2") + "-" + num2.ToString("D4");
			string text3 = text2;
			text2 = string.Concat(new string[]
			{
				text3,
				" (",
				text.Substring(0, 3),
				",",
				text.Substring(3, 5),
				")"
			});
			this.textBoxInfoControl.Text = text2;
		}

		private void tt_adj_sts(byte lra, byte flags, ushort system, byte rfss, byte site, ushort chanT, ushort chanR, byte ss_class)
		{
			int num = chanT >> 12 & 15;
			int num2 = (int)(chanT & 4095);
			if (this.tt_ids_bf[num] == 0u)
			{
				return;
			}
			string text;
			for (int i = 0; i < this.listViewPeers.Items.Count; i++)
			{
				text = rfss.ToString("D3") + "-" + site.ToString("D3");
				if (text == this.listViewPeers.Items[i].SubItems[1].Text)
				{
					text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
					this.listViewPeers.Items[i].SubItems[4].Text = text;
					return;
				}
			}
			text = this.wacn_id.ToString("X5") + "-" + system.ToString("X3");
			ListViewItem listViewItem = this.listViewPeers.Items.Add(text);
			text = rfss.ToString("D3") + "-" + site.ToString("D3");
			listViewItem.SubItems.Add(text);
			text = num.ToString("D2") + "-" + num2.ToString("D4");
			listViewItem.SubItems.Add(text);
			text = ((uint)((ulong)this.tt_ids_bf[num] + (ulong)((long)num2 * (long)((ulong)this.tt_ids_cs[num])))).ToString();
			text = text.Substring(0, 3) + "," + text.Substring(3, 5);
			listViewItem.SubItems.Add(text);
			text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			listViewItem.SubItems.Add(text);
		}

		private void tt_net_sts(byte lra, uint wacn, ushort system, ushort chanT, ushort chanR, byte ss_class)
		{
			if (wacn == 0u)
			{
				return;
			}
			this.wacn_id = wacn;
			this.system_id = system;
			this.textBoxInfoSystem.Text = this.wacn_id.ToString("X5") + "-" + system.ToString("X3");
			this.textBoxInfoNAC.Text = this.nac_id.ToString("X3");
			this.textBoxInfoChans.Text = this.num_chans.ToString();
			this.tt_add_cc(chanT, true);
		}

		private void tt_grp_vc_grant(byte srv, ushort chanT, ushort chanR, ushort group, uint src)
		{
			int num = chanT >> 12 & 15;
			int num2 = (int)(chanT & 4095);
			if (this.tt_ids_bf[num] == 0u)
			{
				return;
			}
			if (this.listViewChans.Items.Count == 0)
			{
				return;
			}
			this.tt_grp_vc_evt(srv, group, src);
			int i;
			string text;
			for (i = 0; i < this.listViewChans.Items.Count; i++)
			{
				text = num.ToString("D2") + "-" + num2.ToString("D4");
				if (text == this.listViewChans.Items[i].SubItems[0].Text)
				{
					if (this.listViewChans.Items[i].SubItems[4].Text != group.ToString() || this.listViewChans.Items[i].SubItems[5].Text != "G" || this.listViewChans.Items[i].SubItems[6].Text != src.ToString())
					{
						int num3;
						int.TryParse(this.listViewChans.Items[i].SubItems[3].Text, out num3);
						num3++;
						this.listViewChans.Items[i].SubItems[3].Text = num3.ToString();
					}
					this.listViewChans.Items[i].SubItems[4].Text = group.ToString();
					this.listViewChans.Items[i].SubItems[5].Text = "G";
					this.listViewChans.Items[i].SubItems[6].Text = src.ToString();
					text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
					this.listViewChans.Items[i].SubItems[7].Text = text;
					this.tt_chan_cnts[i] = this.MAX_HOLD_TIME;
					return;
				}
			}
			this.num_chans++;
			this.tt_add_chan_id(chanT);
			this.textBoxInfoChans.Text = this.num_chans.ToString();
			text = num.ToString("D2") + "-" + num2.ToString("D4");
			ListViewItem listViewItem = this.listViewChans.Items.Add(text);
			text = ((uint)((ulong)this.tt_ids_bf[num] + (ulong)((long)num2 * (long)((ulong)this.tt_ids_cs[num])))).ToString();
			text = text.Substring(0, 3) + "," + text.Substring(3, 5);
			listViewItem.SubItems.Add(text);
			listViewItem.SubItems.Add("Traffic");
			listViewItem.SubItems.Add("1");
			listViewItem.SubItems.Add(group.ToString());
			listViewItem.SubItems.Add("G");
			listViewItem.SubItems.Add(src.ToString());
			text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			listViewItem.SubItems.Add(text);
			this.tt_chan_cnts[i] = this.MAX_HOLD_TIME;
		}

		private void tt_grp_vc_grant_updt(ushort chanT, ushort chanR, ushort group)
		{
			int num = chanT >> 12 & 15;
			int num2 = (int)(chanT & 4095);
			if (this.tt_ids_bf[num] == 0u)
			{
				return;
			}
			for (int i = 0; i < this.listViewChans.Items.Count; i++)
			{
				string a = num.ToString("D2") + "-" + num2.ToString("D4");
				if (a == this.listViewChans.Items[i].SubItems[0].Text)
				{
					this.tt_chan_cnts[i] = this.MAX_HOLD_TIME;
					return;
				}
			}
		}

		private void tt_uu_vc_grant(ushort chanT, ushort chanR, uint tgt_adr, uint src_adr)
		{
			int num = chanT >> 12 & 15;
			int num2 = (int)(chanT & 4095);
			if (this.tt_ids_bf[num] == 0u)
			{
				return;
			}
			if (this.listViewChans.Items.Count == 0)
			{
				return;
			}
			this.tt_uu_vc_evt(tgt_adr, src_adr);
			int i;
			string text;
			for (i = 0; i < this.listViewChans.Items.Count; i++)
			{
				text = num.ToString("D2") + "-" + num2.ToString("D4");
				if (text == this.listViewChans.Items[i].SubItems[0].Text)
				{
					if (this.listViewChans.Items[i].SubItems[4].Text != tgt_adr.ToString() || this.listViewChans.Items[i].SubItems[5].Text != "I" || this.listViewChans.Items[i].SubItems[6].Text != src_adr.ToString())
					{
						int num3;
						int.TryParse(this.listViewChans.Items[i].SubItems[3].Text, out num3);
						num3++;
						this.listViewChans.Items[i].SubItems[3].Text = num3.ToString();
					}
					this.listViewChans.Items[i].SubItems[4].Text = tgt_adr.ToString();
					this.listViewChans.Items[i].SubItems[5].Text = "I";
					this.listViewChans.Items[i].SubItems[6].Text = src_adr.ToString();
					text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
					this.listViewChans.Items[i].SubItems[7].Text = text;
					this.tt_chan_cnts[i] = this.MAX_HOLD_TIME;
					return;
				}
			}
			this.num_chans++;
			this.tt_add_chan_id(chanT);
			this.textBoxInfoChans.Text = this.num_chans.ToString();
			text = num.ToString("D2") + "-" + num2.ToString("D4");
			ListViewItem listViewItem = this.listViewChans.Items.Add(text);
			text = ((uint)((ulong)this.tt_ids_bf[num] + (ulong)((long)num2 * (long)((ulong)this.tt_ids_cs[num])))).ToString();
			text = text.Substring(0, 3) + "," + text.Substring(3, 5);
			listViewItem.SubItems.Add(text);
			listViewItem.SubItems.Add("Traffic");
			listViewItem.SubItems.Add("1");
			listViewItem.SubItems.Add(tgt_adr.ToString());
			listViewItem.SubItems.Add("I");
			listViewItem.SubItems.Add(src_adr.ToString());
			text = DateTime.Now.ToLocalTime().ToString("HH:mm:ss");
			listViewItem.SubItems.Add(text);
			this.tt_chan_cnts[i] = this.MAX_HOLD_TIME;
		}

		private void tt_uu_vc_grant_updt(ushort chanT, ushort chanR, uint tgt_adr, uint src_adr)
		{
			int num = chanT >> 12 & 15;
			int num2 = (int)(chanT & 4095);
			if (this.tt_ids_bf[num] == 0u)
			{
				return;
			}
			for (int i = 0; i < this.listViewChans.Items.Count; i++)
			{
				string a = num.ToString("D2") + "-" + num2.ToString("D4");
				if (a == this.listViewChans.Items[i].SubItems[0].Text)
				{
					this.tt_chan_cnts[i] = this.MAX_HOLD_TIME;
					return;
				}
			}
		}

		public void putTSBK(byte[] data)
		{
			this.load_cnt++;
			if (data[1] == 144)
			{
				byte b = data[0] & 63;
				if (b == 2)
				{
					int num = (int)data[2];
					int num2 = (int)this.parent.getUInt16r(data, 3);
					int num3 = (int)this.parent.getUInt16r(data, 5);
					int num4 = (int)this.parent.getUInt24r(data, 7);
					this.tt_grp_vc_grant((byte)num, (ushort)num2, 0, (ushort)num3, (uint)num4);
					return;
				}
				if (b == 3)
				{
					int num = (int)this.parent.getUInt16r(data, 2);
					int num2 = (int)this.parent.getUInt16r(data, 4);
					this.tt_grp_vc_grant_updt((ushort)num, 0, (ushort)num2);
					num = (int)this.parent.getUInt16r(data, 6);
					num2 = (int)this.parent.getUInt16r(data, 8);
					this.tt_grp_vc_grant_updt((ushort)num, 0, (ushort)num2);
				}
				return;
			}
			else
			{
				if (data[1] != 0)
				{
					return;
				}
				if ((data[0] & 64) == 64)
				{
					return;
				}
				byte b = data[0] & 63;
				if (b == 52)
				{
					int num = data[2] >> 4;
					int num2 = (int)(data[2] & 15);
					int num3 = (int)data[3] << 6 | (data[4] >> 2 & 63);
					int num4 = ((int)(data[4] & 3) << 8 | (int)data[5]) * 125;
					int num5 = (int)(this.parent.getUInt32r(data, 6) * 5u);
					int num6;
					if ((num3 & 8192) == 8192)
					{
						num6 = -1;
					}
					else
					{
						num6 = 1;
					}
					num3 &= -8193;
					num3 = num6 * (num3 * num4);
					this.tt_iden_up((byte)num, (byte)num2, num3, (uint)num4, (uint)num5);
					return;
				}
				if (b == 61)
				{
					int num = data[2] >> 4;
					int num2 = ((int)(data[2] & 15) << 5 | data[3] >> 3) * 125;
					int num3 = (int)(data[3] & 7) << 6 | (data[4] >> 2 & 63);
					int num4 = ((int)(data[4] & 3) << 8 | (int)data[5]) * 125;
					int num5 = (int)(this.parent.getUInt32r(data, 6) * 5u);
					int num6;
					if ((num3 & 8192) == 8192)
					{
						num6 = -1;
					}
					else
					{
						num6 = 1;
					}
					num3 &= -8193;
					num3 = num6 * (num3 * num4);
					this.tt_iden_up((byte)num, (byte)num2, num3, (uint)num4, (uint)num5);
					return;
				}
				if (b == 0)
				{
					int num = (int)data[2];
					int num2 = (int)this.parent.getUInt16r(data, 3);
					int num3 = (int)this.parent.getUInt16r(data, 5);
					int num4 = (int)this.parent.getUInt24r(data, 7);
					this.tt_grp_vc_grant((byte)num, (ushort)num2, 0, (ushort)num3, (uint)num4);
					return;
				}
				if (b == 2)
				{
					int num = (int)this.parent.getUInt16r(data, 2);
					int num2 = (int)this.parent.getUInt16r(data, 4);
					this.tt_grp_vc_grant_updt((ushort)num, 0, (ushort)num2);
					num = (int)this.parent.getUInt16r(data, 6);
					num2 = (int)this.parent.getUInt16r(data, 8);
					this.tt_grp_vc_grant_updt((ushort)num, 0, (ushort)num2);
					return;
				}
				if (b == 3)
				{
					int num = (int)this.parent.getUInt16r(data, 4);
					int num2 = (int)this.parent.getUInt16r(data, 6);
					int num3 = (int)this.parent.getUInt16r(data, 8);
					this.tt_grp_vc_grant_updt((ushort)num, (ushort)num2, (ushort)num3);
					return;
				}
				if (b == 4)
				{
					int num = (int)this.parent.getUInt16r(data, 2);
					int num2 = (int)this.parent.getUInt24r(data, 4);
					int num3 = (int)this.parent.getUInt24r(data, 7);
					this.tt_uu_vc_grant((ushort)num, 0, (uint)num2, (uint)num3);
					return;
				}
				if (b == 5)
				{
					return;
				}
				if (b == 6)
				{
					int num = (int)this.parent.getUInt16r(data, 2);
					int num2 = (int)this.parent.getUInt24r(data, 4);
					int num3 = (int)this.parent.getUInt24r(data, 7);
					this.tt_uu_vc_grant_updt((ushort)num, 0, (uint)num2, (uint)num3);
					return;
				}
				if (b == 20)
				{
					int num = (int)data[2];
					int num2 = (int)this.parent.getUInt16r(data, 3);
					int num3 = (int)this.parent.getUInt16r(data, 5);
					int num4 = (int)this.parent.getUInt24r(data, 7);
					this.tt_grp_dc_grant((byte)num, (ushort)num2, (ushort)num3, (uint)num4);
					return;
				}
				if (b == 21)
				{
					int num = (int)data[2];
					int num2 = (int)this.parent.getUInt16r(data, 5);
					int num3 = (int)this.parent.getUInt24r(data, 7);
					this.tt_page_req((byte)num, (ushort)num2, (uint)num3);
					return;
				}
				if (b == 22)
				{
					return;
				}
				if (b == 40)
				{
					int num = data[2] >> 7 & 1;
					int num2 = (int)(data[2] & 3);
					int num3 = (int)this.parent.getUInt16r(data, 3);
					int num4 = (int)this.parent.getUInt16r(data, 5);
					int num5 = (int)this.parent.getUInt24r(data, 7);
					this.tt_grp_aff((byte)num, (byte)num2, (ushort)num3, (ushort)num4, (uint)num5);
					return;
				}
				if (b == 41)
				{
					int num = (int)data[2];
					int num2 = (int)data[3];
					int num3 = (int)this.parent.getUInt16r(data, 4);
					int num4 = (int)this.parent.getUInt16r(data, 7);
					int num5 = (int)data[9];
					this.tt_sccb_exp((byte)num, (byte)num2, (ushort)num3, (ushort)num4, (byte)num5);
					return;
				}
				if (b == 44)
				{
					int num = data[2] >> 4 & 3;
					int num2 = (int)(data[2] & 15) << 8 | (int)data[3];
					int num3 = (int)this.parent.getUInt24r(data, 4);
					int num4 = (int)this.parent.getUInt24r(data, 7);
					this.tt_reg_rsp((byte)num, (ushort)num2, (uint)num3, (uint)num4);
					return;
				}
				if (b == 47)
				{
					int num = (int)data[3] << 12 | (int)data[4] << 4 | (data[5] >> 4 & 15);
					int num2 = (int)(data[5] & 15) << 8 | (int)data[6];
					int num3 = (int)this.parent.getUInt24r(data, 7);
					this.tt_dereg_rsp((uint)num, (ushort)num2, (uint)num3);
					return;
				}
				if (b == 43)
				{
					int num = (int)(data[2] & 3);
					int num2 = (int)this.parent.getUInt16r(data, 3);
					int num3 = (int)data[5];
					int num4 = (int)data[6];
					int num5 = (int)this.parent.getUInt24r(data, 7);
					this.tt_loc_reg_rsp((byte)num, (ushort)num2, (byte)num3, (byte)num4, (uint)num5);
					return;
				}
				if (b == 57)
				{
					int num = (int)data[2];
					int num2 = (int)data[3];
					int num3 = (int)this.parent.getUInt16r(data, 4);
					int num4 = (int)data[6];
					int num5 = (int)this.parent.getUInt16r(data, 7);
					int num7 = (int)data[9];
					this.tt_sccb((byte)num, (byte)num2, (ushort)num3, (byte)num4, (ushort)num5, (byte)num7);
					return;
				}
				if (b == 58)
				{
					int num = (int)data[2];
					int num2 = data[3] >> 4 & 3;
					int num3 = (int)(data[3] & 15) << 8 | (int)data[4];
					int num4 = (int)data[5];
					int num5 = (int)data[6];
					int num7 = (int)this.parent.getUInt16r(data, 7);
					int num8 = (int)data[9];
					this.tt_rfss_sts((byte)num, (byte)num2, (ushort)num3, (byte)num4, (byte)num5, (ushort)num7, 0, (byte)num8);
					return;
				}
				if (b == 59)
				{
					int num = (int)data[2];
					int num2 = (int)data[3] << 12 | (int)data[4] << 4 | (data[5] >> 4 & 15);
					int num3 = (int)(data[5] & 15) << 8 | (int)data[6];
					int num4 = (int)this.parent.getUInt16r(data, 7);
					int num5 = (int)data[9];
					this.tt_net_sts((byte)num, (uint)num2, (ushort)num3, (ushort)num4, 0, (byte)num5);
					return;
				}
				if (b == 60)
				{
					int num = (int)data[2];
					int num2 = data[3] >> 4 & 15;
					int num3 = (int)(data[3] & 15) << 8 | (int)data[4];
					int num4 = (int)data[5];
					int num5 = (int)data[6];
					int num7 = (int)this.parent.getUInt16r(data, 7);
					int num8 = (int)data[9];
					this.tt_adj_sts((byte)num, (byte)num2, (ushort)num3, (byte)num4, (byte)num5, (ushort)num7, 0, (byte)num8);
				}
				return;
			}
		}

		public void putPDU(byte[] data, int size)
		{
			byte b = data[7];
			this.load_cnt++;
			if (data[2] != 0)
			{
				return;
			}
			if (data[1] == 255)
			{
				return;
			}
			if (data[1] != 253)
			{
				return;
			}
			if (b == 0)
			{
				int num = (int)data[8];
				int num2 = (int)this.parent.getUInt16r(data, 14);
				int num3 = (int)this.parent.getUInt16r(data, 16);
				int num4 = (int)this.parent.getUInt16r(data, 18);
				int num5 = (int)this.parent.getUInt24r(data, 3);
				this.tt_grp_vc_grant((byte)num, (ushort)num2, (ushort)num3, (ushort)num4, (uint)num5);
				return;
			}
			if (b == 4)
			{
				int num = (int)this.parent.getUInt16r(data, 22);
				int num2 = (int)this.parent.getUInt16r(data, 24);
				int num3 = (int)this.parent.getUInt24r(data, 19);
				int num4 = (int)this.parent.getUInt24r(data, 3);
				this.tt_uu_vc_grant((ushort)num, (ushort)num2, (uint)num3, (uint)num4);
				return;
			}
			if (b == 6)
			{
				int num = (int)this.parent.getUInt16r(data, 22);
				int num2 = (int)this.parent.getUInt16r(data, 24);
				int num3 = (int)this.parent.getUInt24r(data, 19);
				int num4 = (int)this.parent.getUInt24r(data, 3);
				this.tt_uu_vc_grant_updt((ushort)num, (ushort)num2, (uint)num3, (uint)num4);
				return;
			}
			if (b == 40)
			{
				int num = data[20] >> 7 & 1;
				int num2 = (int)(data[20] & 3);
				int num3 = (int)this.parent.getUInt16r(data, 16);
				int num4 = (int)this.parent.getUInt16r(data, 18);
				int num5 = (int)this.parent.getUInt24r(data, 3);
				this.tt_grp_aff((byte)num, (byte)num2, (ushort)num3, (ushort)num4, (uint)num5);
				return;
			}
			if (b == 44)
			{
				int num = (int)(data[17] & 3);
				int num2 = (int)(data[12] & 15) << 8 | (int)data[13];
				int num3 = (int)this.parent.getUInt24r(data, 14);
				int num4 = (int)this.parent.getUInt24r(data, 3);
				this.tt_reg_rsp((byte)num, (ushort)num2, (uint)num3, (uint)num4);
				return;
			}
			if (b == 58)
			{
				int num = (int)data[3];
				int num2 = data[4] >> 4 & 3;
				int num3 = (int)(data[4] & 15) << 8 | (int)data[5];
				int num4 = (int)data[12];
				int num5 = (int)data[13];
				int num6 = (int)this.parent.getUInt16r(data, 14);
				int uInt16r = (int)this.parent.getUInt16r(data, 16);
				int num7 = (int)data[18];
				this.tt_rfss_sts((byte)num, (byte)num2, (ushort)num3, (byte)num4, (byte)num5, (ushort)num6, (ushort)uInt16r, (byte)num7);
				return;
			}
			if (b == 59)
			{
				int num = (int)data[3];
				int num2 = (int)data[12] << 12 | (int)data[13] << 4 | (data[14] >> 4 & 15);
				int num3 = (int)(data[4] & 15) << 8 | (int)data[5];
				int num4 = (int)this.parent.getUInt16r(data, 15);
				int num5 = (int)this.parent.getUInt16r(data, 17);
				int num6 = (int)data[19];
				this.tt_net_sts((byte)num, (uint)num2, (ushort)num3, (ushort)num4, (ushort)num5, (byte)num6);
				return;
			}
			if (b == 60)
			{
				int num = (int)data[3];
				int num2 = data[4] >> 4 & 15;
				int num3 = (int)(data[4] & 15) << 8 | (int)data[5];
				int num4 = (int)data[8];
				int num5 = (int)data[9];
				int num6 = (int)this.parent.getUInt16r(data, 12);
				int uInt16r = (int)this.parent.getUInt16r(data, 14);
				int num7 = (int)data[15];
				this.tt_adj_sts((byte)num, (byte)num2, (ushort)num3, (byte)num4, (byte)num5, (ushort)num6, (ushort)uInt16r, (byte)num7);
			}
		}

		public byte[] getData()
		{
			byte[] array = new byte[2];
			array[0] = (array[1] = 0);
			if (this.checkBoxTTEna.Checked)
			{
				array[0] = 1;
			}
			return array;
		}

		public void putData(byte[] data)
		{
			this.par_lock = true;
			ushort uInt = this.parent.getUInt16(this.parent.dev, 24);
			if ((uInt & 1024) == 1024)
			{
				this.checkBoxTTEna.Checked = true;
			}
			else
			{
				this.checkBoxTTEna.Checked = false;
			}
			if ((uInt & 2048) == 2048)
			{
				this.checkBoxCCSEna.Checked = true;
			}
			else
			{
				this.checkBoxCCSEna.Checked = false;
			}
			this.clrTimer.Enabled = false;
			this.Text = "P25 TRUNKING";
			this.par_lock = false;
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

		private void log2file(ListViewItem lvi)
		{
			if (this.logging)
			{
				string text = string.Concat(new string[]
				{
					"\"",
					lvi.SubItems[0].Text,
					"\";\"",
					lvi.SubItems[1].Text,
					"\";\"",
					lvi.SubItems[2].Text,
					"\";\""
				});
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					lvi.SubItems[3].Text,
					"\";\"",
					lvi.SubItems[4].Text,
					"\";\"",
					lvi.SubItems[5].Text,
					"\""
				});
				this.log2file(text);
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

		private void checkBox_MouseDown(object sender, MouseEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			checkBox.Checked = true;
			if (checkBox == this.checkBoxClose)
			{
				checkBox.Checked = false;
				base.Close();
				return;
			}
			if (checkBox == this.checkBoxCHClear)
			{
				checkBox.Checked = false;
				this.listViewCallHist.Items.Clear();
				return;
			}
			if (checkBox == this.checkBoxAdd)
			{
				bool flag = this.filtCheck(this.textBoxTgtID.Text);
				if (flag)
				{
					this.filtAdd(this.textBoxTgtID.Text);
					return;
				}
			}
			else if (checkBox == this.checkBoxRem)
			{
				bool flag = this.filtRemove();
				if (flag)
				{
					this.updTimerStart();
					return;
				}
			}
			else if (checkBox == this.checkBoxLog)
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
						this.log2file("Stamp;Source;Action;Result;Target;Service");
					}
					catch (Exception)
					{
					}
				}
			}
		}

		private void checkBox_MouseUp(object sender, MouseEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			checkBox.Checked = false;
		}

		private void checkBoxTTEna_CheckStateChanged(object sender, EventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			ushort num = this.parent.getUInt16(this.parent.dev, 24);
			if (checkBox.Checked)
			{
				this.parent.state = 2;
			}
			if (this.par_lock)
			{
				return;
			}
			if (this.checkBoxCCSEna.Checked)
			{
				num |= 2048;
			}
			else
			{
				num &= 2047;
			}
			if (checkBox.Checked)
			{
				num |= 1024;
				this.parent.putUInt16(this.parent.dev, 24, num);
				bool flag = this.parent.comParamsWrite();
				if (flag)
				{
					checkBox.Enabled = false;
					this.Text = "P25 TRUNKING (ENABLING...)";
					return;
				}
			}
			else
			{
				this.parent.state = 0;
				num &= 64511;
				this.parent.putUInt16(this.parent.dev, 24, num);
				bool flag2 = this.parent.comParamsWrite();
				if (flag2)
				{
					checkBox.Enabled = false;
					this.Text = "P25 TRUNKING (DISABLING...)";
					this.textBoxCCFreq.Text = "";
					this.textBoxVCFreq.Text = "";
					this.textBoxChanType.Text = "";
					this.textBoxChanRSSI.Text = "";
					this.textBoxSrcID.Text = "";
					this.textBoxTgtID.Text = "";
				}
			}
		}

		private void checkBoxHold_CheckStateChanged(object sender, EventArgs e)
		{
			if (this.checkBoxHold.Checked && this.textBoxTgtID.Text != "")
			{
				ushort grp;
				ushort.TryParse(this.textBoxTgtID.Text, out grp);
				this.parent.comTTHR(grp);
				this.checkBoxHold.Text = "HOLD (" + grp.ToString() + ")";
			}
			if (!this.checkBoxHold.Checked)
			{
				this.parent.comTTHR(0);
				this.checkBoxHold.Text = "HOLD";
			}
		}

		private void radioButtonTabs_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton radioButton = (RadioButton)sender;
			this.panelPeers.Visible = false;
			this.panelTrunkInfo.Visible = false;
			this.panelTracker.Visible = false;
			this.panelBandPlan.Visible = false;
			this.panelChans.Visible = false;
			this.panelCallHist.Visible = false;
			this.panelCHCtrl.Visible = false;
			if (radioButton == this.radioButtonInfo)
			{
				this.panelTrunkInfo.Visible = true;
				return;
			}
			if (radioButton == this.radioButtonTracker)
			{
				this.panelTracker.Visible = true;
				return;
			}
			if (radioButton == this.radioButtonPeers)
			{
				this.panelPeers.Visible = true;
				return;
			}
			if (radioButton == this.radioButtonBandPlan)
			{
				this.panelBandPlan.Visible = true;
				return;
			}
			if (radioButton == this.radioButtonChans)
			{
				this.panelChans.Visible = true;
				return;
			}
			if (radioButton == this.radioButtonCallHist)
			{
				this.panelCallHist.Visible = true;
				this.panelCHCtrl.Visible = true;
			}
		}

		private void updTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.updTimerStop();
			this.parent.comTTFiltWr();
		}

		private void chanTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			for (int i = 0; i < this.MAX_CHANS; i++)
			{
				if (this.tt_chan_ids[i] != 0 && this.tt_chan_cnts[i] > 0)
				{
					this.tt_chan_cnts[i]--;
					if (this.tt_chan_cnts[i] == 0)
					{
						this.listViewChans.Items[i].SubItems[4].Text = "";
						this.listViewChans.Items[i].SubItems[5].Text = "";
						this.listViewChans.Items[i].SubItems[6].Text = "";
					}
				}
			}
		}

		private void loadTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.load_pct = (int)decimal.Round(this.load_cnt / 70m * 100m);
			if (this.load_pct > 100)
			{
				this.load_pct = 100;
			}
			this.load_cnt = 0;
			this.panelTraffic.Invalidate();
		}

		private void clrTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.clrTimer.Enabled = false;
			this.Text = "P25 TRUNKING";
		}

		public byte[] filtGet()
		{
			byte[] array = new byte[132];
			array[0] = (array[1] = 0);
			if (this.checkBoxTTFEna.Checked)
			{
				byte[] expr_29_cp_0 = array;
				int expr_29_cp_1 = 0;
				expr_29_cp_0[expr_29_cp_1] |= 1;
			}
			if (this.checkBoxTTFExclude.Checked)
			{
				byte[] expr_4B_cp_0 = array;
				int expr_4B_cp_1 = 0;
				expr_4B_cp_0[expr_4B_cp_1] |= 2;
			}
			array[1] = 0;
			this.parent.putUInt16(array, 2, 64);
			for (int i = 0; i < this.listViewFilt1.Items.Count; i++)
			{
				ushort v;
				ushort.TryParse(this.listViewFilt1.Items[i].Text, out v);
				this.parent.putUInt16(array, 4 + 2 * i, v);
			}
			for (int i = 0; i < this.listViewFilt2.Items.Count; i++)
			{
				ushort v;
				ushort.TryParse(this.listViewFilt2.Items[i].Text, out v);
				this.parent.putUInt16(array, 36 + 2 * i, v);
			}
			for (int i = 0; i < this.listViewFilt3.Items.Count; i++)
			{
				ushort v;
				ushort.TryParse(this.listViewFilt3.Items[i].Text, out v);
				this.parent.putUInt16(array, 68 + 2 * i, v);
			}
			for (int i = 0; i < this.listViewFilt4.Items.Count; i++)
			{
				ushort v;
				ushort.TryParse(this.listViewFilt4.Items[i].Text, out v);
				this.parent.putUInt16(array, 100 + 2 * i, v);
			}
			return array;
		}

		public byte[] filtSet(byte[] buff)
		{
			if ((buff[0] & 1) == 1)
			{
				this.checkBoxTTFEna.Checked = true;
			}
			else
			{
				this.checkBoxTTFEna.Checked = false;
			}
			if ((buff[0] & 2) == 2)
			{
				this.checkBoxTTFExclude.Checked = true;
			}
			else
			{
				this.checkBoxTTFExclude.Checked = false;
			}
			for (int i = 0; i < this.listViewFilt1.Items.Count; i++)
			{
				ushort uInt = this.parent.getUInt16(buff, 4 + 2 * i);
				this.listViewFilt1.Items[i].Text = uInt.ToString();
			}
			for (int i = 0; i < this.listViewFilt2.Items.Count; i++)
			{
				ushort uInt = this.parent.getUInt16(buff, 36 + 2 * i);
				this.listViewFilt2.Items[i].Text = uInt.ToString();
			}
			for (int i = 0; i < this.listViewFilt3.Items.Count; i++)
			{
				ushort uInt = this.parent.getUInt16(buff, 68 + 2 * i);
				this.listViewFilt3.Items[i].Text = uInt.ToString();
			}
			for (int i = 0; i < this.listViewFilt4.Items.Count; i++)
			{
				ushort uInt = this.parent.getUInt16(buff, 100 + 2 * i);
				this.listViewFilt4.Items[i].Text = uInt.ToString();
			}
			return buff;
		}

		private bool filtCheck(string val)
		{
			for (int i = 0; i < this.listViewFilt1.Items.Count; i++)
			{
				string text = this.listViewFilt1.Items[i].Text;
				if (text != "0" && text == val)
				{
					return false;
				}
			}
			for (int i = 0; i < this.listViewFilt2.Items.Count; i++)
			{
				string text = this.listViewFilt2.Items[i].Text;
				if (text != "0" && text == val)
				{
					return false;
				}
			}
			for (int i = 0; i < this.listViewFilt3.Items.Count; i++)
			{
				string text = this.listViewFilt3.Items[i].Text;
				if (text != "0" && text == val)
				{
					return false;
				}
			}
			for (int i = 0; i < this.listViewFilt4.Items.Count; i++)
			{
				string text = this.listViewFilt4.Items[i].Text;
				if (text != "0" && text == val)
				{
					return false;
				}
			}
			return true;
		}

		private bool filtRemove()
		{
			bool result = false;
			for (int i = 0; i < this.listViewFilt1.SelectedItems.Count; i++)
			{
				if (this.listViewFilt1.SelectedItems[i].Text != "0")
				{
					this.listViewFilt1.SelectedItems[i].Text = "0";
					result = true;
				}
			}
			for (int i = 0; i < this.listViewFilt2.SelectedItems.Count; i++)
			{
				if (this.listViewFilt2.SelectedItems[i].Text != "0")
				{
					this.listViewFilt2.SelectedItems[i].Text = "0";
					result = true;
				}
			}
			for (int i = 0; i < this.listViewFilt3.SelectedItems.Count; i++)
			{
				if (this.listViewFilt3.SelectedItems[i].Text != "0")
				{
					this.listViewFilt3.SelectedItems[i].Text = "0";
					result = true;
				}
			}
			for (int i = 0; i < this.listViewFilt4.SelectedItems.Count; i++)
			{
				if (this.listViewFilt4.SelectedItems[i].Text != "0")
				{
					this.listViewFilt4.SelectedItems[i].Text = "0";
					result = true;
				}
			}
			return result;
		}

		private bool filtAdd(string val)
		{
			if (val == "0" || val == "")
			{
				return false;
			}
			for (int i = 0; i < this.listViewFilt1.Items.Count; i++)
			{
				if (this.listViewFilt1.Items[i].Text == "0")
				{
					this.listViewFilt1.Items[i].Text = val;
					this.updTimerStart();
					return true;
				}
			}
			for (int i = 0; i < this.listViewFilt2.Items.Count; i++)
			{
				if (this.listViewFilt2.Items[i].Text == "0")
				{
					this.listViewFilt2.Items[i].Text = val;
					this.updTimerStart();
					return true;
				}
			}
			for (int i = 0; i < this.listViewFilt3.Items.Count; i++)
			{
				if (this.listViewFilt3.Items[i].Text == "0")
				{
					this.listViewFilt3.Items[i].Text = val;
					this.updTimerStart();
					return true;
				}
			}
			for (int i = 0; i < this.listViewFilt4.Items.Count; i++)
			{
				if (this.listViewFilt4.Items[i].Text == "0")
				{
					this.listViewFilt4.Items[i].Text = val;
					this.updTimerStart();
					return true;
				}
			}
			return false;
		}

		private void listViewFilt_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView listView = (ListView)sender;
			if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
			{
				if (listView == this.listViewFilt1)
				{
					this.listViewFilt2.SelectedItems.Clear();
					this.listViewFilt3.SelectedItems.Clear();
					this.listViewFilt4.SelectedItems.Clear();
				}
				else if (listView == this.listViewFilt2)
				{
					this.listViewFilt1.SelectedItems.Clear();
					this.listViewFilt3.SelectedItems.Clear();
					this.listViewFilt4.SelectedItems.Clear();
				}
				else if (listView == this.listViewFilt3)
				{
					this.listViewFilt1.SelectedItems.Clear();
					this.listViewFilt2.SelectedItems.Clear();
					this.listViewFilt4.SelectedItems.Clear();
				}
				else if (listView == this.listViewFilt4)
				{
					this.listViewFilt1.SelectedItems.Clear();
					this.listViewFilt2.SelectedItems.Clear();
					this.listViewFilt3.SelectedItems.Clear();
				}
			}
			for (int i = 0; i < listView.Items.Count; i++)
			{
				listView.Items[i].BackColor = listView.BackColor;
			}
			for (int j = 0; j < listView.SelectedItems.Count; j++)
			{
				listView.SelectedItems[j].BackColor = SystemColors.Highlight;
			}
		}

		private void checkBoxNFExclude_CheckStateChanged(object sender, EventArgs e)
		{
			this.updTimerStart();
		}

		private void checkBoxNFEna_CheckStateChanged(object sender, EventArgs e)
		{
			this.updTimerStart();
		}

		private void listViewFilt_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			ListView arg_06_0 = (ListView)sender;
			uint num;
			bool flag = uint.TryParse(e.Label, out num);
			bool flag2 = this.filtCheck(e.Label);
			if (!flag || !flag2 || num > 65535u)
			{
				e.CancelEdit = true;
				return;
			}
			this.updTimerStart();
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
			ListViewItem listViewItem = new ListViewItem("0");
			ListViewItem listViewItem2 = new ListViewItem("0");
			ListViewItem listViewItem3 = new ListViewItem("0");
			ListViewItem listViewItem4 = new ListViewItem("0");
			ListViewItem listViewItem5 = new ListViewItem("0");
			ListViewItem listViewItem6 = new ListViewItem("0");
			ListViewItem listViewItem7 = new ListViewItem("0");
			ListViewItem listViewItem8 = new ListViewItem("0");
			ListViewItem listViewItem9 = new ListViewItem("0");
			ListViewItem listViewItem10 = new ListViewItem("0");
			ListViewItem listViewItem11 = new ListViewItem("0");
			ListViewItem listViewItem12 = new ListViewItem("0");
			ListViewItem listViewItem13 = new ListViewItem("0");
			ListViewItem listViewItem14 = new ListViewItem("0");
			ListViewItem listViewItem15 = new ListViewItem("0");
			ListViewItem listViewItem16 = new ListViewItem("0");
			ListViewItem listViewItem17 = new ListViewItem("0");
			ListViewItem listViewItem18 = new ListViewItem("0");
			ListViewItem listViewItem19 = new ListViewItem("0");
			ListViewItem listViewItem20 = new ListViewItem("0");
			ListViewItem listViewItem21 = new ListViewItem("0");
			ListViewItem listViewItem22 = new ListViewItem("0");
			ListViewItem listViewItem23 = new ListViewItem("0");
			ListViewItem listViewItem24 = new ListViewItem("0");
			ListViewItem listViewItem25 = new ListViewItem("0");
			ListViewItem listViewItem26 = new ListViewItem("0");
			ListViewItem listViewItem27 = new ListViewItem("0");
			ListViewItem listViewItem28 = new ListViewItem("0");
			ListViewItem listViewItem29 = new ListViewItem("0");
			ListViewItem listViewItem30 = new ListViewItem("0");
			ListViewItem listViewItem31 = new ListViewItem("0");
			ListViewItem listViewItem32 = new ListViewItem("0");
			ListViewItem listViewItem33 = new ListViewItem("0");
			ListViewItem listViewItem34 = new ListViewItem("0");
			ListViewItem listViewItem35 = new ListViewItem("0");
			ListViewItem listViewItem36 = new ListViewItem("0");
			ListViewItem listViewItem37 = new ListViewItem("0");
			ListViewItem listViewItem38 = new ListViewItem("0");
			ListViewItem listViewItem39 = new ListViewItem("0");
			ListViewItem listViewItem40 = new ListViewItem("0");
			ListViewItem listViewItem41 = new ListViewItem("0");
			ListViewItem listViewItem42 = new ListViewItem("0");
			ListViewItem listViewItem43 = new ListViewItem("0");
			ListViewItem listViewItem44 = new ListViewItem("0");
			ListViewItem listViewItem45 = new ListViewItem("0");
			ListViewItem listViewItem46 = new ListViewItem("0");
			ListViewItem listViewItem47 = new ListViewItem("0");
			ListViewItem listViewItem48 = new ListViewItem("0");
			ListViewItem listViewItem49 = new ListViewItem("0");
			ListViewItem listViewItem50 = new ListViewItem("0");
			ListViewItem listViewItem51 = new ListViewItem("0");
			ListViewItem listViewItem52 = new ListViewItem("0");
			ListViewItem listViewItem53 = new ListViewItem("0");
			ListViewItem listViewItem54 = new ListViewItem("0");
			ListViewItem listViewItem55 = new ListViewItem("0");
			ListViewItem listViewItem56 = new ListViewItem("0");
			ListViewItem listViewItem57 = new ListViewItem("0");
			ListViewItem listViewItem58 = new ListViewItem("0");
			ListViewItem listViewItem59 = new ListViewItem("0");
			ListViewItem listViewItem60 = new ListViewItem("0");
			ListViewItem listViewItem61 = new ListViewItem("0");
			ListViewItem listViewItem62 = new ListViewItem("0");
			ListViewItem listViewItem63 = new ListViewItem("0");
			ListViewItem listViewItem64 = new ListViewItem("0");
			this.panelMain = new Panel();
			this.panelTraffic = new Panel();
			this.radioButtonChans = new RadioButton();
			this.radioButtonTracker = new RadioButton();
			this.radioButtonInfo = new RadioButton();
			this.radioButtonBandPlan = new RadioButton();
			this.checkBoxClose = new CheckBox();
			this.radioButtonCallHist = new RadioButton();
			this.radioButtonPeers = new RadioButton();
			this.panelCCFreq = new Panel();
			this.textBoxCCFreq = new TextBox();
			this.checkBoxTTEna = new CheckBox();
			this.toolTip = new ToolTip(this.components);
			this.checkBoxHold = new CheckBox();
			this.checkBoxCHScroll = new CheckBox();
			this.checkBoxCHCntl = new CheckBox();
			this.checkBoxCHData = new CheckBox();
			this.checkBoxCHVoice = new CheckBox();
			this.checkBoxCHClear = new CheckBox();
			this.checkBoxLog = new CheckBox();
			this.checkBoxRem = new CheckBox();
			this.checkBoxAdd = new CheckBox();
			this.panelTrunkInfo = new Panel();
			this.labelInfoSite = new Label();
			this.panelInfoSite = new Panel();
			this.textBoxInfoSite = new TextBox();
			this.labelInfoSystem = new Label();
			this.panelInfoSystem = new Panel();
			this.textBoxInfoSystem = new TextBox();
			this.labelInfoChans = new Label();
			this.panelInfoChans = new Panel();
			this.textBoxInfoChans = new TextBox();
			this.labelInfoControl = new Label();
			this.panelInfoControl = new Panel();
			this.textBoxInfoControl = new TextBox();
			this.labelInfoNAC = new Label();
			this.panelInfoNAC = new Panel();
			this.textBoxInfoNAC = new TextBox();
			this.panelTracker = new Panel();
			this.checkBoxTTFExclude = new CheckBox();
			this.checkBoxCCSEna = new CheckBox();
			this.checkBoxTTFEna = new CheckBox();
			this.labelTTFExclude = new Label();
			this.labelCCSEna = new Label();
			this.labelTTFEna = new Label();
			this.labelTgtID = new Label();
			this.labelSrcID = new Label();
			this.panelFilt = new Panel();
			this.listViewFilt4 = new ListView();
			this.columnHeader3 = new ColumnHeader();
			this.listViewFilt3 = new ListView();
			this.columnHeader2 = new ColumnHeader();
			this.listViewFilt2 = new ListView();
			this.columnHeader1 = new ColumnHeader();
			this.listViewFilt1 = new ListView();
			this.columnHeaderIDs = new ColumnHeader();
			this.labelChanRSSI = new Label();
			this.labelChanType = new Label();
			this.labelVCFreq = new Label();
			this.labelCC = new Label();
			this.panelTgtID = new Panel();
			this.textBoxTgtID = new TextBox();
			this.panelSrcID = new Panel();
			this.textBoxSrcID = new TextBox();
			this.panelChanRSSI = new Panel();
			this.textBoxChanRSSI = new TextBox();
			this.panelVCFreq = new Panel();
			this.textBoxVCFreq = new TextBox();
			this.panelChanType = new Panel();
			this.textBoxChanType = new TextBox();
			this.panelPeers = new Panel();
			this.labelPeersLast = new Label();
			this.labelPeersFreq = new Label();
			this.labelPeersControl = new Label();
			this.labelPeersSite = new Label();
			this.labelPeersSystem = new Label();
			this.listViewPeers = new ListView();
			this.system = new ColumnHeader();
			this.site = new ColumnHeader();
			this.control = new ColumnHeader();
			this.frequency = new ColumnHeader();
			this.last = new ColumnHeader();
			this.panelBandPlan = new Panel();
			this.labelBandPlanBase = new Label();
			this.labelBandPlanLo = new Label();
			this.labelBandPlanHi = new Label();
			this.labelBandPlanSpacing = new Label();
			this.labelBandPlanBW = new Label();
			this.labelBandPlanTXO = new Label();
			this.labelBandPlanID = new Label();
			this.listViewBandPlan = new ListView();
			this.id = new ColumnHeader();
			this.bf = new ColumnHeader();
			this.lo = new ColumnHeader();
			this.hi = new ColumnHeader();
			this.cs = new ColumnHeader();
			this.bw = new ColumnHeader();
			this.txo = new ColumnHeader();
			this.panelChans = new Panel();
			this.labelChansFreq = new Label();
			this.labelChansLabel = new Label();
			this.labelChansHits = new Label();
			this.labelChansTarget = new Label();
			this.labelChansT = new Label();
			this.labelChansSource = new Label();
			this.labelChansLast = new Label();
			this.labelChansLCN = new Label();
			this.listViewChans = new ListView();
			this.chan_lcn = new ColumnHeader();
			this.chan_freq = new ColumnHeader();
			this.chan_label = new ColumnHeader();
			this.chan_hits = new ColumnHeader();
			this.chan_tgt = new ColumnHeader();
			this.chan_t = new ColumnHeader();
			this.chan_src = new ColumnHeader();
			this.chan_last = new ColumnHeader();
			this.panelCallHist = new Panel();
			this.labelCallHistSrcID = new Label();
			this.labelCallHistAction = new Label();
			this.labelCallHistResult = new Label();
			this.labelCallHistTgtID = new Label();
			this.labelCallHistService = new Label();
			this.labelCallHistStamp = new Label();
			this.listViewCallHist = new ListView();
			this.ch_stamp = new ColumnHeader();
			this.ch_srcid = new ColumnHeader();
			this.ch_action = new ColumnHeader();
			this.ch_res = new ColumnHeader();
			this.ch_tgtid = new ColumnHeader();
			this.ch_svc = new ColumnHeader();
			this.panelCHCtrl = new Panel();
			this.saveFileDialogLog = new SaveFileDialog();
			this.panelMain.SuspendLayout();
			this.panelCCFreq.SuspendLayout();
			this.panelTrunkInfo.SuspendLayout();
			this.panelInfoSite.SuspendLayout();
			this.panelInfoSystem.SuspendLayout();
			this.panelInfoChans.SuspendLayout();
			this.panelInfoControl.SuspendLayout();
			this.panelInfoNAC.SuspendLayout();
			this.panelTracker.SuspendLayout();
			this.panelFilt.SuspendLayout();
			this.panelTgtID.SuspendLayout();
			this.panelSrcID.SuspendLayout();
			this.panelChanRSSI.SuspendLayout();
			this.panelVCFreq.SuspendLayout();
			this.panelChanType.SuspendLayout();
			this.panelPeers.SuspendLayout();
			this.panelBandPlan.SuspendLayout();
			this.panelChans.SuspendLayout();
			this.panelCallHist.SuspendLayout();
			this.panelCHCtrl.SuspendLayout();
			base.SuspendLayout();
			this.panelMain.Controls.Add(this.panelTraffic);
			this.panelMain.Controls.Add(this.radioButtonChans);
			this.panelMain.Controls.Add(this.radioButtonTracker);
			this.panelMain.Controls.Add(this.radioButtonInfo);
			this.panelMain.Controls.Add(this.radioButtonBandPlan);
			this.panelMain.Controls.Add(this.checkBoxClose);
			this.panelMain.Controls.Add(this.radioButtonCallHist);
			this.panelMain.Controls.Add(this.radioButtonPeers);
			this.panelMain.Location = new Point(0, 1);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new Size(640, 317);
			this.panelMain.TabIndex = 0;
			this.panelMain.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.panelTraffic.Location = new Point(516, 10);
			this.panelTraffic.Name = "panelTraffic";
			this.panelTraffic.Size = new Size(70, 23);
			this.panelTraffic.TabIndex = 31;
			this.panelTraffic.Paint += new PaintEventHandler(this.panelTraffic_Paint);
			this.radioButtonChans.Appearance = Appearance.Button;
			this.radioButtonChans.AutoSize = true;
			this.radioButtonChans.Location = new Point(58, 10);
			this.radioButtonChans.Name = "radioButtonChans";
			this.radioButtonChans.Size = new Size(75, 23);
			this.radioButtonChans.TabIndex = 29;
			this.radioButtonChans.Text = "CHANNELS";
			this.radioButtonChans.UseVisualStyleBackColor = true;
			this.radioButtonChans.Paint += new PaintEventHandler(this.radioButton_Paint);
			this.radioButtonChans.CheckedChanged += new EventHandler(this.radioButtonTabs_CheckedChanged);
			this.radioButtonTracker.Appearance = Appearance.Button;
			this.radioButtonTracker.AutoSize = true;
			this.radioButtonTracker.Location = new Point(381, 10);
			this.radioButtonTracker.Name = "radioButtonTracker";
			this.radioButtonTracker.Size = new Size(68, 23);
			this.radioButtonTracker.TabIndex = 29;
			this.radioButtonTracker.Text = "TRACKER";
			this.radioButtonTracker.UseVisualStyleBackColor = true;
			this.radioButtonTracker.Paint += new PaintEventHandler(this.radioButton_Paint);
			this.radioButtonTracker.CheckedChanged += new EventHandler(this.radioButtonTabs_CheckedChanged);
			this.radioButtonInfo.Appearance = Appearance.Button;
			this.radioButtonInfo.AutoSize = true;
			this.radioButtonInfo.Checked = true;
			this.radioButtonInfo.Location = new Point(10, 10);
			this.radioButtonInfo.Name = "radioButtonInfo";
			this.radioButtonInfo.Size = new Size(42, 23);
			this.radioButtonInfo.TabIndex = 29;
			this.radioButtonInfo.TabStop = true;
			this.radioButtonInfo.Text = "INFO";
			this.radioButtonInfo.UseVisualStyleBackColor = true;
			this.radioButtonInfo.Paint += new PaintEventHandler(this.radioButton_Paint);
			this.radioButtonInfo.CheckedChanged += new EventHandler(this.radioButtonTabs_CheckedChanged);
			this.radioButtonBandPlan.Appearance = Appearance.Button;
			this.radioButtonBandPlan.AutoSize = true;
			this.radioButtonBandPlan.Location = new Point(297, 10);
			this.radioButtonBandPlan.Name = "radioButtonBandPlan";
			this.radioButtonBandPlan.Size = new Size(78, 23);
			this.radioButtonBandPlan.TabIndex = 29;
			this.radioButtonBandPlan.Text = "BAND PLAN";
			this.radioButtonBandPlan.UseVisualStyleBackColor = true;
			this.radioButtonBandPlan.Paint += new PaintEventHandler(this.radioButton_Paint);
			this.radioButtonBandPlan.CheckedChanged += new EventHandler(this.radioButtonTabs_CheckedChanged);
			this.checkBoxClose.Appearance = Appearance.Button;
			this.checkBoxClose.FlatStyle = FlatStyle.Flat;
			this.checkBoxClose.Location = new Point(455, 10);
			this.checkBoxClose.Name = "checkBoxClose";
			this.checkBoxClose.Size = new Size(55, 23);
			this.checkBoxClose.TabIndex = 22;
			this.checkBoxClose.Text = "CLOSE";
			this.toolTip.SetToolTip(this.checkBoxClose, "Close the form");
			this.checkBoxClose.UseVisualStyleBackColor = true;
			this.checkBoxClose.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxClose.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxClose.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.radioButtonCallHist.Appearance = Appearance.Button;
			this.radioButtonCallHist.AutoSize = true;
			this.radioButtonCallHist.Location = new Point(139, 10);
			this.radioButtonCallHist.Name = "radioButtonCallHist";
			this.radioButtonCallHist.Size = new Size(94, 23);
			this.radioButtonCallHist.TabIndex = 29;
			this.radioButtonCallHist.Text = "CALL HISTORY";
			this.radioButtonCallHist.UseVisualStyleBackColor = true;
			this.radioButtonCallHist.Paint += new PaintEventHandler(this.radioButton_Paint);
			this.radioButtonCallHist.CheckedChanged += new EventHandler(this.radioButtonTabs_CheckedChanged);
			this.radioButtonPeers.Appearance = Appearance.Button;
			this.radioButtonPeers.AutoSize = true;
			this.radioButtonPeers.Location = new Point(239, 10);
			this.radioButtonPeers.Name = "radioButtonPeers";
			this.radioButtonPeers.Size = new Size(53, 23);
			this.radioButtonPeers.TabIndex = 29;
			this.radioButtonPeers.Text = "PEERS";
			this.radioButtonPeers.UseVisualStyleBackColor = true;
			this.radioButtonPeers.Paint += new PaintEventHandler(this.radioButton_Paint);
			this.radioButtonPeers.CheckedChanged += new EventHandler(this.radioButtonTabs_CheckedChanged);
			this.panelCCFreq.Controls.Add(this.textBoxCCFreq);
			this.panelCCFreq.Location = new Point(171, 12);
			this.panelCCFreq.Name = "panelCCFreq";
			this.panelCCFreq.Size = new Size(100, 19);
			this.panelCCFreq.TabIndex = 26;
			this.panelCCFreq.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxCCFreq.BackColor = Color.Black;
			this.textBoxCCFreq.BorderStyle = BorderStyle.None;
			this.textBoxCCFreq.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxCCFreq.ForeColor = Color.Lime;
			this.textBoxCCFreq.Location = new Point(2, 2);
			this.textBoxCCFreq.MaxLength = 10;
			this.textBoxCCFreq.Multiline = true;
			this.textBoxCCFreq.Name = "textBoxCCFreq";
			this.textBoxCCFreq.ReadOnly = true;
			this.textBoxCCFreq.Size = new Size(96, 15);
			this.textBoxCCFreq.TabIndex = 2;
			this.textBoxCCFreq.TextAlign = HorizontalAlignment.Center;
			this.checkBoxTTEna.Appearance = Appearance.Button;
			this.checkBoxTTEna.Location = new Point(12, 133);
			this.checkBoxTTEna.Name = "checkBoxTTEna";
			this.checkBoxTTEna.Size = new Size(153, 20);
			this.checkBoxTTEna.TabIndex = 24;
			this.checkBoxTTEna.Text = "TRACKER ENABLE";
			this.toolTip.SetToolTip(this.checkBoxTTEna, "Enable/Disable trunk tracker");
			this.checkBoxTTEna.UseVisualStyleBackColor = true;
			this.checkBoxTTEna.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxTTEna.CheckStateChanged += new EventHandler(this.checkBoxTTEna_CheckStateChanged);
			this.checkBoxHold.Appearance = Appearance.Button;
			this.checkBoxHold.FlatStyle = FlatStyle.Flat;
			this.checkBoxHold.Location = new Point(171, 133);
			this.checkBoxHold.Name = "checkBoxHold";
			this.checkBoxHold.Size = new Size(100, 20);
			this.checkBoxHold.TabIndex = 21;
			this.checkBoxHold.Text = "HOLD";
			this.toolTip.SetToolTip(this.checkBoxHold, "Hold/Release current target ID");
			this.checkBoxHold.UseVisualStyleBackColor = true;
			this.checkBoxHold.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxHold.CheckStateChanged += new EventHandler(this.checkBoxHold_CheckStateChanged);
			this.checkBoxCHScroll.Appearance = Appearance.Button;
			this.checkBoxCHScroll.Checked = true;
			this.checkBoxCHScroll.CheckState = CheckState.Checked;
			this.checkBoxCHScroll.FlatStyle = FlatStyle.Flat;
			this.checkBoxCHScroll.Location = new Point(267, 3);
			this.checkBoxCHScroll.Name = "checkBoxCHScroll";
			this.checkBoxCHScroll.Size = new Size(60, 20);
			this.checkBoxCHScroll.TabIndex = 21;
			this.checkBoxCHScroll.Text = "SCROLL";
			this.toolTip.SetToolTip(this.checkBoxCHScroll, "Scroll to the last line");
			this.checkBoxCHScroll.UseVisualStyleBackColor = true;
			this.checkBoxCHScroll.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxCHCntl.Appearance = Appearance.Button;
			this.checkBoxCHCntl.Checked = true;
			this.checkBoxCHCntl.CheckState = CheckState.Checked;
			this.checkBoxCHCntl.FlatStyle = FlatStyle.Flat;
			this.checkBoxCHCntl.Location = new Point(201, 3);
			this.checkBoxCHCntl.Name = "checkBoxCHCntl";
			this.checkBoxCHCntl.Size = new Size(60, 20);
			this.checkBoxCHCntl.TabIndex = 21;
			this.checkBoxCHCntl.Text = "CONTROL";
			this.toolTip.SetToolTip(this.checkBoxCHCntl, "Include control messages");
			this.checkBoxCHCntl.UseVisualStyleBackColor = true;
			this.checkBoxCHCntl.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxCHData.Appearance = Appearance.Button;
			this.checkBoxCHData.Checked = true;
			this.checkBoxCHData.CheckState = CheckState.Checked;
			this.checkBoxCHData.FlatStyle = FlatStyle.Flat;
			this.checkBoxCHData.Location = new Point(135, 3);
			this.checkBoxCHData.Name = "checkBoxCHData";
			this.checkBoxCHData.Size = new Size(60, 20);
			this.checkBoxCHData.TabIndex = 21;
			this.checkBoxCHData.Text = "DATA";
			this.toolTip.SetToolTip(this.checkBoxCHData, "Include data calls");
			this.checkBoxCHData.UseVisualStyleBackColor = true;
			this.checkBoxCHData.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxCHVoice.Appearance = Appearance.Button;
			this.checkBoxCHVoice.Checked = true;
			this.checkBoxCHVoice.CheckState = CheckState.Checked;
			this.checkBoxCHVoice.FlatStyle = FlatStyle.Flat;
			this.checkBoxCHVoice.Location = new Point(69, 3);
			this.checkBoxCHVoice.Name = "checkBoxCHVoice";
			this.checkBoxCHVoice.Size = new Size(60, 20);
			this.checkBoxCHVoice.TabIndex = 21;
			this.checkBoxCHVoice.Text = "VOICE";
			this.toolTip.SetToolTip(this.checkBoxCHVoice, "Include voice calls");
			this.checkBoxCHVoice.UseVisualStyleBackColor = true;
			this.checkBoxCHVoice.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxCHClear.Appearance = Appearance.Button;
			this.checkBoxCHClear.FlatStyle = FlatStyle.Flat;
			this.checkBoxCHClear.Location = new Point(3, 3);
			this.checkBoxCHClear.Name = "checkBoxCHClear";
			this.checkBoxCHClear.Size = new Size(60, 20);
			this.checkBoxCHClear.TabIndex = 21;
			this.checkBoxCHClear.Text = "CLEAR";
			this.toolTip.SetToolTip(this.checkBoxCHClear, "Clear call history");
			this.checkBoxCHClear.UseVisualStyleBackColor = true;
			this.checkBoxCHClear.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxCHClear.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxCHClear.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxLog.Appearance = Appearance.Button;
			this.checkBoxLog.FlatStyle = FlatStyle.Flat;
			this.checkBoxLog.Location = new Point(333, 3);
			this.checkBoxLog.Name = "checkBoxLog";
			this.checkBoxLog.Size = new Size(80, 20);
			this.checkBoxLog.TabIndex = 21;
			this.checkBoxLog.Text = "LOG START";
			this.toolTip.SetToolTip(this.checkBoxLog, "Start/Stop call history log");
			this.checkBoxLog.UseVisualStyleBackColor = true;
			this.checkBoxLog.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxLog.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxLog.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxRem.Appearance = Appearance.Button;
			this.checkBoxRem.FlatStyle = FlatStyle.Flat;
			this.checkBoxRem.Location = new Point(482, 35);
			this.checkBoxRem.Name = "checkBoxRem";
			this.checkBoxRem.Size = new Size(84, 20);
			this.checkBoxRem.TabIndex = 21;
			this.checkBoxRem.Text = "REMOVE";
			this.toolTip.SetToolTip(this.checkBoxRem, "Remove selected IDs from filter");
			this.checkBoxRem.UseVisualStyleBackColor = true;
			this.checkBoxRem.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxRem.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxRem.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.checkBoxAdd.Appearance = Appearance.Button;
			this.checkBoxAdd.FlatStyle = FlatStyle.Flat;
			this.checkBoxAdd.Location = new Point(482, 8);
			this.checkBoxAdd.Name = "checkBoxAdd";
			this.checkBoxAdd.Size = new Size(84, 20);
			this.checkBoxAdd.TabIndex = 21;
			this.checkBoxAdd.Text = "ADD";
			this.toolTip.SetToolTip(this.checkBoxAdd, "Add target ID to filter");
			this.checkBoxAdd.UseVisualStyleBackColor = true;
			this.checkBoxAdd.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxAdd.MouseDown += new MouseEventHandler(this.checkBox_MouseDown);
			this.checkBoxAdd.MouseUp += new MouseEventHandler(this.checkBox_MouseUp);
			this.panelTrunkInfo.Controls.Add(this.labelInfoSite);
			this.panelTrunkInfo.Controls.Add(this.panelInfoSite);
			this.panelTrunkInfo.Controls.Add(this.labelInfoSystem);
			this.panelTrunkInfo.Controls.Add(this.panelInfoSystem);
			this.panelTrunkInfo.Controls.Add(this.labelInfoChans);
			this.panelTrunkInfo.Controls.Add(this.panelInfoChans);
			this.panelTrunkInfo.Controls.Add(this.labelInfoControl);
			this.panelTrunkInfo.Controls.Add(this.panelInfoControl);
			this.panelTrunkInfo.Controls.Add(this.labelInfoNAC);
			this.panelTrunkInfo.Controls.Add(this.panelInfoNAC);
			this.panelTrunkInfo.Location = new Point(659, 11);
			this.panelTrunkInfo.Name = "panelTrunkInfo";
			this.panelTrunkInfo.Size = new Size(573, 113);
			this.panelTrunkInfo.TabIndex = 27;
			this.panelTrunkInfo.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.labelInfoSite.BackColor = Color.DimGray;
			this.labelInfoSite.ForeColor = Color.Gold;
			this.labelInfoSite.Location = new Point(13, 35);
			this.labelInfoSite.Name = "labelInfoSite";
			this.labelInfoSite.Size = new Size(125, 14);
			this.labelInfoSite.TabIndex = 25;
			this.labelInfoSite.Text = "SITE";
			this.panelInfoSite.Controls.Add(this.textBoxInfoSite);
			this.panelInfoSite.Location = new Point(143, 32);
			this.panelInfoSite.Name = "panelInfoSite";
			this.panelInfoSite.Size = new Size(150, 20);
			this.panelInfoSite.TabIndex = 26;
			this.panelInfoSite.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxInfoSite.BackColor = Color.Black;
			this.textBoxInfoSite.BorderStyle = BorderStyle.None;
			this.textBoxInfoSite.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxInfoSite.ForeColor = Color.Lime;
			this.textBoxInfoSite.Location = new Point(2, 2);
			this.textBoxInfoSite.MaxLength = 10;
			this.textBoxInfoSite.Multiline = true;
			this.textBoxInfoSite.Name = "textBoxInfoSite";
			this.textBoxInfoSite.ReadOnly = true;
			this.textBoxInfoSite.Size = new Size(146, 15);
			this.textBoxInfoSite.TabIndex = 2;
			this.textBoxInfoSite.TextAlign = HorizontalAlignment.Center;
			this.labelInfoSystem.BackColor = Color.DimGray;
			this.labelInfoSystem.ForeColor = Color.Gold;
			this.labelInfoSystem.Location = new Point(13, 15);
			this.labelInfoSystem.Name = "labelInfoSystem";
			this.labelInfoSystem.Size = new Size(125, 14);
			this.labelInfoSystem.TabIndex = 25;
			this.labelInfoSystem.Text = "SYSTEM";
			this.panelInfoSystem.Controls.Add(this.textBoxInfoSystem);
			this.panelInfoSystem.Location = new Point(143, 12);
			this.panelInfoSystem.Name = "panelInfoSystem";
			this.panelInfoSystem.Size = new Size(150, 20);
			this.panelInfoSystem.TabIndex = 26;
			this.panelInfoSystem.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxInfoSystem.BackColor = Color.Black;
			this.textBoxInfoSystem.BorderStyle = BorderStyle.None;
			this.textBoxInfoSystem.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxInfoSystem.ForeColor = Color.Lime;
			this.textBoxInfoSystem.Location = new Point(2, 2);
			this.textBoxInfoSystem.MaxLength = 10;
			this.textBoxInfoSystem.Multiline = true;
			this.textBoxInfoSystem.Name = "textBoxInfoSystem";
			this.textBoxInfoSystem.ReadOnly = true;
			this.textBoxInfoSystem.Size = new Size(146, 15);
			this.textBoxInfoSystem.TabIndex = 2;
			this.textBoxInfoSystem.TextAlign = HorizontalAlignment.Center;
			this.labelInfoChans.BackColor = Color.DimGray;
			this.labelInfoChans.ForeColor = Color.Gold;
			this.labelInfoChans.Location = new Point(13, 95);
			this.labelInfoChans.Name = "labelInfoChans";
			this.labelInfoChans.Size = new Size(125, 14);
			this.labelInfoChans.TabIndex = 25;
			this.labelInfoChans.Text = "CHANNELS";
			this.panelInfoChans.Controls.Add(this.textBoxInfoChans);
			this.panelInfoChans.Location = new Point(143, 92);
			this.panelInfoChans.Name = "panelInfoChans";
			this.panelInfoChans.Size = new Size(150, 20);
			this.panelInfoChans.TabIndex = 26;
			this.panelInfoChans.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxInfoChans.BackColor = Color.Black;
			this.textBoxInfoChans.BorderStyle = BorderStyle.None;
			this.textBoxInfoChans.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxInfoChans.ForeColor = Color.Lime;
			this.textBoxInfoChans.Location = new Point(2, 2);
			this.textBoxInfoChans.MaxLength = 10;
			this.textBoxInfoChans.Multiline = true;
			this.textBoxInfoChans.Name = "textBoxInfoChans";
			this.textBoxInfoChans.ReadOnly = true;
			this.textBoxInfoChans.Size = new Size(146, 15);
			this.textBoxInfoChans.TabIndex = 2;
			this.textBoxInfoChans.TextAlign = HorizontalAlignment.Center;
			this.labelInfoControl.BackColor = Color.DimGray;
			this.labelInfoControl.ForeColor = Color.Gold;
			this.labelInfoControl.Location = new Point(13, 55);
			this.labelInfoControl.Name = "labelInfoControl";
			this.labelInfoControl.Size = new Size(125, 14);
			this.labelInfoControl.TabIndex = 25;
			this.labelInfoControl.Text = "CONTROL";
			this.panelInfoControl.Controls.Add(this.textBoxInfoControl);
			this.panelInfoControl.Location = new Point(143, 52);
			this.panelInfoControl.Name = "panelInfoControl";
			this.panelInfoControl.Size = new Size(150, 20);
			this.panelInfoControl.TabIndex = 26;
			this.panelInfoControl.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxInfoControl.BackColor = Color.Black;
			this.textBoxInfoControl.BorderStyle = BorderStyle.None;
			this.textBoxInfoControl.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxInfoControl.ForeColor = Color.Lime;
			this.textBoxInfoControl.Location = new Point(2, 2);
			this.textBoxInfoControl.MaxLength = 10;
			this.textBoxInfoControl.Multiline = true;
			this.textBoxInfoControl.Name = "textBoxInfoControl";
			this.textBoxInfoControl.ReadOnly = true;
			this.textBoxInfoControl.Size = new Size(146, 15);
			this.textBoxInfoControl.TabIndex = 2;
			this.textBoxInfoControl.TextAlign = HorizontalAlignment.Center;
			this.labelInfoNAC.BackColor = Color.DimGray;
			this.labelInfoNAC.ForeColor = Color.Gold;
			this.labelInfoNAC.Location = new Point(13, 75);
			this.labelInfoNAC.Name = "labelInfoNAC";
			this.labelInfoNAC.Size = new Size(125, 14);
			this.labelInfoNAC.TabIndex = 25;
			this.labelInfoNAC.Text = "NAC";
			this.panelInfoNAC.Controls.Add(this.textBoxInfoNAC);
			this.panelInfoNAC.Location = new Point(143, 72);
			this.panelInfoNAC.Name = "panelInfoNAC";
			this.panelInfoNAC.Size = new Size(150, 20);
			this.panelInfoNAC.TabIndex = 26;
			this.panelInfoNAC.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxInfoNAC.BackColor = Color.Black;
			this.textBoxInfoNAC.BorderStyle = BorderStyle.None;
			this.textBoxInfoNAC.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxInfoNAC.ForeColor = Color.Lime;
			this.textBoxInfoNAC.Location = new Point(2, 2);
			this.textBoxInfoNAC.MaxLength = 10;
			this.textBoxInfoNAC.Multiline = true;
			this.textBoxInfoNAC.Name = "textBoxInfoNAC";
			this.textBoxInfoNAC.ReadOnly = true;
			this.textBoxInfoNAC.Size = new Size(146, 15);
			this.textBoxInfoNAC.TabIndex = 2;
			this.textBoxInfoNAC.TextAlign = HorizontalAlignment.Center;
			this.panelTracker.Controls.Add(this.checkBoxTTFExclude);
			this.panelTracker.Controls.Add(this.checkBoxCCSEna);
			this.panelTracker.Controls.Add(this.checkBoxTTFEna);
			this.panelTracker.Controls.Add(this.labelTTFExclude);
			this.panelTracker.Controls.Add(this.labelCCSEna);
			this.panelTracker.Controls.Add(this.labelTTFEna);
			this.panelTracker.Controls.Add(this.labelTgtID);
			this.panelTracker.Controls.Add(this.labelSrcID);
			this.panelTracker.Controls.Add(this.panelFilt);
			this.panelTracker.Controls.Add(this.labelChanRSSI);
			this.panelTracker.Controls.Add(this.labelChanType);
			this.panelTracker.Controls.Add(this.checkBoxRem);
			this.panelTracker.Controls.Add(this.checkBoxAdd);
			this.panelTracker.Controls.Add(this.labelVCFreq);
			this.panelTracker.Controls.Add(this.labelCC);
			this.panelTracker.Controls.Add(this.panelTgtID);
			this.panelTracker.Controls.Add(this.panelSrcID);
			this.panelTracker.Controls.Add(this.panelChanRSSI);
			this.panelTracker.Controls.Add(this.panelVCFreq);
			this.panelTracker.Controls.Add(this.panelChanType);
			this.panelTracker.Controls.Add(this.panelCCFreq);
			this.panelTracker.Controls.Add(this.checkBoxHold);
			this.panelTracker.Controls.Add(this.checkBoxTTEna);
			this.panelTracker.Location = new Point(10, 336);
			this.panelTracker.Name = "panelTracker";
			this.panelTracker.Size = new Size(628, 254);
			this.panelTracker.TabIndex = 28;
			this.panelTracker.Visible = false;
			this.panelTracker.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.checkBoxTTFExclude.Appearance = Appearance.Button;
			this.checkBoxTTFExclude.Location = new Point(548, 62);
			this.checkBoxTTFExclude.Name = "checkBoxTTFExclude";
			this.checkBoxTTFExclude.Size = new Size(19, 19);
			this.checkBoxTTFExclude.TabIndex = 29;
			this.checkBoxTTFExclude.Text = "X";
			this.checkBoxTTFExclude.UseVisualStyleBackColor = true;
			this.checkBoxTTFExclude.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxTTFExclude.CheckStateChanged += new EventHandler(this.checkBoxNFExclude_CheckStateChanged);
			this.checkBoxCCSEna.Appearance = Appearance.Button;
			this.checkBoxCCSEna.Location = new Point(146, 160);
			this.checkBoxCCSEna.Name = "checkBoxCCSEna";
			this.checkBoxCCSEna.Size = new Size(19, 19);
			this.checkBoxCCSEna.TabIndex = 30;
			this.checkBoxCCSEna.Text = "X";
			this.checkBoxCCSEna.UseVisualStyleBackColor = true;
			this.checkBoxCCSEna.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxCCSEna.CheckStateChanged += new EventHandler(this.checkBoxNFEna_CheckStateChanged);
			this.checkBoxTTFEna.Appearance = Appearance.Button;
			this.checkBoxTTFEna.Location = new Point(548, 85);
			this.checkBoxTTFEna.Name = "checkBoxTTFEna";
			this.checkBoxTTFEna.Size = new Size(19, 19);
			this.checkBoxTTFEna.TabIndex = 30;
			this.checkBoxTTFEna.Text = "X";
			this.checkBoxTTFEna.UseVisualStyleBackColor = true;
			this.checkBoxTTFEna.Paint += new PaintEventHandler(this.checkBox_Paint);
			this.checkBoxTTFEna.CheckStateChanged += new EventHandler(this.checkBoxNFEna_CheckStateChanged);
			this.labelTTFExclude.BackColor = Color.DimGray;
			this.labelTTFExclude.ForeColor = Color.Gold;
			this.labelTTFExclude.Location = new Point(483, 65);
			this.labelTTFExclude.Name = "labelTTFExclude";
			this.labelTTFExclude.Size = new Size(60, 13);
			this.labelTTFExclude.TabIndex = 27;
			this.labelTTFExclude.Text = "EXCLUDE";
			this.labelCCSEna.BackColor = Color.DimGray;
			this.labelCCSEna.ForeColor = Color.Gold;
			this.labelCCSEna.Location = new Point(13, 163);
			this.labelCCSEna.Name = "labelCCSEna";
			this.labelCCSEna.Size = new Size(127, 13);
			this.labelCCSEna.TabIndex = 28;
			this.labelCCSEna.Text = "CC SCAN ENABLE";
			this.labelTTFEna.BackColor = Color.DimGray;
			this.labelTTFEna.ForeColor = Color.Gold;
			this.labelTTFEna.Location = new Point(483, 88);
			this.labelTTFEna.Name = "labelTTFEna";
			this.labelTTFEna.Size = new Size(60, 13);
			this.labelTTFEna.TabIndex = 28;
			this.labelTTFEna.Text = "ENABLE";
			this.labelTgtID.BackColor = Color.DimGray;
			this.labelTgtID.ForeColor = Color.Gold;
			this.labelTgtID.Location = new Point(13, 110);
			this.labelTgtID.Name = "labelTgtID";
			this.labelTgtID.Size = new Size(152, 13);
			this.labelTgtID.TabIndex = 23;
			this.labelTgtID.Text = "TARGET ID";
			this.labelSrcID.BackColor = Color.DimGray;
			this.labelSrcID.ForeColor = Color.Gold;
			this.labelSrcID.Location = new Point(13, 91);
			this.labelSrcID.Name = "labelSrcID";
			this.labelSrcID.Size = new Size(152, 13);
			this.labelSrcID.TabIndex = 23;
			this.labelSrcID.Text = "SOURCE ID";
			this.panelFilt.Controls.Add(this.listViewFilt4);
			this.panelFilt.Controls.Add(this.listViewFilt3);
			this.panelFilt.Controls.Add(this.listViewFilt2);
			this.panelFilt.Controls.Add(this.listViewFilt1);
			this.panelFilt.Location = new Point(275, 7);
			this.panelFilt.Name = "panelFilt";
			this.panelFilt.Size = new Size(204, 229);
			this.panelFilt.TabIndex = 0;
			this.panelFilt.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.listViewFilt4.BackColor = Color.Black;
			this.listViewFilt4.BorderStyle = BorderStyle.None;
			this.listViewFilt4.Columns.AddRange(new ColumnHeader[]
			{
				this.columnHeader3
			});
			this.listViewFilt4.ForeColor = Color.Lime;
			this.listViewFilt4.FullRowSelect = true;
			this.listViewFilt4.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewFilt4.Items.AddRange(new ListViewItem[]
			{
				listViewItem,
				listViewItem2,
				listViewItem3,
				listViewItem4,
				listViewItem5,
				listViewItem6,
				listViewItem7,
				listViewItem8,
				listViewItem9,
				listViewItem10,
				listViewItem11,
				listViewItem12,
				listViewItem13,
				listViewItem14,
				listViewItem15,
				listViewItem16
			});
			this.listViewFilt4.LabelEdit = true;
			this.listViewFilt4.Location = new Point(152, 2);
			this.listViewFilt4.Name = "listViewFilt4";
			this.listViewFilt4.Scrollable = false;
			this.listViewFilt4.Size = new Size(50, 225);
			this.listViewFilt4.TabIndex = 0;
			this.listViewFilt4.UseCompatibleStateImageBehavior = false;
			this.listViewFilt4.View = View.Details;
			this.listViewFilt4.AfterLabelEdit += new LabelEditEventHandler(this.listViewFilt_AfterLabelEdit);
			this.listViewFilt4.SelectedIndexChanged += new EventHandler(this.listViewFilt_SelectedIndexChanged);
			this.columnHeader3.Width = 50;
			this.listViewFilt3.BackColor = Color.Black;
			this.listViewFilt3.BorderStyle = BorderStyle.None;
			this.listViewFilt3.Columns.AddRange(new ColumnHeader[]
			{
				this.columnHeader2
			});
			this.listViewFilt3.ForeColor = Color.Lime;
			this.listViewFilt3.FullRowSelect = true;
			this.listViewFilt3.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewFilt3.Items.AddRange(new ListViewItem[]
			{
				listViewItem17,
				listViewItem18,
				listViewItem19,
				listViewItem20,
				listViewItem21,
				listViewItem22,
				listViewItem23,
				listViewItem24,
				listViewItem25,
				listViewItem26,
				listViewItem27,
				listViewItem28,
				listViewItem29,
				listViewItem30,
				listViewItem31,
				listViewItem32
			});
			this.listViewFilt3.LabelEdit = true;
			this.listViewFilt3.Location = new Point(102, 2);
			this.listViewFilt3.Name = "listViewFilt3";
			this.listViewFilt3.Scrollable = false;
			this.listViewFilt3.Size = new Size(50, 225);
			this.listViewFilt3.TabIndex = 0;
			this.listViewFilt3.UseCompatibleStateImageBehavior = false;
			this.listViewFilt3.View = View.Details;
			this.listViewFilt3.AfterLabelEdit += new LabelEditEventHandler(this.listViewFilt_AfterLabelEdit);
			this.listViewFilt3.SelectedIndexChanged += new EventHandler(this.listViewFilt_SelectedIndexChanged);
			this.columnHeader2.Width = 50;
			this.listViewFilt2.BackColor = Color.Black;
			this.listViewFilt2.BorderStyle = BorderStyle.None;
			this.listViewFilt2.Columns.AddRange(new ColumnHeader[]
			{
				this.columnHeader1
			});
			this.listViewFilt2.ForeColor = Color.Lime;
			this.listViewFilt2.FullRowSelect = true;
			this.listViewFilt2.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewFilt2.Items.AddRange(new ListViewItem[]
			{
				listViewItem33,
				listViewItem34,
				listViewItem35,
				listViewItem36,
				listViewItem37,
				listViewItem38,
				listViewItem39,
				listViewItem40,
				listViewItem41,
				listViewItem42,
				listViewItem43,
				listViewItem44,
				listViewItem45,
				listViewItem46,
				listViewItem47,
				listViewItem48
			});
			this.listViewFilt2.LabelEdit = true;
			this.listViewFilt2.Location = new Point(52, 2);
			this.listViewFilt2.Name = "listViewFilt2";
			this.listViewFilt2.Scrollable = false;
			this.listViewFilt2.Size = new Size(50, 225);
			this.listViewFilt2.TabIndex = 0;
			this.listViewFilt2.UseCompatibleStateImageBehavior = false;
			this.listViewFilt2.View = View.Details;
			this.listViewFilt2.AfterLabelEdit += new LabelEditEventHandler(this.listViewFilt_AfterLabelEdit);
			this.listViewFilt2.SelectedIndexChanged += new EventHandler(this.listViewFilt_SelectedIndexChanged);
			this.columnHeader1.Width = 50;
			this.listViewFilt1.BackColor = Color.Black;
			this.listViewFilt1.BorderStyle = BorderStyle.None;
			this.listViewFilt1.Columns.AddRange(new ColumnHeader[]
			{
				this.columnHeaderIDs
			});
			this.listViewFilt1.ForeColor = Color.Lime;
			this.listViewFilt1.FullRowSelect = true;
			this.listViewFilt1.HeaderStyle = ColumnHeaderStyle.None;
			listViewItem49.Tag = "";
			this.listViewFilt1.Items.AddRange(new ListViewItem[]
			{
				listViewItem49,
				listViewItem50,
				listViewItem51,
				listViewItem52,
				listViewItem53,
				listViewItem54,
				listViewItem55,
				listViewItem56,
				listViewItem57,
				listViewItem58,
				listViewItem59,
				listViewItem60,
				listViewItem61,
				listViewItem62,
				listViewItem63,
				listViewItem64
			});
			this.listViewFilt1.LabelEdit = true;
			this.listViewFilt1.Location = new Point(2, 2);
			this.listViewFilt1.Name = "listViewFilt1";
			this.listViewFilt1.Scrollable = false;
			this.listViewFilt1.Size = new Size(50, 225);
			this.listViewFilt1.TabIndex = 0;
			this.listViewFilt1.UseCompatibleStateImageBehavior = false;
			this.listViewFilt1.View = View.Details;
			this.listViewFilt1.AfterLabelEdit += new LabelEditEventHandler(this.listViewFilt_AfterLabelEdit);
			this.listViewFilt1.SelectedIndexChanged += new EventHandler(this.listViewFilt_SelectedIndexChanged);
			this.columnHeaderIDs.Width = 50;
			this.labelChanRSSI.BackColor = Color.DimGray;
			this.labelChanRSSI.ForeColor = Color.Gold;
			this.labelChanRSSI.Location = new Point(13, 72);
			this.labelChanRSSI.Name = "labelChanRSSI";
			this.labelChanRSSI.Size = new Size(152, 13);
			this.labelChanRSSI.TabIndex = 23;
			this.labelChanRSSI.Text = "CHANNEL RSSI";
			this.labelChanType.BackColor = Color.DimGray;
			this.labelChanType.ForeColor = Color.Gold;
			this.labelChanType.Location = new Point(13, 53);
			this.labelChanType.Name = "labelChanType";
			this.labelChanType.Size = new Size(152, 13);
			this.labelChanType.TabIndex = 23;
			this.labelChanType.Text = "CHANNEL TYPE";
			this.labelVCFreq.BackColor = Color.DimGray;
			this.labelVCFreq.ForeColor = Color.Gold;
			this.labelVCFreq.Location = new Point(13, 34);
			this.labelVCFreq.Name = "labelVCFreq";
			this.labelVCFreq.Size = new Size(152, 13);
			this.labelVCFreq.TabIndex = 23;
			this.labelVCFreq.Text = "TRAFFIC FREQUENCY";
			this.labelCC.BackColor = Color.DimGray;
			this.labelCC.ForeColor = Color.Gold;
			this.labelCC.Location = new Point(13, 15);
			this.labelCC.Name = "labelCC";
			this.labelCC.Size = new Size(152, 13);
			this.labelCC.TabIndex = 23;
			this.labelCC.Text = "CONTROL FREQUENCY";
			this.panelTgtID.Controls.Add(this.textBoxTgtID);
			this.panelTgtID.Location = new Point(171, 107);
			this.panelTgtID.Name = "panelTgtID";
			this.panelTgtID.Size = new Size(100, 19);
			this.panelTgtID.TabIndex = 26;
			this.panelTgtID.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxTgtID.BackColor = Color.Black;
			this.textBoxTgtID.BorderStyle = BorderStyle.None;
			this.textBoxTgtID.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxTgtID.ForeColor = Color.Lime;
			this.textBoxTgtID.Location = new Point(2, 2);
			this.textBoxTgtID.MaxLength = 10;
			this.textBoxTgtID.Multiline = true;
			this.textBoxTgtID.Name = "textBoxTgtID";
			this.textBoxTgtID.ReadOnly = true;
			this.textBoxTgtID.Size = new Size(96, 15);
			this.textBoxTgtID.TabIndex = 2;
			this.textBoxTgtID.TextAlign = HorizontalAlignment.Center;
			this.panelSrcID.Controls.Add(this.textBoxSrcID);
			this.panelSrcID.Location = new Point(171, 88);
			this.panelSrcID.Name = "panelSrcID";
			this.panelSrcID.Size = new Size(100, 19);
			this.panelSrcID.TabIndex = 26;
			this.panelSrcID.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxSrcID.BackColor = Color.Black;
			this.textBoxSrcID.BorderStyle = BorderStyle.None;
			this.textBoxSrcID.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxSrcID.ForeColor = Color.Lime;
			this.textBoxSrcID.Location = new Point(2, 2);
			this.textBoxSrcID.MaxLength = 10;
			this.textBoxSrcID.Multiline = true;
			this.textBoxSrcID.Name = "textBoxSrcID";
			this.textBoxSrcID.ReadOnly = true;
			this.textBoxSrcID.Size = new Size(96, 15);
			this.textBoxSrcID.TabIndex = 2;
			this.textBoxSrcID.TextAlign = HorizontalAlignment.Center;
			this.panelChanRSSI.Controls.Add(this.textBoxChanRSSI);
			this.panelChanRSSI.Location = new Point(171, 69);
			this.panelChanRSSI.Name = "panelChanRSSI";
			this.panelChanRSSI.Size = new Size(100, 19);
			this.panelChanRSSI.TabIndex = 26;
			this.panelChanRSSI.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxChanRSSI.BackColor = Color.Black;
			this.textBoxChanRSSI.BorderStyle = BorderStyle.None;
			this.textBoxChanRSSI.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxChanRSSI.ForeColor = Color.Lime;
			this.textBoxChanRSSI.Location = new Point(2, 2);
			this.textBoxChanRSSI.MaxLength = 10;
			this.textBoxChanRSSI.Multiline = true;
			this.textBoxChanRSSI.Name = "textBoxChanRSSI";
			this.textBoxChanRSSI.ReadOnly = true;
			this.textBoxChanRSSI.Size = new Size(96, 15);
			this.textBoxChanRSSI.TabIndex = 2;
			this.textBoxChanRSSI.TextAlign = HorizontalAlignment.Center;
			this.panelVCFreq.Controls.Add(this.textBoxVCFreq);
			this.panelVCFreq.Location = new Point(171, 31);
			this.panelVCFreq.Name = "panelVCFreq";
			this.panelVCFreq.Size = new Size(100, 19);
			this.panelVCFreq.TabIndex = 26;
			this.panelVCFreq.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxVCFreq.BackColor = Color.Black;
			this.textBoxVCFreq.BorderStyle = BorderStyle.None;
			this.textBoxVCFreq.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxVCFreq.ForeColor = Color.Lime;
			this.textBoxVCFreq.Location = new Point(2, 2);
			this.textBoxVCFreq.MaxLength = 10;
			this.textBoxVCFreq.Multiline = true;
			this.textBoxVCFreq.Name = "textBoxVCFreq";
			this.textBoxVCFreq.ReadOnly = true;
			this.textBoxVCFreq.Size = new Size(96, 15);
			this.textBoxVCFreq.TabIndex = 2;
			this.textBoxVCFreq.TextAlign = HorizontalAlignment.Center;
			this.panelChanType.Controls.Add(this.textBoxChanType);
			this.panelChanType.Location = new Point(171, 50);
			this.panelChanType.Name = "panelChanType";
			this.panelChanType.Size = new Size(100, 19);
			this.panelChanType.TabIndex = 26;
			this.panelChanType.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.textBoxChanType.BackColor = Color.Black;
			this.textBoxChanType.BorderStyle = BorderStyle.None;
			this.textBoxChanType.Font = new Font("Microsoft Sans Serif", 8f, FontStyle.Bold, GraphicsUnit.Point, 204);
			this.textBoxChanType.ForeColor = Color.Lime;
			this.textBoxChanType.Location = new Point(2, 2);
			this.textBoxChanType.MaxLength = 10;
			this.textBoxChanType.Multiline = true;
			this.textBoxChanType.Name = "textBoxChanType";
			this.textBoxChanType.ReadOnly = true;
			this.textBoxChanType.Size = new Size(96, 15);
			this.textBoxChanType.TabIndex = 2;
			this.textBoxChanType.TextAlign = HorizontalAlignment.Center;
			this.panelPeers.Controls.Add(this.labelPeersLast);
			this.panelPeers.Controls.Add(this.labelPeersFreq);
			this.panelPeers.Controls.Add(this.labelPeersControl);
			this.panelPeers.Controls.Add(this.labelPeersSite);
			this.panelPeers.Controls.Add(this.labelPeersSystem);
			this.panelPeers.Controls.Add(this.listViewPeers);
			this.panelPeers.Location = new Point(659, 130);
			this.panelPeers.Name = "panelPeers";
			this.panelPeers.Size = new Size(425, 51);
			this.panelPeers.TabIndex = 0;
			this.panelPeers.Visible = false;
			this.panelPeers.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.labelPeersLast.BackColor = Color.Black;
			this.labelPeersLast.ForeColor = Color.Gold;
			this.labelPeersLast.Location = new Point(265, 2);
			this.labelPeersLast.Name = "labelPeersLast";
			this.labelPeersLast.Size = new Size(61, 15);
			this.labelPeersLast.TabIndex = 1;
			this.labelPeersLast.Text = "Last";
			this.labelPeersFreq.BackColor = Color.Black;
			this.labelPeersFreq.ForeColor = Color.Gold;
			this.labelPeersFreq.Location = new Point(199, 2);
			this.labelPeersFreq.Name = "labelPeersFreq";
			this.labelPeersFreq.Size = new Size(61, 15);
			this.labelPeersFreq.TabIndex = 1;
			this.labelPeersFreq.Text = "Frequency";
			this.labelPeersControl.BackColor = Color.Black;
			this.labelPeersControl.ForeColor = Color.Gold;
			this.labelPeersControl.Location = new Point(133, 2);
			this.labelPeersControl.Name = "labelPeersControl";
			this.labelPeersControl.Size = new Size(61, 15);
			this.labelPeersControl.TabIndex = 1;
			this.labelPeersControl.Text = "Control";
			this.labelPeersSite.BackColor = Color.Black;
			this.labelPeersSite.ForeColor = Color.Gold;
			this.labelPeersSite.Location = new Point(67, 2);
			this.labelPeersSite.Name = "labelPeersSite";
			this.labelPeersSite.Size = new Size(61, 15);
			this.labelPeersSite.TabIndex = 1;
			this.labelPeersSite.Text = "Site";
			this.labelPeersSystem.BackColor = Color.Black;
			this.labelPeersSystem.ForeColor = Color.Gold;
			this.labelPeersSystem.Location = new Point(2, 2);
			this.labelPeersSystem.Name = "labelPeersSystem";
			this.labelPeersSystem.Size = new Size(61, 15);
			this.labelPeersSystem.TabIndex = 1;
			this.labelPeersSystem.Text = " System";
			this.listViewPeers.BackColor = Color.Black;
			this.listViewPeers.BorderStyle = BorderStyle.None;
			this.listViewPeers.Columns.AddRange(new ColumnHeader[]
			{
				this.system,
				this.site,
				this.control,
				this.frequency,
				this.last
			});
			this.listViewPeers.ForeColor = Color.Lime;
			this.listViewPeers.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewPeers.Location = new Point(2, 17);
			this.listViewPeers.Name = "listViewPeers";
			this.listViewPeers.Size = new Size(420, 31);
			this.listViewPeers.TabIndex = 0;
			this.listViewPeers.UseCompatibleStateImageBehavior = false;
			this.listViewPeers.View = View.Details;
			this.system.Width = 80;
			this.site.Width = 80;
			this.control.Width = 80;
			this.frequency.Width = 80;
			this.last.Width = 80;
			this.panelBandPlan.Controls.Add(this.labelBandPlanBase);
			this.panelBandPlan.Controls.Add(this.labelBandPlanLo);
			this.panelBandPlan.Controls.Add(this.labelBandPlanHi);
			this.panelBandPlan.Controls.Add(this.labelBandPlanSpacing);
			this.panelBandPlan.Controls.Add(this.labelBandPlanBW);
			this.panelBandPlan.Controls.Add(this.labelBandPlanTXO);
			this.panelBandPlan.Controls.Add(this.labelBandPlanID);
			this.panelBandPlan.Controls.Add(this.listViewBandPlan);
			this.panelBandPlan.Location = new Point(659, 187);
			this.panelBandPlan.Name = "panelBandPlan";
			this.panelBandPlan.Size = new Size(425, 38);
			this.panelBandPlan.TabIndex = 0;
			this.panelBandPlan.Visible = false;
			this.panelBandPlan.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.labelBandPlanBase.BackColor = Color.Black;
			this.labelBandPlanBase.ForeColor = Color.Gold;
			this.labelBandPlanBase.Location = new Point(34, 2);
			this.labelBandPlanBase.Name = "labelBandPlanBase";
			this.labelBandPlanBase.Size = new Size(26, 15);
			this.labelBandPlanBase.TabIndex = 1;
			this.labelBandPlanBase.Text = "Base";
			this.labelBandPlanLo.BackColor = Color.Black;
			this.labelBandPlanLo.ForeColor = Color.Gold;
			this.labelBandPlanLo.Location = new Point(67, 2);
			this.labelBandPlanLo.Name = "labelBandPlanLo";
			this.labelBandPlanLo.Size = new Size(26, 15);
			this.labelBandPlanLo.TabIndex = 1;
			this.labelBandPlanLo.Text = "Lo";
			this.labelBandPlanHi.BackColor = Color.Black;
			this.labelBandPlanHi.ForeColor = Color.Gold;
			this.labelBandPlanHi.Location = new Point(99, 2);
			this.labelBandPlanHi.Name = "labelBandPlanHi";
			this.labelBandPlanHi.Size = new Size(26, 15);
			this.labelBandPlanHi.TabIndex = 1;
			this.labelBandPlanHi.Text = "Hi";
			this.labelBandPlanSpacing.BackColor = Color.Black;
			this.labelBandPlanSpacing.ForeColor = Color.Gold;
			this.labelBandPlanSpacing.Location = new Point(131, 2);
			this.labelBandPlanSpacing.Name = "labelBandPlanSpacing";
			this.labelBandPlanSpacing.Size = new Size(26, 15);
			this.labelBandPlanSpacing.TabIndex = 1;
			this.labelBandPlanSpacing.Text = "Spacing";
			this.labelBandPlanBW.BackColor = Color.Black;
			this.labelBandPlanBW.ForeColor = Color.Gold;
			this.labelBandPlanBW.Location = new Point(164, 2);
			this.labelBandPlanBW.Name = "labelBandPlanBW";
			this.labelBandPlanBW.Size = new Size(26, 15);
			this.labelBandPlanBW.TabIndex = 1;
			this.labelBandPlanBW.Text = "Bandwidth";
			this.labelBandPlanTXO.BackColor = Color.Black;
			this.labelBandPlanTXO.ForeColor = Color.Gold;
			this.labelBandPlanTXO.Location = new Point(196, 2);
			this.labelBandPlanTXO.Name = "labelBandPlanTXO";
			this.labelBandPlanTXO.Size = new Size(26, 15);
			this.labelBandPlanTXO.TabIndex = 1;
			this.labelBandPlanTXO.Text = "TX offset";
			this.labelBandPlanID.BackColor = Color.Black;
			this.labelBandPlanID.ForeColor = Color.Gold;
			this.labelBandPlanID.Location = new Point(2, 2);
			this.labelBandPlanID.Name = "labelBandPlanID";
			this.labelBandPlanID.Size = new Size(26, 15);
			this.labelBandPlanID.TabIndex = 1;
			this.labelBandPlanID.Text = " ID";
			this.listViewBandPlan.BackColor = Color.Black;
			this.listViewBandPlan.BorderStyle = BorderStyle.None;
			this.listViewBandPlan.Columns.AddRange(new ColumnHeader[]
			{
				this.id,
				this.bf,
				this.lo,
				this.hi,
				this.cs,
				this.bw,
				this.txo
			});
			this.listViewBandPlan.ForeColor = Color.Lime;
			this.listViewBandPlan.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewBandPlan.Location = new Point(2, 17);
			this.listViewBandPlan.Name = "listViewBandPlan";
			this.listViewBandPlan.Size = new Size(420, 19);
			this.listViewBandPlan.Sorting = SortOrder.Ascending;
			this.listViewBandPlan.TabIndex = 0;
			this.listViewBandPlan.UseCompatibleStateImageBehavior = false;
			this.listViewBandPlan.View = View.Details;
			this.id.Width = 32;
			this.bf.Width = 80;
			this.lo.Width = 80;
			this.hi.Width = 80;
			this.cs.Width = 80;
			this.bw.Width = 80;
			this.txo.Width = 80;
			this.panelChans.Controls.Add(this.labelChansFreq);
			this.panelChans.Controls.Add(this.labelChansLabel);
			this.panelChans.Controls.Add(this.labelChansHits);
			this.panelChans.Controls.Add(this.labelChansTarget);
			this.panelChans.Controls.Add(this.labelChansT);
			this.panelChans.Controls.Add(this.labelChansSource);
			this.panelChans.Controls.Add(this.labelChansLast);
			this.panelChans.Controls.Add(this.labelChansLCN);
			this.panelChans.Controls.Add(this.listViewChans);
			this.panelChans.Location = new Point(659, 231);
			this.panelChans.Name = "panelChans";
			this.panelChans.Size = new Size(573, 40);
			this.panelChans.TabIndex = 0;
			this.panelChans.Visible = false;
			this.panelChans.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.labelChansFreq.BackColor = Color.Black;
			this.labelChansFreq.ForeColor = Color.Gold;
			this.labelChansFreq.Location = new Point(34, 2);
			this.labelChansFreq.Name = "labelChansFreq";
			this.labelChansFreq.Size = new Size(30, 15);
			this.labelChansFreq.TabIndex = 1;
			this.labelChansFreq.Text = "Frequency";
			this.labelChansLabel.BackColor = Color.Black;
			this.labelChansLabel.ForeColor = Color.Gold;
			this.labelChansLabel.Location = new Point(67, 2);
			this.labelChansLabel.Name = "labelChansLabel";
			this.labelChansLabel.Size = new Size(30, 15);
			this.labelChansLabel.TabIndex = 1;
			this.labelChansLabel.Text = "Label";
			this.labelChansHits.BackColor = Color.Black;
			this.labelChansHits.ForeColor = Color.Gold;
			this.labelChansHits.Location = new Point(99, 2);
			this.labelChansHits.Name = "labelChansHits";
			this.labelChansHits.Size = new Size(30, 15);
			this.labelChansHits.TabIndex = 1;
			this.labelChansHits.Text = "Hits";
			this.labelChansTarget.BackColor = Color.Black;
			this.labelChansTarget.ForeColor = Color.Gold;
			this.labelChansTarget.Location = new Point(131, 2);
			this.labelChansTarget.Name = "labelChansTarget";
			this.labelChansTarget.Size = new Size(30, 15);
			this.labelChansTarget.TabIndex = 1;
			this.labelChansTarget.Text = "Target";
			this.labelChansT.BackColor = Color.Black;
			this.labelChansT.ForeColor = Color.Gold;
			this.labelChansT.Location = new Point(164, 2);
			this.labelChansT.Name = "labelChansT";
			this.labelChansT.Size = new Size(30, 15);
			this.labelChansT.TabIndex = 1;
			this.labelChansT.Text = "T";
			this.labelChansSource.BackColor = Color.Black;
			this.labelChansSource.ForeColor = Color.Gold;
			this.labelChansSource.Location = new Point(196, 2);
			this.labelChansSource.Name = "labelChansSource";
			this.labelChansSource.Size = new Size(30, 15);
			this.labelChansSource.TabIndex = 1;
			this.labelChansSource.Text = "Source";
			this.labelChansLast.BackColor = Color.Black;
			this.labelChansLast.ForeColor = Color.Gold;
			this.labelChansLast.Location = new Point(230, 2);
			this.labelChansLast.Name = "labelChansLast";
			this.labelChansLast.Size = new Size(30, 15);
			this.labelChansLast.TabIndex = 1;
			this.labelChansLast.Text = "Last";
			this.labelChansLCN.BackColor = Color.Black;
			this.labelChansLCN.ForeColor = Color.Gold;
			this.labelChansLCN.Location = new Point(2, 2);
			this.labelChansLCN.Name = "labelChansLCN";
			this.labelChansLCN.Size = new Size(30, 15);
			this.labelChansLCN.TabIndex = 1;
			this.labelChansLCN.Text = " LCN             Frequency       Label                  Hits                 Target                T        Source                Last";
			this.listViewChans.BackColor = Color.Black;
			this.listViewChans.BorderStyle = BorderStyle.None;
			this.listViewChans.Columns.AddRange(new ColumnHeader[]
			{
				this.chan_lcn,
				this.chan_freq,
				this.chan_label,
				this.chan_hits,
				this.chan_tgt,
				this.chan_t,
				this.chan_src,
				this.chan_last
			});
			this.listViewChans.ForeColor = Color.Lime;
			this.listViewChans.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewChans.Location = new Point(2, 17);
			this.listViewChans.Name = "listViewChans";
			this.listViewChans.Size = new Size(568, 22);
			this.listViewChans.TabIndex = 0;
			this.listViewChans.UseCompatibleStateImageBehavior = false;
			this.listViewChans.View = View.Details;
			this.chan_freq.Width = 80;
			this.chan_label.Width = 100;
			this.chan_hits.Width = 70;
			this.chan_tgt.Width = 80;
			this.chan_t.Width = 30;
			this.chan_src.Width = 80;
			this.panelCallHist.Controls.Add(this.labelCallHistSrcID);
			this.panelCallHist.Controls.Add(this.labelCallHistAction);
			this.panelCallHist.Controls.Add(this.labelCallHistResult);
			this.panelCallHist.Controls.Add(this.labelCallHistTgtID);
			this.panelCallHist.Controls.Add(this.labelCallHistService);
			this.panelCallHist.Controls.Add(this.labelCallHistStamp);
			this.panelCallHist.Controls.Add(this.listViewCallHist);
			this.panelCallHist.Location = new Point(659, 277);
			this.panelCallHist.Name = "panelCallHist";
			this.panelCallHist.Size = new Size(573, 40);
			this.panelCallHist.TabIndex = 0;
			this.panelCallHist.Visible = false;
			this.panelCallHist.Paint += new PaintEventHandler(this.panelMain_Paint);
			this.labelCallHistSrcID.BackColor = Color.Black;
			this.labelCallHistSrcID.ForeColor = Color.Gold;
			this.labelCallHistSrcID.Location = new Point(50, 2);
			this.labelCallHistSrcID.Name = "labelCallHistSrcID";
			this.labelCallHistSrcID.Size = new Size(43, 15);
			this.labelCallHistSrcID.TabIndex = 1;
			this.labelCallHistSrcID.Text = "Source ID";
			this.labelCallHistAction.BackColor = Color.Black;
			this.labelCallHistAction.ForeColor = Color.Gold;
			this.labelCallHistAction.Location = new Point(99, 2);
			this.labelCallHistAction.Name = "labelCallHistAction";
			this.labelCallHistAction.Size = new Size(43, 15);
			this.labelCallHistAction.TabIndex = 1;
			this.labelCallHistAction.Text = "Action";
			this.labelCallHistResult.BackColor = Color.Black;
			this.labelCallHistResult.ForeColor = Color.Gold;
			this.labelCallHistResult.Location = new Point(148, 2);
			this.labelCallHistResult.Name = "labelCallHistResult";
			this.labelCallHistResult.Size = new Size(43, 15);
			this.labelCallHistResult.TabIndex = 1;
			this.labelCallHistResult.Text = "Result";
			this.labelCallHistTgtID.BackColor = Color.Black;
			this.labelCallHistTgtID.ForeColor = Color.Gold;
			this.labelCallHistTgtID.Location = new Point(196, 2);
			this.labelCallHistTgtID.Name = "labelCallHistTgtID";
			this.labelCallHistTgtID.Size = new Size(43, 15);
			this.labelCallHistTgtID.TabIndex = 1;
			this.labelCallHistTgtID.Text = "Target ID";
			this.labelCallHistService.BackColor = Color.Black;
			this.labelCallHistService.ForeColor = Color.Gold;
			this.labelCallHistService.Location = new Point(244, 2);
			this.labelCallHistService.Name = "labelCallHistService";
			this.labelCallHistService.Size = new Size(43, 15);
			this.labelCallHistService.TabIndex = 1;
			this.labelCallHistService.Text = "Service";
			this.labelCallHistStamp.BackColor = Color.Black;
			this.labelCallHistStamp.ForeColor = Color.Gold;
			this.labelCallHistStamp.Location = new Point(2, 2);
			this.labelCallHistStamp.Name = "labelCallHistStamp";
			this.labelCallHistStamp.Size = new Size(43, 15);
			this.labelCallHistStamp.TabIndex = 1;
			this.labelCallHistStamp.Text = " Stamp          Source ID    Action          Result          Target ID     Service";
			this.listViewCallHist.BackColor = Color.Black;
			this.listViewCallHist.BorderStyle = BorderStyle.None;
			this.listViewCallHist.Columns.AddRange(new ColumnHeader[]
			{
				this.ch_stamp,
				this.ch_srcid,
				this.ch_action,
				this.ch_res,
				this.ch_tgtid,
				this.ch_svc
			});
			this.listViewCallHist.ForeColor = Color.Lime;
			this.listViewCallHist.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewCallHist.Location = new Point(2, 17);
			this.listViewCallHist.Name = "listViewCallHist";
			this.listViewCallHist.Size = new Size(568, 22);
			this.listViewCallHist.TabIndex = 0;
			this.listViewCallHist.UseCompatibleStateImageBehavior = false;
			this.listViewCallHist.View = View.Details;
			this.ch_stamp.Width = 80;
			this.ch_srcid.Width = 80;
			this.ch_action.Width = 80;
			this.ch_res.Width = 80;
			this.ch_tgtid.Width = 80;
			this.ch_svc.Width = 80;
			this.panelCHCtrl.Controls.Add(this.checkBoxCHScroll);
			this.panelCHCtrl.Controls.Add(this.checkBoxCHCntl);
			this.panelCHCtrl.Controls.Add(this.checkBoxCHData);
			this.panelCHCtrl.Controls.Add(this.checkBoxCHVoice);
			this.panelCHCtrl.Controls.Add(this.checkBoxLog);
			this.panelCHCtrl.Controls.Add(this.checkBoxCHClear);
			this.panelCHCtrl.Location = new Point(659, 323);
			this.panelCHCtrl.Name = "panelCHCtrl";
			this.panelCHCtrl.Size = new Size(570, 26);
			this.panelCHCtrl.TabIndex = 29;
			this.panelCHCtrl.Visible = false;
			this.saveFileDialogLog.Filter = "log files (*.csv)|*.csv|All files (*.*)|*.*";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(1120, 602);
			base.Controls.Add(this.panelCHCtrl);
			base.Controls.Add(this.panelBandPlan);
			base.Controls.Add(this.panelPeers);
			base.Controls.Add(this.panelCallHist);
			base.Controls.Add(this.panelChans);
			base.Controls.Add(this.panelTracker);
			base.Controls.Add(this.panelTrunkInfo);
			base.Controls.Add(this.panelMain);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormTrunk";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "P25 TRUNKING";
			base.Paint += new PaintEventHandler(this.FormTrunk_Paint);
			this.panelMain.ResumeLayout(false);
			this.panelMain.PerformLayout();
			this.panelCCFreq.ResumeLayout(false);
			this.panelCCFreq.PerformLayout();
			this.panelTrunkInfo.ResumeLayout(false);
			this.panelInfoSite.ResumeLayout(false);
			this.panelInfoSite.PerformLayout();
			this.panelInfoSystem.ResumeLayout(false);
			this.panelInfoSystem.PerformLayout();
			this.panelInfoChans.ResumeLayout(false);
			this.panelInfoChans.PerformLayout();
			this.panelInfoControl.ResumeLayout(false);
			this.panelInfoControl.PerformLayout();
			this.panelInfoNAC.ResumeLayout(false);
			this.panelInfoNAC.PerformLayout();
			this.panelTracker.ResumeLayout(false);
			this.panelFilt.ResumeLayout(false);
			this.panelTgtID.ResumeLayout(false);
			this.panelTgtID.PerformLayout();
			this.panelSrcID.ResumeLayout(false);
			this.panelSrcID.PerformLayout();
			this.panelChanRSSI.ResumeLayout(false);
			this.panelChanRSSI.PerformLayout();
			this.panelVCFreq.ResumeLayout(false);
			this.panelVCFreq.PerformLayout();
			this.panelChanType.ResumeLayout(false);
			this.panelChanType.PerformLayout();
			this.panelPeers.ResumeLayout(false);
			this.panelBandPlan.ResumeLayout(false);
			this.panelChans.ResumeLayout(false);
			this.panelCallHist.ResumeLayout(false);
			this.panelCHCtrl.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
