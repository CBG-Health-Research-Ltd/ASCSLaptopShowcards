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
            listConnectedDevices = new ListBox();
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
            txtPassword = new TextBox();
            label4 = new Label();
            txtSSID = new TextBox();
            label3 = new Label();
            label2 = new Label();
            folderBrowserDialog1 = new FolderBrowserDialog();
            saveFileDialog1 = new SaveFileDialog();
            label5 = new Label();
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
            btnStop.Location = new Point(238, 145);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(123, 36);
            btnStop.TabIndex = 5;
            btnStop.Text = "Stop Listening";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(77, 145);
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
            label1.Location = new Point(188, 31);
            label1.Name = "label1";
            label1.Size = new Size(481, 51);
            label1.TabIndex = 1;
            label1.Text = "Welcome to Ipsos Wifi Direct Host v1.0";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(listConnectedDevices);
            groupBox2.Controls.Add(btnDisconnect);
            groupBox2.Location = new Point(473, 102);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(427, 195);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Connection List";
            // 
            // listConnectedDevices
            // 
            listConnectedDevices.FormattingEnabled = true;
            listConnectedDevices.ItemHeight = 15;
            listConnectedDevices.Location = new Point(25, 67);
            listConnectedDevices.Name = "listConnectedDevices";
            listConnectedDevices.Size = new Size(382, 64);
            listConnectedDevices.TabIndex = 6;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(25, 148);
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
            statusStrip1.Location = new Point(0, 598);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(912, 22);
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
            groupBox3.Location = new Point(474, 304);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(426, 174);
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
            txtMessage.Size = new Size(382, 137);
            txtMessage.TabIndex = 1;
            txtMessage.Text = "";
            txtMessage.TextChanged += txtMessage_TextChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnSendMessage);
            groupBox4.Controls.Add(txtSendData);
            groupBox4.Location = new Point(7, 303);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(437, 175);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Test Sender";
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(20, 129);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(396, 31);
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
            txtSendData.Size = new Size(396, 100);
            txtSendData.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnSaveLog);
            groupBox1.Controls.Add(txtLogs);
            groupBox1.Location = new Point(7, 484);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(893, 108);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log Actions";
            // 
            // btnSaveLog
            // 
            btnSaveLog.Location = new Point(733, 19);
            btnSaveLog.Name = "btnSaveLog";
            btnSaveLog.Size = new Size(140, 79);
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
            txtLogs.Size = new Size(696, 76);
            txtLogs.TabIndex = 0;
            txtLogs.Text = "";
            txtLogs.TextChanged += txtLogs_TextChanged;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(btnStop);
            groupBox5.Controls.Add(btnStart);
            groupBox5.Controls.Add(txtPassword);
            groupBox5.Controls.Add(label4);
            groupBox5.Controls.Add(txtSSID);
            groupBox5.Controls.Add(label3);
            groupBox5.Controls.Add(label2);
            groupBox5.Location = new Point(7, 102);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(437, 195);
            groupBox5.TabIndex = 7;
            groupBox5.TabStop = false;
            groupBox5.Text = "Connection Details";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(120, 101);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(296, 23);
            txtPassword.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(27, 109);
            label4.Name = "label4";
            label4.Size = new Size(87, 15);
            label4.TabIndex = 3;
            label4.Text = "Wifi Password :";
            // 
            // txtSSID
            // 
            txtSSID.Location = new Point(120, 72);
            txtSSID.Name = "txtSSID";
            txtSSID.Size = new Size(296, 23);
            txtSSID.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(4, 80);
            label3.Name = "label3";
            label3.Size = new Size(110, 15);
            label3.TabIndex = 1;
            label3.Text = "Connection Name :";
            // 
            // label2
            // 
            label2.Location = new Point(6, 27);
            label2.Name = "label2";
            label2.Size = new Size(401, 34);
            label2.TabIndex = 0;
            label2.Text = "Default Connection Details has been provided. You may change this settings. ";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "LogData.txt";
            // 
            // label5
            // 
            label5.Location = new Point(25, 27);
            label5.Name = "label5";
            label5.Size = new Size(320, 21);
            label5.TabIndex = 7;
            label5.Text = "Make sure Ipsos Laptop and Tablet are paired.  ";
            // 
            // WifiDirect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(912, 620);
            Controls.Add(groupBox5);
            Controls.Add(groupBox1);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(statusStrip1);
            Controls.Add(groupBox2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Name = "WifiDirect";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ipsos Wifi Direct";
            TopMost = true;
            FormClosing += WifiDirect_FormClosing;
            FormClosed += WifiDirect_FormClosed;
            Load += WifiDirect_Load;
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
        private ListBox listConnectedDevices;
        private BindingSource bindingSource1;
        private GroupBox groupBox1;
        private RichTextBox txtLogs;
        private GroupBox groupBox5;
        private Label label2;
        private TextBox txtPassword;
        private Label label4;
        private TextBox txtSSID;
        private Label label3;
        private RichTextBox txtMessage;
        private Button btnSaveLog;
        private FolderBrowserDialog folderBrowserDialog1;
        private SaveFileDialog saveFileDialog1;
        private Label label5;
    }
}
