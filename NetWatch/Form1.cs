namespace NetWatch
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ApplyTheme();

            this.Resize += Form1_Resize;
        }


        // =====================================================
        // FORM LOAD
        // =====================================================

        private async void Form1_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            ApplyRoundedCorners();

            // Form ilk açılırken butonda "Refreshing..." göstermesin.
            await RefreshAll(false);
        }


        // =====================================================
        // REFRESH BUTTON
        // =====================================================

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshAll(true);
        }


        // =====================================================
        // TÜM VERİLERİ YENİLE
        // =====================================================

        private async Task RefreshAll(bool showLoading)
        {
            btnRefresh.Enabled = false;

            if (showLoading)
            {
                btnRefresh.Text = "↻  Refreshing...";

                btnRefresh.BackColor =
                    Color.FromArgb(100, 116, 139);

                // Yazının gözle görülebilmesi için
                // çok kısa bir bekleme.
                await Task.Delay(250);
            }

            try
            {
                await GetNetworkInfo();
                await GetWifiInfo();
            }
            finally
            {
                btnRefresh.Text = "Refresh";

                btnRefresh.BackColor =
                    Color.FromArgb(37, 99, 235);

                btnRefresh.Enabled = true;
            }
        }


        // =====================================================
        // NETWORK BİLGİLERİ
        // =====================================================

        private async Task GetNetworkInfo()
        {
            lblStatus.Text = "Status: Offline";

            lblStatus.ForeColor =
                Color.FromArgb(220, 38, 38);

            lblIp.Text =
                "Local IP: -";

            lblGateway.Text =
                "Gateway: -";

            lblPing.Text =
                "Gateway Ping: -";


            try
            {
                foreach (NetworkInterface network
                         in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (network.OperationalStatus !=
                        OperationalStatus.Up)
                    {
                        continue;
                    }


                    if (network.NetworkInterfaceType ==
                        NetworkInterfaceType.Loopback)
                    {
                        continue;
                    }


                    IPInterfaceProperties properties =
                        network.GetIPProperties();


                    string localIp = "";


                    foreach (UnicastIPAddressInformation ip
                             in properties.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily ==
                            AddressFamily.InterNetwork)
                        {
                            localIp =
                                ip.Address.ToString();

                            break;
                        }
                    }


                    if (localIp == "")
                    {
                        continue;
                    }


                    foreach (GatewayIPAddressInformation gateway
                             in properties.GatewayAddresses)
                    {
                        if (gateway.Address.AddressFamily ==
                            AddressFamily.InterNetwork)
                        {
                            lblStatus.Text =
                                "Status: Online";

                            lblStatus.ForeColor =
                                Color.FromArgb(22, 163, 74);


                            lblIp.Text =
                                "Local IP: " + localIp;


                            lblGateway.Text =
                                "Gateway: " +
                                gateway.Address;


                            await PingGateway(
                                gateway.Address.ToString());


                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text =
                    "Status: Error";

                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                Debug.WriteLine(
                    ex.Message);
            }
        }


        // =====================================================
        // GATEWAY PING
        // =====================================================

        private async Task PingGateway(string gatewayIp)
        {
            try
            {
                using Ping ping =
                    new Ping();


                PingReply reply =
                    await ping.SendPingAsync(
                        gatewayIp,
                        1000);


                if (reply.Status ==
                    IPStatus.Success)
                {
                    lblPing.Text =
                        "Gateway Ping: " +
                        reply.RoundtripTime +
                        " ms";
                }
                else
                {
                    lblPing.Text =
                        "Gateway Ping: " +
                        reply.Status;
                }
            }
            catch (Exception ex)
            {
                lblPing.Text =
                    "Gateway Ping: Error";

                Debug.WriteLine(
                    ex.Message);
            }
        }


        // =====================================================
        // WI-FI BİLGİLERİ
        // =====================================================

        private async Task GetWifiInfo()
        {
            lblSsid.Text =
                "Wi-Fi: -";

            lblBssid.Text =
                "BSSID: -";

            lblSignal.Text =
                "Signal: -";

            lblRssi.Text =
                "RSSI: -";

            lblBand.Text =
                "Band: -";

            lblChannel.Text =
                "Channel: -";

            lblRadioType.Text =
                "Radio Type: -";

            lblReceiveRate.Text =
                "Receive: -";

            lblTransmitRate.Text =
                "Transmit: -";


            try
            {
                ProcessStartInfo startInfo =
                    new ProcessStartInfo
                    {
                        FileName =
                            "netsh",

                        Arguments =
                            "wlan show interfaces",

                        RedirectStandardOutput =
                            true,

                        UseShellExecute =
                            false,

                        CreateNoWindow =
                            true
                    };


                using Process process =
                    new Process();


                process.StartInfo =
                    startInfo;


                process.Start();


                string output =
                    await process
                        .StandardOutput
                        .ReadToEndAsync();


                await process
                    .WaitForExitAsync();


                string[] lines =
                    output.Split('\n');


                string signalValue = "";
                string channelValue = "";


                foreach (string rawLine in lines)
                {
                    string line =
                        rawLine.Trim();


                    // =====================================
                    // SSID
                    // =====================================

                    if (line.StartsWith("SSID") &&
                        !line.StartsWith("BSSID"))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblSsid.Text =
                                "Wi-Fi: " + value;
                        }
                    }


                    // =====================================
                    // BSSID
                    // =====================================

                    else if (
                        line.StartsWith("BSSID") ||
                        line.StartsWith("AP BSSID"))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblBssid.Text =
                                "BSSID: " + value;
                        }
                    }


                    // =====================================
                    // SIGNAL
                    // =====================================

                    else if (
                        line.StartsWith("Signal") ||
                        line.StartsWith("Sinyal"))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            signalValue =
                                value;

                            lblSignal.Text =
                                "Signal: " + value;
                        }
                    }


                    // =====================================
                    // RSSI
                    // =====================================

                    else if (
                        line.StartsWith(
                            "RSSI",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            if (!value.Contains(
                                "dBm",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                value +=
                                    " dBm";
                            }

                            lblRssi.Text =
                                "RSSI: " + value;
                        }
                    }


                    // =====================================
                    // BAND
                    // =====================================

                    else if (
                        line.StartsWith(
                            "Band",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        line.StartsWith(
                            "Bant",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblBand.Text =
                                "Band: " + value;
                        }
                    }


                    // =====================================
                    // CHANNEL
                    // =====================================

                    else if (
                        line.StartsWith("Channel") ||
                        line.StartsWith("Kanal"))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            channelValue =
                                value;

                            lblChannel.Text =
                                "Channel: " + value;
                        }
                    }


                    // =====================================
                    // RADIO TYPE
                    // =====================================

                    else if (
                        line.StartsWith(
                            "Radio type",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        line.StartsWith(
                            "Radyo türü",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblRadioType.Text =
                                "Radio Type: " +
                                value;
                        }
                    }


                    // =====================================
                    // RECEIVE RATE
                    // =====================================

                    else if (
                        line.StartsWith(
                            "Receive rate",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        line.StartsWith(
                            "Alma hızı",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblReceiveRate.Text =
                                "Receive: " +
                                value +
                                " Mbps";
                        }
                    }


                    // =====================================
                    // TRANSMIT RATE
                    // =====================================

                    else if (
                        line.StartsWith(
                            "Transmit rate",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        line.StartsWith(
                            "İletim hızı",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblTransmitRate.Text =
                                "Transmit: " +
                                value +
                                " Mbps";
                        }
                    }
                }


                // =====================================
                // RSSI GELMEDİYSE SIGNAL'DAN HESAPLA
                // =====================================

                if (lblRssi.Text ==
                    "RSSI: -" &&
                    signalValue != "")
                {
                    lblRssi.Text =
                        "RSSI: " +
                        EstimateRssiFromSignal(
                            signalValue);
                }


                // =====================================
                // BAND GELMEDİYSE CHANNEL'DAN BUL
                // =====================================

                if (lblBand.Text ==
                    "Band: -" &&
                    channelValue != "")
                {
                    lblBand.Text =
                        "Band: " +
                        InferBandFromChannel(
                            channelValue);
                }
            }
            catch (Exception ex)
            {
                lblSsid.Text =
                    "Wi-Fi: Error";

                Debug.WriteLine(
                    ex.Message);
            }
        }


        // =====================================================
        // ":" SONRASINDAKİ DEĞERİ AL
        // =====================================================

        private string GetValue(string line)
        {
            int index =
                line.IndexOf(':');


            if (index == -1)
            {
                return "";
            }


            return line
                .Substring(index + 1)
                .Trim();
        }


        // =====================================================
        // SIGNAL % -> YAKLAŞIK RSSI
        // =====================================================

        private string EstimateRssiFromSignal(
            string signalText)
        {
            string cleaned =
                signalText
                    .Replace("%", "")
                    .Trim();


            if (int.TryParse(
                cleaned,
                out int signalPercent))
            {
                int rssi =
                    (signalPercent / 2) -
                    100;


                return rssi +
                       " dBm";
            }


            return "-";
        }


        // =====================================================
        // CHANNEL -> BAND
        // =====================================================

        private string InferBandFromChannel(
            string channelText)
        {
            if (!int.TryParse(
                channelText.Trim(),
                out int channel))
            {
                return "-";
            }


            if (channel >= 1 &&
                channel <= 14)
            {
                return "2.4 GHz";
            }


            if (channel >= 36 &&
                channel <= 177)
            {
                return "5 GHz";
            }


            return "-";
        }


        // =====================================================
        // TEMA
        // =====================================================

        private void ApplyTheme()
        {
            // =====================================
            // FORM
            // =====================================

            this.Text =
                "NetWatch — Network Device Monitor";


            this.BackColor =
                Color.FromArgb(
                    241,
                    245,
                    249);


            this.ForeColor =
                Color.FromArgb(
                    15,
                    23,
                    42);


            this.Font =
                new Font(
                    "Segoe UI",
                    10F);


            this.StartPosition =
                FormStartPosition.CenterScreen;


            // =====================================
            // HEADER
            // =====================================

            pnlHeader.BackColor =
                Color.FromArgb(
                    15,
                    23,
                    42);


            lblTitle.Text =
                "NetWatch";


            lblTitle.ForeColor =
                Color.White;


            lblTitle.BackColor =
                Color.Transparent;


            lblTitle.Font =
                new Font(
                    "Segoe UI Semibold",
                    24F,
                    FontStyle.Bold);


            lblSubtitle.Text =
                "NetWatch • Local network monitoring  ";


            lblSubtitle.ForeColor =
                Color.FromArgb(
                    148,
                    163,
                    184);


            lblSubtitle.BackColor =
                Color.Transparent;


            lblSubtitle.Font =
                new Font(
                    "Segoe UI",
                    10F);


            lblLive.Text =
                "● LIVE";


            lblLive.ForeColor =
                Color.FromArgb(
                    74,
                    222,
                    128);


            lblLive.BackColor =
                Color.Transparent;


            lblLive.Font =
                new Font(
                    "Segoe UI Semibold",
                    10F,
                    FontStyle.Bold);


            // =====================================
            // NETWORK CARD
            // =====================================

            pnlNetwork.BackColor =
                Color.White;


            lblNetworkTitle.Text =
                "NETWORK OVERVIEW";


            lblNetworkTitle.ForeColor =
                Color.FromArgb(
                    37,
                    99,
                    235);


            lblNetworkTitle.BackColor =
                Color.Transparent;


            lblNetworkTitle.Font =
                new Font(
                    "Segoe UI Semibold",
                    11F,
                    FontStyle.Bold);


            // =====================================
            // WIFI CARD
            // =====================================

            pnlWifi.BackColor =
                Color.White;


            lblWifiTitle.Text =
                "WI-FI DETAILS";


            lblWifiTitle.ForeColor =
                Color.FromArgb(
                    37,
                    99,
                    235);


            lblWifiTitle.BackColor =
                Color.Transparent;


            lblWifiTitle.Font =
                new Font(
                    "Segoe UI Semibold",
                    11F,
                    FontStyle.Bold);


            // =====================================
            // NETWORK VALUE LABELS
            // =====================================

            StyleValueLabel(
                lblStatus);

            StyleValueLabel(
                lblIp);

            StyleValueLabel(
                lblGateway);

            StyleValueLabel(
                lblPing);


            // =====================================
            // WIFI VALUE LABELS
            // =====================================

            StyleValueLabel(
                lblSsid);

            StyleValueLabel(
                lblBssid);

            StyleValueLabel(
                lblSignal);

            StyleValueLabel(
                lblRssi);

            StyleValueLabel(
                lblBand);

            StyleValueLabel(
                lblChannel);

            StyleValueLabel(
                lblRadioType);

            StyleValueLabel(
                lblReceiveRate);

            StyleValueLabel(
                lblTransmitRate);


            // =====================================
            // REFRESH BUTTON
            // =====================================

            btnRefresh.Text =
                "Refresh";


            btnRefresh.BackColor =
                Color.FromArgb(
                    37,
                    99,
                    235);


            btnRefresh.ForeColor =
                Color.White;


            btnRefresh.FlatStyle =
                FlatStyle.Flat;


            btnRefresh
                .FlatAppearance
                .BorderSize = 0;


            btnRefresh
                .FlatAppearance
                .MouseOverBackColor =
                Color.FromArgb(
                    29,
                    78,
                    216);


            btnRefresh
                .FlatAppearance
                .MouseDownBackColor =
                Color.FromArgb(
                    30,
                    64,
                    175);


            btnRefresh.Font =
                new Font(
                    "Segoe UI Semibold",
                    10F,
                    FontStyle.Bold);


            btnRefresh.Cursor =
                Cursors.Hand;


            ApplyRoundedCorners();
        }


        // =====================================================
        // DEĞER LABEL TASARIMI
        // =====================================================

        private void StyleValueLabel(
            Label label)
        {
            label.ForeColor =
                Color.FromArgb(
                    30,
                    41,
                    59);


            label.BackColor =
                Color.Transparent;


            label.Font =
                new Font(
                    "Segoe UI",
                    10.5F);
        }


        // =====================================================
        // YUVARLAK KÖŞELER
        // =====================================================

        private void ApplyRoundedCorners()
        {
            RoundControl(
                pnlHeader,
                20);


            RoundControl(
                pnlNetwork,
                18);


            RoundControl(
                pnlWifi,
                18);


            RoundControl(
                btnRefresh,
                10);
        }


        private void RoundControl(
            Control control,
            int radius)
        {
            if (control.Width <= 0 ||
                control.Height <= 0)
            {
                return;
            }


            GraphicsPath path =
                new GraphicsPath();


            int diameter =
                radius * 2;


            Rectangle rectangle =
                new Rectangle(
                    0,
                    0,
                    control.Width,
                    control.Height);


            path.AddArc(
                rectangle.X,
                rectangle.Y,
                diameter,
                diameter,
                180,
                90);


            path.AddArc(
                rectangle.Right -
                diameter,
                rectangle.Y,
                diameter,
                diameter,
                270,
                90);


            path.AddArc(
                rectangle.Right -
                diameter,
                rectangle.Bottom -
                diameter,
                diameter,
                diameter,
                0,
                90);


            path.AddArc(
                rectangle.X,
                rectangle.Bottom -
                diameter,
                diameter,
                diameter,
                90,
                90);


            path.CloseFigure();


            control.Region =
                new Region(path);
        }


        // =====================================================
        // FORM RESIZE
        // =====================================================

        private void Form1_Resize(
            object? sender,
            EventArgs e)
        {
            ApplyRoundedCorners();
        }


        // =====================================================
        // ESKİ DESIGNER EVENT'İ
        //
        // Designer'da flowLayoutPanel1 Paint eventi
        // hâlâ bağlıysa hata vermesin diye duruyor.
        // =====================================================

        private void flowLayoutPanel1_Paint(
            object sender,
            PaintEventArgs e)
        {
        }
    }
}