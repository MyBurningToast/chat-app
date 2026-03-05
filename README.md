# Networked Chat System
## How to download
1. Go to the latest release
2. Download ChatApp.zip
3. Extract the folder
4. There will be 2 exe files, run one instance of the server and then open as many clients as you like
---
This project is a networked chat system built in C# using raw sockets and winforms. The server and client have separate applications.

The solution is split into three projects:

- **ChatServer** — This is a console app that listens for connections and does the routing
- **ChatClient** — A WinForm that the user interacts with, connects to the server
- **ChatShared** — This is just a shared library so that they both understnad the packet system

## Raw Sockets
Rather than using a wrapper for sockets that had abstractions like `TcpClient` and `StreamReader`. I decided to use raw sockets because they are more fun and I wanted to learn how they worked. Also note, this has only been tested on localhost.

## Reading Data
Data is read from a byte array one character at a time until there is a end of message marker, I chose to use `\n` (the new line character). <br> In the official documentation they used `"<|EOM|>"`.

## Packet System
All communication between the server and clients use a custom packet system. Each packet has a `Type` and `Data` array. I also had to add a way to serialize it into a string so it can be sent over the network.

### Packet Types

| Type | Description |
|---|---|
| `Message` | A public message broadcast to all connected clients |
| `PrivateMessage` | A message sent to a specific user only |
| `UserList` | List of all connected users |
| `Join` | Notification that a user has joined |
| `Leave` | Notification that a user has left |

Markdown files are so cool, makes it look very professional :)

## Threads
Every time a client connects the server will spin up a new thread. This is so that all the clients can be handeled at the same time. The main reason for this is beacuse of this line:
```csharp
socket.Recive(buffer);
```

It stops and waits until data arrives. This means is can only listen to ONE client. If it was done on the main thread then everything would freeze (UI will not respond to clicks and no messages will be recived / sent). This is why `Receive()` needs to be on a background thread.

### Thread Saftey
Multiple threads are all accessing the same `clients` dictionary at the same time.

>Thread 1 (Client A) -> adding to clients dictionary <br>
>Thread 2 (Clietn B) -> reading clients dictionary to broadcast <br>
>Thread 3 (Client C) -> removing from clients dictionary <br>

If two threads modify the same data at the exact same time it can corrupt the data, crash the program or make race conditions <br>
The fix is to use the C# `lock` statment

```csharp
object lockObj = new object();

lock (lockObj)
{
    clients.Add(username, clientSocket);
}
```
`lock` means only one thread can be inside that block per object (we are using `lockObj`). <br> Technicaly we could just use `lock (clients)` and lock the clients dictionary but I read that that is bad practice and its better to make a seperate object for it.
<br>
<br>

# Packet code
Anyways, here is the code. I added a bunch of comments, hopefully it explaines how it works

```csharp
namespace ChatShared
{
    /*
    public enum to define all the types of packets
    Under the hood enums are just a named list of numbers.
    Having actual names is more readable.

    Message = 0
    PrivateMessage = 1
    UserList = 2
    ect
    */
    public enum PacketType
    {
        Message,
        PrivateMessage,
        UserList,
        Join,
        Leave
    }

    public class Packet // every packet has 2 things, the type, and the data (an array of strings carrying)
    {
        public PacketType Type { get; set; }
        public string[] Data { get; set; } = Array.Empty<string>(); // initilize empty

        /*
        this converts the packet into a single string so it can be sent over the network.
        Sockets can only send bytes, not objects, so you need to "flatten" it first.
        (int) casts the enum back to its number

        example:
        1) User hits send
        "hello world"
        
        2) Packet created
        Type = 0
        Data = ["hello world"]

        3) Its Serialized
        "0|hello world"

        4) End of message character is created
        "0|hello world\n"

        5) sent to server
        
        6) server deserializes, adds the username and reserializes
        "0|John|hello world\n"

        7) clients deserialize it


        Serialized form:
        "0|John|hello world\n"

        Deserialized form / object form
        Packet {
            Type = PacketType.Message
            Data = ["John", "hello world"]
        }
        */

        public string Serialize()
        {
            return (int)Type + "|" + string.Join("|", Data); // joins the array with a '|' seperator
        }
        
        public static Packet Deserialize(string raw)
        {
            string[] parts = raw.Split('|');
            return new Packet
            {
                Type = (PacketType)int.Parse(parts[0]),
                Data = parts[1..]
            };
        }
    }
}
```
# Client code

