using System.Net.Sockets;
using System.Text;

namespace Client.Client;

public class Client(string path)
{
    private const int ByteArraySize = 1024;
    private Socket Socket { get; set; } = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

    public async Task Connect()
    {
        var endPoint = new UnixDomainSocketEndPoint(path);

        await Socket.ConnectAsync(endPoint);
    }

    public async Task<int> SendCipher(string message, int amount)
    {
        var mergedData = $"{message}|{amount}";
        var messageBytes = Encoding.ASCII.GetBytes(mergedData);
        var descriptor = await Socket.SendAsync(messageBytes, SocketFlags.None);
        return descriptor;
    }

    public async Task<string> ReceiveData()
    {
        var buffer = new byte[ByteArraySize];
        var numberOfBytesReceived = await Socket.ReceiveAsync(buffer, SocketFlags.None);

        if (numberOfBytesReceived <= 0) return string.Empty;
        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, numberOfBytesReceived);
        return receivedMessage;
    }

    public string Decrypt(string message, int shift)
    {
        var inputArray = message.ToCharArray();
        var builder = new StringBuilder();

        foreach (var letter in inputArray)
        {
            var shiftedLetter = (char)(Convert.ToInt32(letter) - shift) % 26;
            builder.Append(shiftedLetter);
        }

        return builder.ToString();
    }

    public void Teardown()
    {
        Socket.Close();
    }

    public void DisplayMessage(string message)
    {
        
    }
}