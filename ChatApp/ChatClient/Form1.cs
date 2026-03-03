using System.Net.Sockets;

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

            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 5000);

                NetworkStream stream = client.GetStream();
                writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                reader = new StreamReader(stream);

                // listen for incoming mesages on a background thread
                Thread t = new Thread(ReceiveMessages);
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed: {e.Message}");
            }
        }

        void ReceiveMessages()
        {
            string? message;
            while ((message = reader.ReadLine()) != null)
            {
                Invoke(() => lstMessages.Items.Add(message)); // runs on ui thread
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
