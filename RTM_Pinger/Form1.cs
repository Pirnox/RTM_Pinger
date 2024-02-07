using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.IO; // For file operations

namespace RTM_Pinger
{
    public partial class Form1 : Form
    {
        private Button btnPing;
        private Button btnStopPing;
        private Button btnSaveGraph; // Save button
        private Button btnOpenGraph; // Open button
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
            btnPing.BackColor = Color.White; // Setting white background
            Controls.Add(btnPing);

            // btnStopPing
            btnStopPing = new Button();
            btnStopPing.Location = new Point(300, 10);
            btnStopPing.Size = new Size(75, 23);
            btnStopPing.Text = "Stop";
            btnStopPing.Enabled = false;
            btnStopPing.Click += new EventHandler(btnStopPing_Click);
            btnStopPing.BackColor = Color.White; // Setting white background
            Controls.Add(btnStopPing);

            // btnSaveGraph
            btnSaveGraph = new Button();
            btnSaveGraph.Location = new Point(380, 10);
            btnSaveGraph.Size = new Size(85, 23);
            btnSaveGraph.Text = "Save Graph";
            btnSaveGraph.Click += new EventHandler(btnSaveGraph_Click);
            btnSaveGraph.BackColor = Color.White;
            Controls.Add(btnSaveGraph);

            // btnOpenGraph
            btnOpenGraph = new Button();
            btnOpenGraph.Location = new Point(470, 10);
            btnOpenGraph.Size = new Size(85, 23);
            btnOpenGraph.Text = "Open Graph";
            btnOpenGraph.Click += new EventHandler(btnOpenGraph_Click);
            btnOpenGraph.BackColor = Color.White;
            Controls.Add(btnOpenGraph);

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
            txtHost.Enabled = false;
            string host = txtHost.Text;

            // Update the chart title to include the IP address being pinged
            zedGraphControl1.GraphPane.Title.Text = $"Ping Response Time for {host}";
            zedGraphControl1.AxisChange();

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
            txtHost.Enabled = true;
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

            GraphPane pane = zedGraphControl1.GraphPane;
            pane.XAxis.Scale.Max = new XDate(DateTime.Now.AddSeconds(1));
            pane.XAxis.Scale.Min = new XDate(DateTime.Now.AddSeconds(-30));

            if (roundtripTime > pane.YAxis.Scale.Max || pane.YAxis.Scale.Max == 1.2)
            {
                pane.YAxis.Scale.Max = roundtripTime + roundtripTime * 0.1;
            }

            if (roundtripTime < pane.YAxis.Scale.Min || pane.YAxis.Scale.Min == 0)
            {
                pane.YAxis.Scale.Min = roundtripTime * 0.9;
            }

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }

        private void btnSaveGraph_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Files (*.png)|*.png";
            saveFileDialog.DefaultExt = "png";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Image graphImage = zedGraphControl1.GraphPane.GetImage();
                graphImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void btnOpenGraph_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Files (*.png)|*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Image graphImage = Image.FromFile(openFileDialog.FileName);
                Form graphForm = new Form
                {
                    Width = 800,
                    Height = 600
                };
                PictureBox pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = graphImage,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                graphForm.Controls.Add(pictureBox);
                graphForm.Show();
            }
        }
    }
}