```csharp
using ChatShared; // import the custom library
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    public partial class ClientForm : Form
    {
        /*
        C# is a statically typed language, meaning vareiable types must be declard explicitly (string, int, bool)
        Im not going to explaine all of c# syntax but though its worth mentioning
        */

        Socket socket; // holds the connection to the server
        string username; // stores the connected users name, used when sending packets
        bool connected = false; // track connection state

        public ClientForm() // Constructor is called automatically by the runtime whenever an instance is created
        {
            InitializeComponent(); // auto generated by WinForms
            btnSend.Enabled = false; // disabled the send message button
        }

        /*
        Takes a packet object and sends it over the network

        Get the packet object,
        Serialize it into a string,
        Add the end of message character,
        convert that entire string into bytes beacsue sockets can only send raw bytes, not strings
        */

        private void SendPacket(Packet packet)
        {
            byte[] data = Encoding.UTF8.GetBytes(packet.Serialize() + '\n');
            socket.Send(data); // sends the byte array thorugh the socket
        }
        
        // Recive line, Its job is to read bytes from the socket one at a time until it hits a \n thhen return the full message as a string
        string ReceiveLine()
        {
            StringBuilder sb = new StringBuilder(); // Think of this as a bucket that accumalates characters as they arrive
            byte[] buffer = new byte[1]; // we read 1 byte at a time
            try
            {
                while (true) // will repeate until we return
                {
                    // this is why we need multithreaded, it waits until a byte arrives then puts it in the buffer
                    int received = socket.Receive(buffer);
                    // When socket.Recived gives exit code 0, we return null.
                    // This means the connection what closed cleanly by the other side
                    if (received == 0) return null;
                    // buffer[0] gets the first (only) byte we recive.
                    // Its given as a byte so we cast that nubmer to its char equivalent 104 -> 'h'
                    char c = (char)buffer[0]; 
                    // The message has fully arrived when we reach \n char.
                    // sb.ToString converts the bucket of chars into one string
                    if (c == '\n') return sb.ToString().Trim();
                     // we havnt reached a new line character,
                     // add the char to the backet and loop back to wait for the next byte to arrive
                    sb.Append(c);
                }
            }

            // If the connection drops mid receive it throws a SocketException
            catch (SocketException)
            {
                // We catch it and return null,
                // same as a clean disconnect,
                // the calling code handles both cases the same way :)
                return null;
            }
        }

        private void Disconnect()
        {
            if (!connected) return; // if we already are diconnected, do nothing
            connected = false; // imediatly set to false so no other code tries to send while we are shutting down

            try
            {
                // tell the server we are done sending and reciving, gives the other side a heads up before fully closing
                socket.Shutdown(SocketShutdown.Both);
                socket.Close(); // destroy the socket (also frees the allocated memory)
            }
            // if the socket was already dead this means the server crashed or the network dropped,
            // we dont care about this because we are disconnecting anyways
            catch { }

            // invoke runs this on the UI thread,
            // we dont want to touch this from a background thread (main thread would be ok)
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

        // Im not sure if python has events but this event is Invoked when the connect button is clicked
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (connected) // if we are already connected, then disconnect
            {
                Disconnect();
                return;
            }

            if (string.IsNullOrEmpty(txtUsername.Text)) // dont allow an empty username
            {
                MessageBox.Show("Please enter a username"); // opens an annoying windows message box
                return;
            }


            // this could fail for a lot of reasons so put it into a try so that we dont crash the program w/ an unhandeled exeption
            // although mainly handles if the server crashes or we have the wrong ip
            try
            {
                username = txtUsername.Text; // cache the username from the text box

                // combine the ip adress and port into one object
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000); // 127.0.0.1 is localhost
                // create a socket obejct with 3 settings
                //endPoint.AddressFamily -> IPv4
                // SocketType.Stream -> we are establishing a continuous stream, rather than a chunck
                // ProtocolType.Tcp -> Packets will arrive in order, and on time
                socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint); // reach out to the server at the address and establish the connection
                
                // convert our name to a byte array
                // then add the \n char so the servers ReciveLine knows the username has finished
                byte[] name = Encoding.UTF8.GetBytes(username + '\n');
                // the very first thing we do is send our username to the server.
                // This is so that it can be added to the dictionary
                socket.Send(name);

                connected = true; // flip the flag so the rest of the app knows we're successfully connected


                // yay, multithreading, my favorite part :)
                Thread t = new Thread(ReceiveMessages); // Creates a new thread that will run ReciveMessages
                t.IsBackground = true;  // mark it as a background thread, this means when the app closes the thread will die wiht it.
                t.Start(); // start the new thread
                // the thread will stay alive for as long as the while (connected) loop is true,
                // after the function returns void it will clean itself up automaticly

                // ui stuff
                btnSend.Enabled = true;
                btnConnect.Text = "Disconnect";
                txtUsername.Enabled = false;
            }
            catch (Exception ex) // catch everything
            {
                MessageBox.Show($"Failed: {ex.Message}"); // just show an annoying box
            }
        }

        private void ReceiveMessages()
        {
            while (connected) // keeps looping as long as we are connected
            {
                string raw = ReceiveLine(); // if this returns null, the server has disconnected so we break out of the loop
                if (raw == null) break;

                Packet packet = Packet.Deserialize(raw); // convert from sting back into an object

                switch (packet.Type) // handle the differnt packet types
                {
                    // Data[0] is the username
                    // Data[1] is the message

                    case PacketType.Message:
                        // display the message in the message logs list
                        Invoke(() => lstMessageLogs.Items.Add($"{packet.Data[0]}: {packet.Data[1]}"));
                        break;

                     // we will only recive this packet from the server if we are the intended recipient
                    case PacketType.PrivateMessage:
                        Invoke(() => lstMessageLogs.Items.Add($"[PM from {packet.Data[0]}]: {packet.Data[1]}"));
                        break;

                    case PacketType.UserList: // update the user list
                        Invoke(() =>
                        {
                            lstUsers.Items.Clear(); // start by clearing the entire thing
                            foreach (string user in packet.Data) // loop though every user in the packet
                                if (user != username) // check its not ourselves
                                    lstUsers.Items.Add(user); // add it to the list
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

            // the loop has breaken, this means the connectiong is lost.
            // we now need to shut down everything and reset the UI
            Disconnect();
        }


        // helper method to reset the reciver back to everyone
        void ClearReceiver()
        {
            rtxtReciver.Clear(); // clear the text box
            lstUsers.ClearSelected(); // clear the selected
            lblSendingTo.Text = "Sending to: Everyone"; // reset the text
        }

        // if you double click the reciver from the list, clear it and go back to broadcating to everyone
        private void lstUsers_DoubleClick(object sender, EventArgs e)
        {
            ClearReceiver();
        }

        private void btnSend_Click(object sender, EventArgs e) // called when the send button is clicked
        {
            string message = rtxtMessage.Text; // get the message from the text box
            if (string.IsNullOrEmpty(message)) return; // dont dont send if there is nothing

            try
            {
                // checks if a user is selected, if so then send a private message
                if (lstUsers.SelectedItem != null)
                {
                    // Send a new Packet
                    SendPacket(new Packet
                    {
                        Type = PacketType.PrivateMessage, // set type

                         // get the selected user and the message.
                         // Data[0] -> user to send to
                         // Data[1] -> message
                        Data = new[] { lstUsers.SelectedItem.ToString(), message }
                    });
                }
                else
                {
                    SendPacket(new Packet
                    {
                        Type = PacketType.Message,
                        Data = new[] { message } // send to everyone
                    });
                }

                // clear the ui stuff
                rtxtMessage.Clear();
                ClearReceiver();
            }
            catch (Exception ex) // catch everything
            {
                MessageBox.Show("Send error: " + ex.Message);
            }
        }

         // fires automaticly when the selection in the user list changes
        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItem != null) // guards against it being fired just because it was cleared
            {
                string selected = lstUsers.SelectedItem.ToString(); // get the users name
                rtxtReciver.Text = selected; // update the ui
                lblSendingTo.Text = $"Sending to: {selected}"; 
                rtxtMessage.Focus(); // put the cursor into the message box (quality of life change)
            }
        }


        private void rtxtMessage_KeyDown(object sender, KeyEventArgs e) // called whenever a key is pressed
        {
            if (e.KeyCode == Keys.Enter) // we only are care about the enter key
            {
                e.SuppressKeyPress = true; // stop that annoying windows "DING" sound, god that sound drives me crazy
                btnSend_Click(sender, e); // manually call the send click event
            }
        }
    }
}

```
# Server code
```csharp
using System.Net;
using System.Net.Sockets;
using System.Text;
using ChatShared; // import the custom library

// creates the server socket with the same settings as the client, ipv4, tcp steam
// this is the socket that listens for incomming connections
Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
// binds the server port to 5000
// IpAdress.any means accept connections on any network interface on this machine
serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000)); 
// tells the OS to start listening for incomming connections
// 10 is the backlog, how many pending connections can queue up waiting to be accepted before the OS starts refusing new ones
serverSocket.Listen(10);
Console.WriteLine("Server started on port 5000..."); // same as print("") in python

// Create a new disctionary with a key value pair of every connected client
// the string (username) and the corrosponding Socket that it can send data though
Dictionary<string, Socket> clients = new Dictionary<string, Socket>();

// A plain object used purely as a lock token
// multiple threads access clients at the same time
// one thread might be adding a new client while another is broadcasting
// without locking this will cause race conditions and crashes
// lockObj ensures only one thread can touch clients at a time

// fun fact, the object type in C# is the "ultimate base class for all other data types"
object lockObj = new object();

// same as the client version but because the server had multiple sockets, it needs to know what one to use
void SendPacket(Socket socket, Packet packet)
{
    byte[] data = Encoding.UTF8.GetBytes(packet.Serialize() + "\n");
    socket.Send(data);
}

// sends the same packet to every client
void Broadcast(Packet packet)
{
    lock (lockObj) // lock protects the loop so no other thread can add or remove clients while itterating
        foreach (var s in clients.Values) // loop through every client's socket in the dictionary
            SendPacket(s, packet);
}

// this builds the user list packet from everyone currently connected and broadcasts it to all clients
void BroadcastUserList()
{
    Broadcast(new Packet // create new packet
    {
        Type = PacketType.UserList, // define the packet
        Data = clients.Keys.ToArray() // the users names (keys)
    });
}

// Exactly the same as the clients ReciveLine, but takes a socket as a paramater
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

// Every time a client connects, it has its own thread. The thread is running this fucntion
// The socket defines what client this thread was assigned
void HandleClient(Socket clientSocket)
{
    string username = ReceiveLine(clientSocket); // The first thing we do is read the username the client sends on connect
    if (username == null) return; // if its null, then the client immediately imediatly / crashed. So we dont establish the connection

    Console.WriteLine($"{username} connected"); // Debug info
    // add the username and socket to the dictionary
    // again, keep it thread safe so we dont crash the program
    lock (lockObj) { clients.Add(username, clientSocket); }

    // send a packet of type PacketType.Join to all clients with the username
    Broadcast(new Packet { Type = PacketType.Join, Data = new[] { username } });
    BroadcastUserList(); // send the updated user list to all clients

    while (true) // the main fucntion to recive and route packets
    {
        string raw = ReceiveLine(clientSocket); // wait for message from a specific client
        if (raw == null) break; // null means the disconnected, so we break out and handle the disconnect

        Packet packet = Packet.Deserialize(raw); // convert the raw string into a packet object

        // Check what packet type it is
        switch (packet.Type)
        {
            case PacketType.Message:
                Console.WriteLine($"{username}: {packet.Data[0]}"); // log the message in the console
                Broadcast(new Packet // build a new packet and send to everyone
                {
                    Type = PacketType.Message,
                    Data = new[] { username, packet.Data[0] }
                });
                break;

            case PacketType.PrivateMessage:
                string target = packet.Data[0]; // in a private message, Data[0] is the reciver
                string msg = packet.Data[1]; // the message
                lock (lockObj) // lock before touching the dictionary
                {
                    if (clients.ContainsKey(target)) // check that the target is still connected before trying to send
                        SendPacket(clients[target], new Packet // Construct the new packet
                        {
                            Type = PacketType.PrivateMessage,
                            Data = new[] { username, msg } // swap target for username in the data
                        });
                }
                break;
        }
    }

    // breaking out of the loop means that the client disconnected

    lock (lockObj) { clients.Remove(username); } // remove the client from the list
    Broadcast(new Packet { Type = PacketType.Leave, Data = new[] { username } }); // send a leave message
    BroadcastUserList(); // tell all users
    clientSocket.Close(); // clean up the socket
    Console.WriteLine($"{username} disconnected"); // log it
}

// the loop that the server runs on its main thread, forever
while (true)
{
    //  wait for a new client to connect, when one does we return their socket
    Socket clientSocket = serverSocket.Accept();
    // create a new thread to handel the client, assign it the client's socket
    Thread t = new Thread(() => HandleClient(clientSocket));
    t.Start(); // start the thread
}
```
