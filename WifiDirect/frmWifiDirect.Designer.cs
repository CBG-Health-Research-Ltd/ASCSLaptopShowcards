namespace WifiDirect
{
    partial class WifiDirect
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
            groupBox1 = new GroupBox();
            btnStop = new Button();
            btnStart = new Button();
            txtPassword = new TextBox();
            label4 = new Label();
            label3 = new Label();
            txtSSID = new TextBox();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            groupBox2 = new GroupBox();
            button1 = new Button();
            listBox1 = new ListBox();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox2.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnStop);
            groupBox1.Controls.Add(btnStart);
            groupBox1.Controls.Add(txtPassword);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtSSID);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(7, 106);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(510, 229);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Connection Details";
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(261, 147);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(123, 58);
            btnStop.TabIndex = 5;
            btnStop.Text = "Stop Listening";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(65, 147);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(123, 58);
            btnStart.TabIndex = 4;
            btnStart.Text = "Start Listening";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(136, 108);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(318, 23);
            txtPassword.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 108);
            label4.Name = "label4";
            label4.Size = new Size(90, 15);
            label4.TabIndex = 2;
            label4.Text = "Wifi Password : ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 36);
            label3.Name = "label3";
            label3.Size = new Size(400, 15);
            label3.TabIndex = 2;
            label3.Text = "Default Connection Data has been provided. You may change this settings.";
            // 
            // txtSSID
            // 
            txtSSID.Location = new Point(136, 70);
            txtSSID.Name = "txtSSID";
            txtSSID.Size = new Size(318, 23);
            txtSSID.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 78);
            label2.Name = "label2";
            label2.Size = new Size(110, 15);
            label2.TabIndex = 0;
            label2.Text = "Connection Name :";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = WifiDirectHost.Properties.Resources.Ipsos_logo_svg;
            pictureBox1.Location = new Point(12, 22);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(80, 78);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(123, 20);
            label1.Name = "label1";
            label1.Size = new Size(350, 80);
            label1.TabIndex = 1;
            label1.Text = "Welcome to Ipsos Wifi Direct Host";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(button1);
            groupBox2.Controls.Add(listBox1);
            groupBox2.Location = new Point(7, 341);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(510, 244);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Connection List";
            // 
            // button1
            // 
            button1.Enabled = false;
            button1.Location = new Point(176, 180);
            button1.Name = "button1";
            button1.Size = new Size(123, 58);
            button1.TabIndex = 5;
            button1.Text = "Disconnect";
            button1.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(20, 32);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(461, 139);
            listBox1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 588);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(529, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // WifiDirect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(529, 610);
            ControlBox = false;
            Controls.Add(statusStrip1);
            Controls.Add(groupBox2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(groupBox1);
            Name = "WifiDirect";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ipsos Wifi Direct";
            Load += WifiDirect_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox2.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private PictureBox pictureBox1;
        private Button btnStop;
        private Button btnStart;
        private TextBox txtPassword;
        private Label label4;
        private Label label3;
        private TextBox txtSSID;
        private Label label2;
        private Label label1;
        private GroupBox groupBox2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button button1;
        private ListBox listBox1;
    }
}
