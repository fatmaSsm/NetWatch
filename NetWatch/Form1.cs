namespace NetWatch
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        // =====================================================
        // PING GRAPH
        // =====================================================

        private readonly List<long> pingHistory =
            new List<long>();

        private readonly System.Windows.Forms.Timer pingTimer =
            new System.Windows.Forms.Timer();

        private string currentGatewayIp = "";

        private const int MaxPingSamples = 30;


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public Form1()
        {
            InitializeComponent();

            ApplyTheme();

            this.Resize += Form1_Resize;

            // Ping grafiğini kendimiz çiziyoruz.
            pnlPingChart.Paint += pnlPingChart_Paint;

            // 2 saniyede bir gateway ping ölçümü.
            pingTimer.Interval = 2000;
            pingTimer.Tick += PingTimer_Tick;
        }


        // =====================================================
        // FORM LOAD
        // =====================================================

        private async void Form1_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            ApplyRoundedCorners();

            await RefreshAll(false);

            pingTimer.Start();
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
                btnRefresh.Text =
                    "↻  Refreshing...";

                btnRefresh.BackColor =
                    Color.FromArgb(
                        29,
                        78,
                        216);

                await Task.Delay(250);
            }

            try
            {
                await GetNetworkInfo();
                await GetWifiInfo();
            }
            finally
            {
                btnRefresh.Text =
                    "Refresh";

                btnRefresh.BackColor =
                    Color.FromArgb(
                        37,
                        99,
                        235);

                btnRefresh.Enabled = true;
            }
        }


        // =====================================================
        // NETWORK BİLGİLERİ
        // =====================================================

        private async Task GetNetworkInfo()
        {
            lblStatus.Text =
                "Status: Offline";

            lblLive.Text = "● OFFLINE";

            lblStatus.ForeColor =
                Color.FromArgb(
                    220,
                    38,
                    38);

            lblLive.ForeColor =
                    Color.FromArgb(
                    220,
                    38,
                    38);

            lblIp.Text =
                "Local IP: -";

            lblGateway.Text =
                "Gateway: -";

            lblPing.Text =
                "Gateway Ping: -";

            currentGatewayIp = "";

            try
            {
                foreach (
                    NetworkInterface network
                    in NetworkInterface
                        .GetAllNetworkInterfaces())
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

                    foreach (
                        UnicastIPAddressInformation ip
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

                    foreach (
                        GatewayIPAddressInformation gateway
                        in properties.GatewayAddresses)
                    {
                        if (gateway.Address.AddressFamily ==
                            AddressFamily.InterNetwork)
                        {
                            lblStatus.Text =
                                "Status: Online";

                            lblLive.Text = "● LIVE";

                            lblStatus.ForeColor =
                                Color.FromArgb(
                                    22,
                                    163,
                                    74);

                            lblLive.ForeColor =
                                Color.FromArgb(
                                    74,
                                    222,
                                    128);

                            lblIp.Text =
                                "Local IP: " +
                                localIp;

                            lblGateway.Text =
                                "Gateway: " +
                                gateway.Address;

                            currentGatewayIp =
                                gateway.Address
                                    .ToString();

                            await PingGateway(
                                currentGatewayIp);

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
                    Color.FromArgb(
                        220,
                        38,
                        38);

                Debug.WriteLine(
                    ex.Message);
            }
        }


        // =====================================================
        // GATEWAY PING
        // =====================================================

        private async Task PingGateway(
            string gatewayIp)
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
                    long pingValue =
                        reply.RoundtripTime;

                    lblPing.Text =
                        "Gateway Ping: " +
                        pingValue +
                        " ms";

                    AddPingSample(
                        pingValue);
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
        // PING HISTORY'YE DEĞER EKLE
        // =====================================================

        private void AddPingSample(long ping)
        {
            pingHistory.Add(ping);

            if (pingHistory.Count >
                MaxPingSamples)
            {
                pingHistory.RemoveAt(0);
            }

            pnlPingChart.Invalidate();
        }


        // =====================================================
        // 2 SANİYEDE BİR PING
        // =====================================================

        private async void PingTimer_Tick(
            object? sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                currentGatewayIp))
            {
                return;
            }

            // Aynı anda birden fazla ping
            // çalışmasın diye timer'ı durdur.
            pingTimer.Stop();

            try
            {
                await PingGateway(
                    currentGatewayIp);
            }
            finally
            {
                pingTimer.Start();
            }
        }


        // =====================================================
        // PING GRAPH ÇİZİMİ
        // =====================================================

        private void pnlPingChart_Paint(
            object? sender,
            PaintEventArgs e)
        {
            Graphics g =
                e.Graphics;

            g.SmoothingMode =
                SmoothingMode.AntiAlias;


            // -------------------------------------
            // COLORS
            // -------------------------------------

            Color backgroundColor =
                Color.FromArgb(
                    248,
                    250,
                    252);

            Color mainText =
                Color.FromArgb(
                    30,
                    41,
                    59);

            Color secondaryText =
                Color.FromArgb(
                    100,
                    116,
                    139);

            Color blue =
                Color.FromArgb(
                    37,
                    99,
                    235);

            Color grid =
                Color.FromArgb(
                    226,
                    232,
                    240);


            g.Clear(
                backgroundColor);


            // -------------------------------------
            // FONTS
            // -------------------------------------

            using Font titleFont =
                new Font(
                    "Segoe UI Semibold",
                    9F,
                    FontStyle.Bold);

            using Font smallFont =
                new Font(
                    "Segoe UI",
                    8F);

            using Font statsFont =
                new Font(
                    "Segoe UI",
                    8F);


            // -------------------------------------
            // TITLE
            // -------------------------------------

            using Brush titleBrush =
                new SolidBrush(
                    blue);

            g.DrawString(
                "PING HISTORY",
                titleFont,
                titleBrush,
                14,
                9);


            // -------------------------------------
            // VERİ YOKSA
            // -------------------------------------

            if (pingHistory.Count == 0)
            {
                using Brush emptyBrush =
                    new SolidBrush(
                        secondaryText);

                g.DrawString(
                    "Waiting for ping samples...",
                    smallFont,
                    emptyBrush,
                    14,
                    55);

                return;
            }


            // -------------------------------------
            // STATS
            // -------------------------------------

            long current =
                pingHistory[
                    pingHistory.Count - 1];

            double average =
                pingHistory.Average();

            long min =
                pingHistory.Min();

            long max =
                pingHistory.Max();


            string stats =
                $"Current {current} ms   " +
                $"Avg {average:0.0} ms   " +
                $"Min {min} ms   " +
                $"Max {max} ms";


            using Brush statsBrush =
                new SolidBrush(
                    secondaryText);

            g.DrawString(
                stats,
                statsFont,
                statsBrush,
                14,
                29);


            // -------------------------------------
            // CHART AREA
            // -------------------------------------

            int left = 14;
            int right = 14;

            int top = 55;
            int bottom = 15;

            int chartWidth =
                pnlPingChart.Width -
                left -
                right;

            int chartHeight =
                pnlPingChart.Height -
                top -
                bottom;


            if (chartWidth <= 0 ||
                chartHeight <= 0)
            {
                return;
            }


            // -------------------------------------
            // GRID
            // -------------------------------------

            using Pen gridPen =
                new Pen(
                    grid,
                    1F);

            for (int i = 0;
                 i <= 3;
                 i++)
            {
                float y =
                    top +
                    (
                        chartHeight /
                        3f
                    ) *
                    i;

                g.DrawLine(
                    gridPen,
                    left,
                    y,
                    left + chartWidth,
                    y);
            }


            // -------------------------------------
            // MAX SCALE
            // -------------------------------------

            long graphMax =
                Math.Max(
                    pingHistory.Max(),
                    10);

            graphMax +=
                Math.Max(
                    5,
                    graphMax / 4);


            // -------------------------------------
            // TEK NOKTA VARSA
            // -------------------------------------

            if (pingHistory.Count == 1)
            {
                float normalized =
                    pingHistory[0] /
                    (float)graphMax;

                float y =
                    top +
                    chartHeight -
                    normalized *
                    chartHeight;

                using Brush pointBrush =
                    new SolidBrush(
                        blue);

                g.FillEllipse(
                    pointBrush,
                    left - 4,
                    y - 4,
                    8,
                    8);

                return;
            }


            // -------------------------------------
            // POINTS
            // -------------------------------------

            PointF[] points =
                new PointF[
                    pingHistory.Count];


            float xStep =
                chartWidth /
                (float)(
                    MaxPingSamples - 1);


            for (int i = 0;
                 i < pingHistory.Count;
                 i++)
            {
                float x =
                    left +
                    i *
                    xStep;

                float normalized =
                    pingHistory[i] /
                    (float)graphMax;

                float y =
                    top +
                    chartHeight -
                    normalized *
                    chartHeight;

                points[i] =
                    new PointF(
                        x,
                        y);
            }


            // -------------------------------------
            // LINE
            // -------------------------------------

            using Pen linePen =
                new Pen(
                    blue,
                    2.5F);

            linePen.StartCap =
                LineCap.Round;

            linePen.EndCap =
                LineCap.Round;

            linePen.LineJoin =
                LineJoin.Round;


            g.DrawLines(
                linePen,
                points);


            // -------------------------------------
            // LAST POINT
            // -------------------------------------

            PointF lastPoint =
                points[
                    points.Length - 1];


            using Brush dotBrush =
                new SolidBrush(
                    blue);

            g.FillEllipse(
                dotBrush,
                lastPoint.X - 4,
                lastPoint.Y - 4,
                8,
                8);


            // -------------------------------------
            // CURRENT VALUE
            // -------------------------------------

            string currentText =
                current + " ms";


            using Brush currentBrush =
                new SolidBrush(
                    mainText);


            SizeF currentSize =
                g.MeasureString(
                    currentText,
                    smallFont);


            float currentX =
                lastPoint.X -
                currentSize.Width / 2;


            if (currentX < left)
            {
                currentX = left;
            }


            if (currentX +
                currentSize.Width >
                pnlPingChart.Width -
                right)
            {
                currentX =
                    pnlPingChart.Width -
                    right -
                    currentSize.Width;
            }


            float currentY =
                lastPoint.Y -
                20;


            if (currentY <
                top)
            {
                currentY =
                    lastPoint.Y +
                    8;
            }


            g.DrawString(
                currentText,
                smallFont,
                currentBrush,
                currentX,
                currentY);
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


                foreach (
                    string rawLine
                    in lines)
                {
                    string line =
                        rawLine.Trim();


                    // ---------------------------------
                    // SSID
                    // ---------------------------------

                    if (line.StartsWith("SSID") &&
                        !line.StartsWith("BSSID"))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblSsid.Text =
                                "Wi-Fi: " +
                                value;
                        }
                    }


                    // ---------------------------------
                    // BSSID
                    // ---------------------------------

                    else if (
                        line.StartsWith("BSSID") ||
                        line.StartsWith("AP BSSID"))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblBssid.Text =
                                "BSSID: " +
                                value;
                        }
                    }


                    // ---------------------------------
                    // SIGNAL
                    // ---------------------------------

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
                                "Signal: " +
                                value;
                        }
                    }


                    // ---------------------------------
                    // RSSI
                    // ---------------------------------

                    else if (
                        line.StartsWith(
                            "RSSI",
                            StringComparison
                                .OrdinalIgnoreCase))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            if (!value.Contains(
                                "dBm",
                                StringComparison
                                    .OrdinalIgnoreCase))
                            {
                                value +=
                                    " dBm";
                            }

                            lblRssi.Text =
                                "RSSI: " +
                                value;
                        }
                    }


                    // ---------------------------------
                    // BAND
                    // ---------------------------------

                    else if (
                        line.StartsWith(
                            "Band",
                            StringComparison
                                .OrdinalIgnoreCase)
                        ||
                        line.StartsWith(
                            "Bant",
                            StringComparison
                                .OrdinalIgnoreCase))
                    {
                        string value =
                            GetValue(line);

                        if (value != "")
                        {
                            lblBand.Text =
                                "Band: " +
                                value;
                        }
                    }


                    // ---------------------------------
                    // CHANNEL
                    // ---------------------------------

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
                                "Channel: " +
                                value;
                        }
                    }


                    // ---------------------------------
                    // RADIO TYPE
                    // ---------------------------------

                    else if (
                        line.StartsWith(
                            "Radio type",
                            StringComparison
                                .OrdinalIgnoreCase)
                        ||
                        line.StartsWith(
                            "Radyo türü",
                            StringComparison
                                .OrdinalIgnoreCase))
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


                    // ---------------------------------
                    // RECEIVE
                    // ---------------------------------

                    else if (
                        line.StartsWith(
                            "Receive rate",
                            StringComparison
                                .OrdinalIgnoreCase)
                        ||
                        line.StartsWith(
                            "Alma hızı",
                            StringComparison
                                .OrdinalIgnoreCase))
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


                    // ---------------------------------
                    // TRANSMIT
                    // ---------------------------------

                    else if (
                        line.StartsWith(
                            "Transmit rate",
                            StringComparison
                                .OrdinalIgnoreCase)
                        ||
                        line.StartsWith(
                            "İletim hızı",
                            StringComparison
                                .OrdinalIgnoreCase))
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


                // ---------------------------------
                // RSSI FALLBACK
                // ---------------------------------

                if (lblRssi.Text ==
                    "RSSI: -" &&
                    signalValue != "")
                {
                    lblRssi.Text =
                        "RSSI: " +
                        EstimateRssiFromSignal(
                            signalValue);
                }


                // ---------------------------------
                // BAND FALLBACK
                // ---------------------------------

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
        // ":" SONRASINDAKİ DEĞER
        // =====================================================

        private string GetValue(
            string line)
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
        // SIGNAL -> RSSI
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
                    (
                        signalPercent /
                        2
                    ) -
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
        // THEME
        // =====================================================

        private void ApplyTheme()
        {
            // -------------------------------------
            // FORM
            // -------------------------------------

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


            // -------------------------------------
            // HEADER
            // -------------------------------------

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
                "Network Device Monitor";

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


            // -------------------------------------
            // NETWORK CARD
            // -------------------------------------

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


            // -------------------------------------
            // WIFI CARD
            // -------------------------------------

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


            // -------------------------------------
            // PING CHART
            // -------------------------------------

            pnlPingChart.BackColor =
                Color.FromArgb(
                    248,
                    250,
                    252);


            // -------------------------------------
            // VALUE LABELS
            // -------------------------------------

            StyleValueLabel(
                lblStatus);

            StyleValueLabel(
                lblIp);

            StyleValueLabel(
                lblGateway);

            StyleValueLabel(
                lblPing);

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


            // -------------------------------------
            // BUTTON
            // -------------------------------------

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
        // VALUE LABEL STYLE
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
        // ROUND CORNERS
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
                pnlPingChart,
                12);

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

            pnlPingChart.Invalidate();
        }


        // =====================================================
        // ESKİ DESIGNER EVENT
        //
        // Designer'da hala bağlıysa hata vermesin.
        // =====================================================

        private void flowLayoutPanel1_Paint(
            object sender,
            PaintEventArgs e)
        {
        }
    }
}