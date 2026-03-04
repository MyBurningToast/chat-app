using System.Net;
using System.Net.Sockets;
using System.Text;
using ChatShared;

Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000));
serverSocket.Listen(10);
Console.WriteLine("Server started on port 5000...");

Dictionary<string, Socket> clients = new Dictionary<string, Socket>();
object lockObj = new object();

void SendPacket(Socket socket, Packet packet)
{
    byte[] data = Encoding.UTF8.GetBytes(packet.Serialize() + "\n");
    socket.Send(data);
}

void Broadcast(Packet packet)
{
    lock (lockObj)
        foreach (var s in clients.Values)
            SendPacket(s, packet);
}

void BroadcastUserList()
{
    Broadcast(new Packet
    {
        Type = PacketType.UserList,
        Data = clients.Keys.ToArray()
    });
}

string ReceiveLine(Socket socket)
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

void HandleClient(Socket clientSocket)
{
    string username = ReceiveLine(clientSocket);
    if (username == null) return;

    Console.WriteLine($"{username} connected");
    lock (lockObj) { clients.Add(username, clientSocket); }

    Broadcast(new Packet { Type = PacketType.Join, Data = new[] { username } });
    BroadcastUserList();

    while (true)
    {
        string raw = ReceiveLine(clientSocket);
        if (raw == null) break;

        Packet packet = Packet.Deserialize(raw);

        switch (packet.Type)
        {
            case PacketType.Message:
                Console.WriteLine($"{username}: {packet.Data[0]}");
                Broadcast(new Packet
                {
                    Type = PacketType.Message,
                    Data = new[] { username, packet.Data[0] }
                });
                break;

            case PacketType.PrivateMessage:
                string target = packet.Data[0];
                string msg = packet.Data[1];
                lock (lockObj)
                {
                    if (clients.ContainsKey(target))
                        SendPacket(clients[target], new Packet
                        {
                            Type = PacketType.PrivateMessage,
                            Data = new[] { username, msg }
                        });
                }
                break;
        }
    }

    lock (lockObj) { clients.Remove(username); }
    Broadcast(new Packet { Type = PacketType.Leave, Data = new[] { username } });
    BroadcastUserList();
    clientSocket.Close();
    Console.WriteLine($"{username} disconnected");
}

while (true)
{
    Socket clientSocket = serverSocket.Accept();
    Thread t = new Thread(() => HandleClient(clientSocket));
    t.Start();
}