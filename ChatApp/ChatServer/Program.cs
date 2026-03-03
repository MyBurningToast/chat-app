using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

TcpListener server = new TcpListener(IPAddress.Any, 5000);
server.Start();
Console.WriteLine($"Server started on port 5000...");

while (true)
{
    TcpClient client = server.AcceptTcpClient();
    Console.WriteLine("Client connected");

    Thread t = new Thread(() => HandleClient(client));
    t.Start();
}

void HandleClient(TcpClient client)
{
    NetworkStream stream = client.GetStream();
    StreamReader reader = new StreamReader(stream);

    string? message;
    while ((message = reader.ReadLine()) != null)
    {
        Console.WriteLine($"Recived: {message}");
    }

    Console.WriteLine("Client discornected");
}