namespace RTM_Pinger
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
            panel1 = new Panel();
            txtHost = new TextBox();
            btnPing = new Button();
            btnStopPing = new Button();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Location = new Point(149, 121);
            panel1.Name = "panel1";
            panel1.Size = new Size(371, 116);
            panel1.TabIndex = 0;
            panel1.Visible = false;
            // 
            // txtHost
            // 
            txtHost.Location = new Point(12, 12);
            txtHost.Name = "txtHost";
            txtHost.Size = new Size(343, 23);
            txtHost.TabIndex = 1;
            txtHost.Visible = false;
            // 
            // btnPing
            // 
            btnPing.Location = new Point(361, 12);
            btnPing.Name = "btnPing";
            btnPing.Size = new Size(132, 23);
            btnPing.TabIndex = 2;
            btnPing.Text = "Ping";
            btnPing.UseVisualStyleBackColor = true;
            btnPing.Visible = false;
            // 
            // btnStopPing
            // 
            btnStopPing.Location = new Point(499, 12);
            btnStopPing.Name = "btnStopPing";
            btnStopPing.Size = new Size(132, 23);
            btnStopPing.TabIndex = 3;
            btnStopPing.Text = "Stop";
            btnStopPing.UseVisualStyleBackColor = true;
            btnStopPing.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(800, 405);
            Controls.Add(btnPing);
            Controls.Add(btnStopPing);
            Controls.Add(txtHost);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "RTM Pinger";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion






    }
}
