using System.Net.Sockets;
using chat.Messenger;

internal class Program
{

    public static List<ChatMessage> Inbox = new List<ChatMessage>();
    public static List<ChatMessage> Sent = new List<ChatMessage>();
    public static int LogicClock = 0;
    private static string? PartnerIp;
    private static int PartnerPort;
    private static int MyPort;

    private static void Main(string[] args)
    {
        PartnerIp = Environment.GetEnvironmentVariable("partnerIp");
        PartnerPort = int.Parse(Environment.GetEnvironmentVariable("partnerPort"));
        MyPort = int.Parse(Environment.GetEnvironmentVariable("myPort"));

        var dotnetUdpClient = new UdpClient(MyPort);
        var udpMessenger = new UDPMessenger(dotnetUdpClient);
        udpMessenger.AddListenerFunction(ReceiveMessage);

        void ReceiveMessage(string message, IUdpContext context)
        {
            Inbox.Add(new ChatMessage(message, context.SourceIP, LogicClock++));
            Render(Inbox, Sent);
        }

        while (true)
        {
            string newSentMessage = Console.ReadLine();
            Sent.Add(new ChatMessage(newSentMessage, "me", LogicClock++));
            udpMessenger.SendUDP(PartnerIp, PartnerPort, newSentMessage);
            Render(Inbox, Sent);
        }

        void Render(List<ChatMessage> inbox, List<ChatMessage> sent)
        {
            Console.Clear();
            foreach (var message in inbox.Concat(sent).OrderBy(x => x.Seq))
            {
                Console.WriteLine(message.Render());
            }
        }
    }
}

public class ChatMessage
{
    private string message;
    private string from;
    public int Seq;

    public ChatMessage(string message, string from, int seq)
    {
        this.message = message;
        this.from = from;
        this.Seq = seq;
    }

    public string Render() => $"'{from}': {message}";
}