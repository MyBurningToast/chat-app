using ChatShared;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatClient
{
    public partial class ClientForm : Form
    {
        Socket socket;
        string username;

        public ClientForm()
        {
            InitializeComponent();
            btnSend.Enabled = false;
        }

        private void SendPacket(Packet packet)
        {
            byte[] data = Encoding.UTF8.GetBytes(packet.Serialize() + '\n');
            socket.Send(data);
        }

        string ReciveLine()
        {
            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[1];
            while (true)
            {
                int recived = socket.Receive(buffer);
                if (recived == 0) return null;
                char c = (char)buffer[0];
                if (c == '\n') return sb.ToString().Trim();
                sb.Append(c);
            }
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
                username = txtUsername.Text;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000));

                byte[] name = Encoding.UTF8.GetBytes(username + '\n');
                socket.Send(name);

                Thread t = new Thread(ReceiveMessages);
                t.IsBackground = true;
                t.Start();

                btnSend.Enabled = true;
                btnConnect.Enabled = false;
                txtUsername.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed {ex.Message}");
            }
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                string raw = ReciveLine();
                if (raw == null) break;

                Packet packet = Packet.Deserialize(raw);

                switch (packet.Type)
                {
                    case PacketType.Message:
                        Invoke(() => lstMessageLogs.Items.Add($"{packet.Data[0]}: {packet.Data[1]}"));
                        break;


                    case PacketType.PrivateMessage:
                        Invoke(() => lstMessageLogs.Items.Add($"[PM from {packet.Data[0]}]: {packet.Data[1]}"));
                        break;

                    case PacketType.UserList:
                        Invoke(() =>
                        {
                            lstUsers.Items.Clear();
                            foreach (string user in packet.Data)
                            {
                                if (user != username)
                                {
                                    lstUsers.Items.Add(user);
                                }
                            }
                        });
                        break;

                    case PacketType.Join:
                        Invoke(() => lstMessageLogs.Items.Add($" -> {packet.Data[0]} has joined"));
                        break;

                    case PacketType.Leave:
                        Invoke(() => lstMessageLogs.Items.Add($" <- {packet.Data[0]} has left"));
                        break;
                }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = rtxtMessage.Text;
            if (string.IsNullOrEmpty(message)) return;

            try
            {
                if (lstUsers.SelectedItem != null)
                {
                    SendPacket(new Packet
                    {
                        Type = PacketType.PrivateMessage,
                        Data = new[] { lstUsers.SelectedItem.ToString(), message }
                    });
                }
                else
                {
                    SendPacket(new Packet
                    {
                        Type = PacketType.Message,
                        Data = new[] { message }
                    });
                }

                rtxtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send error: " + ex.Message);
            }
        }
    }
}
