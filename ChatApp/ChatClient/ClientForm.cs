using ChatShared;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    public partial class ClientForm : Form
    {
        Socket socket;
        string username;
        bool connected = false;

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

        string ReceiveLine()
        {
            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[1];
            try
            {
                while (true)
                {
                    int received = socket.Receive(buffer);
                    if (received == 0) return null;
                    char c = (char)buffer[0];
                    if (c == '\n') return sb.ToString().Trim();
                    sb.Append(c);
                }
            }
            catch (SocketException)
            {
                return null;
            }
        }

        private void Disconnect()
        {
            if (!connected) return;
            connected = false;

            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch { }

            Invoke(() =>
            {
                btnSend.Enabled = false;
                btnConnect.Text = "Connect";
                btnConnect.Enabled = true;
                txtUsername.Enabled = true;
                lstUsers.Items.Clear();
                lblSendingTo.Text = "Sending to: Everyone";
            });
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                Disconnect();
                return;
            }

            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username");
                return;
            }

            try
            {
                username = txtUsername.Text;

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
                socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);

                byte[] name = Encoding.UTF8.GetBytes(username + '\n');
                socket.Send(name);

                connected = true;

                Thread t = new Thread(ReceiveMessages);
                t.IsBackground = true;
                t.Start();

                btnSend.Enabled = true;
                btnConnect.Text = "Disconnect";
                txtUsername.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed: {ex.Message}");
            }
        }

        private void ReceiveMessages()
        {
            while (connected)
            {
                string raw = ReceiveLine();
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
                                if (user != username)
                                    lstUsers.Items.Add(user);
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

            Disconnect();
        }

        void ClearReceiver()
        {
            rtxtReciver.Clear();
            lstUsers.ClearSelected();
            lblSendingTo.Text = "Sending to: Everyone";
        }

        private void lstUsers_DoubleClick(object sender, EventArgs e)
        {
            ClearReceiver();
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
                ClearReceiver();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send error: " + ex.Message);
            }
        }

        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem != null)
            {
                string selected = lstUsers.SelectedItem.ToString();
                rtxtReciver.Text = selected;
                lblSendingTo.Text = $"Sending to: {selected}";
                rtxtMessage.Focus();
            }
        }
    }
}