namespace NetWatch
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            lblStatus = new Label();
            lblIp = new Label();
            lblGateway = new Label();
            lblPing = new Label();
            btnRefresh = new Button();
            lblSsid = new Label();
            lblBssid = new Label();
            lblSignal = new Label();
            lblChannel = new Label();
            lblRssi = new Label();
            lblRadioType = new Label();
            lblReceiveRate = new Label();
            lblBand = new Label();
            lblTransmitRate = new Label();
            lblTitle = new Label();
            lblLive = new Label();
            pnlNetwork = new Panel();
            pnlPingChart = new Panel();
            lblNetworkTitle = new Label();
            pnlWifi = new Panel();
            lblWifiTitle = new Label();
            pnlFooter = new Panel();
            lblSubtitle = new Label();
            pnlHeader = new Panel();
            label1 = new Label();
            pnlNetwork.SuspendLayout();
            pnlWifi.SuspendLayout();
            pnlFooter.SuspendLayout();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(16, 80);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(136, 25);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Network Status:";
            // 
            // lblIp
            // 
            lblIp.AutoSize = true;
            lblIp.Location = new Point(16, 128);
            lblIp.Name = "lblIp";
            lblIp.Size = new Size(76, 25);
            lblIp.TabIndex = 0;
            lblIp.Text = "Local IP:";
            // 
            // lblGateway
            // 
            lblGateway.AutoSize = true;
            lblGateway.Location = new Point(16, 176);
            lblGateway.Name = "lblGateway";
            lblGateway.Size = new Size(83, 25);
            lblGateway.TabIndex = 0;
            lblGateway.Text = "Gateway:";
            // 
            // lblPing
            // 
            lblPing.AutoSize = true;
            lblPing.Location = new Point(16, 224);
            lblPing.Name = "lblPing";
            lblPing.Size = new Size(123, 25);
            lblPing.TabIndex = 0;
            lblPing.Text = "Gateway Ping:";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(831, 17);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(179, 54);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "↻  Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lblSsid
            // 
            lblSsid.AutoSize = true;
            lblSsid.Location = new Point(30, 80);
            lblSsid.Name = "lblSsid";
            lblSsid.Size = new Size(57, 25);
            lblSsid.TabIndex = 2;
            lblSsid.Text = "Wi-Fi:";
            // 
            // lblBssid
            // 
            lblBssid.AutoSize = true;
            lblBssid.Location = new Point(30, 128);
            lblBssid.Name = "lblBssid";
            lblBssid.Size = new Size(64, 25);
            lblBssid.TabIndex = 2;
            lblBssid.Text = "BSSID:";
            // 
            // lblSignal
            // 
            lblSignal.AutoSize = true;
            lblSignal.Location = new Point(30, 176);
            lblSignal.Name = "lblSignal";
            lblSignal.Size = new Size(64, 25);
            lblSignal.TabIndex = 2;
            lblSignal.Text = "Signal:";
            // 
            // lblChannel
            // 
            lblChannel.AutoSize = true;
            lblChannel.Location = new Point(30, 464);
            lblChannel.Name = "lblChannel";
            lblChannel.Size = new Size(79, 25);
            lblChannel.TabIndex = 2;
            lblChannel.Text = "Channel:";
            // 
            // lblRssi
            // 
            lblRssi.AutoSize = true;
            lblRssi.Location = new Point(30, 224);
            lblRssi.Name = "lblRssi";
            lblRssi.Size = new Size(52, 25);
            lblRssi.TabIndex = 0;
            lblRssi.Text = "RSSI:";
            // 
            // lblRadioType
            // 
            lblRadioType.AutoSize = true;
            lblRadioType.Location = new Point(30, 320);
            lblRadioType.Name = "lblRadioType";
            lblRadioType.Size = new Size(104, 25);
            lblRadioType.TabIndex = 0;
            lblRadioType.Text = "Radio Type:";
            // 
            // lblReceiveRate
            // 
            lblReceiveRate.AutoSize = true;
            lblReceiveRate.Location = new Point(30, 368);
            lblReceiveRate.Name = "lblReceiveRate";
            lblReceiveRate.Size = new Size(74, 25);
            lblReceiveRate.TabIndex = 0;
            lblReceiveRate.Text = "Receive:";
            // 
            // lblBand
            // 
            lblBand.AutoSize = true;
            lblBand.Location = new Point(30, 272);
            lblBand.Name = "lblBand";
            lblBand.Size = new Size(56, 25);
            lblBand.TabIndex = 0;
            lblBand.Text = "Band:";
            // 
            // lblTransmitRate
            // 
            lblTransmitRate.AutoSize = true;
            lblTransmitRate.Location = new Point(30, 416);
            lblTransmitRate.Name = "lblTransmitRate";
            lblTransmitRate.Size = new Size(122, 25);
            lblTransmitRate.TabIndex = 0;
            lblTransmitRate.Text = "Transmit Rate:";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(16, 11);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(89, 25);
            lblTitle.TabIndex = 7;
            lblTitle.Text = "NetWatch";
            // 
            // lblLive
            // 
            lblLive.AutoSize = true;
            lblLive.Location = new Point(919, 34);
            lblLive.Name = "lblLive";
            lblLive.Size = new Size(61, 25);
            lblLive.TabIndex = 7;
            lblLive.Text = "● LIVE";
            // 
            // pnlNetwork
            // 
            pnlNetwork.Controls.Add(pnlPingChart);
            pnlNetwork.Controls.Add(lblNetworkTitle);
            pnlNetwork.Controls.Add(lblPing);
            pnlNetwork.Controls.Add(lblStatus);
            pnlNetwork.Controls.Add(lblIp);
            pnlNetwork.Controls.Add(lblGateway);
            pnlNetwork.Location = new Point(43, 142);
            pnlNetwork.Name = "pnlNetwork";
            pnlNetwork.Size = new Size(496, 544);
            pnlNetwork.TabIndex = 4;
            // 
            // pnlPingChart
            // 
            pnlPingChart.Location = new Point(16, 293);
            pnlPingChart.Name = "pnlPingChart";
            pnlPingChart.Size = new Size(465, 226);
            pnlPingChart.TabIndex = 8;
            // 
            // lblNetworkTitle
            // 
            lblNetworkTitle.AutoSize = true;
            lblNetworkTitle.Location = new Point(16, 20);
            lblNetworkTitle.Name = "lblNetworkTitle";
            lblNetworkTitle.Size = new Size(187, 25);
            lblNetworkTitle.TabIndex = 7;
            lblNetworkTitle.Text = "NETWORK OVERVIEW";
            // 
            // pnlWifi
            // 
            pnlWifi.Controls.Add(lblWifiTitle);
            pnlWifi.Controls.Add(lblSignal);
            pnlWifi.Controls.Add(lblSsid);
            pnlWifi.Controls.Add(lblBssid);
            pnlWifi.Controls.Add(lblChannel);
            pnlWifi.Controls.Add(lblTransmitRate);
            pnlWifi.Controls.Add(lblBand);
            pnlWifi.Controls.Add(lblRssi);
            pnlWifi.Controls.Add(lblRadioType);
            pnlWifi.Controls.Add(lblReceiveRate);
            pnlWifi.Location = new Point(557, 142);
            pnlWifi.Name = "pnlWifi";
            pnlWifi.Size = new Size(496, 544);
            pnlWifi.TabIndex = 5;
            // 
            // lblWifiTitle
            // 
            lblWifiTitle.AutoSize = true;
            lblWifiTitle.Location = new Point(30, 20);
            lblWifiTitle.Name = "lblWifiTitle";
            lblWifiTitle.Size = new Size(125, 25);
            lblWifiTitle.TabIndex = 7;
            lblWifiTitle.Text = "WI-FI DETAILS";
            // 
            // pnlFooter
            // 
            pnlFooter.Controls.Add(btnRefresh);
            pnlFooter.Controls.Add(lblSubtitle);
            pnlFooter.Location = new Point(43, 692);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(1010, 82);
            pnlFooter.TabIndex = 6;
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(16, 32);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(319, 25);
            lblSubtitle.TabIndex = 7;
            lblSubtitle.Text = "NetWatch • Local network monitoring  ";
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(label1);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblLive);
            pnlHeader.Location = new Point(43, 33);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1010, 94);
            pnlHeader.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 6F);
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(26, 70);
            label1.Name = "label1";
            label1.Size = new Size(136, 15);
            label1.TabIndex = 2;
            label1.Text = "Network Device Monitor";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1102, 811);
            Controls.Add(pnlHeader);
            Controls.Add(pnlWifi);
            Controls.Add(pnlNetwork);
            Controls.Add(pnlFooter);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "NetWatch — Network Device Monitor";
            Load += Form1_Load;
            pnlNetwork.ResumeLayout(false);
            pnlNetwork.PerformLayout();
            pnlWifi.ResumeLayout(false);
            pnlWifi.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblStatus;
        private Label lblIp;
        private Label lblGateway;
        private Label lblPing;
        private Button btnRefresh;
        private Label lblSsid;
        private Label lblBssid;
        private Label lblSignal;
        private Label lblChannel;
        private Label lblRssi;
        private Label lblRadioType;
        private Label lblReceiveRate;
        private Label lblBand;
        private Label lblTransmitRate;
        private Panel pnlNetwork;
        private Panel pnlWifi;
        private Panel pnlFooter;
        private Label lblLive;
        private Label lblNetworkTitle;
        private Label lblWifiTitle;
        private Label lblSubtitle;
        private Label lblTitle;
        private Panel pnlHeader;
        private Label label1;
        private Panel pnlPingChart;
    }
}
