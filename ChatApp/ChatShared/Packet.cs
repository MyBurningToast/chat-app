namespace ChatShared
{
    public enum PacketType
    {
        Message,
        PrivateMessage,
        UserList,
        Join,
        Leave
    }

    public class Packet
    {
        public PacketType Type { get; set; }
        public string[] Data { get; set; } = Array.Empty<string>();

        public string Serialize()
        {
            return (int)Type + "|" + string.Join("|", Data);
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
