using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace RTM_Pinger
{
    public partial class Form1 : Form
    {
        private Button btnPing;
        private Button btnStopPing;
        private TextBox txtHost;
        private Panel panel1;
        private ZedGraphControl zedGraphControl1;
        private bool isPinging = false;

        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeFormControls();
            InitializeChart();
        }

        private void InitializeFormControls()
        {
            // txtHost
            txtHost = new TextBox();
            txtHost.Location = new Point(12, 10);
            txtHost.Size = new Size(200, 20);
            Controls.Add(txtHost);

            // btnPing
            btnPing = new Button();
            btnPing.Location = new Point(220, 10);
            btnPing.Size = new Size(75, 23);
            btnPing.Text = "Ping";
            btnPing.Click += new EventHandler(btnPing_Click);
            btnPing.BackColor = Color.White; // Ustawienie bia³ego t³a
            Controls.Add(btnPing);

            // btnStopPing
            btnStopPing = new Button();
            btnStopPing.Location = new Point(300, 10);
            btnStopPing.Size = new Size(75, 23);
            btnStopPing.Text = "Stop";
            btnStopPing.Enabled = false;
            btnStopPing.Click += new EventHandler(btnStopPing_Click);
            btnStopPing.BackColor = Color.White; // Ustawienie bia³ego t³a
            Controls.Add(btnStopPing);

            // panel1
            panel1 = new Panel();
            panel1.Location = new Point(12, 40);
            panel1.Size = new Size(776, 356);
            panel1.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(panel1);

            // zedGraphControl1
            zedGraphControl1 = new ZedGraphControl();
            zedGraphControl1.Dock = DockStyle.Fill;
            panel1.Controls.Add(zedGraphControl1);
        }

        private void InitializeChart()
        {
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Ping Response Time";
            myPane.XAxis.Title.Text = "Time";
            myPane.YAxis.Title.Text = "Milliseconds";

            myPane.XAxis.Type = AxisType.Date;
            myPane.XAxis.Scale.Format = "HH:mm:ss";
            myPane.Chart.Fill = new Fill(Color.FromArgb(30, 30, 30));
            myPane.Fill = new Fill(Color.FromArgb(50, 50, 50));
            myPane.Chart.Border.Color = Color.White;
            myPane.XAxis.Color = Color.White;
            myPane.YAxis.Color = Color.White;
            myPane.XAxis.MajorGrid.Color = Color.Gray;
            myPane.YAxis.MajorGrid.Color = Color.Gray;
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;

            // Setting the text color to white for visibility
            myPane.Title.FontSpec.FontColor = Color.WhiteSmoke;
            myPane.XAxis.Title.FontSpec.FontColor = Color.WhiteSmoke;
            myPane.YAxis.Title.FontSpec.FontColor = Color.WhiteSmoke;
            myPane.XAxis.Scale.FontSpec.FontColor = Color.WhiteSmoke;
            myPane.YAxis.Scale.FontSpec.FontColor = Color.WhiteSmoke;


            RollingPointPairList list = new RollingPointPairList(1200);
            myPane.AddCurve("Ping", list, Color.Red, SymbolType.None);

            zedGraphControl1.AxisChange();
        }

        private async void btnPing_Click(object sender, EventArgs e)
        {
            isPinging = true;
            btnPing.Enabled = false;
            btnStopPing.Enabled = true;
            txtHost.Enabled = false; // Disable the textbox to prevent changing the host while pinging
            string host = txtHost.Text;
            Ping pingSender = new Ping();

            while (isPinging)
            {
                try
                {
                    PingReply reply = await pingSender.SendPingAsync(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        AddDataPoint(reply.RoundtripTime);
                    }
                }
                catch
                {
                    // Handle any exceptions here
                }
                await Task.Delay(1000); // Delay between pings
            }
        }

        private void btnStopPing_Click(object sender, EventArgs e)
        {
            isPinging = false;
            btnPing.Enabled = true;
            btnStopPing.Enabled = false;
            txtHost.Enabled = true; // Re-enable the textbox
        }

        private void AddDataPoint(long roundtripTime)
        {
            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)
                return;

            LineItem curve = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            if (curve == null)
                return;

            IPointListEdit list = curve.Points as IPointListEdit;
            if (list == null)
                return;

            double time = new XDate(DateTime.Now);
            list.Add(time, roundtripTime);

            // Ustawienie zakresu osi X na ostatnie 30 sekund danych
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.XAxis.Scale.Max = new XDate(DateTime.Now.AddSeconds(1)); // Add a buffer of 1 second
            pane.XAxis.Scale.Min = new XDate(DateTime.Now.AddSeconds(-30)); // Show last 30 seconds of data

            // Adjust the Y axis scale if needed
            if (roundtripTime > pane.YAxis.Scale.Max || pane.YAxis.Scale.Max == 1.2)
            {
                pane.YAxis.Scale.Max = roundtripTime + roundtripTime * 0.1; // Add 10% buffer
            }

            if (roundtripTime < pane.YAxis.Scale.Min || pane.YAxis.Scale.Min == 0)
            {
                pane.YAxis.Scale.Min = roundtripTime * 0.9; // Subtract 10% if needed, avoid going into negative
            }

            // Make sure the graph is up-to-date
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }
    }
}
