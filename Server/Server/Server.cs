using System.Net.Sockets;
using System.Text;

namespace Server.Server;

public class Server(string path)
{
    private const int Word = 0;
    private const int Shift = 1;
    private static readonly byte[] Buffer = new byte[1024];

    public Socket Socket { get; set; } = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

    public async Task StartServer()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var endPoint = new UnixDomainSocketEndPoint(path);

        Socket.Bind(endPoint);
        Socket.Listen(100);


        await Run();
    }

    private async Task Run()
    {
        while (true)
        {
            var clientSocket = await Socket.AcceptAsync();
            
            var numberOfBytesReceived = clientSocket.Receive(Buffer, 0, Buffer.Length, SocketFlags.None);

            var message = Encoding.UTF8.GetString(Buffer, 0, numberOfBytesReceived);

            SendEncryptedMessage(clientSocket, message);
            Flush();
        }
    }

    private static void Flush()
    {
        Buffer.AsSpan().Clear();
    }

    private static void SendEncryptedMessage(Socket socket, string message)
    {
        Console.WriteLine("Sending message to server: " + message);
        var split = message.Split("|");
        var cipherText = ShiftCipher(split[0], int.Parse(split[1]));

        socket.Send(Encoding.UTF8.GetBytes(cipherText));
    }

    private static string ShiftCipher(string input, int shift)
    {
        var inputArray = input.ToCharArray();
        var builder = new StringBuilder();

        foreach (var letter in inputArray)
        {
            var shiftedLetter = (char)(Convert.ToInt32(letter) + shift);
            builder.Append(shiftedLetter);
        }

        return builder.ToString();
    }
}