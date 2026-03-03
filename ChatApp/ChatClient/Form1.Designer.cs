namespace ChatClient
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
            txtMessage = new TextBox();
            btnSend = new Button();
            lstMessages = new ListBox();
            txtUsername = new TextBox();
            btnConnect = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(130, 364);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(349, 23);
            txtMessage.TabIndex = 0;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(485, 364);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 1;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // lstMessages
            // 
            lstMessages.FormattingEnabled = true;
            lstMessages.Location = new Point(130, 9);
            lstMessages.Name = "lstMessages";
            lstMessages.Size = new Size(430, 349);
            lstMessages.TabIndex = 2;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(12, 30);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(112, 23);
            txtUsername.TabIndex = 3;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(12, 59);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(60, 15);
            label1.TabIndex = 5;
            label1.Text = "Username";
            // 
            // Form1
            // 
            ClientSize = new Size(579, 401);
            Controls.Add(label1);
            Controls.Add(btnConnect);
            Controls.Add(txtUsername);
            Controls.Add(lstMessages);
            Controls.Add(btnSend);
            Controls.Add(txtMessage);
            Name = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMessage;
        private Button btnSend;
        private ListBox lstMessages;
        private TextBox txtUsername;
        private Button btnConnect;
        private Label label1;
    }
}
