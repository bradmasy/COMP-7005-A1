using System.Net.Sockets;
using System.Text;

namespace Client.Client;

public class Client(string path)
{
    private Socket Socket { get; set; } = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

    public async Task Connect()
    {
        Console.WriteLine($"Connecting to {path}");
        var endPoint = new UnixDomainSocketEndPoint(path);
        
        Console.WriteLine($"Connecting to {endPoint}");
       
        await Socket.ConnectAsync(endPoint);

    }

    public async Task<int> SendCipher(string message, string amount)
    {
        var mergedData = $"{message}|{amount}";
        var messageBytes = Encoding.ASCII.GetBytes(mergedData);
        var descriptor = await Socket.SendAsync(messageBytes, SocketFlags.None);
        return descriptor;
    }
    
    public async Task<string> ReceiveData()
    {
        var buffer = new byte[1024];
        var numberOfBytesReceived = await Socket.ReceiveAsync(buffer, SocketFlags.None);

        if (numberOfBytesReceived <= 0) return string.Empty;
        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, numberOfBytesReceived);
        return receivedMessage;

    }
}