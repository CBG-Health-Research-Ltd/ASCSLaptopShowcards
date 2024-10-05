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
            components = new System.ComponentModel.Container();
            btnStop = new Button();
            btnStart = new Button();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            groupBox2 = new GroupBox();
            listConnectedDevices = new ComboBox();
            btnDisconnect = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            groupBox3 = new GroupBox();
            txtMessage = new RichTextBox();
            groupBox4 = new GroupBox();
            btnSendMessage = new Button();
            txtSendData = new TextBox();
            bindingSource1 = new BindingSource(components);
            groupBox1 = new GroupBox();
            btnSaveLog = new Button();
            txtLogs = new RichTextBox();
            groupBox5 = new GroupBox();
            qrCodeControl1 = new WifiDirectHost.QrCodeControl();
            txtPassword = new TextBox();
            label4 = new Label();
            txtSSID = new TextBox();
            label3 = new Label();
            label2 = new Label();
            folderBrowserDialog1 = new FolderBrowserDialog();
            saveFileDialog1 = new SaveFileDialog();
            pollTextTimer = new System.Windows.Forms.Timer(components);
            label6 = new Label();
            linkLabel2 = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox2.SuspendLayout();
            statusStrip1.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            groupBox1.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(149, 154);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(123, 36);
            btnStop.TabIndex = 5;
            btnStop.Text = "Stop Listening";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(9, 154);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(123, 36);
            btnStart.TabIndex = 4;
            btnStart.Text = "Start Listening";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = WifiDirectHost.Properties.Resources.Ipsos_logo_svg;
            pictureBox1.Location = new Point(13, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(80, 78);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(129, 9);
            label1.Name = "label1";
            label1.Size = new Size(294, 33);
            label1.TabIndex = 1;
            label1.Text = "Ipsos Wifi Direct Host";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(listConnectedDevices);
            groupBox2.Controls.Add(btnDisconnect);
            groupBox2.Location = new Point(6, 194);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(427, 104);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Connection List";
            // 
            // listConnectedDevices
            // 
            listConnectedDevices.FormattingEnabled = true;
            listConnectedDevices.Location = new Point(15, 27);
            listConnectedDevices.Name = "listConnectedDevices";
            listConnectedDevices.Size = new Size(382, 23);
            listConnectedDevices.TabIndex = 10;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(15, 56);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(382, 33);
            btnDisconnect.TabIndex = 5;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 416);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(459, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(txtMessage);
            groupBox3.Location = new Point(792, 103);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(339, 156);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Communications Panel";
            // 
            // txtMessage
            // 
            txtMessage.BackColor = Color.White;
            txtMessage.BorderStyle = BorderStyle.FixedSingle;
            txtMessage.ForeColor = Color.Black;
            txtMessage.Location = new Point(24, 22);
            txtMessage.Name = "txtMessage";
            txtMessage.ReadOnly = true;
            txtMessage.Size = new Size(304, 116);
            txtMessage.TabIndex = 1;
            txtMessage.Text = "";
            txtMessage.TextChanged += txtMessage_TextChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnSendMessage);
            groupBox4.Controls.Add(txtSendData);
            groupBox4.Location = new Point(463, 102);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(323, 157);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Test Sender";
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(20, 116);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(282, 31);
            btnSendMessage.TabIndex = 6;
            btnSendMessage.Text = "Send Data";
            btnSendMessage.UseVisualStyleBackColor = true;
            btnSendMessage.Click += btnSendMessage_Click;
            // 
            // txtSendData
            // 
            txtSendData.Location = new Point(20, 23);
            txtSendData.Multiline = true;
            txtSendData.Name = "txtSendData";
            txtSendData.Size = new Size(282, 90);
            txtSendData.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnSaveLog);
            groupBox1.Controls.Add(txtLogs);
            groupBox1.Location = new Point(463, 265);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(668, 141);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log Actions";
            // 
            // btnSaveLog
            // 
            btnSaveLog.Location = new Point(20, 96);
            btnSaveLog.Name = "btnSaveLog";
            btnSaveLog.Size = new Size(637, 42);
            btnSaveLog.TabIndex = 1;
            btnSaveLog.Text = "Save Log Data";
            btnSaveLog.UseVisualStyleBackColor = true;
            btnSaveLog.Click += btnSaveLog_Click;
            // 
            // txtLogs
            // 
            txtLogs.BackColor = Color.White;
            txtLogs.BorderStyle = BorderStyle.FixedSingle;
            txtLogs.ForeColor = Color.Red;
            txtLogs.Location = new Point(20, 22);
            txtLogs.Name = "txtLogs";
            txtLogs.ReadOnly = true;
            txtLogs.Size = new Size(637, 68);
            txtLogs.TabIndex = 0;
            txtLogs.Text = "";
            txtLogs.TextChanged += txtLogs_TextChanged;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(qrCodeControl1);
            groupBox5.Controls.Add(btnStop);
            groupBox5.Controls.Add(btnStart);
            groupBox5.Controls.Add(txtPassword);
            groupBox5.Controls.Add(label4);
            groupBox5.Controls.Add(txtSSID);
            groupBox5.Controls.Add(label3);
            groupBox5.Controls.Add(groupBox2);
            groupBox5.Controls.Add(label2);
            groupBox5.Location = new Point(7, 102);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(450, 304);
            groupBox5.TabIndex = 7;
            groupBox5.TabStop = false;
            groupBox5.Text = "Connection Details";
            // 
            // qrCodeControl1
            // 
            qrCodeControl1.BinaryData = null;
            qrCodeControl1.BorderWidth = 3;
            qrCodeControl1.ErrorCorrection = 2;
            qrCodeControl1.Location = new Point(295, 15);
            qrCodeControl1.Name = "qrCodeControl1";
            qrCodeControl1.Size = new Size(149, 142);
            qrCodeControl1.TabIndex = 6;
            qrCodeControl1.Text = "qrCodeControl1";
            qrCodeControl1.TextData = "Test";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(5, 123);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(284, 23);
            txtPassword.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(75, 103);
            label4.Name = "label4";
            label4.Size = new Size(87, 15);
            label4.TabIndex = 3;
            label4.Text = "Wifi Password :";
            // 
            // txtSSID
            // 
            txtSSID.Location = new Point(5, 77);
            txtSSID.Name = "txtSSID";
            txtSSID.Size = new Size(280, 23);
            txtSSID.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(75, 60);
            label3.Name = "label3";
            label3.Size = new Size(110, 15);
            label3.TabIndex = 1;
            label3.Text = "Connection Name :";
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(64, 25);
            label2.Name = "label2";
            label2.Size = new Size(192, 21);
            label2.TabIndex = 0;
            label2.Text = "Scan QR Code to Connect";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "LogData.txt";
            // 
            // pollTextTimer
            // 
            pollTextTimer.Interval = 50;
            pollTextTimer.Tick += pollTextTimer_Tick;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label6.Location = new Point(147, 53);
            label6.Name = "label6";
            label6.Size = new Size(75, 21);
            label6.TabIndex = 9;
            label6.Text = "Version :";
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(147, 80);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(64, 15);
            linkLabel2.TabIndex = 10;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Show Logs";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // WifiDirect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(459, 438);
            Controls.Add(linkLabel2);
            Controls.Add(label6);
            Controls.Add(groupBox5);
            Controls.Add(groupBox1);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(statusStrip1);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "WifiDirect";
            StartPosition = FormStartPosition.Manual;
            Text = "Ipsos Wifi Direct";
            TopMost = true;
            FormClosing += WifiDirect_FormClosing;
            FormClosed += WifiDirect_FormClosed;
            Load += WifiDirect_Load;
            Resize += WifiDirect_Resize;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox2.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox pictureBox1;
        private Button btnStop;
        private Button btnStart;
        private Label label1;
        private GroupBox groupBox2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button btnDisconnect;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Button btnSendMessage;
        private TextBox txtSendData;
        private BindingSource bindingSource1;
        private GroupBox groupBox1;
        private RichTextBox txtLogs;
        private GroupBox groupBox5;
        private TextBox txtPassword;
        private Label label4;
        private TextBox txtSSID;
        private Label label3;
        private RichTextBox txtMessage;
        private Button btnSaveLog;
        private FolderBrowserDialog folderBrowserDialog1;
        private SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Timer pollTextTimer;
        private Label label2;
        private WifiDirectHost.QrCodeControl qrCodeControl1;
        private Label label6;
        private ComboBox listConnectedDevices;
        private LinkLabel linkLabel2;
    }
}
