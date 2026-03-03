using System.Net.Sockets;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        TcpClient client;
        StreamWriter writer;

        public Form1()
        {
            InitializeComponent();

            try
            {
                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", 5000);
                writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                MessageBox.Show("Connected to server");
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed: {e.Message}");
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
