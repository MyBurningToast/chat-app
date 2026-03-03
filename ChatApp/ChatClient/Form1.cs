using System.Net.Sockets;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            try
            {
                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", 5000);
                MessageBox.Show("Connected to server");
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to connect: {e.Message}");
            }
        }
        
    }
}
