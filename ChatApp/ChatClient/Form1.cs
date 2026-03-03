using System.Net.Sockets;
using SharedSettings;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        TcpClient client;
        StreamWriter writer;
        StreamReader reader;

        public Form1()
        {
            InitializeComponent();
            btnSend.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username");
                return;
            }

            try
            {
                client = new TcpClient();
                client.Connect(Settings.ServerIP, Settings.ServerPort);

                NetworkStream stream = client.GetStream();
                writer = new StreamWriter(stream) { AutoFlush = true };
                reader = new StreamReader(stream);

                //send username first
                writer.WriteLine(txtUsername.Text);

                // listen for incoming mesages on a background thread
                Thread t = new Thread(ReceiveMessages);
                t.IsBackground = true;
                t.Start();

                btnSend.Enabled = true;
                btnConnect.Enabled = false;
                txtUsername.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed: {ex.Message}");
            }
        }

        void ReceiveMessages()
        {
            string? message;
            while ((message = reader.ReadLine()) != null)
            {
                Invoke(() => lstMessages.Items.Add(message));
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text;
            if (!string.IsNullOrEmpty(message))
            {
                writer.WriteLine(message);
                txtMessage.Clear();
            }
        }
    }
}
