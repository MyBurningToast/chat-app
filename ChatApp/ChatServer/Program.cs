using System.Net;
using System.Net.Sockets;

TcpListener server = new TcpListener(IPAddress.Any, 5000);
server.Start();
Console.WriteLine($"Server started on port 5000...");

List<StreamWriter> clients = new();
object lockObj = new();

void Broadcast(string message)
{
    lock (lockObj)
    {
        foreach (var writer in clients)
        {
            writer.WriteLine(message);
        }
    }
}


void HandleClient(TcpClient client)
{
    NetworkStream stream = client.GetStream();
    StreamReader reader = new StreamReader(stream);
    StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

    // the first message send will be the username
    string? username = reader.ReadLine();
    Console.WriteLine($"{username} connected");

    lock (lockObj) { clients.Add(writer); }

    Broadcast($"{username} has joined");

    string? message;
    while ((message = reader.ReadLine()) != null)
    {
        Console.WriteLine($"Recived from {username}: {message}");
        Broadcast($"{username}: {message}");
    }

    lock (lockObj) { clients.Remove(writer); }
    Console.WriteLine($"{username} discornected");
}

while (true)
{
    TcpClient client = server.AcceptTcpClient();
    //Console.WriteLine("Client connected");

    Thread t = new Thread(() => HandleClient(client));
    t.Start();
}