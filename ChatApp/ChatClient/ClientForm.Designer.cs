namespace ChatClient
{
    partial class ClientForm
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
            txtUsername = new TextBox();
            btnConnect = new Button();
            label1 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            tableLayoutPanel5 = new TableLayoutPanel();
            lstMessageLogs = new ListBox();
            lstUsers = new ListBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel7 = new TableLayoutPanel();
            label3 = new Label();
            btnSend = new Button();
            lblSendingTo = new Label();
            rtxtReciver = new RichTextBox();
            rtxtMessage = new RichTextBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(3, 26);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(148, 23);
            txtUsername.TabIndex = 3;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(3, 59);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 22);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(60, 15);
            label1.TabIndex = 5;
            label1.Text = "Username";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(txtUsername, 0, 1);
            tableLayoutPanel1.Controls.Add(btnConnect, 0, 2);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(154, 84);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(tableLayoutPanel5, 0, 0);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel4.Location = new Point(12, 12);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 376F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 68.8963242F));
            tableLayoutPanel4.Size = new Size(843, 437);
            tableLayoutPanel4.TabIndex = 9;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 3;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 156F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 520F));
            tableLayoutPanel5.Controls.Add(lstMessageLogs, 2, 0);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel1, 0, 0);
            tableLayoutPanel5.Controls.Add(lstUsers, 1, 0);
            tableLayoutPanel5.Location = new Point(3, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(837, 370);
            tableLayoutPanel5.TabIndex = 0;
            // 
            // lstMessageLogs
            // 
            lstMessageLogs.FormattingEnabled = true;
            lstMessageLogs.Location = new Point(320, 3);
            lstMessageLogs.Name = "lstMessageLogs";
            lstMessageLogs.Size = new Size(514, 364);
            lstMessageLogs.TabIndex = 8;
            // 
            // lstUsers
            // 
            lstUsers.FormattingEnabled = true;
            lstUsers.Location = new Point(164, 3);
            lstUsers.Name = "lstUsers";
            lstUsers.Size = new Size(150, 364);
            lstUsers.TabIndex = 7;
            lstUsers.SelectedIndexChanged += lstUsers_SelectedIndexChanged;
            lstUsers.DoubleClick += lstUsers_DoubleClick;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 676F));
            tableLayoutPanel2.Controls.Add(tableLayoutPanel7, 1, 0);
            tableLayoutPanel2.Location = new Point(3, 379);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(837, 55);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 3;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 440F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75F));
            tableLayoutPanel7.Controls.Add(label3, 0, 0);
            tableLayoutPanel7.Controls.Add(btnSend, 2, 1);
            tableLayoutPanel7.Controls.Add(lblSendingTo, 1, 0);
            tableLayoutPanel7.Controls.Add(rtxtReciver, 0, 1);
            tableLayoutPanel7.Controls.Add(rtxtMessage, 1, 1);
            tableLayoutPanel7.Location = new Point(164, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 2;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 36.7346954F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 63.2653046F));
            tableLayoutPanel7.Size = new Size(670, 49);
            tableLayoutPanel7.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(54, 15);
            label3.TabIndex = 10;
            label3.Text = "Receiver:";
            // 
            // btnSend
            // 
            btnSend.Location = new Point(598, 21);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(69, 23);
            btnSend.TabIndex = 1;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // lblSendingTo
            // 
            lblSendingTo.AutoSize = true;
            lblSendingTo.Location = new Point(158, 0);
            lblSendingTo.Name = "lblSendingTo";
            lblSendingTo.Size = new Size(56, 15);
            lblSendingTo.TabIndex = 2;
            lblSendingTo.Text = "Message:";
            // 
            // rtxtReciver
            // 
            rtxtReciver.Enabled = false;
            rtxtReciver.Location = new Point(3, 21);
            rtxtReciver.Name = "rtxtReciver";
            rtxtReciver.Size = new Size(149, 25);
            rtxtReciver.TabIndex = 11;
            rtxtReciver.Text = "";
            // 
            // rtxtMessage
            // 
            rtxtMessage.Location = new Point(158, 21);
            rtxtMessage.Name = "rtxtMessage";
            rtxtMessage.Size = new Size(434, 25);
            rtxtMessage.TabIndex = 12;
            rtxtMessage.Text = "";
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(866, 460);
            Controls.Add(tableLayoutPanel4);
            Name = "ClientForm";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private TextBox txtUsername;
        private Button btnConnect;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel5;
        private ListBox lstUsers;
        private TableLayoutPanel tableLayoutPanel7;
        private Label label3;
        private Button btnSend;
        private Label lblSendingTo;
        private RichTextBox rtxtReciver;
        private RichTextBox rtxtMessage;
        private TableLayoutPanel tableLayoutPanel2;
        private ListBox lstMessageLogs;
    }
}
