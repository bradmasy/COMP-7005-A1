using System.Net.Sockets;
using System.Text;
using static Client.Constants;

namespace Client;

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

        if (numberOfBytesReceived <= NoBytes) return string.Empty;
        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, numberOfBytesReceived);
        return receivedMessage;
    }

    public string Decrypt(string message, int shift)
    {
        var builder = new StringBuilder();

        foreach (var letter in message)
        {
            if (char.IsLetter(letter))
            {
                var offset = char.IsUpper(letter) ? UpperAscii : LowerAscii;
                var decrypted = (char)(((letter - offset - shift) % AsciiShift + AsciiShift) % AsciiShift + offset);
                builder.Append(decrypted);
            }
            else if (char.IsDigit(letter))
            {
                var digit = letter - Zero;
                var decryptedDigit = (digit - shift % NumericShift + NumericShift) % NumericShift;
                var decrypted = (char)(decryptedDigit + Zero);
                builder.Append(decrypted);
            }
            else
            {
                builder.Append(letter);
            }
        }

        return builder.ToString();
    }


    public void Teardown()
    {
        Socket.Close();
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine($"The Decrypted Message is: \"{message}\"");
    }
}