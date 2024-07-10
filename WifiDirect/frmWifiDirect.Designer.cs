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
            txtMessage = new TextBox();
            groupBox4 = new GroupBox();
            btnSendMessage = new Button();
            txtSendData = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            bindingSource1 = new BindingSource(components);
            groupBox1 = new GroupBox();
            txtLogs = new RichTextBox();
            groupBox5 = new GroupBox();
            txtReceive = new TextBox();
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
            btnStop.Location = new Point(158, 159);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(123, 58);
            btnStop.TabIndex = 5;
            btnStop.Text = "Stop Listening";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(25, 159);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(123, 58);
            btnStart.TabIndex = 4;
            btnStart.Text = "Start Listening";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = LaptopShowcards.Properties.Resources.Ipsos_logo_svg;
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
            label1.Location = new Point(214, 38);
            label1.Name = "label1";
            label1.Size = new Size(481, 51);
            label1.TabIndex = 1;
            label1.Text = "Welcome to Ipsos Wifi Direct Host";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnStart);
            groupBox2.Controls.Add(btnStop);
            groupBox2.Controls.Add(listConnectedDevices);
            groupBox2.Controls.Add(btnDisconnect);
            groupBox2.Location = new Point(12, 106);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(434, 244);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Connection List";
            // 
            // listConnectedDevices
            // 
            listConnectedDevices.FormattingEnabled = true;
            listConnectedDevices.ItemHeight = 15;
            listConnectedDevices.Location = new Point(25, 22);
            listConnectedDevices.Name = "listConnectedDevices";
            listConnectedDevices.Size = new Size(390, 109);
            listConnectedDevices.TabIndex = 6;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(292, 159);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(123, 58);
            btnDisconnect.TabIndex = 5;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 726);
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
            groupBox3.Location = new Point(7, 366);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(439, 195);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Sent Data";
            // 
            // txtMessage
            // 
            txtMessage.BackColor = Color.White;
            txtMessage.BorderStyle = BorderStyle.FixedSingle;
            txtMessage.Location = new Point(20, 22);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.ReadOnly = true;
            txtMessage.Size = new Size(400, 155);
            txtMessage.TabIndex = 0;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnSendMessage);
            groupBox4.Controls.Add(txtSendData);
            groupBox4.Location = new Point(468, 107);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(432, 243);
            groupBox4.TabIndex = 5;
            groupBox4.TabStop = false;
            groupBox4.Text = "Test Sender";
            // 
            // btnSendMessage
            // 
            btnSendMessage.Location = new Point(152, 158);
            btnSendMessage.Name = "btnSendMessage";
            btnSendMessage.Size = new Size(123, 58);
            btnSendMessage.TabIndex = 6;
            btnSendMessage.Text = "Send Data";
            btnSendMessage.UseVisualStyleBackColor = true;
            btnSendMessage.Click += btnSendMessage_Click;
            // 
            // txtSendData
            // 
            txtSendData.Location = new Point(20, 22);
            txtSendData.Multiline = true;
            txtSendData.Name = "txtSendData";
            txtSendData.Size = new Size(392, 108);
            txtSendData.TabIndex = 0;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick_1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtLogs);
            groupBox1.Location = new Point(7, 567);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(893, 145);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Log Actions";
            // 
            // txtLogs
            // 
            txtLogs.BackColor = Color.White;
            txtLogs.BorderStyle = BorderStyle.FixedSingle;
            txtLogs.ForeColor = Color.Red;
            txtLogs.Location = new Point(20, 19);
            txtLogs.Name = "txtLogs";
            txtLogs.ReadOnly = true;
            txtLogs.Size = new Size(853, 120);
            txtLogs.TabIndex = 0;
            txtLogs.Text = "";
            txtLogs.TextChanged += txtLogs_TextChanged;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(txtReceive);
            groupBox5.Location = new Point(468, 366);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(432, 195);
            groupBox5.TabIndex = 7;
            groupBox5.TabStop = false;
            groupBox5.Text = "Received Data";
            // 
            // txtReceive
            // 
            txtReceive.BackColor = Color.White;
            txtReceive.BorderStyle = BorderStyle.FixedSingle;
            txtReceive.Location = new Point(20, 22);
            txtReceive.Multiline = true;
            txtReceive.Name = "txtReceive";
            txtReceive.ReadOnly = true;
            txtReceive.Size = new Size(392, 155);
            txtReceive.TabIndex = 1;
            // 
            // WifiDirect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(912, 748);
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
            Load += WifiDirect_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox2.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
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
        private TextBox txtMessage;
        private GroupBox groupBox4;
        private Button btnSendMessage;
        private TextBox txtSendData;
        private ListBox listConnectedDevices;
        private System.Windows.Forms.Timer timer1;
        private BindingSource bindingSource1;
        private GroupBox groupBox1;
        private RichTextBox txtLogs;
        private GroupBox groupBox5;
        private TextBox txtReceive;
    }
}
