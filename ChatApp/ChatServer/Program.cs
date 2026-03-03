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

    string? username = null;
    username = reader.ReadLine();
    Console.WriteLine($"{username} connected.");

    lock (lockObj) { clients.Add(writer); }

    Broadcast($"{username} has joined.");

    try
    {
        string? message;
        while ((message = reader.ReadLine()) != null)
        {
            Console.WriteLine($"Received from {username}: {message}");
            Broadcast($"{username}: {message}");
        }
    }
    catch (IOException)
    {
        Console.WriteLine($"{username} connection lost unexpectedly.");
    }
    catch (SocketException)
    {
        Console.WriteLine($"{username} socket error.");
    }
    finally
    {
        lock (lockObj) { clients.Remove(writer); }

        if (username != null)
            Broadcast($"{username} has left.");

        client.Close();
        Console.WriteLine($"{username} disconnected.");
    }
}


while (true)
{
    TcpClient client = server.AcceptTcpClient();

    // new thread for each client
    Thread t = new Thread(() => HandleClient(client));
    t.Start();
}